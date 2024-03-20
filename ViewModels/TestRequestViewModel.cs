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
using SicoreQMS.Common.Server;
using Prism.Regions;

namespace SicoreQMS.ViewModels
{
    public class TestRequestViewModel : BindableBase, IRegionMemberLifetime
    {

        public DelegateCommand<string> BtnSumbit { get; set; }




        public TestRequestViewModel(IEventAggregator aggregator)
        {
            BtnSumbit = new DelegateCommand<string>(ExecuteBtn);
            TestTypes = ProdBasicService.GetTestType();
            CheckCommand = new DelegateCommand(HandelSelect);
            TestModel = Service.TestProcessService.GetTestModel();
            HandleSelectModel= new DelegateCommand<SelectBasic>(SelectModel);
           Aggregator = aggregator;
        }

        private void SelectModel(SelectBasic basic)
        {
            if (basic is null)
            {
                return;
            }
            ModelId=basic.Value;
        }

        private void HandelSelect()
        {

        }

        public void ExecuteBtn(string obj)
        {
            switch (obj)
            {
                case "Sumbit":
                    OnSumbit();
                    break;
                case "Clear":
                    ClearForm();
                    break;

            }
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

            if (TestTypeStr.Length == 0)
            {
                Aggregator.SendMessage("请勾选实验类型");
                return;
            }
            //去除TestTypeStr最后一个分号
            TestTypeStr = TestTypeStr.Substring(0, TestTypeStr.Length - 1);

            string lastChar = ProdType.Substring(ProdType.Length - 1, 1).ToUpper();
            string qualityLevel;
            if (lastChar == "J")
            {
                qualityLevel = "军品";
            }
            else
            {
                qualityLevel = "民品";
            }

            var prodinfo = new ProdInfo // 创建一个新的 ProdInfo 对象o = 
            {
                Id = Guid.NewGuid().ToString(),
                TestLot = TestLot,
                ProdNumber = ProdNumber,
                ProdName = ProdName,
                ProdType = ProdType,
                ProdStatus = 0,
                Qty = Qty,
                OriginQty = Qty,
                ProdLot = ProdLot,
                QualityLevel = qualityLevel,
                Prodstandard = Prodstandard,
                TestType = TestTypeStr,
                TestNo = TestNo,
                ProdNo = ProdNo,
                CreateUser = AppSession.UserID,
            };

            bool result = ProdBasicService.CreateProdBasic(prodinfo);

            if (result)
            {
                Aggregator.SendMessage("新增成功!");
                // MessageBox.Show("新增成功!");

                this.ClearForm();
            }
            else
            {
                Aggregator.SendMessage("新增失败!");
                // MessageBox.Show("新增失败!");
            }

        }


        public void ClearForm()
        {
            ProdName = "";
            ProdType = "";
            ProdLot = "";
            Prodstandard = "";
            TestLot = "";
            ProdNumber = "";
            Qty = 0;
            TestNo = "";
            ProdNo = "";
            modelId = "";
            //清空checkbox
            foreach (var item in TestTypes)
            {
                item.IsCheck = false;

            }
        }

        #region 属性

        private string modelId;

        public string ModelId
        {
            get { return modelId; }
            set { modelId = value; RaisePropertyChanged(); }
        }

         public DelegateCommand<SelectBasic> HandleSelectModel { get; private set;}


        private ObservableCollection<SelectBasic> tsetModel;

        public ObservableCollection<SelectBasic> TestModel
        {
            get { return tsetModel; }
            set { tsetModel = value; RaisePropertyChanged(); }
        }

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

        private string testNo;

        public string TestNo
        {
            get { return testNo; }
            set { SetProperty(ref testNo, value); }
        }

        private string prodNo;

        public string ProdNo
        {
            get { return prodNo; }
            set { SetProperty(ref prodNo, value); }
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

        public bool KeepAlive { get; set; } = false;


        #endregion



    }
}
