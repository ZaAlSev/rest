using RestaurantDesktop.Restaurant_DB1DataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для OptionalsWindow.xaml
    /// </summary>
    public partial class OptionalsWindow : Window
    {
        OrderTableAdapter orders = new OrderTableAdapter();
        Payment_MethodTableAdapter methods = new Payment_MethodTableAdapter();
        PaymentTableAdapter payments = new PaymentTableAdapter();
        Positions_In_OrderTableAdapter posInOrd = new Positions_In_OrderTableAdapter();
        PositionTableAdapter positions = new PositionTableAdapter();
        HallTableAdapter halls = new HallTableAdapter();
        TableTableAdapter tables = new TableTableAdapter();

        public static int Count;
        public static DateTime dateSt;
        public static TimeSpan timeSt;
        public static int methodSt;
        public static int idTable, idHall;

        public OptionalsWindow()
        {
            InitializeComponent();
            paymentCombo.ItemsSource = methods.GetData().Select(x => x.Method_Name);
            tableCombo.ItemsSource = tables.GetData().Select(x => x.Table_Num);
            hallCombo.ItemsSource = halls.GetData().Select(x => x.Hall_Name);
        }

        private void newOrder_Click(object sender, RoutedEventArgs e)
        {
            if(paymentCombo.Text == "В ресторане")
            {
                if (time.Text != null && date.SelectedDate != null && count.Text != null)
                {

                    Random random = new Random();
                    SqlConnection connection = payments.Connection;
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;
                    try
                    {
                        command.CommandText = $"insert into payment values ('{random.Next(100000000, 999999999)}', '{DateTime.Now.Date}', '{DateTime.Now.TimeOfDay}', {methods.GetData().First(x => paymentCombo.SelectedItem.ToString() == x.Method_Name).ID_Method}); SELECT SCOPE_IDENTITY(); ";
                        int newPaymentId = Convert.ToInt32(command.ExecuteScalar());
                        command.CommandText = $"insert into [order] values ('{random.Next(100000000, 999999999)}', {int.Parse(count.Text)}, '{(DateTime)date.SelectedDate}', '{TimeSpan.Parse(time.Text)}', {CustomerWindow.itog}, {MainWindow.curUser.ID_User}, {halls.GetData().FirstOrDefault(x => x.Hall_Name == hallCombo.Text).ID_Hall}, {tables.GetData().FirstOrDefault(x => x.Table_Num == tableCombo.Text).ID_Table}, {newPaymentId}, {CustomerWindow.idRest}); SELECT SCOPE_IDENTITY();";
                        int newOrderId = Convert.ToInt32(command.ExecuteScalar());
                        foreach(var value in CustomerWindow.positionsInOrder)
                        {
                            command.CommandText = $"insert into Positions_In_Order values ({value.ID_Position}, {newOrderId})";
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        connection.Close();
                        MessageBox.Show("Заказ успешно оформлен!");
                        StreamWriter fs = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\logs.txt", true);
                        fs.Write($"Новый заказ под ID {orders.GetData().Last().ID_Order} был оформлен пользователем {MainWindow.curUser.User_Login} в {DateTime.Now}\n");
                        fs.Close();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        transaction.Rollback();
                    }
                }
            }
            else
            {
                Count = int.Parse(count.Text);
                dateSt = (DateTime)date.SelectedDate;
                timeSt = TimeSpan.Parse(time.Text);
                methodSt = methods.GetData().First(x => paymentCombo.SelectedItem.ToString() == x.Method_Name).ID_Method;
                idTable = tables.GetData().FirstOrDefault(x => x.Table_Num == tableCombo.Text).ID_Table;
                idHall = halls.GetData().FirstOrDefault(x => x.Hall_Name == hallCombo.Text).ID_Hall;
                PaymentWindow p = new PaymentWindow();
                p.Show();
            }
        }
    }
}
