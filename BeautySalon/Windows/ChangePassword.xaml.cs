using System;
using System.Text;
using System.Windows;
using BeautySalon.Data;

namespace BeautySalon.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private BaseWindow baseWindow;
        User userChanging;
        public ChangePassword(User currentUser, BaseWindow baseWindow)
        {
            InitializeComponent();
            userChanging = currentUser;
            this.baseWindow = baseWindow;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Вы действительно желаете сменить пароль?","Смена пароля", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            { 
                int count = 0;
                StringBuilder stringBuilder = new StringBuilder();
                if (string.IsNullOrEmpty(passBoxOld.Password))
                    stringBuilder.AppendLine("Не введен старый пароль");
                if (string.IsNullOrEmpty(passBoxNew.Password))
                    stringBuilder.AppendLine("Не введен новый пароль");
                if (string.IsNullOrEmpty(passBoxCommit.Password))
                    stringBuilder.AppendLine("Подтверждающий пароль не введен");
                if (userChanging.Password != passBoxOld.Password)
                    stringBuilder.AppendLine("Старый пароль не совпадает");
                else
                    count++;
                if (passBoxNew.Password != passBoxCommit.Password)
                    stringBuilder.AppendLine("Новые пароли не совпадают");
                else
                    count++;
                if (passBoxNew.Password == passBoxCommit.Password && passBoxCommit.Password == userChanging.Password)
                    stringBuilder.AppendLine("Старый пароль совпадает с новым");
                else
                    count++;
            
                if (stringBuilder.Length != 0)
                {
                    MessageBox.Show(stringBuilder.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                try
                {
                    if(count == 3)
                    {
                        userChanging.Password = passBoxNew.Password;
                        BeautySalonBaseEntities.getContext().SaveChanges();
                        MessageBox.Show("Данные успешно сохранены!\nДля дальнейшей работы войдите в учетную запись снова", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        AuthWindow authWindow = new AuthWindow();
                        authWindow.Show();
                        baseWindow.Close();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка" + ex.Message.ToString());
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
