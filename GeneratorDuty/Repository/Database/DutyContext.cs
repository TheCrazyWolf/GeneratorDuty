using GeneratorDuty.Models.Duty;
using GeneratorDuty.Models.Properties;
using GeneratorDuty.Models.Schedule;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Repository.Database;

public sealed class DutyContext : DbContext
{
    public DbSet<MemberDuty> MemberDuties { get; set; }
    public DbSet<ScheduleProp> ScheduleProps { get; set; }
    public DbSet<LogDutyMember> LogDutyMembers { get; set; }
    public DbSet<LogDutyMemberPriority> LogDutyMemberPriorities { get; set; }
    public DbSet<ScheduleHistory> History { get; set; }
    public DbSet<ScheduleCustomRules> ScheduleCustomRules { get; set; }

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