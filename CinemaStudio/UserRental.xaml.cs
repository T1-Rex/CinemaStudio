using CinemaStudio.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CinemaStudio
{
    /// <summary>
    /// Логика взаимодействия для UserRental.xaml
    /// </summary>
    public partial class UserRental : UserControl
    {
        private int rating = 0;

        public UserRental()
        {
            InitializeComponent();

            CbFormProject.ItemsSource = MainWindow.bd.Projects.Where(x => x.Status.Name == "Завершен").ToList();
            CbFormProject.SelectedIndex = 0;
            DGRental.ItemsSource = MainWindow.bd.Rentals.ToList();
        }

        private void CbProject_Loaded(object sender, RoutedEventArgs e)
        {
            var project = new ObservableCollection<Project>(MainWindow.bd.Projects);

            CbFormProject.ItemsSource = project;
            CbFormProject.SelectedIndex = 0;
        }

        private void Rating_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            rating = Convert.ToInt32(btn.Tag.ToString());

            StackPanel st = btn.Parent as StackPanel;

            SetRating(st, rating);
        }

        private void SetRating(StackPanel st, int rating)
        {
            if (st == null) return;

            foreach (var child in st.Children)
            {
                if (child is Button btnChild)
                {
                    int btnRating = Convert.ToInt32(btnChild.Tag.ToString());

                    if (btnRating <= rating)
                    {
                        btnChild.Foreground = Brushes.Gold;
                    }
                    else
                    {
                        btnChild.Foreground = Brushes.LightGray;
                    }
                }
            }
        }

        private void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            List<string> errorsEmpty = new List<string>();

            if (string.IsNullOrWhiteSpace(DPDate.Text))
                errorsEmpty.Add("Дата");
            if (string.IsNullOrWhiteSpace(TbCountCinema.Text))
                errorsEmpty.Add("Кол-во кинотеатров");
            if (string.IsNullOrWhiteSpace(TbFees.Text))
                errorsEmpty.Add("Сборы");

            if (errorsEmpty.Count == 3)
            {
                MessageBox.Show(
                    "Все поля незаполнены!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (errorsEmpty.Count > 0 && errorsEmpty.Count < 3)
            {
                string errorMessage = $"Незаполнены поля:\n{string.Join("\n", errorsEmpty)}\nПожалуйста, заполните!";

                MessageBox.Show(
                    errorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (DPDate.SelectedDate.Value > DateTime.Today)
            {
                MessageBox.Show(
                   "Дата должна быть текущей или ранее!",
                   "Ошибка",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);

                return;
            }

            if (Convert.ToInt32(TbCountCinema.Text) <= 0)
            {
                MessageBox.Show(
                    "Кол-во кинотеатров должно быть больше 0!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (Convert.ToInt32(TbFees.Text) < 0)
            {
                MessageBox.Show(
                    "Сборы должны быть больше или равны 0!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            string textAction = "";
            var selectedProject = CbFormProject.SelectedItem as Project;
            if (DGRental.SelectedItem is Rental rental)
            {
                rental.ProjectId = selectedProject.Id;
                rental.Project = selectedProject;
                rental.DateExit = DateOnly.FromDateTime(DPDate.SelectedDate.Value.Date);
                rental.CountCinema = Convert.ToInt32(TbCountCinema.Text);
                rental.Fees = Convert.ToDouble(TbFees.Text);
                rental.Rating = rating;
                textAction = "Данные успешно изменены!";
            }
            else
            {
                MainWindow.bd.Rentals.Add(new Rental
                {
                  

                    ProjectId = selectedProject.Id,
                    Project = selectedProject,
                    DateExit = DateOnly.FromDateTime(DPDate.SelectedDate.Value.Date),
                    CountCinema = Convert.ToInt32(TbCountCinema.Text),
                    Fees = Convert.ToDouble(TbFees.Text),
                    Rating = rating
                });
                textAction = "Данные успешно добавлены!";
            }

            MainWindow.bd.SaveChanges();

            ClearField();
            DGRental.ItemsSource = MainWindow.bd.Rentals.ToList();
            MessageBox.Show(
                    textAction,
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

        }

        private void DGRental_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGRental.SelectedItem is Rental rental)
            {
                CbFormProject.SelectedValue = rental.Project.Name ?? "";
                DPDate.SelectedDate = rental.DateExit.ToDateTime(TimeOnly.MinValue);
                TbCountCinema.Text = rental.CountCinema.ToString();
                TbFees.Text = rental.Fees.ToString();
                SetRating(StackRating, rental.Rating);
            }
            else
            {
                ClearField();
            }
        }

        private void BtnClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearField();
        }

        private void ClearField()
        {
            CbFormProject.SelectedIndex = 0;
            DPDate.SelectedDate = DateTime.Today;
            TbCountCinema.Text = string.Empty;
            TbFees.Text = string.Empty;
            SetRating(StackRating, 0);
            DGRental.SelectedIndex = -1;
            rating = 0;
        }
    }
}
