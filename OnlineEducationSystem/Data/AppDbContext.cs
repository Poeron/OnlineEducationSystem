using Microsoft.EntityFrameworkCore;
using OnlineEducationSystem.Models;

namespace OnlineEducationSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}
