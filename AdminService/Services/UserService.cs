using AdminService.Data;

namespace AdminService.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        public (bool success, string message, object? data, int total) GetAllUsers(string? loaiNguoiDung)
        {
            try
            {
                var result = new List<object>();

                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "nongdan")
                {
                    result.AddRange(_repository.GetAllNongDan());
                }

                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "daily")
                {
                    result.AddRange(_repository.GetAllDaiLy());
                }

                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "sieuthi")
                {
                    result.AddRange(_repository.GetAllSieuThi());
                }

                return (true, "Lấy danh sách người dùng thành công", result, result.Count);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null, 0);
            }
        }

        public (bool success, string message, object? data) GetNongDanDetail(int id)
        {
            try
            {
                var result = _repository.GetNongDanDetail(id);
                if (result == null)
                {
                    return (false, "Không tìm thấy nông dân", null);
                }

                return (true, "Lấy thông tin nông dân thành công", result);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public (bool success, string message, object? data) GetDaiLyDetail(int id)
        {
            try
            {
                var result = _repository.GetDaiLyDetail(id);
                if (result == null)
                {
                    return (false, "Không tìm thấy đại lý", null);
                }

                return (true, "Lấy thông tin đại lý thành công", result);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public (bool success, string message, object? data) GetSieuThiDetail(int id)
        {
            try
            {
                var result = _repository.GetSieuThiDetail(id);
                if (result == null)
                {
                    return (false, "Không tìm thấy siêu thị", null);
                }

                return (true, "Lấy thông tin siêu thị thành công", result);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }

        public (bool success, string message, object? data) SearchUsers(string keyword, string? loaiNguoiDung)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    return (false, "Từ khóa tìm kiếm không được để trống", null);
                }

                var result = new List<object>();

                if (string.IsNullOrEmpty(loaiNguoiDung) || loaiNguoiDung == "nongdan")
                {
                    result.AddRange(_repository.SearchNongDan(keyword));
                }

                return (true, "Tìm kiếm thành công", result);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }
    }
}
