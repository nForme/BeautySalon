using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BeautySalon.Data;

namespace BeautySalon.Pages
{
    /// <summary>
    /// Логика взаимодействия для HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : Page
    {
        public HistoryPage()
        {
            InitializeComponent();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            { 
                if (Visibility == Visibility.Visible)
                {
                    BeautySalonBaseEntities.getContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                    DGridHistory.ItemsSource = BeautySalonBaseEntities.getContext().AuthHistory.ToList();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка обновления записей!\nПопробуйте снова обновить записи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
