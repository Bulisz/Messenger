using Messenger.Backend.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Backend.DataBase;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
