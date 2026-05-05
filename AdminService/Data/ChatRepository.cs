using AdminService.Models.DTOs;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdminService.Data
{
    public class ChatRepository
    {
        private readonly string _connectionString;

        public ChatRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string not found");
        }

        // Lấy danh sách cuộc trò chuyện
        public async Task<List<CuocTroChuyenDTO>> GetConversationsAsync(int maNguoi, string loaiNguoi)
        {
            var conversations = new List<CuocTroChuyenDTO>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    ct.MaCuocTroChuyen,
                    ct.MaNguoi1,
                    ct.LoaiNguoi1,
                    ct.MaNguoi2,
                    ct.LoaiNguoi2,
                    ct.TinNhanCuoi,
                    ct.NgayCapNhat,
                    ct.NgayTao,
                    CASE 
                        WHEN ct.MaNguoi1 = @MaNguoi AND ct.LoaiNguoi1 = @LoaiNguoi 
                        THEN ct.MaNguoi2 
                        ELSE ct.MaNguoi1 
                    END AS MaNguoiKia,
                    CASE 
                        WHEN ct.MaNguoi1 = @MaNguoi AND ct.LoaiNguoi1 = @LoaiNguoi 
                        THEN ct.LoaiNguoi2 
                        ELSE ct.LoaiNguoi1 
                    END AS LoaiNguoiKia,
                    (SELECT COUNT(*) FROM TinNhan 
                     WHERE MaCuocTroChuyen = ct.MaCuocTroChuyen 
                     AND DaDoc = 0 
                     AND NOT (MaNguoiGui = @MaNguoi AND LoaiNguoiGui = @LoaiNguoi)) AS SoTinNhanChuaDoc
                FROM CuocTroChuyen ct
                WHERE (ct.MaNguoi1 = @MaNguoi AND ct.LoaiNguoi1 = @LoaiNguoi)
                   OR (ct.MaNguoi2 = @MaNguoi AND ct.LoaiNguoi2 = @LoaiNguoi)
                ORDER BY ct.NgayCapNhat DESC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaNguoi", maNguoi);
            command.Parameters.AddWithValue("@LoaiNguoi", loaiNguoi);

            using var reader = await command.ExecuteReaderAsync();
            
            // Đọc tất cả conversations trước
            var tempList = new List<(CuocTroChuyenDTO conv, int maNguoiKia, string loaiNguoiKia)>();
            while (await reader.ReadAsync())
            {
                var conversation = new CuocTroChuyenDTO
                {
                    MaCuocTroChuyen = reader.GetInt32("MaCuocTroChuyen"),
                    MaNguoi1 = reader.GetInt32("MaNguoi1"),
                    LoaiNguoi1 = reader.GetString("LoaiNguoi1"),
                    MaNguoi2 = reader.GetInt32("MaNguoi2"),
                    LoaiNguoi2 = reader.GetString("LoaiNguoi2"),
                    TinNhanCuoi = reader.IsDBNull("TinNhanCuoi") ? null : reader.GetString("TinNhanCuoi"),
                    NgayCapNhat = reader.GetDateTime("NgayCapNhat"),
                    NgayTao = reader.GetDateTime("NgayTao"),
                    SoTinNhanChuaDoc = reader.GetInt32("SoTinNhanChuaDoc")
                };

                var maNguoiKia = reader.GetInt32("MaNguoiKia");
                var loaiNguoiKia = reader.GetString("LoaiNguoiKia");
                
                tempList.Add((conversation, maNguoiKia, loaiNguoiKia));
            }
            
            // Đóng reader trước khi query tiếp
            await reader.CloseAsync();

            // Lấy thông tin người kia cho từng conversation
            foreach (var item in tempList)
            {
                var userInfo = await GetUserInfoAsync(item.maNguoiKia, item.loaiNguoiKia, connection);
                item.conv.TenNguoiKia = userInfo.Ten;
                item.conv.AnhDaiDienNguoiKia = userInfo.AnhDaiDien;
                conversations.Add(item.conv);
            }

            return conversations;
        }

        // Lấy tin nhắn trong cuộc trò chuyện
        public async Task<List<TinNhanDTO>> GetMessagesAsync(int maCuocTroChuyen)
        {
            var messages = new List<TinNhanDTO>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    MaTinNhan,
                    MaCuocTroChuyen,
                    MaNguoiGui,
                    LoaiNguoiGui,
                    NoiDung,
                    DaDoc,
                    NgayGui
                FROM TinNhan
                WHERE MaCuocTroChuyen = @MaCuocTroChuyen
                ORDER BY NgayGui ASC";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaCuocTroChuyen", maCuocTroChuyen);

            using var reader = await command.ExecuteReaderAsync();
            
            // Đọc tất cả messages trước
            var tempList = new List<(TinNhanDTO msg, int maNguoiGui, string loaiNguoiGui)>();
            while (await reader.ReadAsync())
            {
                var message = new TinNhanDTO
                {
                    MaTinNhan = reader.GetInt32("MaTinNhan"),
                    MaCuocTroChuyen = reader.GetInt32("MaCuocTroChuyen"),
                    MaNguoiGui = reader.GetInt32("MaNguoiGui"),
                    LoaiNguoiGui = reader.GetString("LoaiNguoiGui"),
                    NoiDung = reader.GetString("NoiDung"),
                    DaDoc = reader.GetBoolean("DaDoc"),
                    NgayGui = reader.GetDateTime("NgayGui")
                };

                tempList.Add((message, message.MaNguoiGui, message.LoaiNguoiGui));
            }
            
            // Đóng reader trước khi query tiếp
            await reader.CloseAsync();

            // Lấy thông tin người gửi cho từng message
            foreach (var item in tempList)
            {
                var userInfo = await GetUserInfoAsync(item.maNguoiGui, item.loaiNguoiGui, connection);
                item.msg.TenNguoiGui = userInfo.Ten;
                item.msg.AnhDaiDienNguoiGui = userInfo.AnhDaiDien;
                messages.Add(item.msg);
            }

            return messages;
        }

        // Gửi tin nhắn
        public async Task<TinNhanDTO> SendMessageAsync(int maNguoiGui, string loaiNguoiGui, GuiTinNhanRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            int maCuocTroChuyen;

            // Nếu chưa có cuộc trò chuyện, tạo mới
            if (request.MaCuocTroChuyen == null || request.MaCuocTroChuyen == 0)
            {
                // Kiểm tra xem đã có cuộc trò chuyện giữa 2 người chưa
                var checkQuery = @"
                    SELECT MaCuocTroChuyen 
                    FROM CuocTroChuyen
                    WHERE (MaNguoi1 = @MaNguoi1 AND LoaiNguoi1 = @LoaiNguoi1 AND MaNguoi2 = @MaNguoi2 AND LoaiNguoi2 = @LoaiNguoi2)
                       OR (MaNguoi1 = @MaNguoi2 AND LoaiNguoi1 = @LoaiNguoi2 AND MaNguoi2 = @MaNguoi1 AND LoaiNguoi2 = @LoaiNguoi1)";

                using var checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@MaNguoi1", maNguoiGui);
                checkCommand.Parameters.AddWithValue("@LoaiNguoi1", loaiNguoiGui);
                checkCommand.Parameters.AddWithValue("@MaNguoi2", request.MaNguoiNhan);
                checkCommand.Parameters.AddWithValue("@LoaiNguoi2", request.LoaiNguoiNhan);

                var existingId = await checkCommand.ExecuteScalarAsync();
                if (existingId != null)
                {
                    maCuocTroChuyen = Convert.ToInt32(existingId);
                }
                else
                {
                    // Tạo cuộc trò chuyện mới
                    var insertConvQuery = @"
                        INSERT INTO CuocTroChuyen (MaNguoi1, LoaiNguoi1, MaNguoi2, LoaiNguoi2, TinNhanCuoi, NgayCapNhat)
                        VALUES (@MaNguoi1, @LoaiNguoi1, @MaNguoi2, @LoaiNguoi2, @NoiDung, SYSDATETIME());
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using var insertConvCommand = new SqlCommand(insertConvQuery, connection);
                    insertConvCommand.Parameters.AddWithValue("@MaNguoi1", maNguoiGui);
                    insertConvCommand.Parameters.AddWithValue("@LoaiNguoi1", loaiNguoiGui);
                    insertConvCommand.Parameters.AddWithValue("@MaNguoi2", request.MaNguoiNhan);
                    insertConvCommand.Parameters.AddWithValue("@LoaiNguoi2", request.LoaiNguoiNhan);
                    insertConvCommand.Parameters.AddWithValue("@NoiDung", request.NoiDung);

                    maCuocTroChuyen = Convert.ToInt32(await insertConvCommand.ExecuteScalarAsync());
                }
            }
            else
            {
                maCuocTroChuyen = request.MaCuocTroChuyen.Value;

                // Cập nhật tin nhắn cuối và thời gian
                var updateConvQuery = @"
                    UPDATE CuocTroChuyen 
                    SET TinNhanCuoi = @NoiDung, NgayCapNhat = SYSDATETIME()
                    WHERE MaCuocTroChuyen = @MaCuocTroChuyen";

                using var updateConvCommand = new SqlCommand(updateConvQuery, connection);
                updateConvCommand.Parameters.AddWithValue("@NoiDung", request.NoiDung);
                updateConvCommand.Parameters.AddWithValue("@MaCuocTroChuyen", maCuocTroChuyen);
                await updateConvCommand.ExecuteNonQueryAsync();
            }

            // Thêm tin nhắn
            var insertMsgQuery = @"
                INSERT INTO TinNhan (MaCuocTroChuyen, MaNguoiGui, LoaiNguoiGui, NoiDung, DaDoc, NgayGui)
                VALUES (@MaCuocTroChuyen, @MaNguoiGui, @LoaiNguoiGui, @NoiDung, 0, SYSDATETIME());
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var insertMsgCommand = new SqlCommand(insertMsgQuery, connection);
            insertMsgCommand.Parameters.AddWithValue("@MaCuocTroChuyen", maCuocTroChuyen);
            insertMsgCommand.Parameters.AddWithValue("@MaNguoiGui", maNguoiGui);
            insertMsgCommand.Parameters.AddWithValue("@LoaiNguoiGui", loaiNguoiGui);
            insertMsgCommand.Parameters.AddWithValue("@NoiDung", request.NoiDung);

            var maTinNhan = Convert.ToInt32(await insertMsgCommand.ExecuteScalarAsync());

            // Lấy thông tin người gửi
            var userInfo = await GetUserInfoAsync(maNguoiGui, loaiNguoiGui, connection);

            return new TinNhanDTO
            {
                MaTinNhan = maTinNhan,
                MaCuocTroChuyen = maCuocTroChuyen,
                MaNguoiGui = maNguoiGui,
                LoaiNguoiGui = loaiNguoiGui,
                NoiDung = request.NoiDung,
                DaDoc = false,
                NgayGui = DateTime.Now,
                TenNguoiGui = userInfo.Ten,
                AnhDaiDienNguoiGui = userInfo.AnhDaiDien
            };
        }

        // Đánh dấu đã đọc
        public async Task MarkAsReadAsync(int maCuocTroChuyen, int maNguoi, string loaiNguoi)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE TinNhan 
                SET DaDoc = 1
                WHERE MaCuocTroChuyen = @MaCuocTroChuyen
                  AND NOT (MaNguoiGui = @MaNguoi AND LoaiNguoiGui = @LoaiNguoi)
                  AND DaDoc = 0";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaCuocTroChuyen", maCuocTroChuyen);
            command.Parameters.AddWithValue("@MaNguoi", maNguoi);
            command.Parameters.AddWithValue("@LoaiNguoi", loaiNguoi);

            await command.ExecuteNonQueryAsync();
        }

        // Đếm tin nhắn chưa đọc
        public async Task<int> GetUnreadCountAsync(int maNguoi, string loaiNguoi)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT COUNT(DISTINCT tn.MaCuocTroChuyen)
                FROM TinNhan tn
                INNER JOIN CuocTroChuyen ct ON tn.MaCuocTroChuyen = ct.MaCuocTroChuyen
                WHERE tn.DaDoc = 0
                  AND NOT (tn.MaNguoiGui = @MaNguoi AND tn.LoaiNguoiGui = @LoaiNguoi)
                  AND ((ct.MaNguoi1 = @MaNguoi AND ct.LoaiNguoi1 = @LoaiNguoi)
                    OR (ct.MaNguoi2 = @MaNguoi AND ct.LoaiNguoi2 = @LoaiNguoi))";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaNguoi", maNguoi);
            command.Parameters.AddWithValue("@LoaiNguoi", loaiNguoi);

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        // Lấy danh sách người dùng có thể nhắn tin
        public async Task<List<UserListDTO>> GetAvailableUsersAsync(string loaiNguoi)
        {
            var users = new List<UserListDTO>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = loaiNguoi.ToLower() switch
            {
                "nongdan" => @"
                    SELECT MaNongDan AS MaNguoi, 'nongdan' AS LoaiNguoi, HoTen AS Ten, 
                           AnhDaiDien, DiaChi, SoDienThoai 
                    FROM NongDan 
                    ORDER BY HoTen",
                
                "daily" => @"
                    SELECT MaDaiLy AS MaNguoi, 'daily' AS LoaiNguoi, TenDaiLy AS Ten, 
                           AnhDaiDien, DiaChi, SoDienThoai 
                    FROM DaiLy 
                    ORDER BY TenDaiLy",
                
                "sieuthi" => @"
                    SELECT MaSieuThi AS MaNguoi, 'sieuthi' AS LoaiNguoi, TenSieuThi AS Ten, 
                           AnhDaiDien, DiaChi, SoDienThoai 
                    FROM SieuThi 
                    ORDER BY TenSieuThi",
                
                _ => throw new ArgumentException("Loại người dùng không hợp lệ")
            };

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new UserListDTO
                {
                    MaNguoi = reader.GetInt32(0),
                    LoaiNguoi = reader.GetString(1),
                    Ten = reader.GetString(2),
                    AnhDaiDien = reader.IsDBNull(3) ? null : reader.GetString(3),
                    DiaChi = reader.IsDBNull(4) ? null : reader.GetString(4),
                    SoDienThoai = reader.IsDBNull(5) ? null : reader.GetString(5)
                });
            }

            return users;
        }

        // Xóa cuộc trò chuyện
        public async Task DeleteConversationAsync(int maCuocTroChuyen)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Xóa cuộc trò chuyện (tin nhắn sẽ tự động xóa do CASCADE)
            var query = "DELETE FROM CuocTroChuyen WHERE MaCuocTroChuyen = @MaCuocTroChuyen";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaCuocTroChuyen", maCuocTroChuyen);

            await command.ExecuteNonQueryAsync();
        }

        // Helper: Lấy thông tin user
        private async Task<(string Ten, string? AnhDaiDien)> GetUserInfoAsync(int maNguoi, string loaiNguoi, SqlConnection connection)
        {
            string query = loaiNguoi.ToLower() switch
            {
                "nongdan" => "SELECT HoTen AS Ten, AnhDaiDien FROM NongDan WHERE MaNongDan = @MaNguoi",
                "daily" => "SELECT TenDaiLy AS Ten, AnhDaiDien FROM DaiLy WHERE MaDaiLy = @MaNguoi",
                "sieuthi" => "SELECT TenSieuThi AS Ten, AnhDaiDien FROM SieuThi WHERE MaSieuThi = @MaNguoi",
                _ => throw new ArgumentException("Loại người dùng không hợp lệ")
            };

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MaNguoi", maNguoi);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return (
                    reader.GetString(0), // Ten
                    reader.IsDBNull(1) ? null : reader.GetString(1) // AnhDaiDien
                );
            }

            return ("Unknown", null);
        }
    }
}
