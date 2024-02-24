using System;
using System.Linq;
using System.Text;
using System.Windows;
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
