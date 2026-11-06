using CinemaStudio.Models;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для UserFilmingGroup.xaml
    /// </summary>
    public partial class UserFilmingGroup : UserControl
    {
        ListView list;
        FilmGroup filmGroup;
        public UserFilmingGroup()
        {
            InitializeComponent();

            CbProject.ItemsSource = MainWindow.bd.Projects.ToList();
            CbEmployee.ItemsSource = CbEmployee.ItemsSource = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();
            CbProject.SelectedIndex = 0;
            CbEmployee.SelectedIndex = 0;

            ListFilmGroup.ItemsSource = MainWindow.bd.FilmGroups.GroupBy(x => x.Project.Name).ToList();
        }

        private void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbRoleInProject.Text))
            {
                MessageBox.Show(
                    "Вы не заполнили поле роль в проекте!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            string textAction = "";
            var selectedProject = CbProject.SelectedItem as Project;
            var selectedEmployee = CbEmployee.SelectedItem as Employee;
            if (filmGroup != null)
            {
                filmGroup.ProjectId = selectedProject.Id;
                filmGroup.Project = selectedProject;
                filmGroup.EmployeeId = selectedEmployee.Id;
                filmGroup.Employee = selectedEmployee;
                filmGroup.RoleInProject = TbRoleInProject.Text.Trim();

                textAction = "Данные успешно изменены!";
            }
            else
            {
                MainWindow.bd.FilmGroups.Add(new FilmGroup
                {
                  

                    ProjectId = selectedProject.Id,
                    Project = selectedProject,
                    EmployeeId = selectedEmployee.Id,
                    Employee = selectedEmployee,
                    RoleInProject = TbRoleInProject.Text.Trim()
                });
                textAction = "Данные успешно добавлены!";
            }

            MainWindow.bd.SaveChanges();

            ListFilmGroup.ItemsSource = MainWindow.bd.FilmGroups.GroupBy(x => x.Project.Name).ToList();
            ClearField();

            MessageBox.Show(
                   textAction,
                   "Информация",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);
        }

        private void BtnClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearField();
        }

        private void CbProject_Loaded(object sender, RoutedEventArgs e)
        {
            var project = new ObservableCollection<Project>(MainWindow.bd.Projects);

            CbProject.ItemsSource = project;
            CbProject.SelectedIndex = 0;
        }

        private void ListFilmGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListView listView)) return;
            if (listView.SelectedItem == null) return;

            list = listView;

            if (listView.SelectedItem is FilmGroup group)
            {
                filmGroup = group;
                CbProject.SelectedValue = group.Project.Name ?? "";
                CbEmployee.SelectedValue = group.Employee.FullName ?? "";
                TbRoleInProject.Text = group.RoleInProject ?? "";
            }

            if (list != null)
                list.SelectedIndex = -1;
        }

        private void ClearField()
        {
            filmGroup = null;
            CbProject.SelectedIndex = 0;
            CbEmployee.SelectedIndex = 0;
            TbRoleInProject.Text = string.Empty;
            if (list != null)
                list.SelectedIndex = -1;
            list = null;
        }

        private void CbEmployee_Loaded(object sender, RoutedEventArgs e)
        {
            CbEmployee.ItemsSource = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();
            CbEmployee.SelectedIndex = 0;
        }
    }
}
