using Leave_Managment_Sytem.Models;
using Leave_Managment_Sytem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Leave_Managment_Sytem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveController : ControllerBase
    {
        private readonly ILogger<LeaveController> _logger;
        private readonly ILeaveService _leaveService;


        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [Authorize()]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveRequest request)
        {
            try
            {
                var leaveId = await _leaveService.ApplyLeaveAsync(request);

                return Ok(new
                {
                    message = "Leave applied successfully",
                    leaveId = leaveId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateLeaveStatusRequest request)
        {
            try
            {
                await _leaveService.UpdateLeaveStatusAsync(request);

                return Ok(new
                {
                    message = "Leave status updated successfully"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

}

