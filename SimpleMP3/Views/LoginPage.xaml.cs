using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleMP3.Views
{
    public partial class LoginPage : Page
    {
        private readonly AuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = App.AppHost.Services.GetRequiredService<AuthService>();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            var user = await _authService.LoginAsync(username, password);
            if (user != null)
            {
                App.CurrentUser = user;
                MessageBox.Show("Đăng nhập thành công!");

                // Lấy MainFrame từ MainWindow và điều hướng về HomePage
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new HomePage(mainWindow.MainFrame));
                }
            }
            else
            {
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new HomePage(mainWindow.MainFrame));
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new RegisterPage());
            }
        }
    }
}
