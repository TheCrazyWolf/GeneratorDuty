﻿using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Database;

public sealed class DutyContext : DbContext
{
    public DutyContext() => Database.MigrateAsync();
    public DbSet<MemberDuty> MemberDuties { get; set; }
    public DbSet<ScheduleProp> ScheduleProps { get; set; }
    public DbSet<LogDutyMember> LogDutyMembers { get; set; }
    public DbSet<LogDutyMemberPriority> LogDutyMemberPriorities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = app.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogDutyMember>()
            .HasOne(ldm => ldm.Duty)
            .WithMany()
            .HasForeignKey(ldm => ldm.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<LogDutyMemberPriority>()
            .HasOne(ldm => ldm.Duty)
            .WithMany()
            .HasForeignKey(ldm => ldm.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}