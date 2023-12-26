using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SicoreQMS.Common.Models.Operation;
using System.Diagnostics;
using System.Data.Entity.Validation;
using System.Collections.ObjectModel;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Service;
using Prism.Events;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using SicoreQMS.Extensions;

namespace SicoreQMS.ViewModels
{
    public class TestRequestViewModel : BindableBase
    {

        public DelegateCommand BtnSumbit { get; set; }




        public TestRequestViewModel(IEventAggregator aggregator)
        {
            BtnSumbit = new DelegateCommand(OnSumbit);
            TestTypes = ProdBasicService.GetTestType();
            CheckCommand = new DelegateCommand(HandelSelect);
            Aggregator = aggregator;
        }

        private void HandelSelect()
        {

        }

        private void OnSumbit()
        {

            if (string.IsNullOrEmpty(ProdName) || string.IsNullOrEmpty(ProdType) || string.IsNullOrEmpty(ProdLot))
            {
                Aggregator.SendMessage("请完善数据后再提交");
                //   MessageBox.Show("请完善数据后再提交");
                return;
            }
            TestTypeStr = "";

            foreach (var item in TestTypes)
            {

                if (item.IsCheck == true)
                {
                    TestTypeStr += item.Label + ";";
                }

            }

            //去除TestTypeStr最后一个分号
            TestTypeStr = TestTypeStr.Substring(0, TestTypeStr.Length - 1);
      
            bool result = ProdBasicService.CreateProdBasic(prodName: this.ProdName, prodType: this.ProdType,
                        qty: this.Qty, prodLot: this.ProdLot, testLot: this.TestLot, prodNumber: this.ProdNumber,prodstandard:this.Prodstandard,testType:this.TestTypeStr);

            if (result)
            {
                Aggregator.SendMessage("新增成功!");
                // MessageBox.Show("新增成功!");

                ProdName = "";
                ProdType = "";
                ProdLot = "";

            }
            else
            {
                Aggregator.SendMessage("新增失败!");
                // MessageBox.Show("新增失败!");
            }

        }


        #region 属性

        public DelegateCommand CheckCommand { get; set; }

        private ObservableCollection<CheckBasic> _testTypes;

        public ObservableCollection<CheckBasic> TestTypes
        {
            get { return _testTypes; }
            set { _testTypes = value; RaisePropertyChanged(); }
        }


        

       private string _prodstandard;

        public string Prodstandard
        {
            get { return _prodstandard; }
            set { SetProperty(ref _prodstandard, value); }
        }

        public string TestTypeStr { get; set; }
        private string _prodNumber;

        public string ProdNumber
        {
            get { return _prodNumber; }
            set { SetProperty(ref _prodNumber, value); }
        }

        private string _testLot;

        public string TestLot
        {
            get { return _testLot; }
            set { SetProperty(ref _testLot, value); }
        }



        private int _qty;

        public int Qty
        {
            get { return _qty; }
            set { SetProperty(ref _qty, value); }
        }


        private string _prodLot;

        private string _qualitylevel;
        private string _prodType;
        private string _prodName;

        public string ProdName
        {
            get { return _prodName; }
            set { SetProperty(ref _prodName, value); }
        }

        public string ProdType
        {
            get { return _prodType; }
            set { SetProperty(ref _prodType, value); }
        }

        public string ProdLot
        {
            get { return _prodLot; }
            set { SetProperty(ref _prodLot, value); }
        }

        public string QualityLevel
        {
            get { return _qualitylevel; }
            set { SetProperty(ref _qualitylevel, value); }
        }

        public IEventAggregator Aggregator { get; }


        #endregion



    }
}
