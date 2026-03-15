using Microsoft.Data.SqlClient;
using DaiLyService.Models.DTOs;
using System.Data;

namespace DaiLyService.Data
{
    public class TonKhoRepository : ITonKhoRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<TonKhoRepository> _logger;

        public TonKhoRepository(IConfiguration config, ILogger<TonKhoRepository> logger)
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
                    SELECT tk.MaTonKho, tk.MaKho, tk.MaLoNongSan, tk.SoLuongTon, tk.SoLuongToiThieu, 
                           tk.SoLuongToiDa, tk.NgayCapNhat, tk.TrangThai, tk.GhiChu,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLoNongSan = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    ORDER BY tk.NgayCapNhat DESC", conn);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inventory records from database", list.Count);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting all inventory records");
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
                    SELECT tk.MaTonKho, tk.MaKho, tk.MaLoNongSan, tk.SoLuongTon, tk.SoLuongToiThieu, 
                           tk.SoLuongToiDa, tk.NgayCapNhat, tk.TrangThai, tk.GhiChu,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLoNongSan = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE tk.MaKho = @MaKho
                    ORDER BY tk.NgayCapNhat DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaKho", maKho);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inventory records for warehouse {WarehouseId}", list.Count, maKho);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inventory records for warehouse {WarehouseId}", maKho);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<TonKhoDTO> GetByDaiLy(int maDaiLy)
        {
            var list = new List<TonKhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaTonKho, tk.MaKho, tk.MaLoNongSan, tk.SoLuongTon, tk.SoLuongToiThieu, 
                           tk.SoLuongToiDa, tk.NgayCapNhat, tk.TrangThai, tk.GhiChu,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLoNongSan = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE k.MaDaiLy = @MaDaiLy
                    ORDER BY tk.NgayCapNhat DESC", conn);
                
                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inventory records for distributor {DistributorId}", list.Count, maDaiLy);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inventory records for distributor {DistributorId}", maDaiLy);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public TonKhoDTO? GetById(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaTonKho, tk.MaKho, tk.MaLoNongSan, tk.SoLuongTon, tk.SoLuongToiThieu, 
                           tk.SoLuongToiDa, tk.NgayCapNhat, tk.TrangThai, tk.GhiChu,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLoNongSan = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE tk.MaTonKho = @MaTonKho", conn);
                
                cmd.Parameters.AddWithValue("@MaTonKho", id);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Inventory record with ID {InventoryId} not found", id);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inventory record with ID {InventoryId}", id);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public TonKhoDTO? GetByLoNongSan(int maLoNongSan)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaTonKho, tk.MaKho, tk.MaLoNongSan, tk.SoLuongTon, tk.SoLuongToiThieu, 
                           tk.SoLuongToiDa, tk.NgayCapNhat, tk.TrangThai, tk.GhiChu,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLoNongSan = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE tk.MaLoNongSan = @MaLoNongSan", conn);
                
                cmd.Parameters.AddWithValue("@MaLoNongSan", maLoNongSan);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    _logger.LogWarning("Inventory record for crop lot {CropLotId} not found", maLoNongSan);
                    return null;
                }
                return MapToDTO(reader);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inventory record for crop lot {CropLotId}", maLoNongSan);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
        }

        public List<TonKhoDTO> GetByTrangThai(string trangThai)
        {
            var list = new List<TonKhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaTonKho, tk.MaKho, tk.MaLoNongSan, tk.SoLuongTon, tk.SoLuongToiThieu, 
                           tk.SoLuongToiDa, tk.NgayCapNhat, tk.TrangThai, tk.GhiChu,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLoNongSan = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE tk.TrangThai = @TrangThai
                    ORDER BY tk.NgayCapNhat DESC", conn);
                
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} inventory records with status {Status}", list.Count, trangThai);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting inventory records with status {Status}", trangThai);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public List<TonKhoDTO> GetSapHetHang(int maDaiLy)
        {
            var list = new List<TonKhoDTO>();
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    SELECT tk.MaTonKho, tk.MaKho, tk.MaLoNongSan, tk.SoLuongTon, tk.SoLuongToiThieu, 
                           tk.SoLuongToiDa, tk.NgayCapNhat, tk.TrangThai, tk.GhiChu,
                           k.TenKho, sp.TenSanPham, sp.DonViTinh, ln.MaQR
                    FROM TonKho tk
                    LEFT JOIN Kho k ON tk.MaKho = k.MaKho
                    LEFT JOIN LoNongSan ln ON tk.MaLoNongSan = ln.MaLo
                    LEFT JOIN SanPham sp ON ln.MaSanPham = sp.MaSanPham
                    WHERE k.MaDaiLy = @MaDaiLy AND tk.SoLuongTon <= tk.SoLuongToiThieu
                    ORDER BY tk.SoLuongTon ASC", conn);
                
                cmd.Parameters.AddWithValue("@MaDaiLy", maDaiLy);

                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapToDTO(reader));
                }
                _logger.LogInformation("Retrieved {Count} low stock items for distributor {DistributorId}", list.Count, maDaiLy);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while getting low stock items for distributor {DistributorId}", maDaiLy);
                throw new Exception("Lỗi truy vấn cơ sở dữ liệu", ex);
            }
            return list;
        }

        public int Create(TonKhoCreateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    INSERT INTO TonKho (MaKho, MaLoNongSan, SoLuongTon, SoLuongToiThieu, SoLuongToiDa, 
                                       NgayCapNhat, TrangThai, GhiChu) 
                    OUTPUT INSERTED.MaTonKho 
                    VALUES (@MaKho, @MaLoNongSan, @SoLuongTon, @SoLuongToiThieu, @SoLuongToiDa, 
                            @NgayCapNhat, @TrangThai, @GhiChu)", conn);

                cmd.Parameters.AddWithValue("@MaKho", dto.MaKho);
                cmd.Parameters.AddWithValue("@MaLoNongSan", dto.MaLoNongSan);
                cmd.Parameters.AddWithValue("@SoLuongTon", dto.SoLuongTon);
                cmd.Parameters.AddWithValue("@SoLuongToiThieu", dto.SoLuongToiThieu);
                cmd.Parameters.AddWithValue("@SoLuongToiDa", dto.SoLuongToiDa);
                cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);
                cmd.Parameters.AddWithValue("@TrangThai", dto.SoLuongTon <= dto.SoLuongToiThieu ? "sap_het" : "binh_thuong");
                cmd.Parameters.AddWithValue("@GhiChu", (object?)dto.GhiChu ?? DBNull.Value);

                conn.Open();
                var maTonKho = (int)cmd.ExecuteScalar();
                
                _logger.LogInformation("Created new inventory record with ID {InventoryId}", maTonKho);
                return maTonKho;
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

        public bool Update(int id, TonKhoUpdateDTO dto)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE TonKho 
                    SET SoLuongTon = @SoLuongTon, SoLuongToiThieu = @SoLuongToiThieu, 
                        SoLuongToiDa = @SoLuongToiDa, TrangThai = @TrangThai, 
                        GhiChu = @GhiChu, NgayCapNhat = @NgayCapNhat 
                    WHERE MaTonKho = @MaTonKho", conn);

                cmd.Parameters.AddWithValue("@MaTonKho", id);
                cmd.Parameters.AddWithValue("@SoLuongTon", dto.SoLuongTon);
                cmd.Parameters.AddWithValue("@SoLuongToiThieu", dto.SoLuongToiThieu);
                cmd.Parameters.AddWithValue("@SoLuongToiDa", dto.SoLuongToiDa);
                cmd.Parameters.AddWithValue("@TrangThai", dto.TrangThai);
                cmd.Parameters.AddWithValue("@GhiChu", (object?)dto.GhiChu ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated inventory record with ID {InventoryId}", id);
                    return true;
                }
                
                _logger.LogWarning("No inventory record found with ID {InventoryId} to update", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating inventory record with ID {InventoryId}", id);
                throw new Exception("Lỗi cập nhật tồn kho trong cơ sở dữ liệu", ex);
            }
        }

        public bool UpdateSoLuong(int id, decimal soLuongMoi)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand(@"
                    UPDATE TonKho 
                    SET SoLuongTon = @SoLuongMoi, NgayCapNhat = @NgayCapNhat,
                        TrangThai = CASE 
                            WHEN @SoLuongMoi <= SoLuongToiThieu THEN 'sap_het'
                            WHEN @SoLuongMoi = 0 THEN 'het_hang'
                            ELSE 'binh_thuong'
                        END
                    WHERE MaTonKho = @MaTonKho", conn);

                cmd.Parameters.AddWithValue("@MaTonKho", id);
                cmd.Parameters.AddWithValue("@SoLuongMoi", soLuongMoi);
                cmd.Parameters.AddWithValue("@NgayCapNhat", DateTime.Now);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Updated quantity for inventory record {InventoryId} to {NewQuantity}", id, soLuongMoi);
                    return true;
                }
                
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while updating quantity for inventory record {InventoryId}", id);
                throw new Exception("Lỗi cập nhật số lượng tồn kho", ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("DELETE FROM TonKho WHERE MaTonKho = @MaTonKho", conn);
                cmd.Parameters.AddWithValue("@MaTonKho", id);

                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Deleted inventory record with ID {InventoryId}", id);
                    return true;
                }
                
                _logger.LogWarning("No inventory record found with ID {InventoryId} to delete", id);
                return false;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error occurred while deleting inventory record with ID {InventoryId}", id);
                if (ex.Number == 547)
                    throw new Exception("Không thể xóa tồn kho này vì đang có dữ liệu liên quan", ex);
                throw new Exception("Lỗi xóa tồn kho trong cơ sở dữ liệu", ex);
            }
        }

        private static TonKhoDTO MapToDTO(SqlDataReader reader)
        {
            return new TonKhoDTO
            {
                MaTonKho = reader.GetInt32("MaTonKho"),
                MaKho = reader.GetInt32("MaKho"),
                MaLoNongSan = reader.GetInt32("MaLoNongSan"),
                SoLuongTon = reader.GetDecimal("SoLuongTon"),
                SoLuongToiThieu = reader.GetDecimal("SoLuongToiThieu"),
                SoLuongToiDa = reader.GetDecimal("SoLuongToiDa"),
                NgayCapNhat = reader.GetDateTime("NgayCapNhat"),
                TrangThai = reader.GetString("TrangThai"),
                GhiChu = reader.IsDBNull("GhiChu") ? null : reader.GetString("GhiChu"),
                TenKho = reader.IsDBNull("TenKho") ? null : reader.GetString("TenKho"),
                TenSanPham = reader.IsDBNull("TenSanPham") ? null : reader.GetString("TenSanPham"),
                DonViTinh = reader.IsDBNull("DonViTinh") ? null : reader.GetString("DonViTinh"),
                MaQR = reader.IsDBNull("MaQR") ? null : reader.GetString("MaQR")
            };
        }
    }
}