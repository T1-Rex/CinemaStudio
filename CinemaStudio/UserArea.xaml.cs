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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CinemaStudio
{
    /// <summary>
    /// Логика взаимодействия для UserArea.xaml
    /// </summary>
    public partial class UserArea : UserControl
    {
        public UserArea()
        {
            InitializeComponent();

            DGArea.ItemsSource = MainWindow.bd.Areas.ToList();
            CbType.ItemsSource = MainWindow.bd.TypeAreas.ToList();
            CbType.SelectedIndex = 0;
        }

        private void BtnAddEdit_Click(object sender, RoutedEventArgs e)
        {
            List<string> errorsEmpty = new List<string>();

            if (string.IsNullOrWhiteSpace(TbAddress.Text))
                errorsEmpty.Add("Адрес");
            if (string.IsNullOrWhiteSpace(TbCapacity.Text))
                errorsEmpty.Add("Вместимость");
            if (string.IsNullOrWhiteSpace(TbCostRentInDay.Text))
                errorsEmpty.Add("Стоимость аренды в день");

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

            if (Convert.ToInt32(TbCapacity.Text) <= 0)
            {
                MessageBox.Show(
                    "Вместимость должна быть больше 0!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (Convert.ToDouble(TbCostRentInDay.Text) <= 0)
            {
                MessageBox.Show(
                    "Стоимость аренды должна быть больше 0!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            var selectedType = CbType.SelectedItem as TypeArea;
            string textAction = "";
            if (DGArea.SelectedItem is Area area)
            {
                area.Address = TbAddress.Text.Trim();
                area.TypeId = selectedType.Id;
                area.Type = selectedType;
                area.CostRentalInDay = Convert.ToDouble(TbCostRentInDay.Text);
                area.CapacityArea = Convert.ToInt32(TbCapacity.Text);
                textAction = "Данные успешно изменены!";
            }
            else
            {
                var newArea = new Area
                {
                   

                    Address = TbAddress.Text.Trim(),
                    TypeId = selectedType.Id,
                    Type = selectedType,
                    CostRentalInDay = Convert.ToDouble(TbCostRentInDay.Text),
                    CapacityArea = Convert.ToInt32(TbCapacity.Text)
                };
               MainWindow.bd.Areas.Add(newArea);
                textAction = "Данные успешно добавлены!";
            }

            MainWindow.bd.SaveChanges();
            DGArea.ItemsSource = MainWindow.bd.Areas.ToList();
            ClearField();
            MessageBox.Show(
                   textAction,
                   "Информация",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);
        }

        private void DGArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGArea.SelectedItem is Area area)
            {
                TbAddress.Text = area.Address ?? "";
                CbType.SelectedValue = area.Type.Name ?? "";
                TbCapacity.Text = area.CapacityArea.ToString() ?? "";
                TbCostRentInDay.Text = area.CostRentalInDay.ToString() ?? "";
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
            TbAddress.Text = string.Empty;
            CbType.SelectedIndex = 0;
            TbCapacity.Text = string.Empty;
            TbCostRentInDay.Text = string.Empty;
            DGArea.SelectedIndex = -1;
        }

        private void TbCapacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.OnlyNumber_TextChanged(sender, e);
        }

        private void TbCostRentInDay_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.OnlyRealNumber_TextChanged(sender, e);
        }
    }
}
