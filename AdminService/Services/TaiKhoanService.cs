using AdminService.Data;

namespace AdminService.Services
{
    public class TaiKhoanService
    {
        private readonly TaiKhoanRepository _repository;

        public TaiKhoanService(TaiKhoanRepository repository)
        {
            _repository = repository;
        }

        public (bool success, string message, object? data, int total) GetAll(string? loaiTaiKhoan)
        {
            try
            {
                var result = _repository.GetAll(loaiTaiKhoan);
                return (true, "Lấy danh sách tài khoản thành công", result, result.Count);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null, 0);
            }
        }

        public (bool success, string message) ChangePassword(int id, string matKhauMoi)
        {
            try
            {
                if (string.IsNullOrEmpty(matKhauMoi))
                {
                    return (false, "Mật khẩu mới không được để trống");
                }

                var result = _repository.ChangePassword(id, matKhauMoi);
                if (!result)
                {
                    return (false, "Không tìm thấy tài khoản");
                }

                return (true, "Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public (bool success, string message) ToggleStatus(int id)
        {
            try
            {
                var result = _repository.ToggleStatus(id);
                if (!result)
                {
                    return (false, "Không tìm thấy tài khoản");
                }

                return (true, "Thay đổi trạng thái tài khoản thành công");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        public (bool success, string message) Delete(int id)
        {
            try
            {
                var loaiTaiKhoan = _repository.GetLoaiTaiKhoan(id);

                if (string.IsNullOrEmpty(loaiTaiKhoan))
                {
                    return (false, "Không tìm thấy tài khoản");
                }

                // Xóa dữ liệu liên quan theo loại tài khoản
                if (loaiTaiKhoan == "nongdan")
                {
                    var maNongDan = _repository.GetMaNongDan(id);
                    if (maNongDan.HasValue)
                    {
                        _repository.DeleteNongDanRelatedData(maNongDan.Value);
                    }
                }
                else if (loaiTaiKhoan == "daily")
                {
                    var maDaiLy = _repository.GetMaDaiLy(id);
                    if (maDaiLy.HasValue)
                    {
                        var tenDangNhap = _repository.GetTenDangNhap(id) ?? "";
                        _repository.DeleteDaiLyRelatedData(maDaiLy.Value, tenDangNhap);
                    }
                }
                else if (loaiTaiKhoan == "sieuthi")
                {
                    var maSieuThi = _repository.GetMaSieuThi(id);
                    if (maSieuThi.HasValue)
                    {
                        _repository.DeleteSieuThiRelatedData(maSieuThi.Value);
                    }
                }
                else if (loaiTaiKhoan == "admin")
                {
                    _repository.DeleteAdmin(id);
                }

                // Cuối cùng xóa tài khoản
                var result = _repository.DeleteTaiKhoan(id);
                if (!result)
                {
                    return (false, "Không thể xóa tài khoản");
                }

                return (true, "Xóa tài khoản thành công");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
    }
}
