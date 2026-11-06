using CinemaStudio.Models;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
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
    /// Логика взаимодействия для UserEmployee.xaml
    /// </summary>
    public partial class UserEmployee : UserControl
    {
        Employee employee;
        public UserEmployee(string login)
        {
            InitializeComponent();

            employee = MainWindow.bd.Employees.FirstOrDefault(x => x.Login == login);

            DGEmployee.ItemsSource = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbSearch.Text)) return;

            string textSearch = TbSearch.Text.ToLower();

            var users = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();

            var searchUsers = users.Where(x => x.Surname.ToLower().Contains(textSearch)).ToList();

            DGEmployee.ItemsSource = searchUsers;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            var win = Window.GetWindow(this) as MostMainWindow;

            EmployeeWindow employeeWindow = new EmployeeWindow("");

            employeeWindow.Owner = win;

            if (employeeWindow.ShowDialog() == true)
            {
                DGEmployee.ItemsSource = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            if (DGEmployee.SelectedItem is Employee employee)
            {
                var win = Window.GetWindow(this) as MostMainWindow;

                EmployeeWindow employeeWindow = new EmployeeWindow(employee.Login);

                employeeWindow.Owner = win;

                if (employeeWindow.ShowDialog() == true)
                {
                    DGEmployee.ItemsSource = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();
                }
            }
            else
            {
                MessageBox.Show(
                    "Выберите сотрудника для редактирования!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            if (DGEmployee.SelectedItem is Employee employee)
            {
                if (employee.Id == this.employee.Id)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Вы, удаляете свой аккаунт! Вы уверены? При нажатии 'OK' вас выкинет из системы!",
                        "Предупреждение",
                        MessageBoxButton.OKCancel, 
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK) 
                    {
                        employee.Status = 2;
                        MainWindow.bd.SaveChanges();
                        DGEmployee.ItemsSource = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();

                        MessageBox.Show(
                           "Вы, удалили сами себя - круто \nПс: (Вы лох!!!)!",
                           "Информация",
                           MessageBoxButton.OK,
                           MessageBoxImage.Information);

                        MainWindow win = new MainWindow();
                        win.Show();
                        var win1 = Window.GetWindow(this) as MostMainWindow;
                        win1.Close();  
                        return;
                    }
                    else
                    {
                        return;
                    }
                }


                employee.Status = 2;
                MainWindow.bd.SaveChanges();
                DGEmployee.ItemsSource = MainWindow.bd.Employees.Include(x => x.Specialization).Where(x => x.Status != 2).ToList();


                MessageBox.Show(
                    "Сотрудник был успешно удален из системы!",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    "Выберите сотрудника для удаления!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TbSearch.Text = string.Empty;
            TbSearch.Focus();
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbSearch.Text))
            {
                BtnClearSearch.Visibility = Visibility.Collapsed;
                DGEmployee.ItemsSource = MainWindow.bd.Employees.ToList();
            }
            else
            {
                BtnClearSearch.Visibility = Visibility.Visible;
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var exportData = MainWindow.bd.Employees
               .Include(x => x.Specialization)
               .AsNoTracking()
               .Select(x => new
               {
                   ФИО = x.FullName,                          
                   Специализация = x.Specialization != null ? x.Specialization.Name : "Нет",
                   Телефон = x.Phone,
                   Email = x.Email,
                   Ставка = x.Fees,
                   Логин = x.Login
               })
               .ToList();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"Список_сотрудников_{DateTime.Now.ToString("G")}";
            saveFileDialog.Filter = "Книга Excel (*.xlsx)|*.xlsx";

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Сотрудники");

                    
                    worksheet.Cell("A2").InsertData(exportData);
                    worksheet.Cell("A1").Value = "ФИО";
                    worksheet.Cell("B1").Value = "Специализация";
                    worksheet.Cell("C1").Value = "Телефон";
                    worksheet.Cell("D1").Value = "Email";
                    worksheet.Cell("E1").Value = "Ставка";
                    worksheet.Cell("F1").Value = "Логин";


                    worksheet.Row(1).Style.Font.Bold = true;
                    worksheet.Columns().AdjustToContents();

               
                    workbook.SaveAs(saveFileDialog.FileName);
                }

                MessageBox.Show("Данные успешно экспортированы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
