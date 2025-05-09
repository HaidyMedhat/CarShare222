using CarShare.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShare.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _userService.GetPendingUsersAsync();
            return HandleResult(users);
        }

        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(Guid userId)
        {
            await _userService.ApproveUserAsync(userId);
            return NoContent();
        }

        [HttpPost("reject-user/{userId}")]
        public async Task<IActionResult> RejectUser(Guid userId)
        {
            await _userService.RejectUserAsync(userId);
            return NoContent();
        }
    }
}
