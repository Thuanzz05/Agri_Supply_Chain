using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace NongDanService.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TestController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Test kết nối database
        /// </summary>
        /// <returns>Kết quả test</returns>
        [HttpGet("connection")]
        public IActionResult TestConnection()
        {
            try
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connectionString);
                conn.Open();
                
                using var cmd = new SqlCommand("SELECT COUNT(*) FROM TaiKhoan", conn);
                var count = cmd.ExecuteScalar();
                
                return Ok(new
                {
                    success = true,
                    message = "Kết nối database thành công",
                    data = new { 
                        server = conn.DataSource,
                        database = conn.Database,
                        taiKhoanCount = count
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi kết nối database: " + ex.Message,
                    connectionString = _config.GetConnectionString("DefaultConnection")?.Replace("Password=", "Password=***")
                });
            }
        }
    }
}