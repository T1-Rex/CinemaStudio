using CinemaStudio.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        string loginUser;
        Employee employee;

        byte[] photoUser;

        public EmployeeWindow(string loginUser)
        {
            InitializeComponent();

            this.loginUser = loginUser;

            employee = MainWindow.bd.Employees.Include(x => x.Specialization).FirstOrDefault(x => x.Login == loginUser);

            CbSpecialization.ItemsSource = MainWindow.bd.Specializations.ToList();

            if (loginUser == "")
            {
                this.Height = 690;
                GridMain.Height = 600;
                TextTitle.Text = "ДОБАВЛЕНИЕ СОТРУДНИКА";
                BtnLogIn.Content = "Добавить";
                StAddPwd.Visibility = Visibility.Visible;
                StEditPwd.Visibility = Visibility.Collapsed;
                CbSpecialization.SelectedIndex = 0;
            }
            else
            {
                this.Height = 710;
                GridMain.Height = 660;
                TextTitle.Text = "ИЗМЕНЕНИЕ СОТРУДНИКА";
                BtnLogIn.Content = "Редактировать";
                StAddPwd.Visibility = Visibility.Collapsed;
                StEditPwd.Visibility = Visibility.Visible;

                TbSurname.Text = employee.Surname ?? "";
                TbName.Text = employee.Name ?? "";
                TbPatronymic.Text = employee.Patronymic ?? "";
                CbSpecialization.SelectedValue = employee.Specialization.Name ?? "";
                TbPhone.Text = employee.Phone ?? "";
                TbEmail.Text = employee.Email ?? "";
                TbBet.Text = employee.Fees.ToString() ?? "";
                TbLogin.Text = employee.Login ?? "";
                photoUser = employee.Photo;
            }
        }

        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            int countErrors = 6;
            List<string> errorsEmpty = new List<string>();

            if (string.IsNullOrWhiteSpace(TbSurname.Text))
                errorsEmpty.Add("Фамилия");
            if (string.IsNullOrWhiteSpace(TbName.Text))
                errorsEmpty.Add("Имя");
            if (string.IsNullOrWhiteSpace(TbPhone.Text))
                errorsEmpty.Add("Телефон");
            if (string.IsNullOrWhiteSpace(TbEmail.Text))
                errorsEmpty.Add("Email");
            if (string.IsNullOrWhiteSpace(TbBet.Text))
                errorsEmpty.Add("Ставка");
            if (string.IsNullOrWhiteSpace(TbLogin.Text))
                errorsEmpty.Add("Логин");
            if (loginUser == "")
            {
                if (string.IsNullOrWhiteSpace(PbPasswordText.Password))
                    errorsEmpty.Add("Пароль");
                if (string.IsNullOrWhiteSpace(PbReapetPassword.Password))
                    errorsEmpty.Add("Подтверждение пароля");
                countErrors = 8;
            }

            if (errorsEmpty.Count == countErrors)
            {
                MessageBox.Show(
                    "Все поля незаполнены!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (errorsEmpty.Count > 0 && errorsEmpty.Count < countErrors)
            {
                string errorMessage = $"Незаполнены поля:\n{string.Join("\n", errorsEmpty)}\nПожалуйста, заполните!";

                MessageBox.Show(
                    errorMessage,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (!Regex.IsMatch(TbEmail.Text, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9]+\.[a-zA-Z]{2,}$"))
            {
                MessageBox.Show(
                    "Email не соответствует корректному формату!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            if (TbPhone.Text.Trim().Length != 12)
            {
                MessageBox.Show(
                   "Телефон не соответствует корректному формату!",
                   "Ошибка",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);

                return;
            }

            if (Convert.ToDouble(TbBet.Text) <= 0)
            {
                MessageBox.Show(
                   "Ставка должна быть больше 0!",
                   "Ошибка",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);

                return;
            }

            var spec = CbSpecialization.SelectedItem as Specialization;

            if (loginUser == "")
            {

                var checkLogin = MainWindow.bd.Employees.Any(x => x.Login == TbLogin.Text.Trim());

                if (checkLogin)
                {
                    MessageBox.Show(
                       "Пользователь с таким логином уже есть в системе!",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);

                    return;
                }

                if (PbPasswordText.Password != PbReapetPassword.Password)
                {
                    MessageBox.Show(
                       "Пароль не совпадает с подтверждением!",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);

                    return;
                }

                if (TbTextStatusPwd.Text != "Надежный пароль")
                {
                    MessageBox.Show(
                      "Пароль не надежный!\nПароль должен содержать:\n8 и больше символов\nСпец. символ\nЦифру\nЗаглавную букву",
                      "Ошибка",
                      MessageBoxButton.OK,
                      MessageBoxImage.Error);

                    return;
                }

                var newEmployee = new Employee
                {
                    Surname = TbSurname.Text.Trim(),
                    Name = TbName.Text.Trim(),
                    Patronymic = TbPatronymic.Text.Trim(),
                    Specialization = spec,
                    SpecializationId = spec.Id,
                    Email = TbEmail.Text.Trim(),
                    Phone = TbPhone.Text.Trim(),
                    Fees = Convert.ToDouble(TbBet.Text.Trim()),
                    Login = TbLogin.Text.Trim(),
                    Password = PasswordHashing.HashPassword(PbPasswordText.Password),
                    Photo = photoUser,
                    Status = 1
                };

                MainWindow.bd.Employees.Add(newEmployee);
                MainWindow.bd.SaveChanges();

                MessageBox.Show(
                      "Данные успешно добавлены!",
                      "Информация",
                      MessageBoxButton.OK,
                      MessageBoxImage.Information);
            }
            else if (loginUser != null)
            {
                var checkLogin = MainWindow.bd.Employees.Any(x => x.Login == TbLogin.Text.Trim() && x.Id != employee.Id);


                if (checkLogin)
                {
                    MessageBox.Show(
                       "Пользователь с таким логином уже есть в системе!",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);

                    return;
                }

                employee.Surname = TbSurname.Text.Trim();
                employee.Name = TbName.Text.Trim();
                employee.Patronymic = TbPatronymic.Text.Trim();
                employee.Specialization = spec;
                employee.SpecializationId = spec.Id;
                employee.Email = TbEmail.Text.Trim();
                employee.Phone = TbPhone.Text.Trim();
                employee.Fees = Convert.ToDouble(TbBet.Text.Trim());
                employee.Login = TbLogin.Text.Trim();
                employee.Photo = photoUser;

                if (!string.IsNullOrWhiteSpace(PbNewPasswordText.Password)
                    && !string.IsNullOrEmpty(PbPasswordTextOld.Password)
                    && !string.IsNullOrEmpty(PbReapetPasswordNew.Password))
                {
                    if (!PasswordHashing.PasswordChecking(PbPasswordTextOld.Password, employee.Password))
                    {
                        MessageBox.Show(
                           "Старый пароль не совпадает с текущим!",
                           "Ошибка",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);

                        return;
                    }

                    if (PbNewPasswordText.Password != PbReapetPasswordNew.Password)
                    {
                        MessageBox.Show(
                           "Новый пароль не совпадает с подтверждением!",
                           "Ошибка",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);

                        return;
                    }

                    if (TbTextStatusPwdNew.Text != "Надежный пароль")
                    {
                        MessageBox.Show(
                          "Пароль не надежный!\nПароль должен содержать:\n8 и больше символов\nСпец. символ\nЦифру\nЗаглавную букву",
                          "Ошибка",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);

                        return;
                    }

                    employee.Password = PasswordHashing.HashPassword(PbNewPasswordText.Password);
                }           

                MainWindow.bd.SaveChanges();
                var mainWindow = App.Current.Windows.OfType<MostMainWindow>().FirstOrDefault();

                if (mainWindow != null)
                {
                    mainWindow.TgBtnUser.Tag = employee.Photo;
                    mainWindow.TgBtnUser.Content = employee.Login;
                }
                MessageBox.Show(
                      "Данные успешно изменены!",
                      "Информация",
                      MessageBoxButton.OK,
                      MessageBoxImage.Information);

            }

            this.DialogResult = true;
        }

        private void BtnPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpg)|*.png;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                photoUser = System.IO.File.ReadAllBytes(filePath);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show("Вы уверены, что хотите вернуться?\nНажимая 'ОК' данные не будут сохранены!",
                    "Подтверждение",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                this.DialogResult = false;
            }
        }

        private void PbPasswordText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            StackPanelLvlPwd.Visibility =
                string.IsNullOrEmpty(PbPasswordText.Password)
                ? Visibility.Collapsed
                : Visibility.Visible;

            PbPassword.Value =
                PasswordChecked.CheckedPassword(PbPasswordText.Password);

            TbTextStatusPwd.Text =
                PasswordChecked.TextCheckedPassword(
                    PasswordChecked.CheckedPassword(PbPasswordText.Password));
        }

        private void PbNewPasswordText_PasswordChanged(object sender, RoutedEventArgs e)
        {
            StackPanelLvlPwdNew.Visibility =
                string.IsNullOrEmpty(PbNewPasswordText.Password)
                ? Visibility.Collapsed
                : Visibility.Visible;

            PbPasswordNew.Value =
                PasswordChecked.CheckedPassword(PbNewPasswordText.Password);

            TbTextStatusPwdNew.Text =
                PasswordChecked.TextCheckedPassword(
                    PasswordChecked.CheckedPassword(PbNewPasswordText.Password));
        }

        private void LettersWrite_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.LettersWrite_TextChanged(sender, e);
        }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.TbLogin_TextChanged(sender, e);
        }

        private void TbPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.TbPhone_TextChanged(sender, e);
        }

        private void TbEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.TbEmail_TextChanged(sender, e);
        }

        private void TbBet_TextChanged(object sender, TextChangedEventArgs e)
        {
            FieldChecks.OnlyRealNumber_TextChanged(sender, e);
        }
    }
}
