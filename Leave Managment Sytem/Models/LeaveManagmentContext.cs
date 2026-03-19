using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Leave_Managment_Sytem.Models;

public partial class LeaveManagmentContext : DbContext
{
    public LeaveManagmentContext()
    {
    }

    public LeaveManagmentContext(DbContextOptions<LeaveManagmentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LeaveBalance> LeaveBalances { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=LeaveManagment.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.ToTable("Leave Balance");

            entity.HasIndex(e => e.Id, "IX_Leave Balance_Id").IsUnique();

            entity.HasOne(d => d.User).WithMany(p => p.LeaveBalances)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.ToTable("LeaveRequest");

            entity.HasIndex(e => e.Id, "IX_LeaveRequest_Id").IsUnique();

            entity.Property(e => e.Status).HasDefaultValue("Pending");

            entity.HasOne(d => d.User).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Id, "IX_User_Id").IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
