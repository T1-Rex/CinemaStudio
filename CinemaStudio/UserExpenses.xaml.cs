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
    /// Логика взаимодействия для UserExpenses.xaml
    /// </summary>
    public partial class UserExpenses : UserControl
    {
        public UserExpenses()
        {
            InitializeComponent();

            var project = new ObservableCollection<Project>(MainWindow.bd.Projects);
            var typeExpenses = MainWindow.bd.TypeExpenses.ToList();

            CbFormProject.ItemsSource = project;
            CbFormProject.SelectedIndex = 0;
            CbProjectFilter.ItemsSource = project;

            CbTypeExpenses.ItemsSource = typeExpenses;
            CbTypeExpenses.SelectedIndex = 0;
            CbTypeExpensesFilter.ItemsSource = typeExpenses;

            DGExpenses.ItemsSource = MainWindow.bd.Expenses.ToList();
        }

        private void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            List<string> errorsEmpty = new List<string>();

            if (string.IsNullOrWhiteSpace(DPDate.Text))
                errorsEmpty.Add("Дата");
            if (string.IsNullOrWhiteSpace(TbSum.Text))
                errorsEmpty.Add("Сумма");

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

            if (DPDate.SelectedDate.Value > DateTime.Today)
            {
                MessageBox.Show(
                   "Дата должна быть текущей или ранее!",
                   "Ошибка",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);

                return;
            }

            if (Convert.ToInt32(TbSum.Text) <= 0)
            {
                MessageBox.Show(
                    "Сумма должна быть больше 0!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            string textAction = "";
            var selectesProject = CbFormProject.SelectedItem as Project;
            var selectedType = CbTypeExpenses.SelectedItem as TypeExpense;
            if (DGExpenses.SelectedItem is Expense expense)
            {
                expense.ProjectId = selectesProject.Id;
                expense.Project = selectesProject;
                expense.Date = DateOnly.FromDateTime(DPDate.SelectedDate.Value.Date);
                expense.TypeId = selectedType.Id;
                expense.Type = selectedType;
                expense.Sum = Convert.ToDouble(TbSum.Text);

                textAction = "Данные успешно изменены!";
            }
            else
            {
                MainWindow.bd.Expenses.Add(new Expense
                {


                    ProjectId = selectesProject.Id,
                    Project = selectesProject,
                    Date = DateOnly.FromDateTime(DPDate.SelectedDate.Value.Date),
                    TypeId = selectedType.Id,
                    Type = selectedType,
                    Sum = Convert.ToDouble(TbSum.Text)
                });
                textAction = "Данные успешно добавлены!";
            }

            MainWindow.bd.SaveChanges();

           ClearField();
            DGExpenses.ItemsSource = MainWindow.bd.Expenses.ToList();

            MessageBox.Show(
                    textAction,
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
        }

        private void DGExpenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGExpenses.SelectedItem is Expense expense)
            {
                CbFormProject.SelectedValue = expense.Project.Name ?? "";
                DPDate.SelectedDate = expense.Date.ToDateTime(TimeOnly.MinValue);
                CbTypeExpenses.SelectedValue = expense.Type.Name ?? "";
                TbSum.Text = expense.Sum.ToString() ?? "";
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
            CbTypeExpenses.SelectedIndex = 0;
            DPDate.SelectedDate = DateTime.Today;
            TbSum.Text = string.Empty;
            CbProjectFilter.SelectedIndex = -1;
            CbTypeExpensesFilter.SelectedIndex = -1;
            DGExpenses.SelectedIndex = -1;
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            CbProjectFilter.SelectedIndex = -1;
            CbTypeExpensesFilter.SelectedIndex = -1;
            DGExpenses.ItemsSource = MainWindow.bd.Expenses.ToList();
        }

        private void TbSum_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.OnlyRealNumber_TextChanged(sender, e);
        }

        private void Filters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = MainWindow.bd.Expenses.ToList();

            if (CbProjectFilter.SelectedItem is Project project)
            {
                list = list.Where(x => x.ProjectId == project.Id).ToList();
            }

            if (CbTypeExpensesFilter.SelectedItem is TypeExpense type)
            {
                list = list.Where(x => x.TypeId == type.Id).ToList();
            }

            DGExpenses.ItemsSource = list;
        }

        private void CbProject_Loaded(object sender, RoutedEventArgs e)
        {
            var project = new ObservableCollection<Project>(MainWindow.bd.Projects);

            CbFormProject.ItemsSource = project;
            CbFormProject.SelectedIndex = 0;
            CbProjectFilter.ItemsSource = project;
        }
    }
}
