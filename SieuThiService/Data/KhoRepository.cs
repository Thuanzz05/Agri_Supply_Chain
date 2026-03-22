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
    }
}