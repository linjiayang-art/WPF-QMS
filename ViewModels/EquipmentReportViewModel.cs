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
using Prism.Regions;
using SicoreQMS.Extensions;
using Prism.Services.Dialogs;
using Prism.Ioc;


namespace SicoreQMS.ViewModels
{
    public class EquipmentReportViewModel: BindableBase
    {
        private readonly IEventAggregator aggregator;

        #region 属性


        public DelegateCommand<EquipmentDateModel> RowClickCommand { get; private set; }
     
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

        private string _equipmentNo;

        public string EquipmentNo
        {
            get => _equipmentNo;
            set => SetProperty(ref _equipmentNo, value);

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
        public IDialogService dialogService { get; set; }


        #endregion

        public EquipmentReportViewModel(IDialogService dialogService=null)
        {
         
            ReportData = new ObservableCollection<EquipmentDateModel>();
            BtnExecute=new DelegateCommand<string>(Execute);
            this.StartDate = DateTime.Today.AddDays(-30); ;
            RowClickCommand = new DelegateCommand<EquipmentDateModel>(ExecuteCommand);
            //往前30天
            this.EndDate=DateTime.Today;
            this.EquipmentNo = "";
            this.EquipmentType = "";
            //this.aggregator = aggregator;
            LoadTestData(this.StartDate, this.EndDate);  // 加载1月份的数据
            this.dialogService = dialogService;

            //GenerateColumnsAndLoadData(DateTime.Today.AddDays(-7), DateTime.Today);
            //GenerateColumnsAndLoadData(DateTime.Today.AddDays(-7), DateTime.Today);
        }

        private void ExecuteCommand(EquipmentDateModel model)
        {
            if (this.dialogService is null)
            {
                this.dialogService = ContainerLocator.Current.Resolve<IDialogService>();


            }

            var parameters = new DialogParameters();
            parameters.Add("equipmentNo", model.EquipmentNo);
            parameters.Add("startDate", StartDate);
            parameters.Add("endDate", this.EndDate);
            var Resultlist= Service.EquipmentService.GetEquipmentUsageDetails(model.EquipmentNo, StartDate, EndDate);
            if (Resultlist.Count==0)
            {
                MessageBox.Show("无设备运行数据！");
                return;
            }
            dialogService.ShowDialog("EquipmentUsageDetailView",parameters,result =>
                    {
                       
                    });
   
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
       
            ReportData =Service.EquipmentService.GetEquipmentReport(startDate, endDate, this.EquipmentType, this.EquipmentNo);
      
           
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
