using CinemaStudio.Models;
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
using System.Windows.Shapes;

namespace CinemaStudio
{
    /// <summary>
    /// Логика взаимодействия для MostMainWindow.xaml
    /// </summary>
    public partial class MostMainWindow : Window
    {
        Employee employee;

        // Экземпляры всех элементов основной части.
        private UserEmployee userEmployee;
        private UserProject userProject = new UserProject();
        private UserArea userArea = new UserArea();
        private UserFilmingGroup userFilmingGroup = new UserFilmingGroup();
        private UserShootingSchedule userShootingSchedule = new UserShootingSchedule();
        private UserExpenses userExpenses = new UserExpenses();
        private UserRental userRental = new UserRental();

        // Время закрытия.
        private DateTime _closed = DateTime.MinValue;

        public MostMainWindow(string login)
        {
            InitializeComponent();
            employee = MainWindow.bd.Employees.FirstOrDefault(x => x.Login == login);
            userEmployee = new UserEmployee(employee.Login);
            TextUser.Text = login;
            MainPart.Content = userEmployee;



            TgBtnUser.Tag = employee.Photo;
        }

        private void TgBtnUser_Click(object sender, RoutedEventArgs e)
        {
            // Предотвращение случайного открытия меню заново
            if ((DateTime.Now - _closed).TotalMilliseconds < 200)
            {
                // Выкл кнопку.
                TgBtnUser.IsChecked = false;
                // Закрываем меню.
                MenuUser.IsOpen = false;

                // Предотвращаем дальшнейшее выполнение.
                return;
            }

            // Если нажали вкл.
            if (TgBtnUser.IsChecked == true)
            {
                // Открываем меню.
                MenuUser.IsOpen = true;
            }
        }

        // Закрытие меню.
        private void MenuUser_Closed(object sender, EventArgs e)
        {
            // Выкл кнопку.
            TgBtnUser.IsChecked = false;

            // Время, когда было закрыто меню.
            _closed = DateTime.Now;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = new MainWindow();
            win.Show();
            this.Close();
        }

        private void RbMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (MainPart == null) return;

            if (RbEmployee.IsChecked == true)
                MainPart.Content = userEmployee;
            else if (RbProject.IsChecked == true)
                MainPart.Content = userProject;
            else if (RbArea.IsChecked == true)
                MainPart.Content = userArea;
            else if (RbFilming.IsChecked == true)
                MainPart.Content = userShootingSchedule;
            else if (RbExpencens.IsChecked == true)
                MainPart.Content = userExpenses;
            else if (RbRental.IsChecked == true)
                MainPart.Content = userRental;
            else if (RbGroup.IsChecked == true)
                MainPart.Content = userFilmingGroup;
        }
    }
}
