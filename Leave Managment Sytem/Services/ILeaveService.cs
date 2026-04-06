using Leave_Managment_Sytem.Models;

namespace Leave_Managment_Sytem.Services
{
    public interface ILeaveService
    {
        Task<List<LeaveResponse>> GetMyLeavesAsync();
        Task<int> ApplyLeaveAsync(ApplyLeaveRequest applyLeave);
        Task UpdateLeaveStatusAsync(UpdateLeaveStatusRequest updateLeave);
    }
}
