using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Report;
using System;
using System.Collections.ObjectModel;

namespace SicoreQMS.ViewModels
{
    public class IndexViewModel:BindableBase
    {

        #region

        public DelegateCommand<string> ExecuetCommand { get; private set; }

        public ObservableCollection<TestCountReport> _testReportList { get; set; }

        public ObservableCollection<TestCountReport>  TestReportList { 
            get { return _testReportList; }
            set { _testReportList = value; RaisePropertyChanged(); } }

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
            TestReportList= Service.IndexService.GetTestCountReport();
            ExecuetCommand=new DelegateCommand<string>(Execuet);
        }

        private void Execuet(string obj)
        {
            switch (obj)
            {
                case "查询": GetList();break; 
            }
        }

        public void GetList()
        {
            _testReportList.Clear();
            TestReportList = Service.IndexService.GetTestCountReport(prodType:prodType,lot:Lot);
        }
    }
}
