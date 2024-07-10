using SicoreQMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SicoreQMS.Common.Models.Basic;
using DocumentFormat.OpenXml;
using Prism.Services.Dialogs;
using Prism.Ioc;
namespace SicoreQMS.Views
{
    /// <summary>
    /// EquipmentReportView.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentReportView : System.Windows.Controls.UserControl
    {

        public bool isCheck { get; set; } = false;
       
        public  EquipmentReportViewModel viewModel=new EquipmentReportViewModel();
        public EquipmentReportView()
        {
            InitializeComponent();
            this.startDate.SelectedDate = DateTime.Today;
            //var viewModel = new EquipmentReportViewModel();  // 创建 ViewModel
            this.DataContext = this.viewModel;               // 设置 DataContext
            dataGrid.ItemsSource = viewModel.ReportData; // 绑定 DataGrid 的 ItemsSource
            CreateColumns(this.startDate.SelectedDate ?? DateTime.Today, this.endDate.SelectedDate ?? DateTime.Today);
        }

        public void CreateColumns(DateTime startDate, DateTime endDate)
        {

            this.DataContext = this.viewModel;               // 设置 DataContext
            this.viewModel.LoadTestData( startDate,  endDate);
            dataGrid.ItemsSource = this.viewModel.ReportData; // 绑定 DataGrid 的 ItemsSource
            var fixedColumns = dataGrid.Columns.Take(4).ToList(); // 假设前4列是固定列
            dataGrid.Columns.Clear();

            // 重新添加固定的列
            foreach (var column in fixedColumns)
            {
                dataGrid.Columns.Add(column);
            }
            //dataGrid.Columns.Clear();
            //dataGrid.Columns.Add(new DataGridTextColumn { Header = "设备",Binding = new System.Windows.Data.Binding("Equipment")});
            //dataGrid.Columns.Add(new DataGridTextColumn { Header = "型号", Binding = new System.Windows.Data.Binding("Model") });
            //dataGrid.Columns.Add(new DataGridTextColumn { Header = "产品编号", Binding = new System.Windows.Data.Binding("EquipmentNo") });

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var column = new DataGridTextColumn
                {
                    Header = date.ToShortDateString(),
                    Binding = new System.Windows.Data.Binding($"DailyData[{date:yyyy-MM-dd}]")
                };
                // 设置单元格样式，动态绑定颜色
                column.ElementStyle = new Style(typeof(TextBlock));
                column.ElementStyle.Setters.Add(new Setter(TextBlock.BackgroundProperty, new System.Windows.Data.Binding($"ColorData[{date:yyyy-MM-dd}]")
                {
                    Converter = new Common.Models.Basic.ColorConverter() // 假设您有一个颜色转换器
                }));
                dataGrid.Columns.Add(column);
            }
        }
        //private void Item_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    var item = (DataGridRow)sender;
        //    FrameworkElement objElement = dataGrid.Columns[2].GetCellContent(item);
        //    if (objElement != null)
        //    {
        //        if(objElement.GetType() == typeof(System.Windows.Controls.TextBox))
        //        {
        //            return;
        //        }
        //        if (isCheck)
        //        {
        //            isCheck = false;
        //            return;
        //        }
        //        System.Windows.Controls.TextBlock text = (System.Windows.Controls.TextBlock)objElement;

        //        System.Windows.MessageBox.Show(text.Text);
        //        isCheck=true;
        //    }
        //}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var startDate = this.startDate.SelectedDate ?? DateTime.Today;
            var endDate = this.endDate.SelectedDate ?? DateTime.Today;

            //var viewModel = new EquipmentReportViewModel();  // 创建 ViewModel
          
            
            //this.DataContext = this.viewModel;               // 设置 DataContext
            //dataGrid.ItemsSource = this.viewModel.ReportData; // 绑定 DataGrid 的 ItemsSource
            CreateColumns(startDate, endDate);
        }
    }
}
