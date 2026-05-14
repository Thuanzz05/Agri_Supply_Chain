using Microsoft.Data.SqlClient;
using SieuThiService.Models.DTOs;
using System.Data;

namespace SieuThiService.Data
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

        public KhoDTO? GetById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT k.MaKho, k.TenKho, k.LoaiKho, k.MaChuSoHuu, k.LoaiChuSoHuu, k.DiaChi,
                           st.TenSieuThi AS TenChuSoHuu
                    FROM Kho k
                    LEFT JOIN SieuThi st ON k.MaChuSoHuu = st.MaSieuThi
                    WHERE k.MaKho = @MaKho", conn);
                
                cmd.Parameters.AddWithValue("@MaKho", id);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting warehouse by ID {WarehouseId}", id);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
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

        public int Create(KhoCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    INSERT INTO Kho (TenKho, LoaiKho, MaChuSoHuu, LoaiChuSoHuu, DiaChi)
                    OUTPUT INSERTED.MaKho
                    VALUES (@TenKho, @LoaiKho, @MaChuSoHuu, @LoaiChuSoHuu, @DiaChi)", conn);

                cmd.Parameters.Add("@TenKho", SqlDbType.NVarChar, 100).Value = dto.TenKho;
                cmd.Parameters.Add("@LoaiKho", SqlDbType.NVarChar, 20).Value = dto.LoaiKho;
                cmd.Parameters.Add("@MaChuSoHuu", SqlDbType.Int).Value = dto.MaChuSoHuu;
                cmd.Parameters.Add("@LoaiChuSoHuu", SqlDbType.NVarChar, 20).Value = dto.LoaiChuSoHuu;
                cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 255).Value = (object?)dto.DiaChi ?? DBNull.Value;

                conn.Open();
                var maKho = (int)cmd.ExecuteScalar();
                
                _logger.LogInformation("Created new warehouse with ID {WarehouseId}", maKho);
                return maKho;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating warehouse");
                throw new Exception("Lỗi tạo kho trong cơ sở dữ liệu: " + ex.Message, ex);
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

                cmd.Parameters.Add("@MaKho", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@TenKho", SqlDbType.NVarChar, 100).Value = dto.TenKho;
                cmd.Parameters.Add("@LoaiKho", SqlDbType.NVarChar, 20).Value = dto.LoaiKho;
                cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 255).Value = (object?)dto.DiaChi ?? DBNull.Value;

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
                throw new Exception("Lỗi cập nhật kho trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                
                // Kiểm tra xem kho có tồn kho không
                using var checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM TonKho WHERE MaKho = @MaKho", conn);
                checkCmd.Parameters.Add("@MaKho", SqlDbType.Int).Value = id;
                
                conn.Open();
                var count = (int)checkCmd.ExecuteScalar();
                
                if (count > 0)
                {
                    _logger.LogWarning("Cannot delete warehouse {WarehouseId} because it has {Count} inventory records", id, count);
                    throw new Exception($"Không thể xóa kho này vì đang có {count} bản ghi tồn kho. Vui lòng xóa tồn kho trước.");
                }
                
                // Nếu không có tồn kho, thực hiện xóa
                using var cmd = new SqlCommand("DELETE FROM Kho WHERE MaKho = @MaKho", conn);
                cmd.Parameters.Add("@MaKho", SqlDbType.Int).Value = id;

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
                throw new Exception("Lỗi xóa kho trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }
    }
}