using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Курс.Models;

namespace Курс.Windows
{
    /// <summary>
    /// Логика взаимодействия для RequestWindow.xaml
    /// </summary>
    public partial class RequestWindow : Window
    {
        User currentUser;
        Request currentRequest;

        bool editingRequest = false;

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

        public RequestWindow(User user)
        {
            InitializeComponent();

            Title = "Добавление заявки";

            currentUser = user;

            DatePanel.Visibility = Visibility.Collapsed;
            LastDatePanel.Visibility = Visibility.Collapsed;
            StatusPanel.Visibility = Visibility.Collapsed;

            Height = 300;
            MaxHeight = Height;
            MinHeight = Height;

            currentRequest = new Request();

            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                StatusBox.ItemsSource = db.States.Select(u => u.StatusName).ToList();
            }

            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        public RequestWindow(User user, Request request)
        {
            InitializeComponent();

            Title = "Просмотр заявки";

            currentUser = user;

            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                StatusBox.ItemsSource = db.States.Select(u => u.StatusName).ToList();
            }

            StatusBox.SelectedIndex = request.Status;
            editingRequest = true;

            TroubleDevicesBox.Text = request.TroubleDevices;
            ProblemDescriptionBox.Text = request.ProblemDescription;

            CreationDateLabel.Content = request.CreationDate;
            LastChangeDateLabel.Content = request.LastChangeDate;

            currentRequest = request;

            switch (user.Role)
            {
                case 0:
                    TroubleDevicesBox.IsEnabled = false;
                    ProblemDescriptionBox.IsEnabled = false;
                    break;
                case 1:
                    StatusBox.IsEnabled = false;
                    break;
            }

            this.Loaded += new RoutedEventHandler(Window_Loaded);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(TroubleDevicesBox.Text))
                errors.AppendLine("Укажите проблемные устройства");
            if (string.IsNullOrWhiteSpace(ProblemDescriptionBox.Text))
                errors.AppendLine("Укажите наблюдаемые проблемы/неполадки");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            using (ComputerRepairShopContext db = new ComputerRepairShopContext())
            {
                try
                {
                    if (editingRequest)
                    {
                        if (checkChanges())
                            currentRequest.LastChangeDate = DateTime.Now;

                        currentRequest.TroubleDevices = TroubleDevicesBox.Text;
                        currentRequest.ProblemDescription = ProblemDescriptionBox.Text;
                        currentRequest.Status = StatusBox.SelectedIndex;

                        db.Requests.Update(currentRequest);
                        db.SaveChanges();

                        MessageBox.Show("Заявка успешно изменена!");

                        this.Close();
                    }
                    else
                    {
                        currentRequest.UserId = currentUser.UserId;
                        currentRequest.CreationDate = DateTime.Now;
                        currentRequest.TroubleDevices = TroubleDevicesBox.Text;
                        currentRequest.ProblemDescription = ProblemDescriptionBox.Text;
                        currentRequest.Status = 0;

                        db.Requests.Add(currentRequest);
                        db.SaveChanges();

                        MessageBox.Show("Заявка успешно добавлена!");

                        this.Close();
                    }     
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.ToString());
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool checkChanges()
        {
            bool changesApplied = false;

            if (TroubleDevicesBox.Text != currentRequest.TroubleDevices)
                changesApplied = true;
            if (ProblemDescriptionBox.Text != currentRequest.ProblemDescription)
                changesApplied = true;
            if (StatusBox.SelectedIndex != currentRequest.Status)
                changesApplied = true;

            return changesApplied;
        }
    }
}
