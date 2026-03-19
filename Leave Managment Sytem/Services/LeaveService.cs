using Leave_Managment_Sytem.Models;
using Microsoft.EntityFrameworkCore;

namespace Leave_Managment_Sytem.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly LeaveManagmentContext _context;

        public LeaveService(LeaveManagmentContext context)
        {
            _context = context;
        }

        public async Task<int> ApplyLeaveAsync(ApplyLeaveRequest applyLeave)
        {
            if (applyLeave.StartDate > applyLeave.EndDate)
                throw new ArgumentException("Start date cannot be after end date");

            var validTypes = new[] { "Sick", "Casual", "Earned" };
            if (!validTypes.Contains(applyLeave.LeaveType))
                throw new ArgumentException("Invalid leave type");

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == applyLeave.UserId);

            if (!userExists)
                throw new Exception("User not found");

            var hasOverlap = await _context.LeaveRequests.AnyAsync(l =>
                l.UserId == applyLeave.UserId &&
                l.Status != "Rejected" &&
                applyLeave.StartDate <= DateTime.Parse(l.EndDate) &&
                applyLeave.EndDate >= DateTime.Parse(l.StartDate)
            );

            if (hasOverlap)
                throw new Exception("Leave dates overlap with existing leave");

            var leave = new LeaveRequest
            {
                UserId = applyLeave.UserId,
                LeaveType = applyLeave.LeaveType,
                StartDate = applyLeave.StartDate.ToString("yyyy-MM-dd"),
                EndDate = applyLeave.EndDate.ToString("yyyy-MM-dd"),
                Status = "Pending",
                Reason = applyLeave.Reason
            };

            await _context.LeaveRequests.AddAsync(leave);

            await _context.SaveChangesAsync();

            return leave.Id;
        }

        public async Task UpdateLeaveStatusAsync(UpdateLeaveStatusRequest updateLeave)
        {
            var validStatus = new[] { "Approved", "Rejected" };
            if (!validStatus.Contains(updateLeave.Status))
                throw new ArgumentException("Invalid status");

            var leave = await _context.LeaveRequests
                .FirstOrDefaultAsync(l => l.Id == updateLeave.LeaveId);

            if (leave == null)
                throw new Exception("Leave request not found");

            if (leave.Status != "Pending")
                throw new Exception("Only pending requests can be updated");

            leave.Status = updateLeave.Status;

            _context.LeaveRequests.Update(leave);

            await _context.SaveChangesAsync();
        }
    }
}
