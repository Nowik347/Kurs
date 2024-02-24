using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Курс.Models;

namespace Курс.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private void ConfirmRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(NameBox.Text))
                errors.AppendLine("Укажите вашу имя");
            if (string.IsNullOrWhiteSpace(EmailBox.Text))
                errors.AppendLine("Укажите вашу электронную почту");
            if (string.IsNullOrWhiteSpace(LoginBox.Text))
                errors.AppendLine("Укажите ваш логин");
            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                errors.AppendLine("Укажите ваш пароль");

            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                User? checkUser = db.Users.Where(u => u.Email == EmailBox.Text).FirstOrDefault() as User;

                if (checkUser != null)
                    errors.AppendLine("Указанный адрес электронной почты уже используется");

                checkUser = db.Users.Where(u => u.Login == LoginBox.Text).FirstOrDefault() as User;

                if (checkUser != null)
                    errors.AppendLine("Указанный логин уже используется");

                //
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }

                CAPTCHAWindow captchaWindow = new CAPTCHAWindow();

                captchaWindow.ShowDialog();
                if (captchaWindow.DialogResult != true)
                    return;

                try
                {
                    User user = new User()
                    {
                        Name = NameBox.Text,
                        Surname = SurnameBox.Text,
                        Email = EmailBox.Text,
                        Login = LoginBox.Text,
                        Password = PasswordBox.Password,
                        Role = 1
                    };

                    db.Users.Add(user);
                    db.SaveChanges();

                    MessageBox.Show("Успешная регистрация!");

                    new LoginWindow().Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString());
                }
            }

        }
    }
}
