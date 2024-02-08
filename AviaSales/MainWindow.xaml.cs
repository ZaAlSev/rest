using System;
using RestaurantDesktop.Restaurant_DB1DataSetTableAdapters;
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
using AviaSales;
using System.Xml.Linq;

namespace RestaurantDesktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserTableAdapter users = new UserTableAdapter();
        RoleTableAdapter roles = new RoleTableAdapter();
        public static Restaurant_DB1DataSet.UserRow curUser;
        HashClass hash = new HashClass(); 
        public MainWindow()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                curUser = users.GetData().FirstOrDefault(x => x.User_Login == loginBox.Text);
                if(curUser != null && hash.AreEqual(passwordBox.Text, curUser.User_Password, curUser.Salt))
                {
                    var userRole = roles.GetData().FirstOrDefault(x => x.ID_Role == curUser.Role_ID).Role_Name;
                    switch (userRole)
                    {
                        case "Администратор":
                            MessageBox.Show($"Добро пожаловать в систему, {curUser.User_Name}");
                            AdminWindow adminWindow = new AdminWindow();
                            adminWindow.Show();
                            break;
                        case "Пользователь":
                            MessageBox.Show($"Добро пожаловать в систему, {curUser.User_Name}");
                            CustomerWindow customerWindow = new CustomerWindow();
                            customerWindow.Show();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Вы не зарегистрированы в системе!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void reg_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            regWindow.Show();
        }
    }
}
