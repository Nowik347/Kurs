using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Курс.Models;
using Курс.Windows;

namespace Курс
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User currentUser;
        bool editingUsers;

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

        public MainWindow(User user)
        {
            InitializeComponent();

            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                if (user != null)
                {
                    CurrentUserLabel.Header = $"{db.Roles.Where(u => u.RoleId == user.Role).FirstOrDefault().RoleName}: {user.Surname} {user.Name}";
                    MessageBox.Show($"{db.Roles.Where(u => u.RoleId == user.Role).FirstOrDefault().RoleName}: {user.Surname} {user.Name}. \r\t");
                }

                switch (user.Role)
                {
                    case 0:
                        AddNewRequestButton.Visibility = Visibility.Collapsed;
                        DeleteRequestButton.Visibility = Visibility.Collapsed;
                        EditUsersButton.Visibility = Visibility.Collapsed;
                        break;
                    case 1:
                        DeleteRequestButton.Visibility = Visibility.Collapsed;
                        EditUsersButton.Visibility = Visibility.Collapsed;
                        break;
                }

                currentUser = user;

                UpdateRequestTableSource();
            }

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

        private void UpdateRequestTableSource()
        {
            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                if (currentUser.Role != 1)
                {
                    RequestsList.ItemsSource = db.Requests.ToList();
                    return;
                }

                RequestsList.ItemsSource = db.Requests.Where(u => u.UserId == currentUser.UserId).ToList();
                return;
            }
        }

        private void UpdateUsersTableSource()
        {
            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                UsersList.ItemsSource = db.Users.ToList();
                return;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            this.Close();
        }

        private void UpdateTableButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateRequestTableSource();
        }

        private void DeleteRequestButton_Click(object sender, RoutedEventArgs e)
        {
            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                var request = (RequestsList.SelectedItem) as Request;

                if (request != null)
                {
                    if (MessageBox.Show($"Вы точно хотите удалить заявку №{request.RequestId}?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        db.Requests.Remove(request);
                        db.SaveChanges();
                        MessageBox.Show($"Заявка удаленна!");
                        RequestsList.ItemsSource = db.Requests.ToList();
                    }
                }
            }
        }

        private void OpenRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (RequestsList.SelectedItem != null)
            {
                new RequestWindow(currentUser, RequestsList.SelectedItem as Request).ShowDialog();
                UpdateTableButton_Click(sender, e);
            }
        }

        private void AddNewRequestButton_Click(object sender, RoutedEventArgs e)
        {
            new RequestWindow(currentUser).ShowDialog();
            UpdateTableButton_Click(sender, e);
        }

        private void RequestsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                new RequestWindow(currentUser, RequestsList.SelectedItem as Request).ShowDialog();
                UpdateTableButton_Click(sender, e);
            }
        }

        private void EditUsersButton_Click(object sender, RoutedEventArgs e)
        {
            if (editingUsers)
            {
                editingUsers = false;

                UsersList.Visibility = Visibility.Collapsed;
                ColumnHeadersPanel.Visibility = Visibility.Visible;
                RequestsList.Visibility = Visibility.Visible;

                using (ComputerRepairShopContext db = new ComputerRepairShopContext())
                {
                    foreach (User user in UsersList.Items)
                    {
                        db.Users.Update(user);
                    }

                    db.SaveChanges();
                }

                EditUsersButton.Header = "Настроить права пользователей";

                UpdateRequestTableSource();
            }
            else
            {
                editingUsers = true;

                UsersList.Visibility = Visibility.Visible;
                ColumnHeadersPanel.Visibility = Visibility.Collapsed;
                RequestsList.Visibility = Visibility.Collapsed;

                EditUsersButton.Header = "Сохранить изменения";

                UpdateUsersTableSource();
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Help.ShowHelp(null, @"help.chm");
        }
    }
}
