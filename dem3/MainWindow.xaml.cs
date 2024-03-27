using dem3.UserForm.Form;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace dem3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Counter = 0;
        public MainWindow()
        {
            InitializeComponent();
            Data("","");
        }
        public void Data(string Filter, string Sort)
        {
            Flower.Children.Clear();
            using (SqlConnection Connection = new SqlConnection($@"Data Source = DESKTOP-7ONBNPB; Initial Catalog = Flowers; Integrated Security = true;"))
            {
                Connection.Open();
                SqlCommand Command = new SqlCommand($@"SELECT [Title],[Cost],[MaxDiscount],[TitleManufacturer],[Discription],[Image]
  FROM [Flowers].[dbo].[Products],[Flowers].[dbo].[Manufacturers]
  where Products.ManufacturerCode = Manufacturers.ManufacturerCode {Filter + Sort}",Connection);
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        UserForm.FlowerProduct flowerProduct= new UserForm.FlowerProduct();
                        flowerProduct.Title.Content = Reader[0];
                        flowerProduct.Cost.Content = Reader[1];
                        flowerProduct.Discount.Content = Reader[2];
                        flowerProduct.Manufacturer.Content = Reader[3];
                        flowerProduct.Discription.Content = Reader[4];
                        double Price = Convert.ToInt32(Reader[1]);
                        double CurrntDiscount = Convert.ToInt32(Reader[2]);
                        double FinalPrice = Price /100 * CurrntDiscount;
                        Price = Price - FinalPrice;
                        if (CurrntDiscount > 15)
                        {
                            flowerProduct.ProductBackground.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#7fff00");
                            flowerProduct.FinalCost.Content = Math.Round(FinalPrice,2);
                            flowerProduct.FinalCost.Visibility= Visibility.Visible;
                            flowerProduct.Line.Visibility= Visibility.Visible;
                        }
                        try
                        {
                            flowerProduct.Image.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + Reader[5].ToString()));
                        }catch(Exception) { }
                        Counter++;
                        Flower.Children.Add(flowerProduct);
                    }
                }
            }
            QuantityAndMaximum();
        }
        public void QuantityAndMaximum()
        {
            Label Quantity = new Label();
            QuantityWrapPanel.Children.Clear();
            Quantity.Content = Convert.ToInt32(Counter);
            Quantity.Width = 50;
            Quantity.Height = 50;
            Quantity.FontSize = 20;
            QuantityWrapPanel.Children.Add(Quantity);
            Counter = 0;
            using (SqlConnection Connection = new SqlConnection($@"Data Source = DESKTOP-7ONBNPB; Initial Catalog = Flowers; Integrated Security = true;"))
            {
                Connection.Open();
                SqlCommand Command = new SqlCommand($@"SELECT Count(DISTINCT[Title]) FROM [Flowers].[dbo].[Products]", Connection);
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    while (Reader.Read())
                    {
                        MaxRecord.Content = Reader[0];
                    }
                }
            }
        }

        private void Filtering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FunctionFiltering();
        }
        public void FunctionFiltering()
        {
            switch (Filtering.SelectedIndex)
            {
                case 0:
                    Data("","");
                    break;
                case 1:
                    Data(" and [MaxDiscount] between 0 and 9.99", "");
                    break;
                case 2:
                    Data(" and [MaxDiscount] between 10 and 14.99", "");
                    break;
                case 3:
                    Data(" and [MaxDiscount] > 15", "");
                    break;
            }
        }

        private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FunctionSorting();
        }
        public void FunctionSorting()
        {
            switch (Sorting.SelectedIndex)
            {
                case 0:
                    Data("", " Order by [Cost] asc");
                    break;
                case 1:
                    Data("", " Order by [Cost] desc");
                    break;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
           EntranceWindow Entrance = new EntranceWindow();
            Entrance.Show();
            this.Close();
        }
    }
}
