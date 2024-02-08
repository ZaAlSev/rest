using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestaurantDesktop.Restaurant_DB1DataSetTableAdapters;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using static RestaurantDesktop.Restaurant_DB1DataSet;
using System.Data;
using MaterialDesignThemes.Wpf.Transitions;

namespace RestaurantDesktop
{
    /// <summary>
    /// Логика взаимодействия для CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        RestaurantTableAdapter restaurants = new RestaurantTableAdapter();
        PositionTableAdapter positions = new PositionTableAdapter();
        OrderTableAdapter orders = new OrderTableAdapter();
        MenuTableAdapter menus = new MenuTableAdapter();
        public static int idRest;
        public static double itog;
        public static List<PositionRow> positionsInOrder = new List<PositionRow>();
        OrdersDataTableAdapter view = new OrdersDataTableAdapter();
        MenuDataTableAdapter menuView = new MenuDataTableAdapter();
        public CustomerWindow()
        {
            itog = 0;
            InitializeComponent();
            restaurantCombo.ItemsSource = restaurants.GetData().Select(x => x.Rest_Addr);
            try
            {
                ordersOfUser.ItemsSource = view.GetData().Where(x => x.Логин_пользователя == MainWindow.curUser.User_Login).CopyToDataTable().DefaultView;
            }
            catch(Exception ex)
            {

            }
        }

        private void addToBasket_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var val = positions.GetData().First(x => x.Position_Name == posCombo.SelectedItem.ToString());
                basket.Items.Add(val.Position_Name + "-" + val.Position_Price);
                itog += val.Position_Price;
                positionsInOrder.Add(val);
                sum.Text = $"Общая сумма: {itog}";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void restaurantCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                idRest = restaurants.GetData().FirstOrDefault(x => x.Rest_Addr == restaurantCombo.SelectedItem.ToString()).ID_Restaurant;
                posCombo.ItemsSource = menuView.GetData().Where(x => x.Адрес_ресторана.Equals(restaurantCombo.SelectedItem.ToString())).Select(y => y.Название_товара);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void deleteFromBasket_Click(object sender, RoutedEventArgs e)
        {
            if (basket.SelectedItem != null)
            {
                itog -= GetPrice(basket.SelectedItem.ToString().Split('-')[0]);
                sum.Text = $"Общая сумма: {itog}";
                positionsInOrder.Remove(positions.GetData().First(x => x.Position_Name == basket.SelectedItem.ToString().Split('-')[0]));
                basket.Items.Remove(basket.SelectedItem);
            }
            else MessageBox.Show("Выберите элемент прежде чем его удалять!");
        }

        private double GetPrice(string name)
        {
            return positions.GetData().Where(x => x.Position_Name == name).FirstOrDefault().Position_Price;
        }

        private void createOrder_Click(object sender, RoutedEventArgs e)
        {
            OptionalsWindow optionals = new OptionalsWindow();
            optionals.Show();
        }

        private void ordersOfUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {

            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
