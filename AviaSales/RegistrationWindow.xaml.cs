using AviaSales;
using RestaurantDesktop.Restaurant_DB1DataSetTableAdapters;
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
using System.Windows.Shapes;

namespace RestaurantDesktop
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        UserTableAdapter users = new UserTableAdapter();
        RoleTableAdapter roles = new RoleTableAdapter();
        HashClass hash = new HashClass();
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userRoleId = roles.GetData().FirstOrDefault(x => x.Role_Name == "Пользователь").ID_Role;
                var salt = hash.CreateSalt(8);
                if (phoneNumber.Text.Length > 0 && password.Text.Length > 0 && login.Text.Length > 0 && fName.Text.Length > 0 && sName.Text.Length > 0)
                {
                    users.Insert(fName.Text, sName.Text, phoneNumber.Text, login.Text, hash.GenerateHash(password.Text, salt), salt, userRoleId);
                    MessageBox.Show("Вы успешно зарегистрированы в системе!");
                    Close();
                }
                else
                {
                    MessageBox.Show("Не все требования выполнены! (Пустые или неправильно заполненные поля)");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
