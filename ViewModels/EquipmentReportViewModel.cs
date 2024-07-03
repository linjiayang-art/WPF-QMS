using LiveCharts.Wpf;
using LiveCharts;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Dynamic;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Windows.Forms;
using SicoreQMS.Common.Models.Report;
using Prism.Commands;
using DocumentFormat.OpenXml.Drawing.Diagrams;


namespace SicoreQMS.ViewModels
{
    public class EquipmentReportViewModel: BindableBase
    {
        private readonly IEventAggregator aggregator;

        #region 属性
     
        private DateTime _startDate;

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);

        }
        private DateTime _endDate;

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);

        }

        private string _equipmentType;

        public string EquipmentType
        {
            get => _equipmentType;
            set => SetProperty(ref _equipmentType, value);

        }


        public class ReportDataItem
        {
            public DateTime Date { get; set; }
            public double SomeMetric { get; set; }
        }
        private ObservableCollection<EquipmentDateModel> _reportData;
        public ObservableCollection<EquipmentDateModel> ReportData
        {
            get { return _reportData; }
            set { SetProperty(ref _reportData, value); }
        }

        public DelegateCommand<string> BtnExecute { get; private set; }

        #endregion

        public EquipmentReportViewModel()
        {
            ReportData = new ObservableCollection<EquipmentDateModel>();
            BtnExecute=new DelegateCommand<string>(Execute);
            this.StartDate = DateTime.Today.AddDays(-30); ;
            //往前30天
            this.EndDate=DateTime.Today;
            //this.aggregator = aggregator;
            LoadTestData(this.StartDate, this.EndDate);  // 加载1月份的数据
            //GenerateColumnsAndLoadData(DateTime.Today.AddDays(-7), DateTime.Today);
            //GenerateColumnsAndLoadData(DateTime.Today.AddDays(-7), DateTime.Today);
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "query":
                    LoadTestData(this.StartDate,this.EndDate);
                    break;
                default:
                    break;
            }
        }

        public void LoadTestData(DateTime startDate, DateTime endDate)
        {
            
            ReportData=Service.EquipmentService.GetEquipmentReport(startDate, endDate);
      
            //Random random = new Random();
            //EquipmentDateModel model = new EquipmentDateModel
            //{
            //    Equipment = "测试设备",
            //    Model = "型号X"
            //};

            //for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            //{
            //    // 假设每天的数据是随机的一个值
            //    if (!model.DailyData.ContainsKey(date))
            //    {
            //        model.DailyData.Add(date, random.Next(0, 100).ToString());
            //    }
            //}

            //ReportData.Add(model);
        }



        #region
        //public EquipmentReportViewModel(IEventAggregator aggregator)
        //{
        //    this.aggregator = aggregator;
        //    SeriesCollection = new SeriesCollection();
        //    XAxis = new AxesCollection();
        //    YAxis = new AxesCollection();
        //    LoadData();
        //    //GenerateColumnsAndLoadData(DateTime.Today.AddDays(-7), DateTime.Today);
        //}
        //public SeriesCollection SeriesCollection { get; set; }
        //public AxesCollection XAxis { get; set; }
        //public AxesCollection YAxis { get; set; }

        //private void LoadData()
        //{
        //    var random = new Random();

        //    SeriesCollection = new SeriesCollection
        //{
        //    new LineSeries
        //    {
        //        Title = "Daily Values",
        //        Values = new ChartValues<double>
        //        {
        //            random.Next(100, 500),
        //            random.Next(100, 500),
        //            random.Next(100, 500),
        //            random.Next(100, 500),
        //            random.Next(100, 500),
        //            random.Next(100, 500),
        //            random.Next(100, 500)
        //        }
        //    }
        //};

        //    XAxis = new AxesCollection
        //{
        //    new Axis
        //    {
        //        Title = "日期",
        //        Labels = Enumerable.Range(0, 7)
        //                           .Select(i => DateTime.Today.AddDays(-i).ToShortDateString())
        //                           .Reverse()
        //                           .ToArray()
        //    }
        //};

        //    YAxis = new AxesCollection
        //{
        //    new Axis
        //    {
        //        Title = "Value",
        //        LabelFormatter = value => value.ToString("N")
        //    }
        //};

        //    RaisePropertyChanged(nameof(SeriesCollection));
        //    RaisePropertyChanged(nameof(XAxis));
        //    RaisePropertyChanged(nameof(YAxis));
        //}
        #endregion
    }
}
