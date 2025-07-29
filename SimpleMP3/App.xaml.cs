using BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repos.Interfaces;
using Repos.Repos;
using SimpleMP3.DataAccess;
using SimpleMP3.Models;
using SimpleMP3.Services;
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
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // EF DbContext
                services.AddDbContext<MusicPlayerDbContext>(options =>
                    options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                // Add Repositories & Services
                services.AddScoped<MainWindow>();
                services.AddScoped<ITrackRepository, TrackRepository>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IPlaylistRepository, PlaylistRepository>();
                services.AddScoped<IPlayHistoryRepository, PlayHistoryRepository>();
                services.AddScoped<ITrainingRepo, TrainingRepo>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<AuthService>();
                services.AddScoped<TrackService>();
                services.AddScoped<PlaylistService>();
                services.AddScoped<PlayHistoryService>();
                services.AddScoped<UserService>();
                services.AddScoped<RecommendationService>();
                services.AddSingleton<MusicPlayerService>();
                // thêm các service khác
            })
            .Build();
    }
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        await AppHost.StartAsync(); 

        var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

}

