using Microsoft.EntityFrameworkCore;
using Outlay.Models;

namespace Outlay.Db;

public class OutlayInMemoryContext : DbContext
{
    public OutlayInMemoryContext(DbContextOptions<OutlayInMemoryContext> options)
        : base(options)
    {
    }

    public DbSet<MccInfo> MccInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MccInfo>().HasKey(x => x.Mcc);
    }
}