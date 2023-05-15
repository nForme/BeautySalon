using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using BeautySalon.Data;
using BeautySalon.Windows;

namespace BeautySalon
{
    /// <summary>
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private int countErrorAuths = 0;
        public AuthWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            DataObject.AddCopyingHandler(txtBoxPassword, this.OnCancelCommand);
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginStatus = false;
                var loginHistory = new AuthHistory();
                User currentUser = null;
                if (BeautySalonBaseEntities.getContext().User.Count(p => p.Login == txtBoxLogin.Text) != 0)
                {
                    currentUser = BeautySalonBaseEntities.getContext().User.First(p => p.Login == txtBoxLogin.Text);
                    var result = BeautySalonBaseEntities.getContext().User.ToList().Select(p => p).Where(p =>
                        p.Login == txtBoxLogin.Text && (p.Password == txtBoxPassword.Password ||
                                                        p.Password == txtBoxPassword.Password));
                    if (result.Count() != 0)
                    {
                        currentUser.Lastenter = DateTime.Now;
                        BeautySalonBaseEntities.getContext().SaveChanges();
                        Manager.loginedUser = currentUser;
                        loginStatus = true;
                        MessageBox.Show("Вы успешно авторизовались", "Авторизация", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        var baseWindow = new BaseWindow(currentUser);
                        baseWindow.Show();
                        Close();
                    }
                    else
                    {
                        countErrorAuths++;
                        MessageBox.Show("Неверный пароль!", "Авторизация", MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        if (countErrorAuths == 3)
                        {
                            countErrorAuths = 0;
                            MessageBox.Show("Превышено количество попыток входа!\nПовторите попытку через 10 секунд.",
                                "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                            Thread.Sleep(10000);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин!", "Авторизация", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                if (currentUser != null)
                {
                    loginHistory.DateTime = DateTime.Now;
                    loginHistory.Status = loginStatus;
                    loginHistory.UserId = currentUser.Id;
                    BeautySalonBaseEntities.getContext().AuthHistory.Add(loginHistory);
                    BeautySalonBaseEntities.getContext().SaveChanges();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка связи с базой данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void checkBoxShowPass_Checked(object sender, RoutedEventArgs e)
        {
            txtBoxPassword.Visibility = Visibility.Collapsed;
            txtBoxPasswordShow.Visibility = Visibility.Visible;

            txtBoxPasswordShow.Text = new NetworkCredential(string.Empty, txtBoxPassword.SecurePassword).Password;
            txtBoxPasswordShow.Focus();
        }
        private void checkBoxShowPass_Unchecked(object sender, RoutedEventArgs e)
        {
            txtBoxPassword.Visibility = Visibility.Visible;
            txtBoxPasswordShow.Visibility = Visibility.Collapsed;

            txtBoxPasswordShow.Text = "";
            txtBoxPassword.Focus();
        }
        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }
    }
}
