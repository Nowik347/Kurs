using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Курс.Infrastructure;
using Курс.Models;
using Курс.Windows;

namespace Курс
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        bool verify = true;
        int verifyCheck = 0;
        public LoginWindow()
        {
            InitializeComponent();

            // REMOVE THIS
            //using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            //{
            //    User? user = db.Users.Where(u => u.Login == "Admin" && u.Password == "admin").FirstOrDefault() as User;

            //    new MainWindow(user).Show();
            //    this.Close();
            //}
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                // проверка, если есть каптча
                if (CaptchaImage.Visibility == Visibility.Visible)
                {
                    if (CaptchaImage.Text == CaptchaBox.Text)
                    {
                        verify = true;
                    }
                }

                User? user = db.Users.Where(u => u.Login == LoginBox.Text && u.Password == PasswordBox.Password).FirstOrDefault() as User;

                // admin
                if (user != null && verify)
                {
                    new MainWindow(user).Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неуспешная авторизация");
                    verifyCheck += 1;
                    // captcha view
                    CaptchaPanel.Visibility = Visibility.Visible;
                    CaptchaImage.Text = CaptchaBuilder.Refresh();
                    verify = false;
                    if (verifyCheck > 1)
                    {
                        disableButton();
                        CaptchaImage.Text = CaptchaBuilder.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Асинхронное выключение кнопки на 10 сек.
        /// </summary>
        async void disableButton()
        {
            LoginButton.IsEnabled = false;
            await Task.Delay(TimeSpan.FromSeconds(10));
            LoginButton.IsEnabled = true;
        }

        private void CaptchaBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CaptchaBox.Opacity = 1;
            CaptchaBox.Text = "";
        }

        private void CaptchaBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CaptchaBox.Text == "")
            {
                CaptchaBox.Opacity = 0.5;
                CaptchaBox.Text = "Пройдите CAPTCHA";
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            new RegistrationWindow().Show();
            this.Close();
        }
    }
}
