using ControlzEx.Standard;
using RestaurantDesktop.Restaurant_DB1DataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для PaymentWindow.xaml
    /// </summary>
    public partial class PaymentWindow : Window
    {
        OrderTableAdapter orders = new OrderTableAdapter();
        Payment_MethodTableAdapter methods = new Payment_MethodTableAdapter();
        PaymentTableAdapter payments = new PaymentTableAdapter();
        Positions_In_OrderTableAdapter posInOrd = new Positions_In_OrderTableAdapter();
        PositionTableAdapter positions = new PositionTableAdapter();
        public PaymentWindow()
        {
            InitializeComponent();
        }

        private void cardNum_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void date_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void payButton_Click(object sender, RoutedEventArgs e)
        {
            if(cardNum.Text.Length == 16 && date.Text.Length == 4 && cvc.Text.Length == 3)
            {
                Random random = new Random();
                payments.Insert(random.Next(100000000, 999999999).ToString(), DateTime.Now.Date, DateTime.Now.TimeOfDay, OptionalsWindow.methodSt);
                orders.Insert(random.Next(100000000, 999999999).ToString(), OptionalsWindow.Count, OptionalsWindow.dateSt, OptionalsWindow.timeSt, CustomerWindow.itog, MainWindow.curUser.ID_User, OptionalsWindow.idHall, OptionalsWindow.idTable, payments.GetData().Last().ID_Payment, CustomerWindow.idRest);
                foreach (var value in CustomerWindow.positionsInOrder)
                {
                    posInOrd.Insert(value.ID_Position, orders.GetData().Last().ID_Order);
                }
                MessageBox.Show("Заказ успешно оформлен!");
                StreamWriter fs = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\logs.txt", true);
                fs.Write($"Новый заказ под ID {orders.GetData().Last().ID_Order} был оформлен пользователем {MainWindow.curUser.User_Login} в {DateTime.Now}\n");
                fs.Close();
                Close();
                Close();
            }
        }
    }
}
