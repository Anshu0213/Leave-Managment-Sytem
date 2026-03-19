using System;
using System.Collections.Generic;

namespace Leave_Managment_Sytem.Models;

public partial class LeaveRequest
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string LeaveType { get; set; } = null!;

    public string StartDate { get; set; } = null!;

    public string EndDate { get; set; } = null!;

    public string? Reason { get; set; }

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
