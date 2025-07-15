using BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repos.Interfaces;
using Repos.Repos;
using SimpleMP3.DataAccess;
using SimpleMP3.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SimpleMP3;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static User? CurrentUser { get; set; }
    public static IHost AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsetting.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // EF DbContext
                services.AddDbContext<MusicPlayerDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                // Add Repositories & Services
                services.AddScoped<ITrackRepository, TrackRepository>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IPlaylistRepository, PlaylistRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<AuthService>();
                services.AddScoped<TrackService>();
                services.AddScoped<PlaylistService>();
                // thêm các service khác
            })
            .Build();
    }
}

