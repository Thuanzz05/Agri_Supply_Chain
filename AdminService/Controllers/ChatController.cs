using AdminService.Data;
using AdminService.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AdminService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ChatRepository _chatRepository;

        public ChatController(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        // GET: api/chat/conversations?maNguoi=1&loaiNguoi=nongdan
        [HttpGet("conversations")]
        public async Task<ActionResult<List<CuocTroChuyenDTO>>> GetConversations(
            [FromQuery] int maNguoi,
            [FromQuery] string loaiNguoi)
        {
            try
            {
                var conversations = await _chatRepository.GetConversationsAsync(maNguoi, loaiNguoi);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách cuộc trò chuyện: {ex.Message}");
            }
        }

        // GET: api/chat/conversations/1/messages
        [HttpGet("conversations/{id}/messages")]
        public async Task<ActionResult<List<TinNhanDTO>>> GetMessages(int id)
        {
            try
            {
                var messages = await _chatRepository.GetMessagesAsync(id);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy tin nhắn: {ex.Message}");
            }
        }

        // POST: api/chat/messages
        [HttpPost("messages")]
        public async Task<ActionResult<TinNhanDTO>> SendMessage(
            [FromQuery] int maNguoiGui,
            [FromQuery] string loaiNguoiGui,
            [FromBody] GuiTinNhanRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NoiDung))
                {
                    return BadRequest("Nội dung tin nhắn không được để trống");
                }

                var message = await _chatRepository.SendMessageAsync(maNguoiGui, loaiNguoiGui, request);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi gửi tin nhắn: {ex.Message}");
            }
        }

        // PUT: api/chat/messages/read
        [HttpPut("messages/read")]
        public async Task<IActionResult> MarkAsRead(
            [FromQuery] int maNguoi,
            [FromQuery] string loaiNguoi,
            [FromBody] DanhDauDaDocRequest request)
        {
            try
            {
                await _chatRepository.MarkAsReadAsync(request.MaCuocTroChuyen, maNguoi, loaiNguoi);
                return Ok(new { message = "Đã đánh dấu đã đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi đánh dấu đã đọc: {ex.Message}");
            }
        }

        // GET: api/chat/users?loaiNguoi=daily
        [HttpGet("users")]
        public async Task<ActionResult<List<UserListDTO>>> GetAvailableUsers(
            [FromQuery] string loaiNguoi)
        {
            try
            {
                var users = await _chatRepository.GetAvailableUsersAsync(loaiNguoi);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi lấy danh sách người dùng: {ex.Message}");
            }
        }

        // DELETE: api/chat/conversations/1
        [HttpDelete("conversations/{id}")]
        public async Task<IActionResult> DeleteConversation(int id)
        {
            try
            {
                await _chatRepository.DeleteConversationAsync(id);
                return Ok(new { message = "Đã xóa cuộc trò chuyện" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi xóa cuộc trò chuyện: {ex.Message}");
            }
        }

        // GET: api/chat/unread-count?maNguoi=1&loaiNguoi=nongdan
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount(
            [FromQuery] int maNguoi,
            [FromQuery] string loaiNguoi)
        {
            try
            {
                var count = await _chatRepository.GetUnreadCountAsync(maNguoi, loaiNguoi);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi đếm tin nhắn chưa đọc: {ex.Message}");
            }
        }
    }
}
