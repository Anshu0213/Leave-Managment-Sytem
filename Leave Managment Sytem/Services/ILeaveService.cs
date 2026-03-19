using Leave_Managment_Sytem.Models;

namespace Leave_Managment_Sytem.Services
{
    public interface ILeaveService
    {
        Task<int> ApplyLeaveAsync(ApplyLeaveRequest applyLeave);
        Task UpdateLeaveStatusAsync(UpdateLeaveStatusRequest updateLeave);
    }
}
