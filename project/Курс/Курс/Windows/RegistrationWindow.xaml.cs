using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Курс.Models;

namespace Курс.Windows
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        #region Win32 API Stuff

        // Define the Win32 API methods we are going to use
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, Int32 wPosition, Int32 wFlags, Int32 wIDNewItem, string lpNewItem);

        /// Define our Constants we will use
        public const Int32 WM_SYSCOMMAND = 0x112;
        public const Int32 MF_SEPARATOR = 0x800;
        public const Int32 MF_BYPOSITION = 0x400;
        public const Int32 MF_STRING = 0x0;

        #endregion

        // The constants we'll use to identify our custom system menu items
        public const Int32 _AboutSysMenuID = 1001;

        public IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Check if a System Command has been executed
            if (msg == WM_SYSCOMMAND)
            {
                if (wParam.ToInt32() == _AboutSysMenuID)
                {
                    System.Windows.Forms.Help.ShowHelp(null, @"help.chm");
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        public RegistrationWindow()
        {
            InitializeComponent(); 
            
            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /// Get the Handle for the Forms System Menu
            IntPtr systemMenuHandle = GetSystemMenu(this.Handle, false);

            /// Create our new System Menu items just before the Close menu item
            InsertMenu(systemMenuHandle, 7, MF_BYPOSITION, _AboutSysMenuID, "О программе...");

            // Attach our WndProc handler to this Window
            HwndSource source = HwndSource.FromHwnd(this.Handle);
            source.AddHook(new HwndSourceHook(WndProc));
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
                    MessageBox.Show(errors.ToString(), "Ошибка!");
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

                    MessageBox.Show("Успешная регистрация!", "Регистрация");

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
