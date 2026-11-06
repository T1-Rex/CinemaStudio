using CinemaStudio.Models;
using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CinemaStudio
{
    /// <summary>
    /// Логика взаимодействия для UserProject.xaml
    /// </summary>
    public partial class UserProject : UserControl
    {
        private List<Genre> selectedGenres = new List<Genre>();

        public UserProject()
        {
            InitializeComponent();

            var status = MainWindow.bd.StatusProjects.ToList();

            CbStatusFilter.ItemsSource = status;
            CbFormStatus.ItemsSource = status;
            CbGenre.ItemsSource = MainWindow.bd.Genres.ToList();
            CbFormStatus.SelectedIndex = 0;

            DGProject.ItemsSource = MainWindow.bd.Projects.Include(x => x.Status).Include(x => x.GenresInProjects).ToList();
        }

        private void CheckBoxGenre_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox checkBox)) return;

            var clickedItem = checkBox.DataContext as Genre;

            if (checkBox.IsChecked == true)
            {
                if (!selectedGenres.Contains(clickedItem))
                    selectedGenres.Add(clickedItem);
            }
            else
            {
                selectedGenres.Remove(clickedItem);
            }

            if (selectedGenres.Count == 0)
            {
                CbGenre.Text = "Выберите";
            }
            else
            {
                var names = selectedGenres.Select(x => x.Name);
                CbGenre.Text = string.Join(", ", names);
            }
        }

        private void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            List<string> errorsEmpty = new List<string>();

            if (string.IsNullOrWhiteSpace(TbName.Text))
                errorsEmpty.Add("Название");
            if (string.IsNullOrWhiteSpace(DPDateStart.Text))
                errorsEmpty.Add("Дата начала");
            if (string.IsNullOrWhiteSpace(DPDateEnd.Text))
                errorsEmpty.Add("Дата окончания");
            if (string.IsNullOrWhiteSpace(TbBudget.Text))
                errorsEmpty.Add("Бюджет");
            if (string.IsNullOrWhiteSpace(TbTargetAudience.Text))
                errorsEmpty.Add("Целевая аудитория");
            if (selectedGenres.Count <= 0)
                errorsEmpty.Add("Жанр");

            if (errorsEmpty.Count == 6)
            {
                MessageBox.Show(
                    "Все поля незаполнены!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (errorsEmpty.Count > 0 && errorsEmpty.Count < 6)
            {
                string errorMessage = $"Незаполнены поля:\n{string.Join("\n", errorsEmpty)}\nПожалуйста, заполните!";

                MessageBox.Show(
                    errorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            DateTime dateStart = DPDateStart.SelectedDate.Value;
            DateTime dateEnd = DPDateEnd.SelectedDate.Value;

            if (dateStart > dateEnd)
            {
                MessageBox.Show(
                    "Неправильно указан период дат!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (Convert.ToDouble(TbBudget.Text) <= 0)
            {
                MessageBox.Show(
                   "Бюджет должен быть больше нуля!",
                   "Ошибка",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);

                return;
            }

            if (Convert.ToInt32(TbTargetAudience.Text) < 0)
            {
                MessageBox.Show(
                  "Целевая аудитория не может быть отрицательным значением!",
                  "Ошибка",
                  MessageBoxButton.OK,
                  MessageBoxImage.Error);

                return;
            }
            string textAction = "";
            var selectedStatus = CbFormStatus.SelectedItem as StatusProject;
            if (DGProject.SelectedItem is Project project)
            {
                project.Name = TbName.Text.Trim();
                project.DateStart = DateOnly.FromDateTime(DPDateStart.SelectedDate.Value.Date);
                project.DateEnd = DateOnly.FromDateTime(DPDateEnd.SelectedDate.Value.Date);
                project.StatusId = selectedStatus.Id;
                project.Status = selectedStatus;
                project.Budget = Convert.ToDouble(TbBudget.Text);
                project.TargetAudience = TbTargetAudience.Text.Trim();

                var oldGenres = MainWindow.bd.GenresInProjects.Where(x => x.ProjectId == project.Id).ToList();

                foreach (var itemRemove in oldGenres)
                {
                    MainWindow.bd.GenresInProjects.Remove(itemRemove);
                }

                //var nextG = MainWindow.bd.GenresInProjects.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;

                foreach (var itemGenre in selectedGenres)
                {
                    MainWindow.bd.GenresInProjects.Add(new GenresInProject
                    {
                    
                        ProjectId = project.Id,
                        Project = project,
                        GenreId = itemGenre.Id,
                        Genre = itemGenre
                    });
                   // nextG++;
                }
                textAction = "Данные успешно изменены!";
            }
            else
            {
                var newProject = new Project
                {
                    Name = TbName.Text.Trim(),
                    DateStart = DateOnly.FromDateTime(DPDateStart.SelectedDate.Value.Date),
                    DateEnd = DateOnly.FromDateTime(DPDateEnd.SelectedDate.Value.Date),
                    StatusId = selectedStatus.Id,
                    Status = selectedStatus,
                    Budget = Convert.ToDouble(TbBudget.Text),
                    TargetAudience = TbTargetAudience.Text.Trim()
                };
                MainWindow.bd.Projects.Add(newProject);

                foreach (var itemGenre in selectedGenres)
                {
                    var newGenreInProject = new GenresInProject
                    {
                        ProjectId = newProject.Id,
                        Project = newProject,
                        GenreId = itemGenre.Id,
                        Genre = itemGenre
                    };
                    MainWindow.bd.GenresInProjects.Add(newGenreInProject);
                    //newProject.GenresInProjects.Add(newGenreInProject);
                }
                textAction = "Данные успешно добавлены!";
            }

            MainWindow.bd.SaveChanges();

            DGProject.ItemsSource = MainWindow.bd.Projects.ToList();
            ClearField();
            MessageBox.Show(
                textAction,
                "Информация",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            
        }

        private void DGProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGProject.SelectedItem is Project project)
            {
                TbName.Text = project.Name ?? "";
                DPDateStart.SelectedDate = project.DateStart.ToDateTime(TimeOnly.MinValue);
                DPDateEnd.SelectedDate = project.DateEnd.ToDateTime(TimeOnly.MinValue);
                CbFormStatus.SelectedValue = project.Status.Name;
                TbBudget.Text = project.Budget.ToString() ?? "";
                TbTargetAudience.Text = project.TargetAudience.ToString() ?? "";

                selectedGenres.Clear();
                foreach (var genre in project.GenresInProjects)
                {
                    if (genre != null)
                    {
                        selectedGenres.Add(genre.Genre);
                    }
                }

                if (selectedGenres.Count == 0)
                {
                    CbGenre.Text = "Выберите";
                }
                else
                {
                    var names = selectedGenres.Select(x => x.Name);
                    CbGenre.Text = string.Join(", ", names);
                }
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
            TbName.Text = string.Empty;
            DPDateStart.SelectedDate = DateTime.Today;
            DPDateEnd.SelectedDate = DateTime.Today;
            TbBudget.Text = string.Empty;
            TbTargetAudience.Text = string.Empty;
            CbFormStatus.SelectedIndex = 0;
            CbGenre.Text = "Выберите";
            DGProject.SelectedIndex = -1;
            selectedGenres.Clear();
            CbStatusFilter.SelectedIndex = -1;
        }

        private void CheckBoxGenre_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Genre genre)
            {
                checkBox.IsChecked = selectedGenres.Any(x => x.Id == genre.Id);
            }
        }

        private void CbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbStatusFilter.SelectedItem is StatusProject status)
            {
                DGProject.ItemsSource = MainWindow.bd.Projects.Where(x => x.Status.Name == status.Name).ToList();
            }
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            CbStatusFilter.SelectedIndex = -1;
            DGProject.ItemsSource = MainWindow.bd.Projects.ToList();
        }

        private void OnlyRealNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.OnlyRealNumber_TextChanged(sender, e);
        }
    }
}
