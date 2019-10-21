using Domain.DAL;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using System.Reflection;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Diagnostics;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DBLoader dbLoader;

        public MainWindow()
        {
            InitializeComponent();

            dateStart.SelectedDate =DateTime.Now.Subtract(new TimeSpan(1,0,0,0));
            dateEnd.SelectedDate = DateTime.Now;

            dbLoader = new DBLoader(new ViDBContext());
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private async void ButtonDateDiffDiscount_Click(object sender, RoutedEventArgs e)
        {
            ButtonDateDiffDiscount.IsEnabled = false;
            if (dateStart.SelectedDate == null || dateEnd.SelectedDate == null)
            {
               MessageBox.Show("Некорректные даты!");
            }
            else
            {
                LoaderRect1.Visibility = Visibility.Visible;                
                IEnumerable<TwoDaysPriceDiffereceModel> some = await dbLoader.GetDiffernceAsync(
                                                        dateStart.SelectedDate ?? DateTime.Now.Subtract(new TimeSpan(1,0,0,0)), 
                                                        dateEnd.SelectedDate ?? DateTime.Now , 
                                                        (float)sliderPercent.Value, 
                                                        CheckBoxChoosen.IsChecked ?? false
                                                        );

                TwoDaysDifferenceDataGrid.ItemsSource = some;
                LoaderRect1.Visibility = Visibility.Collapsed;            
            }
            ButtonDateDiffDiscount.IsEnabled = true;
        }

        private void ButtonSearchClick(object sender, RoutedEventArgs e)
        {
            this.ButtonSearch.IsEnabled = false;
            if (!String.IsNullOrEmpty(TextBoxSearch.Text))
            {                
                DataGridSearched.ItemsSource = dbLoader.GetSearchResult(TextBoxSearch.Text);
            }
            this.ButtonSearch.IsEnabled = true;
        }

        private async void DataGridSearched_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            DataGrid dgv = (DataGrid)sender;
            ProductSearchResultModel model = (ProductSearchResultModel)dgv.SelectedItem;
            
            if (model!= null)
            {
                string messageText = "";
                if (model.IsFavorite == "-")
                {
                    messageText = "Добавить товар \"" + model.ProductName + " \" в избранное?";
                }
                else
                {
                    messageText = "Убрать товар \"" + model.ProductName + " \" из избранного?";
                }
                var result =MessageBox.Show(messageText, "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await dbLoader.SaveFavoriteAsync(model);
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(ButtonSearch);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv.Invoke();
                }               
            }         
        }

        private async void Button_ButtonGraphic(object sender, RoutedEventArgs e)
        {
            ButtonGraphic.IsEnabled = false;
            bool isDaily = (bool)CheckBoxChoosenDaily.IsChecked;
            bool isFavorite = (bool)CheckBoxFavorites.IsChecked;
            bool isNonConst = (bool)CheckBoxDynamicOnly.IsChecked;
            //IEnumerable<PriceDynamicModel> some = await dbLoader.GetDynamicAsync(true);
            //IEnumerable<PriceDynamicModel> some = await dbLoader.GetDynamicAsync(true);
            //var some = await dbLoader.GetDynamicAsync(true);
            var some = await dbLoader.GetDynamicAsync(isDaily, isFavorite, isNonConst, TextBoxSearchPriceDynamic.Text);
            //some = 
            DataGridPriceDynamic.ItemsSource = some;

            var defs = DataGridPriceDynamic.Columns;

            int i = 0;

            List<DateTime> dates = await dbLoader.GetDatesAsync(isDaily);

            foreach (var item in defs)
            {
                if (item.Header.ToString().Contains("Date"))
                {

                    //item.Header = "Дата" + i;
                    item.Header = dates[i].Date.ToString("dd.MM.yyyy");
                    i++;
                }
                
            }
            ButtonGraphic.IsEnabled = true;

        }


        private IEnumerable<DateTime> GetModelDates()
        {
            return null;
        }

        private IEnumerable<DateTime> GetModelPrices()
        {
            return null;
        }

        private void DataGridPriceDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dgv = (DataGrid)sender;
            PriceDynamicModel model = (PriceDynamicModel)dgv.SelectedItem;

            MainChart.Model.Series.Clear();
            MainChart.Model.Axes.Clear();
            MainChart.Model.Title = "Товар не выбран";
            MainChart.Model.InvalidatePlot(true);

            if (model != null)
            {
                bool isDaily = (bool)CheckBoxChoosenDaily.IsChecked;


                PropertyInfo[] props = model.GetType().GetProperties(BindingFlags.Public |
                                                  BindingFlags.NonPublic |
                                                  BindingFlags.Instance).OrderBy(x=>x.Name).ToArray();
            
                List<int> prices = new List<int>();           
                           
                foreach (PropertyInfo item in props)
                {
                    if (item.Name.Contains("Date") && item.Name.Length > 4 && item.Name.Length < 7)
                    {
                        try
                        {
                            int? a = (int)item.GetValue(model);
                             prices.Add(a ?? 0);                            
                        }
                        catch (Exception)
                        {                        
                        }
                    }
                }

                List<DateTime> dates = dbLoader.GetTopStatistic(isDaily, prices.Count).Select(x=>x.CreationDate).ToList();

                if (dates.Count != 0 && dates.Count==prices.Count)
                {
                    MainChart.Model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = DateTimeAxis.ToDouble(dates.First().Date), Maximum = DateTimeAxis.ToDouble(dates.Last().Date), StringFormat = "dd-MM-yyyy", MinimumMinorStep = 1, MinimumMajorStep=1 });
                    MainChart.Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = LinearAxis.ToDouble(prices.Min()*0.95), Maximum = LinearAxis.ToDouble(prices.Max() * 1.05), MinimumMinorStep = 1 });
                    var areaSeries = new AreaSeries();

                    if (prices.Count == dates.Count)
                    {
                        for (int i = 0; i < prices.Count; i++)
                        {
                            DataPoint dp = new DataPoint(DateTimeAxis.ToDouble(dates[i]), (double)prices[i]);
                            areaSeries.Points.Add(dp);
                        }
                        MainChart.Model.Series.Add(areaSeries);
                        MainChart.Model.Title = model.Name;
                    }
           
                    MainChart.Model.InvalidatePlot(true);
                }
            }
        }

        private async Task GetUrlAsync(int code)
        {
            string url = await dbLoader.GetUrlAsync(code);
            if (!String.IsNullOrEmpty(url))
            {
                Process.Start(url);
            }
         }
        

        private async void DataGridPriceDynamic_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dgv = (DataGrid)sender;
            PriceDynamicModel model = (PriceDynamicModel)dgv.SelectedItem;
            await GetUrlAsync(model.Code);

            //MessageBox.Show(model.Code.ToString());
        }

        private async void TwoDaysDifferenceDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dgv = (DataGrid)sender;
            TwoDaysPriceDiffereceModel model = (TwoDaysPriceDiffereceModel)dgv.SelectedItem;
            await GetUrlAsync(model.Code);
        }       

        //TODO реализовать удаление вендора
        private void DataGridVendorForDelete_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        //TODO реализовать добавление нового вендора

        private async void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            var some = await dbLoader.GetVendorsAsync();
            DataGridVendors.ItemsSource = some;
        }
    }
}
