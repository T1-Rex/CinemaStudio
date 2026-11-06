using CinemaStudio.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace CinemaStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static CinemaStudioContext bd = new CinemaStudioContext();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text)
                && string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show(
                    "Заполните поля учетными данными!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(TbLogin.Text))
            {
                MessageBox.Show(
                    "Заполните поле логин!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show(
                    "Заполните поле пароль!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var user = bd.Employees.FirstOrDefault(x => x.Login == TbLogin.Text);

            if (user != null)
            {
                if (user.Status == 2)
                {
                    MessageBox.Show(
                       "Пользователь удален!",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);
                    return;
                }

                if (PasswordHashing.PasswordChecking(PbPassword.Password, user.Password))
                {
                    MostMainWindow win = new MostMainWindow(user.Login);
                    win.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                       "Не верный пароль!",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(
                   "Такого пользователя не существует!",
                   "Ошибка",
                   MessageBoxButton.OK,
                   MessageBoxImage.Error);
            }
        }
       
    }
}