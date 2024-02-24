using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        }
    }
}
