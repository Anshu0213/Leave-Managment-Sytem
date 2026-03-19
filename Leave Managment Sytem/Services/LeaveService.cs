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

            if (applyLeave.StartDate.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Cannot apply for backdated leave");

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

            // Check leave balance
            var balance = await _context.LeaveBalances.FirstOrDefaultAsync(b => b.UserId == applyLeave.UserId);
            if (balance == null)
                throw new Exception("Leave balance not found for user");

            var days = (applyLeave.EndDate.Date - applyLeave.StartDate.Date).Days + 1;

            switch (applyLeave.LeaveType)
            {
                case "Sick":
                    if (balance.Sick < days)
                        throw new Exception("Insufficient sick leave balance");
                    break;
                case "Casual":
                    if (balance.Casual < days)
                        throw new Exception("Insufficient casual leave balance");
                    break;
                case "Earned":
                    if (balance.Earned < days)
                        throw new Exception("Insufficient earned leave balance");
                    break;
            }

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

            if (!string.IsNullOrWhiteSpace(updateLeave.Comment))
            {
                leave.Reason = (leave.Reason ?? "") + "\nManagerComment: " + updateLeave.Comment;
            }

            // If approved, deduct balance
            if (updateLeave.Status == "Approved")
            {
                var balance = await _context.LeaveBalances.FirstOrDefaultAsync(b => b.UserId == leave.UserId);
                if (balance == null)
                    throw new Exception("Leave balance not found for user");

                var days = (DateTime.Parse(leave.EndDate).Date - DateTime.Parse(leave.StartDate).Date).Days + 1;

                switch (leave.LeaveType)
                {
                    case "Sick":
                        if (balance.Sick < days) throw new Exception("Insufficient sick leave balance");
                        balance.Sick -= days;
                        break;
                    case "Casual":
                        if (balance.Casual < days) throw new Exception("Insufficient casual leave balance");
                        balance.Casual -= days;
                        break;
                    case "Earned":
                        if (balance.Earned < days) throw new Exception("Insufficient earned leave balance");
                        balance.Earned -= days;
                        break;
                }

                _context.LeaveBalances.Update(balance);
            }

            _context.LeaveRequests.Update(leave);

            await _context.SaveChangesAsync();
        }
    }
}
