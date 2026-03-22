using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;
using System.Data;

namespace DaiLyService.Data
{
    public class KhoRepository : IKhoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<KhoRepository> _logger;

        public KhoRepository(IConfiguration config, ILogger<KhoRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<KhoDTO> GetAll()
        {
            var list = new List<KhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT k.MaKho, k.TenKho, k.LoaiKho, k.MaChuSoHuu, k.LoaiChuSoHuu, k.DiaChi,
                           CASE 
                               WHEN k.LoaiChuSoHuu = 'daily' THEN dl.TenDaiLy
                               WHEN k.LoaiChuSoHuu = 'sieuthi' THEN st.TenSieuThi
                               ELSE NULL
                           END AS TenChuSoHuu
                    FROM Kho k
                    LEFT JOIN DaiLy dl ON k.MaChuSoHuu = dl.MaDaiLy AND k.LoaiChuSoHuu = 'daily'
                    LEFT JOIN SieuThi st ON k.MaChuSoHuu = st.MaSieuThi AND k.LoaiChuSoHuu = 'sieuthi'
                    ORDER BY k.MaKho DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} warehouses from database", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all warehouses");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<KhoDTO> GetByDaiLy(int maDaiLy)
        {
            var list = new List<KhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT k.MaKho, k.TenKho, k.LoaiKho, k.MaChuSoHuu, k.LoaiChuSoHuu, k.DiaChi,
                           dl.TenDaiLy AS TenChuSoHuu
                    FROM Kho k
                    LEFT JOIN DaiLy dl ON k.MaChuSoHuu = dl.MaDaiLy
                    WHERE k.MaChuSoHuu = @MaDaiLy AND k.LoaiChuSoHuu = 'daily'
                    ORDER BY k.MaKho DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} warehouses for distributor {DistributorId}", list.Count, maDaiLy);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting warehouses for distributor {DistributorId}", maDaiLy);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public KhoDTO? GetById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT k.MaKho, k.TenKho, k.LoaiKho, k.MaChuSoHuu, k.LoaiChuSoHuu, k.DiaChi,
                           CASE 
                               WHEN k.LoaiChuSoHuu = 'daily' THEN dl.TenDaiLy
                               WHEN k.LoaiChuSoHuu = 'sieuthi' THEN st.TenSieuThi
                               ELSE NULL
                           END AS TenChuSoHuu
                    FROM Kho k
                    LEFT JOIN DaiLy dl ON k.MaChuSoHuu = dl.MaDaiLy AND k.LoaiChuSoHuu = 'daily'
                    LEFT JOIN SieuThi st ON k.MaChuSoHuu = st.MaSieuThi AND k.LoaiChuSoHuu = 'sieuthi'
                    WHERE k.MaKho = @MaKho", conn);
                
                cmd.Parameters.AddWithValue("@MaKho", id);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Warehouse with ID {WarehouseId} not found", id);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting warehouse with ID {WarehouseId}", id);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public int Create(KhoCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                
                // Kiểm tra tính hợp lệ của LoaiChuSoHuu
                if (dto.LoaiChuSoHuu != "daily" && dto.LoaiChuSoHuu != "sieuthi")
                {
                    throw new Exception("Loại chủ sở hữu chỉ được phép là 'daily' hoặc 'sieuthi'");
                }
                
                // Kiểm tra tính hợp lệ của MaChuSoHuu
                bool isValidOwner = false;
                if (dto.LoaiChuSoHuu == "daily")
                {
                    using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM DaiLy WHERE MaDaiLy = @MaChuSoHuu", conn);
                    checkCmd.Parameters.AddWithValue("@MaChuSoHuu", dto.MaChuSoHuu);
                    isValidOwner = (int)checkCmd.ExecuteScalar() > 0;
                }
                else if (dto.LoaiChuSoHuu == "sieuthi")
                {
                    using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM SieuThi WHERE MaSieuThi = @MaChuSoHuu", conn);
                    checkCmd.Parameters.AddWithValue("@MaChuSoHuu", dto.MaChuSoHuu);
                    isValidOwner = (int)checkCmd.ExecuteScalar() > 0;
                }
                
                if (!isValidOwner)
                {
                    throw new Exception($"Chủ sở hữu với mã {dto.MaChuSoHuu} không tồn tại trong hệ thống");
                }
                
                // Tạo kho
                using var cmd = new SqlCommand(@"
                    INSERT INTO Kho (TenKho, LoaiKho, MaChuSoHuu, LoaiChuSoHuu, DiaChi) 
                    OUTPUT INSERTED.MaKho 
                    VALUES (@TenKho, @LoaiKho, @MaChuSoHuu, @LoaiChuSoHuu, @DiaChi)", conn);

                cmd.Parameters.AddWithValue("@TenKho", dto.TenKho);
                cmd.Parameters.AddWithValue("@LoaiKho", dto.LoaiKho);
                cmd.Parameters.AddWithValue("@MaChuSoHuu", dto.MaChuSoHuu);
                cmd.Parameters.AddWithValue("@LoaiChuSoHuu", dto.LoaiChuSoHuu);
                cmd.Parameters.AddWithValue("@DiaChi", (object?)dto.DiaChi ?? DBNull.Value);

                var maKho = (int)cmd.ExecuteScalar();
                
                _logger.LogInformation("Created new warehouse with ID {WarehouseId}", maKho);
                return maKho;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating warehouse");
                throw new Exception("Lỗi tạo kho trong cơ sở dữ liệu: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating warehouse");
                throw;
            }
        }

        public bool Update(int id, KhoUpdateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE Kho 
                    SET TenKho = @TenKho, LoaiKho = @LoaiKho, DiaChi = @DiaChi
                    WHERE MaKho = @MaKho", conn);

                cmd.Parameters.AddWithValue("@MaKho", id);
                cmd.Parameters.AddWithValue("@TenKho", dto.TenKho);
                cmd.Parameters.AddWithValue("@LoaiKho", dto.LoaiKho);
                cmd.Parameters.AddWithValue("@DiaChi", (object?)dto.DiaChi ?? DBNull.Value);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated warehouse with ID {WarehouseId}", id);
                    return true;
                }
                
                _logger.LogWarning("No warehouse found with ID {WarehouseId} to update", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating warehouse with ID {WarehouseId}", id);
                throw new Exception("Lỗi cập nhật kho trong cơ sở dữ liệu", ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM Kho WHERE MaKho = @MaKho", conn);
                cmd.Parameters.AddWithValue("@MaKho", id);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted warehouse with ID {WarehouseId}", id);
                    return true;
                }
                
                _logger.LogWarning("No warehouse found with ID {WarehouseId} to delete", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting warehouse with ID {WarehouseId}", id);
                if (ex.Number == 547)
                    throw new Exception("Không thể xóa kho này vì đang có dữ liệu liên quan", ex);
                throw new Exception("Lỗi xóa kho trong cơ sở dữ liệu", ex);
            }
        }

        private static KhoDTO MapToDTO(SqlDataReader reader)
        {
            return new KhoDTO
            {
                MaKho = reader.GetInt32("MaKho"),
                TenKho = reader.GetString("TenKho"),
                LoaiKho = reader.GetString("LoaiKho"),
                MaChuSoHuu = reader.GetInt32("MaChuSoHuu"),
                LoaiChuSoHuu = reader.GetString("LoaiChuSoHuu"),
                DiaChi = reader.IsDBNull("DiaChi") ? null : reader.GetString("DiaChi"),
                TenChuSoHuu = reader.IsDBNull("TenChuSoHuu") ? null : reader.GetString("TenChuSoHuu")
            };
        }
    }
}
        public List<KhoDTO> GetBySieuThi()
        {
            var list = new List<KhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT k.MaKho, k.TenKho, k.LoaiKho, k.MaChuSoHuu, k.LoaiChuSoHuu, k.DiaChi,
                           st.TenSieuThi AS TenChuSoHuu
                    FROM Kho k
                    LEFT JOIN SieuThi st ON k.MaChuSoHuu = st.MaSieuThi
                    WHERE k.LoaiChuSoHuu = 'sieuthi'
                    ORDER BY k.MaKho DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} supermarket warehouses", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting supermarket warehouses");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<KhoDTO> GetBySieuThi(int maSieuThi)
        {
            var list = new List<KhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT k.MaKho, k.TenKho, k.LoaiKho, k.MaChuSoHuu, k.LoaiChuSoHuu, k.DiaChi,
                           st.TenSieuThi AS TenChuSoHuu
                    FROM Kho k
                    LEFT JOIN SieuThi st ON k.MaChuSoHuu = st.MaSieuThi
                    WHERE k.MaChuSoHuu = @MaSieuThi AND k.LoaiChuSoHuu = 'sieuthi'
                    ORDER BY k.MaKho DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaSieuThi", maSieuThi);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} warehouses for supermarket {SupermarketId}", list.Count, maSieuThi);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting warehouses for supermarket {SupermarketId}", maSieuThi);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }