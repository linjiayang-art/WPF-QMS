using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Models.Report;
using SicoreQMS.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace SicoreQMS.ViewModels
{
    public class IndexViewModel:BindableBase
    {

        #region
        private string lot;
        private string _prodNo;
        private string prodType;

        public DelegateCommand<TestCountReport>  BtnCommand { get; private set; }
        public DelegateCommand<TestCount> TestExecuetCommand { get; private set; }
        public DelegateCommand<TestCount> TestEditCommand { get; private set; }
        public DelegateCommand<string> ExecuetCommand { get; private set; }
     

        public ObservableCollection<TestCountReport> _testReportList { get; set; }

        public ObservableCollection<TestCountReport>  TestReportList { 
            get { return _testReportList; }
            set { _testReportList = value; RaisePropertyChanged(); } }


        //private ObservableCollection<TestCount> newtest;

        //public ObservableCollection<TestCount>  NewTest
        //{
        //    get { return newtest; }
        //    set { newtest = value; RaisePropertyChanged(); }
        //}



        private ObservableCollection<TestCount> testItems;

        public ObservableCollection<TestCount> TestItems
        {
            get { return testItems; }
            set { testItems = value; RaisePropertyChanged(); }
        }


      

        public string ProdNo
        {
            get => _prodNo;
            set => SetProperty(ref _prodNo, value);

        }


        public string ProdType
        {
            get { return prodType; }
            set {SetProperty( ref prodType,value ); }
        }


        

        public string Lot
        {
            get { return lot; }
            set { SetProperty(ref lot, value); }
        }

        public IDialogService Dialog { get; }
        public IEventAggregator Aggregator { get; }
        #endregion


        public IndexViewModel(IDialogService dialog,IEventAggregator aggregator)
        {
            ProdType = "";
            ProdNo = "";
            Lot = "";
            TestReportList = Service.IndexService.GetTestCountReport();
            ExecuetCommand=new DelegateCommand<string>(Execuet);
            BtnCommand= new DelegateCommand<TestCountReport>(BtnExecute);
            TestExecuetCommand=new DelegateCommand<TestCount>(TestExecute);
            TestEditCommand=new DelegateCommand<TestCount>(TestEdit);
            Dialog = dialog;
            Aggregator = aggregator;
        }

        private void TestEdit(TestCount count)
        {

            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id", count.Id }

            };

            Dialog.ShowDialog("TestStatusEditView", dialogParameters, result =>
            {
                GetList();
                //var newid = result.Parameters.GetValue<string>("NewId");
                //Console.WriteLine(newid);

            });
        }

        private void BtnExecute(TestCountReport report)
        {
            Service.IndexService.DelProd(report);
            TestReportList = Service.IndexService.GetTestCountReport();
            return;
        
        }
        private void TestExecute(TestCount report)
        {
          var a=   Service.IndexService.DelTestProcess(report.Id);
            if (a==true)
            {
                Aggregator.SendMessage("删除成功");
            }
            else
            {
                Aggregator.SendMessage("删除失败");
            }
            return;

        }


        private void Execuet(string obj)
        {
            switch (obj)
            {
                case "查询": GetList();break;

                case "导出":ExportFile();break;
            }


        }

        private void ExportFile()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 使用 Path.Combine 构建完整的文件路径
            string fullPath = Path.Combine(desktopPath, "testresult.xlsx");
            Service.IndexService.ExportTestCountReportListToExcel( TestReportList, fullPath);

            MessageBox.Show("导出数据");
        }

        public void GetList()
        {

            _testReportList.Clear();
            TestReportList = Service.IndexService.GetTestCountReport(prodType:prodType,lot:Lot,prodNo:ProdNo);
            //TestItems = Service.IndexService.GetTestItems(prodType: prodType, lot: Lot);
            TestItems = Service.IndexService.GetTestCounts(prodType: prodType, lot: Lot);
        
        }
    }
}
