using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
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

        public LoginWindow()
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
