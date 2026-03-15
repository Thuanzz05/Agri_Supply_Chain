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
                    SELECT k.MaKho, k.MaDaiLy, k.TenKho, k.DiaChi, k.DienTich, k.SucChua, 
                           k.TrangThai, k.MoTa, k.NgayTao, dl.TenDaiLy
                    FROM Kho k
                    LEFT JOIN DaiLy dl ON k.MaDaiLy = dl.MaDaiLy
                    ORDER BY k.NgayTao DESC", conn);

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
                    SELECT k.MaKho, k.MaDaiLy, k.TenKho, k.DiaChi, k.DienTich, k.SucChua, 
                           k.TrangThai, k.MoTa, k.NgayTao, dl.TenDaiLy
                    FROM Kho k
                    LEFT JOIN DaiLy dl ON k.MaDaiLy = dl.MaDaiLy
                    WHERE k.MaDaiLy = @MaDaiLy
                    ORDER BY k.NgayTao DESC", conn);
                
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
                    SELECT k.MaKho, k.MaDaiLy, k.TenKho, k.DiaChi, k.DienTich, k.SucChua, 
                           k.TrangThai, k.MoTa, k.NgayTao, dl.TenDaiLy
                    FROM Kho k
                    LEFT JOIN DaiLy dl ON k.MaDaiLy = dl.MaDaiLy
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

        public List<KhoDTO> GetByTrangThai(string trangThai)
        {
            var list = new List<KhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT k.MaKho, k.MaDaiLy, k.TenKho, k.DiaChi, k.DienTich, k.SucChua, 
                           k.TrangThai, k.MoTa, k.NgayTao, dl.TenDaiLy
                    FROM Kho k
                    LEFT JOIN DaiLy dl ON k.MaDaiLy = dl.MaDaiLy
                    WHERE k.TrangThai = @TrangThai
                    ORDER BY k.NgayTao DESC", conn);
                
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} warehouses with status {Status}", list.Count, trangThai);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting warehouses with status {Status}", trangThai);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public int Create(KhoCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    INSERT INTO Kho (MaDaiLy, TenKho, DiaChi, DienTich, SucChua, TrangThai, MoTa, NgayTao) 
                    OUTPUT INSERTED.MaKho 
                    VALUES (@MaDaiLy, @TenKho, @DiaChi, @DienTich, @SucChua, @TrangThai, @MoTa, @NgayTao)", conn);

                cmd.Parameters.AddWithValue("@MaDaiLy", dto.MaDaiLy);
                cmd.Parameters.AddWithValue("@TenKho", dto.TenKho);
                cmd.Parameters.AddWithValue("@DiaChi", dto.DiaChi);
                cmd.Parameters.AddWithValue("@DienTich", dto.DienTich);
                cmd.Parameters.AddWithValue("@SucChua", dto.SucChua);
                cmd.Parameters.AddWithValue("@TrangThai", "hoat_dong");
                cmd.Parameters.AddWithValue("@MoTa", (object?)dto.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayTao", DateTime.Now);

                conn.Open();
                var maKho = (int)cmd.ExecuteScalar();
                
                _logger.LogInformation("Created new warehouse with ID {WarehouseId}", maKho);
                return maKho;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating warehouse");
                if (ex.Number == 547) // Foreign key constraint
                    throw new Exception("Đại lý không tồn tại trong hệ thống", ex);
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
                    SET TenKho = @TenKho, DiaChi = @DiaChi, DienTich = @DienTich, 
                        SucChua = @SucChua, TrangThai = @TrangThai, MoTa = @MoTa 
                    WHERE MaKho = @MaKho", conn);

                cmd.Parameters.AddWithValue("@MaKho", id);
                cmd.Parameters.AddWithValue("@TenKho", dto.TenKho);
                cmd.Parameters.AddWithValue("@DiaChi", dto.DiaChi);
                cmd.Parameters.AddWithValue("@DienTich", dto.DienTich);
                cmd.Parameters.AddWithValue("@SucChua", dto.SucChua);
                cmd.Parameters.AddWithValue("@TrangThai", dto.TrangThai);
                cmd.Parameters.AddWithValue("@MoTa", (object?)dto.MoTa ?? DBNull.Value);

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
                MaDaiLy = reader.GetInt32("MaDaiLy"),
                TenKho = reader.GetString("TenKho"),
                DiaChi = reader.GetString("DiaChi"),
                DienTich = reader.GetDecimal("DienTich"),
                SucChua = reader.GetDecimal("SucChua"),
                TrangThai = reader.GetString("TrangThai"),
                MoTa = reader.IsDBNull("MoTa") ? null : reader.GetString("MoTa"),
                NgayTao = reader.GetDateTime("NgayTao"),
                TenDaiLy = reader.IsDBNull("TenDaiLy") ? null : reader.GetString("TenDaiLy")
            };
        }
    }
}