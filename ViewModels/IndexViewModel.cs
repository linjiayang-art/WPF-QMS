using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Models.Report;
using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Windows;


namespace SicoreQMS.ViewModels
{
    public class IndexViewModel:BindableBase
    {

        #region


        public DelegateCommand<TestCountReport>  BtnCommand { get; private set; }

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



        private string prodType;

        public string ProdType
        {
            get { return prodType; }
            set {SetProperty( ref prodType,value ); }
        }


        private string lot;

        public string Lot
        {
            get { return lot; }
            set { SetProperty(ref lot, value); }
        }
        #endregion


        public IndexViewModel()
        {
            ProdType = "";
            Lot = "";
            TestReportList = Service.IndexService.GetTestCountReport();
            ExecuetCommand=new DelegateCommand<string>(Execuet);
            BtnCommand= new DelegateCommand<TestCountReport>(BtnExecuet);
        }

        private void BtnExecuet(TestCountReport report)
        {
            Service.IndexService.DelProd(report);
            TestReportList = Service.IndexService.GetTestCountReport();
            return;
            throw new NotImplementedException();
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
            TestReportList = Service.IndexService.GetTestCountReport(prodType:prodType,lot:Lot);
            //TestItems = Service.IndexService.GetTestItems(prodType: prodType, lot: Lot);
            TestItems = Service.IndexService.GetTestCounts(prodType: prodType, lot: Lot);
        
        }
    }
}
