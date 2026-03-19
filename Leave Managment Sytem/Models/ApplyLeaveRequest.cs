namespace Leave_Managment_Sytem.Models
{
    public class ApplyLeaveRequest
    {
        public int UserId { get; set; }
        public string LeaveType { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}
