namespace Leave_Managment_Sytem.Models
{
    public class LeaveResponse
    {
        public int Id { get; set; }
        public string LeaveType { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public string? Reason { get; set; }
        public string Status { get; set; } = null!;
    }
}