using Messenger.Backend.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("No connectionString");
        builder.Services.AddDbContext <AppDbContext > (options => options.UseSqlServer(connectionString));

        // Add services to the container.
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }
}