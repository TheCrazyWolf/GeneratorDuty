using GeneratorDuty.Models;
using Microsoft.EntityFrameworkCore;

namespace GeneratorDuty.Database;

public sealed class DutyContext : DbContext
{
    public DutyContext() => Database.MigrateAsync();
    
    public DbSet<MemberDuty> MemberDuties { get; set; }
    public DbSet<ScheduleProp> ScheduleProps { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = app.db");
    }
}