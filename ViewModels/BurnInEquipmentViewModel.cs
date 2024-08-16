
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
using System.IO;
using SicoreQMS.Service;
using SicoreQMS.Common.Models.Operation;
using DocumentFormat.OpenXml.EMMA;
using System.Runtime.InteropServices.ComTypes;


namespace SicoreQMS.ViewModels
{
  public class BurnInEquipmentViewModel:BindableBase
    {
        #region

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
        public BurnInEquipmentViewModel(IDialogService dialogService = null)
        {
            ReportData = new ObservableCollection<EquipmentDateModel>();
            BtnExecute = new DelegateCommand<string>(Execute);
            this.StartDate = DateTime.Today.AddDays(-30); ;
            RowClickCommand = new DelegateCommand<EquipmentDateModel>(ExecuteCommand);
            //往前30天
            this.EndDate = DateTime.Today.AddDays(1);
            this.EquipmentNo = "";
            this.EquipmentType = "";
            //this.aggregator = aggregator;
            LoadTestData(this.StartDate, this.EndDate);  // 加载1月份的数据
            this.dialogService = dialogService;

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
            var Resultlist = Service.EquipmentService.GetEquipmentUsageDetails(model.EquipmentNo, StartDate, EndDate);
            if (Resultlist.Count == 0)
            {
                MessageBox.Show("无设备运行数据！");
                return;
            }
            dialogService.ShowDialog("EquipmentUsageDetailView", parameters, result =>
            {

            });

        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "query":
                    LoadTestData(this.StartDate, this.EndDate);
                    break;
                case "export":
                    ExportRunFile();
                    break;
                default:
                    break;
            }
        }

        private void ExportRunFile()
        {

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(desktopPath, "机台使用记录.xlsx");
            var equipmentList = Service.EquipmentService.GetEquipmentUsageDetails(this.StartDate, this.EndDate);
            ExcelExporter.ExportToExcel(ReportData, equipmentList, fullPath);

            MessageBox.Show("导出成功！");
        }

        public void LoadTestData(DateTime startDate, DateTime endDate)
        {

            ReportData = Service.EquipmentService.GetEquipmentReport(startDate, endDate, this.EquipmentType, this.EquipmentNo,equipmentType:"1");


        }


    }
}
