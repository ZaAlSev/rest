using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignColors.Recommended;
using Microsoft.Win32;
using RestaurantDesktop.Restaurant_DB1DataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using RestaurantDesktop;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.SqlEnum;
using System.Configuration;
using System.Collections.Specialized;
using Table = Microsoft.SqlServer.Management.Smo.Table;

namespace RestaurantDesktop
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        OrderTableAdapter orders = new OrderTableAdapter();
        PaymentTableAdapter payments = new PaymentTableAdapter();
        Payment_MethodTableAdapter methods = new Payment_MethodTableAdapter();
        PositionTableAdapter positions = new PositionTableAdapter();
        UserTableAdapter users = new UserTableAdapter();
        RestaurantTableAdapter rests = new RestaurantTableAdapter();
        MenuTableAdapter menu = new MenuTableAdapter();
        MenuDataTableAdapter menuView = new MenuDataTableAdapter();
        TableTableAdapter tables = new TableTableAdapter();
        HallTableAdapter halls = new HallTableAdapter();
        QueriesTableAdapter queries = new QueriesTableAdapter();
        public ChartValues<double> values = new ChartValues<double>();
        public List<String> Labels = new List<string>();
        public SeriesCollection SeriesCollection { get; set; }
        public AdminWindow()
        {
            InitializeComponent();
            orderCombo.ItemsSource = orders.GetData().Select(x => x.Order_Num.ToString());
            comboPos.ItemsSource = positions.GetData().Select(x => x.Position_Name);
            comboRes.ItemsSource = rests.GetData().Select(x => x.Rest_Addr);
            SqlConnection connection = users.Connection;
            connection.Open();
            foreach (DataRow row in connection.GetSchema("Tables").Rows)
            {
                string tablename = (string)row[2];
                comboTable.Items.Add(tablename);
            }
            connection.Close();
            DataContext = this;
            xAxis.Labels = Labels;
            chart.Series = SeriesCollection;
        }

        private void checkPayment_Click(object sender, RoutedEventArgs e)
        {
            if(orderCombo.SelectedItem != null)
            {
                var orderChoosed = orders.GetData().FirstOrDefault(x => x.Order_Num == orderCombo.Text);
                MessageBox.Show($"К данному заказу привязана оплата № {payments.GetData().FindByID_Payment(orderChoosed.Payment_ID).Payment_Num} на сумму: {orderChoosed.Order_Sum} от пользователя {users.GetData().FindByID_User(orderChoosed.User_ID).User_Login}. Метод оплаты: {methods.GetData().FindByID_Method(payments.GetData().FindByID_Payment(orderChoosed.Payment_ID).Method_ID).Method_Name}");
            }
            else
            {
                MessageBox.Show("Выберите заказ для проверки оплаты!");
            }
        }

        private void addPos_Click(object sender, RoutedEventArgs e)
        {
            positions.Insert(posName.Text, Double.Parse(posPrice.Text));
            posName.Clear();
            posPrice.Clear();
            MessageBox.Show("Блюдо успешно добавлено в систему!");
            comboPos.ItemsSource = positions.GetData().Select(x => x.Position_Name);
        }

        private void deletePos_Click(object sender, RoutedEventArgs e)
        {
            var posToDel = positions.GetData().FirstOrDefault(x => x.Position_Name == comboPos.Text);
            positions.Delete(posToDel.ID_Position, posToDel.Position_Price);
            comboPos.SelectedItem = null;
            MessageBox.Show("Блюдо успешно удалено из системы!");
            comboPos.ItemsSource = positions.GetData().Select(x => x.Position_Name);
            if (comboRes.SelectedItem != null)
                menuGrid.ItemsSource = menuView.GetData().Where(x => x.Адрес_ресторана.Equals(comboRes.SelectedItem.ToString())).CopyToDataTable().DefaultView;
        }

        private void addMenu_Click(object sender, RoutedEventArgs e)
        {
            var restToAdd = rests.GetData().FirstOrDefault(x => x.Rest_Addr == comboRes.Text);
            var posToAdd = positions.GetData().FirstOrDefault(x => x.Position_Name == comboPos.Text);
            queries.InsertNewMenuPosition(posToAdd.ID_Position, restToAdd.ID_Restaurant);
            if (comboRes.SelectedItem != null)
                menuGrid.ItemsSource = menuView.GetData().Where(x => x.Адрес_ресторана.Equals(comboRes.SelectedItem.ToString())).CopyToDataTable().DefaultView;
            comboRes.SelectedItem = null;
            comboPos.SelectedItem = null;
            MessageBox.Show("Блюдо успешно добавлено в меню!");
        }

        private void addRest_Click(object sender, RoutedEventArgs e)
        {
            rests.Insert(addrText.Text, int.Parse(countText.Text));
            addrText.Clear();
            countText.Clear();
            MessageBox.Show("Ресторан успешно добавлен!");
            comboRes.ItemsSource = rests.GetData().Select(x => x.Rest_Addr);
        }

        private void addTable_Click(object sender, RoutedEventArgs e)
        {
            tables.Insert(valText.Text);
            valText.Clear();
            MessageBox.Show("Столик успешно добавлен!");
        }

        private void addHall_Click(object sender, RoutedEventArgs e)
        {
            halls.Insert(valText.Text);
            valText.Clear();
            MessageBox.Show("Зал успешно добавлен!");
        }

        private void comboRes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(comboRes.SelectedItem != null)
                    menuGrid.ItemsSource = menuView.GetData().Where(x => x.Адрес_ресторана.Equals(comboRes.SelectedItem.ToString())).CopyToDataTable().DefaultView;
            }
            catch (Exception ex)
            {

            }
        }

        private void backupButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(users.Connection.ConnectionString);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = sqlConnection;
                command.CommandText = $"Backup database {users.Connection.Database} to disk='C:\\Program Files\\Microsoft SQL Server\\MSSQL14.FIRSTSERV\\MSSQL\\Backup\\{users.Connection.Database}.bak'";
                command.ExecuteNonQuery();
                sqlConnection.Close();
                MessageBox.Show("Резервная копия БД успешно создана по пути C:\\Program Files\\Microsoft SQL Server\\MSSQL14.FIRSTSERV\\MSSQL\\Backup!");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exportCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(comboTable.SelectedItem != null)
                {
                    SqlConnection connection = users.Connection;
                    connection.Open();
                    SqlCommand sqlCmd = new SqlCommand($"Select * from {comboTable.Text}", connection);
                    SqlDataReader reader = sqlCmd.ExecuteReader();

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "CSV Files | *.csv";
                    saveFileDialog.DefaultExt = "csv";
                    saveFileDialog.ShowDialog();
                    StreamWriter sw = new StreamWriter(saveFileDialog.FileName);
                    object[] output = new object[reader.FieldCount];

                    for (int i = 0; i < reader.FieldCount; i++)
                        output[i] = reader.GetName(i);
                    sw.WriteLine(string.Join(",", output));
                    while (reader.Read())
                    {
                        reader.GetValues(output);
                        sw.WriteLine(string.Join(",", output));
                    }
                    sw.Close();
                    reader.Close();
                    connection.Close();
                    MessageBox.Show("Успешный экспорт в CSV!");
                    Process.Start(saveFileDialog.FileName);
                }
                else
                {
                    MessageBox.Show("Выберите таблицу для экспорта данных в CSV!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void exportSQL_Click(object sender, RoutedEventArgs e)
        {
            Server srv = new Server();
            srv.ConnectionContext.ServerInstance = users.Connection.DataSource;
            srv.ConnectionContext.DatabaseName = users.Connection.Database;
            string dbName = users.Connection.Database;

            Database db = new Database();
            db = srv.Databases[dbName];

            StringBuilder sb = new StringBuilder();

            foreach (Table tbl in db.Tables)
            {
                if(tbl.Name == comboTable.Text)
                {
                    ScriptingOptions options = new ScriptingOptions();
                    options.ClusteredIndexes = true;
                    options.Default = true;
                    options.DriAll = true;
                    options.Indexes = true;
                    options.IncludeHeaders = true;

                    StringCollection coll = tbl.Script(options);
                    foreach (string str in coll)
                    {
                        sb.Append(str);
                        sb.Append(Environment.NewLine);
                    }
                    StreamWriter fs = File.CreateText($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\output.txt");
                    fs.Write(sb.ToString());
                    fs.Close();
                    MessageBox.Show("Скрипт таблицы успешно экспортирован!");
                }
            }
        }

        private void addToDiagramm_Click(object sender, RoutedEventArgs e)
        {
            var sumToAdd = queries.GetSumOfOrdersByDate(dateChoose.SelectedDate);
            SeriesCollection = new SeriesCollection { };
            Labels.Add(dateChoose.SelectedDate.ToString());
            values.Add((double)sumToAdd);
            if(SeriesCollection.Count > 0)
            {
                SeriesCollection.Clear();
            }
            SeriesCollection.Add(new LineSeries
            {
                Title = "Продажи",
                Values = values
            });
            xAxis.Labels = Labels;
            chart.Series = SeriesCollection;
        }
    }
}
