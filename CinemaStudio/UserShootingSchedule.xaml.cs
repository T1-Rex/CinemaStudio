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
    /// Логика взаимодействия для UserShootingSchedule.xaml
    /// </summary>
    public partial class UserShootingSchedule : UserControl
    {

        public UserShootingSchedule()
        {
            InitializeComponent();
            var projects = MainWindow.bd.Projects.ToList();

            CbProjectFilter.ItemsSource = projects;
            CbFormProject.ItemsSource = projects;
            CbArea.ItemsSource = MainWindow.bd.Areas.ToList();
            CbFormProject.SelectedIndex = 0;
            CbArea.SelectedIndex = 0;

            DGShootingSchedule.ItemsSource = MainWindow.bd.ShootingSchedules.ToList();
        }

        private void CbProject_Loaded(object sender, RoutedEventArgs e)
        {
            var project = new ObservableCollection<Project>(MainWindow.bd.Projects);

            CbFormProject.ItemsSource = project;
            CbFormProject.SelectedIndex = 0;
            CbProjectFilter.ItemsSource = project;
        }

        private void CbArea_Loaded(object sender, RoutedEventArgs e)
        {
            CbArea.ItemsSource = new ObservableCollection<Area>(MainWindow.bd.Areas);
            CbArea.SelectedIndex = 0;
        }

        private void CbHours_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 24; i++)
            {
                CbHours.Items.Add(i.ToString("D2"));
            }

            CbHours.SelectedIndex = 12;
        }

        private void CbMinutes_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 60; i++)
            {
                CbMinutes.Items.Add(i.ToString("D2"));
            }

            CbMinutes.SelectedIndex = 0;
        }

        private void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            List<string> errorsEmpty = new List<string>();

            if (string.IsNullOrWhiteSpace(DPDate.Text))
                errorsEmpty.Add("Дата");
            if (string.IsNullOrWhiteSpace(TbScene.Text))
                errorsEmpty.Add("Сцена");

            if (errorsEmpty.Count == 2)
            {
                MessageBox.Show(
                    "Все поля незаполнены!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (errorsEmpty.Count > 0 && errorsEmpty.Count < 2)
            {
                string errorMessage = $"Незаполнены поля:\n{string.Join("\n", errorsEmpty)}\nПожалуйста, заполните!";

                MessageBox.Show(
                    errorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            string textAction = "";
            var selectedProject = CbFormProject.SelectedItem as Project;
            var selectedArea = CbArea.SelectedItem as Area;

            var timeText = CbHours.Text + ":" + CbMinutes.Text;
            TimeSpan time = TimeSpan.Parse(timeText);

            if (DGShootingSchedule.SelectedItem is ShootingSchedule schedule)
            {
                schedule.ProjectId = selectedProject.Id;
                schedule.Project = selectedProject;
                schedule.Date = DateOnly.FromDateTime(DPDate.SelectedDate.Value.Date);
                schedule.Time = TimeOnly.FromTimeSpan(time);
                schedule.AreaId = selectedArea.Id;
                schedule.Area = selectedArea;
                schedule.Scene = TbScene.Text.Trim();
                schedule.Description = TbDescription.Text;
                textAction = "Данные успешно изменены";
            }
            else
            {
                MainWindow.bd.ShootingSchedules.Add(new ShootingSchedule
                {
                   

                    ProjectId = selectedProject.Id,
                    Project = selectedProject,
                    Date = DateOnly.FromDateTime(DPDate.SelectedDate.Value.Date),
                    Time = TimeOnly.FromTimeSpan(time),
                    AreaId = selectedArea.Id,
                    Area = selectedArea,
                    Scene = TbScene.Text.Trim(),
                    Description = TbDescription.Text
                });

                textAction = "Данные успешно добавлены";
            }

            MainWindow.bd.SaveChanges();

            DGShootingSchedule.ItemsSource = MainWindow.bd.ShootingSchedules.ToList();
            ClearField();
            MessageBox.Show(
                   textAction,
                   "Информация",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);
        }
        private void DGShootingSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGShootingSchedule.SelectedItem is ShootingSchedule schedule)
            {
                CbFormProject.SelectedValue = schedule.Project.Name ?? "";
                DPDate.SelectedDate = schedule.Date.ToDateTime(TimeOnly.MinValue);
                CbHours.SelectedValue = schedule.Time.Hour.ToString("D2");
                CbMinutes.SelectedValue = schedule.Time.Minute.ToString("D2");
                CbArea.SelectedValue = schedule.Area.Address ?? "";
                TbScene.Text = schedule.Scene ?? "";
                TbDescription.Text = schedule.Description ?? "";
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
            CbArea.SelectedIndex = 0;
            DPDate.SelectedDate = DateTime.Today;
            CbHours.SelectedIndex = 12;
            CbMinutes.SelectedIndex = 0;
            TbScene.Text = string.Empty;
            TbDescription.Text = string.Empty;
            DGShootingSchedule.SelectedIndex = -1;
            DPDateFilter.SelectedDate = null;
            CbProjectFilter.SelectedIndex = -1;
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            DPDateFilter.SelectedDate = null;
            CbProjectFilter.SelectedIndex = -1;
            FilterSchedule();
        }

        private void DPDateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSchedule();
        }

        private void CbProjectFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSchedule();
        }

        private void FilterSchedule()
        {
            var list = MainWindow.bd.ShootingSchedules.ToList();

            if (DPDateFilter.SelectedDate != null)
            {
                DateOnly date = DateOnly.FromDateTime(DPDateFilter.SelectedDate.Value.Date);
                list = list.Where(x => x.Date == date).ToList();
            }

            if (CbProjectFilter.SelectedItem is Project project)
            {
                list = list.Where(x => x.Project.Id == project.Id).ToList();
            }

            DGShootingSchedule.ItemsSource = list;
        }

       
    }
}
