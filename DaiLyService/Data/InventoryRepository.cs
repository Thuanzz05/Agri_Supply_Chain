using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;
using System.Data;

namespace DaiLyService.Data
{
    public class InventoryRepository : ITonKhoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<InventoryRepository> _logger;

        public InventoryRepository(IConfiguration config, ILogger<InventoryRepository> logger)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        public List<TonKhoDTO> GetAll()
        {
            var list = new List<TonKhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaKho, tk.MaLo, tk.SoLuong, tk.NgayCapNhat,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE k.LoaiChuSoHuu = 'daily'
                    ORDER BY tk.NgayCapNhat DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} distributor inventory records from database", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting distributor inventory records");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<TonKhoDTO> GetByKho(int maKho)
        {
            var list = new List<TonKhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaKho, tk.MaLo, tk.SoLuong, tk.NgayCapNhat,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE tk.MaKho = @MaKho AND k.LoaiChuSoHuu = 'daily'
                    ORDER BY tk.NgayCapNhat DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaKho", maKho);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inventory records for distributor warehouse {WarehouseId}", list.Count, maKho);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inventory records for distributor warehouse {WarehouseId}", maKho);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public TonKhoDTO? GetByKhoAndLo(int maKho, int maLo)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaKho, tk.MaLo, tk.SoLuong, tk.NgayCapNhat,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLo = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE tk.MaKho = @MaKho AND tk.MaLo = @MaLo AND k.LoaiChuSoHuu = 'daily'", conn);
                
                cmd.Parameters.AddWithValue("@MaKho", maKho);
                cmd.Parameters.AddWithValue("@MaLo", maLo);

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
                _logger.LogError(ex, "SQL error occurred while getting distributor inventory record");
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public bool Create(int maKho, int maLo, decimal soLuong)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();
                
                // Kiểm tra kho có thuộc đại lý không
                using var checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM Kho 
                    WHERE MaKho = @MaKho AND LoaiChuSoHuu = 'daily'", conn);
                checkCmd.Parameters.AddWithValue("@MaKho", maKho);
                
                if ((int)checkCmd.ExecuteScalar() == 0)
                {
                    throw new Exception("Kho không thuộc quyền quản lý của đại lý");
                }
                
                // Tạo tồn kho
                using var cmd = new SqlCommand(@"
                    INSERT INTO TonKho (MaKho, MaLo, SoLuong, NgayCapNhat) 
                    VALUES (@MaKho, @MaLo, @SoLuong, @NgayCapNhat)", conn);

                cmd.Parameters.AddWithValue("@MaKho", maKho);
                cmd.Parameters.AddWithValue("@MaLo", maLo);
                cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                var rowsAffected = cmd.ExecuteNonQuery();
                
                _logger.LogInformation("Created inventory record for distributor warehouse {WarehouseId}, lot {LotId}", maKho, maLo);
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while creating inventory record");
                if (ex.Number == 547) // Foreign key constraint
                    throw new Exception("Kho hoặc lô nông sản không tồn tại trong hệ thống", ex);
                if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint
                    throw new Exception("Lô nông sản đã tồn tại trong kho này", ex);
                throw new Exception("Lỗi tạo tồn kho trong cơ sở dữ liệu: " + ex.Message, ex);
            }
        }

        public bool UpdateSoLuong(int maKho, int maLo, decimal soLuongMoi)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE TonKho 
                    SET SoLuong = @SoLuongMoi, NgayCapNhat = @NgayCapNhat
                    FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    WHERE tk.MaKho = @MaKho AND tk.MaLo = @MaLo AND k.LoaiChuSoHuu = 'daily'", conn);

                cmd.Parameters.AddWithValue("@MaKho", maKho);
                cmd.Parameters.AddWithValue("@MaLo", maLo);
                cmd.Parameters.AddWithValue("@SoLuongMoi", soLuongMoi);
                cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated quantity for distributor warehouse {WarehouseId}, lot {LotId} to {NewQuantity}", maKho, maLo, soLuongMoi);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating distributor inventory quantity");
                throw new Exception("Lỗi cập nhật số lượng tồn kho", ex);
            }
        }

        public bool Delete(int maKho, int maLo)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    DELETE tk FROM TonKho tk
                    INNER JOIN Kho k ON tk.MaKho = k.MaKho
                    WHERE tk.MaKho = @MaKho AND tk.MaLo = @MaLo AND k.LoaiChuSoHuu = 'daily'", conn);
                
                cmd.Parameters.AddWithValue("@MaKho", maKho);
                cmd.Parameters.AddWithValue("@MaLo", maLo);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted distributor inventory record for warehouse {WarehouseId}, lot {LotId}", maKho, maLo);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting distributor inventory record");
                throw new Exception("Lỗi xóa tồn kho trong cơ sở dữ liệu", ex);
            }
        }

        private static TonKhoDTO MapToDTO(SqlDataReader reader)
        {
            return new TonKhoDTO
            {
                MaKho = reader.GetInt32("MaKho"),
                MaLo = reader.GetInt32("MaLo"),
                SoLuong = reader.GetDecimal("SoLuong"),
                NgayCapNhat = reader.GetDateTime("NgayCapNhat"),
                TenKho = reader.IsDBNull("TenKho") ? null : reader.GetString("TenKho"),
                TenSanPham = reader.IsDBNull("TenSanPham") ? null : reader.GetString("TenSanPham"),
                DonViTinh = reader.IsDBNull("DonViTinh") ? null : reader.GetString("DonViTinh"),
                MaQR = reader.IsDBNull("MaQR") ? null : reader.GetString("MaQR")
            };
        }
    }
}