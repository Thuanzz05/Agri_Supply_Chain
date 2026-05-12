using Microsoft.Data.SqlClient;
using NongDanService.Models.DTOs;
using System.Data;

namespace NongDanService.Data
{
    public class SanPhamRepository : ISanPhamRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<SanPhamRepository> _logger;

        public SanPhamRepository(IConfiguration config, ILogger<SanPhamRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<SanPhamDTO> GetAll()
        {
            var list = new List<SanPhamDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT MaSanPham, TenSanPham, DonViTinh, MoTa, HinhAnh, MaTrangTrai FROM SanPham ORDER BY TenSanPham", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} products from database", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all products");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public SanPhamDTO? GetById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SELECT MaSanPham, TenSanPham, DonViTinh, MoTa, HinhAnh, MaTrangTrai FROM SanPham WHERE MaSanPham = @MaSanPham", conn);
                cmd.Parameters.AddWithValue("@MaSanPham", id);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting product with ID {ProductId}", id);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public List<SanPhamDTO> SearchByName(string tenSanPham)
        {
            var list = new List<SanPhamDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT MaSanPham, TenSanPham, DonViTinh, MoTa, HinhAnh, MaTrangTrai 
                    FROM SanPham 
                    WHERE TenSanPham LIKE @TenSanPham 
                    ORDER BY TenSanPham", conn);
                
                cmd.Parameters.AddWithValue("@TenSanPham", $"%{tenSanPham}%");

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Found {Count} products matching '{SearchTerm}'", list.Count, tenSanPham);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while searching products with term '{SearchTerm}'", tenSanPham);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<SanPhamDTO> GetByNongDan(int maNongDan)
        {
            var list = new List<SanPhamDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT sp.MaSanPham, sp.TenSanPham, sp.DonViTinh, sp.MoTa, sp.HinhAnh, sp.MaTrangTrai 
                    FROM SanPham sp
                    INNER JOIN TrangTrai tt ON sp.MaTrangTrai = tt.MaTrangTrai
                    WHERE tt.MaNongDan = @MaNongDan
                    ORDER BY sp.TenSanPham", conn);
                
                cmd.Parameters.AddWithValue("@MaNongDan", maNongDan);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} products for farmer {FarmerId}", list.Count, maNongDan);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting products for farmer {FarmerId}", maNongDan);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<SanPhamDTO> GetByTrangTrai(int maTrangTrai)
        {
            var list = new List<SanPhamDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT MaSanPham, TenSanPham, DonViTinh, MoTa, HinhAnh, MaTrangTrai 
                    FROM SanPham 
                    WHERE MaTrangTrai = @MaTrangTrai
                    ORDER BY TenSanPham", conn);
                
                cmd.Parameters.AddWithValue("@MaTrangTrai", maTrangTrai);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} products for farm {FarmId}", list.Count, maTrangTrai);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting products for farm {FarmId}", maTrangTrai);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public int Create(SanPhamCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    INSERT INTO SanPham (TenSanPham, DonViTinh, MoTa, HinhAnh, MaTrangTrai) 
                    OUTPUT INSERTED.MaSanPham 
                    VALUES (@TenSanPham, @DonViTinh, @MoTa, @HinhAnh, @MaTrangTrai)", conn);

                cmd.Parameters.AddWithValue("@TenSanPham", dto.TenSanPham);
                cmd.Parameters.AddWithValue("@DonViTinh", dto.DonViTinh);
                cmd.Parameters.AddWithValue("@MoTa", (object?)dto.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HinhAnh", (object?)dto.HinhAnh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaTrangTrai", dto.MaTrangTrai);

                conn.Open();
                var maSanPham = (int)cmd.ExecuteScalar();
                
                _logger.LogInformation("Created new product with ID {ProductId} for farm {FarmId}", maSanPham, dto.MaTrangTrai);
                return maSanPham;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating product");
                if (ex.Number == 2627 || ex.Number == 2601)
                    throw new Exception("Tên sản phẩm đã tồn tại trong hệ thống", ex);
                if (ex.Number == 547)
                    throw new Exception("Mã trang trại không tồn tại", ex);
                throw new Exception("Lỗi tạo sản phẩm trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        public bool Update(int id, SanPhamUpdateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE SanPham 
                    SET TenSanPham = @TenSanPham, DonViTinh = @DonViTinh, MoTa = @MoTa, HinhAnh = @HinhAnh 
                    WHERE MaSanPham = @MaSanPham", conn);

                cmd.Parameters.AddWithValue("@MaSanPham", id);
                cmd.Parameters.AddWithValue("@TenSanPham", dto.TenSanPham);
                cmd.Parameters.AddWithValue("@DonViTinh", dto.DonViTinh);
                cmd.Parameters.AddWithValue("@MoTa", (object?)dto.MoTa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@HinhAnh", (object?)dto.HinhAnh ?? DBNull.Value);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated product with ID {ProductId}", id);
                    return true;
                }
                
                _logger.LogWarning("No product found with ID {ProductId} to update", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating product with ID {ProductId}", id);
                throw new Exception("Lỗi cập nhật sản phẩm trong cơ sở dữ liệu", ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                
                // Kiểm tra xem sản phẩm có đang được sử dụng không
                using var checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM LoNongSan WHERE MaSanPham = @MaSanPham", conn);
                checkCmd.Parameters.AddWithValue("@MaSanPham", id);
                
                conn.Open();
                var count = (int)checkCmd.ExecuteScalar();
                
                if (count > 0)
                {
                    _logger.LogWarning("Cannot delete product {ProductId} because it has {Count} related lots", id, count);
                    throw new Exception($"Không thể xóa sản phẩm này vì đang có {count} lô nông sản liên quan. Vui lòng xóa các lô nông sản trước.");
                }
                
                using var cmd = new SqlCommand("DELETE FROM SanPham WHERE MaSanPham = @MaSanPham", conn);
                cmd.Parameters.AddWithValue("@MaSanPham", id);

                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted product with ID {ProductId}", id);
                    return true;
                }
                
                _logger.LogWarning("No product found with ID {ProductId} to delete", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting product with ID {ProductId}", id);
                if (ex.Number == 547)
                    throw new Exception("Không thể xóa sản phẩm này vì đang có dữ liệu liên quan (lô nông sản, đơn hàng, v.v.)", ex);
                throw new Exception("Lỗi xóa sản phẩm trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        private static SanPhamDTO MapToDTO(SqlDataReader reader)
        {
            return new SanPhamDTO
            {
                MaSanPham = reader.GetInt32("MaSanPham"),
                TenSanPham = reader.GetString("TenSanPham"),
                DonViTinh = reader.GetString("DonViTinh"),
                MoTa = reader.IsDBNull("MoTa") ? null : reader.GetString("MoTa"),
                HinhAnh = reader.IsDBNull("HinhAnh") ? null : reader.GetString("HinhAnh"),
                MaTrangTrai = reader.GetInt32("MaTrangTrai")
            };
        }
    }
}