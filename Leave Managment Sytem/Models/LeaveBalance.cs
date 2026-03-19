using System;
using System.Collections.Generic;

namespace Leave_Managment_Sytem.Models;

public partial class LeaveBalance
{
    public int Id { get; set; }

    public int Sick { get; set; }

    public int Casual { get; set; }

    public int Earned { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
