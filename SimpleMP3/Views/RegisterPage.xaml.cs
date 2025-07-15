using BusinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleMP3.Models;
using SimpleMP3.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleMP3.Views
{
    public partial class RegisterPage : Page
    {
        private readonly AuthService _authService;
        private TextBlock _passwordHint;
        private TextBlock _confirmPasswordHint;

        public RegisterPage()
        {
            InitializeComponent();
            _authService = App.AppHost.Services.GetRequiredService<AuthService>();

            AddPasswordPlaceholder(PasswordBox, "Mật khẩu", out _passwordHint);
            AddPasswordPlaceholder(ConfirmPasswordBox, "Xác nhận mật khẩu", out _confirmPasswordHint);
        }

        private void AddPasswordPlaceholder(PasswordBox box, string text, out TextBlock placeholder)
        {
            var parent = box.Parent as Grid;
            var localPlaceholder = new TextBlock
            {
                Text = text,
                Foreground = Brushes.Gray,
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false
            };
            parent.Children.Add(localPlaceholder);

            box.PasswordChanged += (s, e) =>
            {
                localPlaceholder.Visibility = string.IsNullOrEmpty(box.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            };

            placeholder = localPlaceholder;
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu không khớp.");
                return;
            }

            var success = await _authService.RegisterAsync(username, email, password);
            if (success)
            {
                MessageBox.Show("Đăng ký thành công!");
                NavigationService?.Navigate(new LoginPage());
            }
            else
            {
                MessageBox.Show("Tên người dùng hoặc email đã tồn tại.");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new LoginPage());
        }
    }
}
