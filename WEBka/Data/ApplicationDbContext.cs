using Microsoft.EntityFrameworkCore;
using WEBka.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<AnalysisResult> AnalysisResults { get; set; }
}
