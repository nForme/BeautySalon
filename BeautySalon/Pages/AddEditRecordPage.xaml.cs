using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using BeautySalon.Data;
using MessageBox = System.Windows.MessageBox;

namespace BeautySalon.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditRecord.xaml
    /// </summary>
    public partial class AddEditRecordPage : Page
    {
        private Discount.Discount discount = new Discount.Discount();
        private ClientService currentRecord;
        private Dictionary<int, string> comboBoxClientData = new Dictionary<int, string>();
        private Dictionary<int, string> comboBoxEmployeeData = new Dictionary<int, string>();
        private string selectedValueClient;
        private string selectedValueEmployee;
        public AddEditRecordPage(ClientServiceView record)
        {
            InitializeComponent();
            datePickerDate.Minimum = DateTime.Now - TimeSpan.FromMinutes(10);
            currentRecord = new ClientService();
            try
            {
                if (record != null)
                {
                    currentRecord = BeautySalonBaseEntities.getContext().ClientService.Select(p => p).Where(p => p.Id == record.Id).First();
                    
                }
                else
                    currentRecord.Date = DateTime.Now;
                DataContext = currentRecord;
            }
            catch
            {
                MessageBox.Show("Ошибка в получении данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            comboBoxService.ItemsSource = BeautySalonBaseEntities.getContext().Service.Where(p=>p.IsActual == true).ToList();
            foreach (Client client in BeautySalonBaseEntities.getContext().Client.ToList())
            {
                comboBoxClientData.Add(client.Id, client.LastName + " " + client.FirstName.First() + ". " + (!String.IsNullOrWhiteSpace(client.Patronymic) ? client.Patronymic.First() + "." : ""));
            }
            comboBoxClient.ItemsSource = comboBoxClientData.Values;
            fillComboBoxEmployeeData(null);
            
            
            if (comboBoxClientData.TryGetValue(currentRecord.ClientId, out selectedValueClient))
            {
                comboBoxClient.SelectedItem = selectedValueClient;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (comboBoxClient.SelectedItem == null)
            {
                stringBuilder.AppendLine("Не выбран клиент");
            }
            if (comboBoxService.SelectedItem == null)
            {
                stringBuilder.AppendLine("Не выбрана услуга");
            }
            if (comboBoxEmployee.SelectedItem == null)
            {
                stringBuilder.AppendLine("Не выбран сотрудник");
            }
            if (currentRecord.Date == null)
            {
                stringBuilder.AppendLine("Не выбрана дата");
            }
            if (stringBuilder.Length != 0)
            {
                MessageBox.Show(stringBuilder.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                currentRecord.EmployeeId = comboBoxEmployeeData.FirstOrDefault(p => p.Value == comboBoxEmployee.SelectedItem.ToString()).Key;
                currentRecord.ClientId = comboBoxClientData.FirstOrDefault(p => p.Value == comboBoxClient.SelectedItem.ToString()).Key;
                currentRecord.Date = (DateTime) datePickerDate.Value;
                if (currentRecord.Id == 0)
                {
                    var client = comboBoxClientData.FirstOrDefault(p => p.Value == comboBoxClient.SelectedItem.ToString()).Key;
                    Client client1 = BeautySalonBaseEntities.getContext().Client.Find(client);
                    int CountOrder = BeautySalonBaseEntities.getContext().ClientService.ToList().Count(p => p.ClientId == client) +1;     
                    if (client1.Birthday.Day == datePickerDate.Value.Value.Day && client1.Birthday.Month == datePickerDate.Value.Value.Month)
                    {
                        discount.BirthDayDiscount(currentRecord);
                        BeautySalonBaseEntities.getContext().ClientService.Add(currentRecord);
                        MessageBox.Show(client1.LastName + " сегодня праздует день рождения и получает скидку 20%", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (CountOrder % 6 == 0)
                    {
                        discount.EverySixDiscount(currentRecord);
                        BeautySalonBaseEntities.getContext().ClientService.Add(currentRecord);
                        MessageBox.Show(client1.LastName + " Посещает наш салон 6 раз и получает скидку 15%", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else 
                    {
                        discount.DefaultRecord(currentRecord);
                        BeautySalonBaseEntities.getContext().ClientService.Add(currentRecord);
                    }

                }
                BeautySalonBaseEntities.getContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                Manager.baseFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка" + ex.Message.ToString());
            }
        }

        private void comboBoxService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxService.SelectedItem != null)
            { 
                Service currentService = (Service)comboBoxService.SelectedItem;
                ServiceCategory serviceCategory = BeautySalonBaseEntities.getContext().ServiceCategory.Where(c => c.ServiceId == currentService.Id).FirstOrDefault();
                Category currentCategory = BeautySalonBaseEntities.getContext().Category.Where(p => p.Id == serviceCategory.CategoryId).FirstOrDefault();
                if (currentCategory.Id == 1 || currentCategory.Id == 2 || currentCategory.Id == 3 || currentCategory.Id == 4)
                    fillComboBoxEmployeeData("Парикмахер");
                else if (currentCategory.Id == 5)
                    fillComboBoxEmployeeData("Визажист");
                else if (currentCategory.Id == 6)
                    fillComboBoxEmployeeData("Мастер маникюра");
                if (comboBoxEmployeeData.TryGetValue(currentRecord.EmployeeId, out selectedValueEmployee))
                {
                    comboBoxEmployee.SelectedItem = selectedValueEmployee;
                }
            }
        }
        private void fillComboBoxEmployeeData(string post)
        {
            try
            {
                comboBoxEmployeeData.Clear();
                comboBoxEmployee.ItemsSource = null;
                if (post == null)
                {
                    foreach (Employee employee in BeautySalonBaseEntities.getContext().Employee.ToList())
                    {
                        comboBoxEmployeeData.Add(employee.Id, employee.LastName + " " + employee.FirstName.First() + ". " + (!String.IsNullOrWhiteSpace(employee.Patronymic) ? employee.Patronymic.First() + "." : "") + ". (" + employee.Post + ")");
                    }
                }
                else
                {
                    foreach (Employee employee in BeautySalonBaseEntities.getContext().Employee.Where(p => p.Post == post).ToList())
                    {
                        comboBoxEmployeeData.Add(employee.Id, employee.LastName + " " + employee.FirstName.First() + ". " + (!String.IsNullOrWhiteSpace(employee.Patronymic) ? employee.Patronymic.First() + "." : "") + ". (" + employee.Post + ")");
                    }
                }
                comboBoxEmployee.ItemsSource = comboBoxEmployeeData.Values;
            }
            catch
            {
                MessageBox.Show("Ошибка в получении данных", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
