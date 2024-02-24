using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Курс.Infrastructure;

namespace Курс.Windows
{
    /// <summary>
    /// Логика взаимодействия для CAPTCHAWindow.xaml
    /// </summary>
    public partial class CAPTCHAWindow : Window
    {
        public CAPTCHAWindow()
        {
            InitializeComponent();

            CaptchaImage.Text = CaptchaBuilder.Refresh();
        }

        private void CheckCAPTCHAButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaImage.Text == CaptchaBox.Text)
            {
                this.DialogResult = true;;
                this.Close();
                return;
            }

            MessageBox.Show("Неуспешная регистрация");
            this.DialogResult = false;
            this.Close();
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
    }
}
