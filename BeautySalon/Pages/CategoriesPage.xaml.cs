using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BeautySalon.Data;

namespace BeautySalon.Pages
{
    /// <summary>
    /// Логика взаимодействия для CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        public CategoriesPage()
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
                    ListViewCategories.ItemsSource = BeautySalonBaseEntities.getContext().Category.ToList();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка обновления данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ListViewCategories_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Category choosenCategory = (Category)ListViewCategories.SelectedItem;
            Manager.baseFrame.Navigate(new ServicesPage(choosenCategory));
        }

        private void btnExpandServices_Click(object sender, RoutedEventArgs e)
        {
            Manager.baseFrame.Navigate(new ServicesPage(null));
        }
    }
}
