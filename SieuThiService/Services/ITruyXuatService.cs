using SieuThiService.Models.DTOs;

namespace SieuThiService.Services
{
    public interface ITruyXuatService
    {
        TruyXuatResultDTO? TraceProductByQR(string maQR);
    }
}
