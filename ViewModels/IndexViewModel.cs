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

    

        public ObservableCollection<TestCountReport> _testReportList { get; set; }

        public ObservableCollection<TestCountReport>  TestReportList { 
            get { return _testReportList; }
            set { _testReportList = value; RaisePropertyChanged(); } }
        #endregion


        public IndexViewModel()
        {
            TestReportList= Service.IndexService.GetTestCountReport();
        }



  

      

    }
}
