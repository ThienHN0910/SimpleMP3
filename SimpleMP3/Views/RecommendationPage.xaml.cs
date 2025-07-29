using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using Models;
using SimpleMP3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleMP3.Views
{
    public partial class RecommendationPage : Page
    {
        private readonly RecommendationService _recommendationService;
        private readonly TrackService _trackService;

        private const string ModelPath = "MLModels/recommendation-model.zip";

        public RecommendationPage()
        {
            InitializeComponent();
            _recommendationService = App.AppHost.Services.GetRequiredService<RecommendationService>();
            _trackService = App.AppHost.Services.GetRequiredService<TrackService>();

            Loaded += (s, e) =>
            {
                if (App.CurrentUser == null)
                {
                    GuestPanel.Visibility = Visibility.Visible;
                    UserPanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    GuestPanel.Visibility = Visibility.Collapsed;
                    UserPanel.Visibility = Visibility.Visible;
                    UsernameText.Text = App.CurrentUser.Username;
                }

                RecommendationPage_Loaded(s, e);
            };
        }

        private async void RecommendationPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Bạn cần đăng nhập để xem gợi ý.");
                return;
            }

            try
            {
                // Tạo thư mục lưu model nếu chưa có
                Directory.CreateDirectory(Path.GetDirectoryName(ModelPath)!);

                // Train lại mô hình và lưu ra file
                await Task.Run(() =>
                {
                    _recommendationService.Train();
                    _recommendationService.SaveModel(ModelPath);
                });

                // Tải danh sách tất cả track
                var allTracks = await _trackService.GetAllTracksAsync();
                var allTrackIds = allTracks.Select(t => (uint)t.Id).ToList();

                // Gợi ý danh sách top track
                var recommendedTrackIds = await Task.Run(() =>
                    _recommendationService.RecommendTopTracks((uint)App.CurrentUser.Id, allTrackIds));

                // Lọc ra các bài hát được gợi ý
                var recommendedTracks = allTracks
                    .Where(t => recommendedTrackIds.Contains((uint)t.Id))
                    .ToList();

                // Tính điểm và gán vào ListView
                var displayData = recommendedTracks.Select(t => new RecommendationResult
                {
                    Title = t.Title,
                    ArtistName = t.Artist?.Name ?? "Unknown",
                    Score = _recommendationService.PredictScore((uint)App.CurrentUser.Id, (uint)t.Id)
                })
                .OrderByDescending(r => r.Score)
                .ToList();

                RecommendationListView.ItemsSource = displayData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi hiển thị gợi ý: " + ex.Message);
            }
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new HomePage(mainWindow.MainFrame));
        }

        private void Playlist_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new PlaylistPage());
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new HistoryPage());
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new LoginPage());
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new LoginPage());
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.MainFrame.Navigate(new RegisterPage());
        }
    }
}
