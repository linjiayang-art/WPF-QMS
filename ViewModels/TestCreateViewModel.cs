using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SicoreQMS.Service;
using System.Windows.Forms.VisualStyles;
using Prism.Events;
using SicoreQMS.Extensions;
using Prism.Services.Dialogs;
using SicoreQMS.Views.Dialogs;
using Prism.Regions;

namespace SicoreQMS.ViewModels
{
    public class TestCreateViewModel : BindableBase, IRegionMemberLifetime
    {
        private IEventAggregator aggregator;
        private IDialogService dialog;

        public TestCreateViewModel(IEventAggregator aggregator, IDialogService dialog)
        {

            TestTypes = new ObservableCollection<CheckBasic>();
            ModelType = new ObservableCollection<SelectBasic>();

            HandleSelect = new DelegateCommand<string>(GetInfo);
            HandleDel = new DelegateCommand<TestProcessItem>(DelInfo);
            HandleAdd = new DelegateCommand(AddInfo);

            CheckCommand = new DelegateCommand<string>(ExcuteCheckCommand);
            TestModelItem = new ObservableCollection<TestProcessItem>();
            OnSumbit = new DelegateCommand(UpdateTestProcess);

            ProductNameBasic = ProdBasicService.CreateProductSelection();
            CreateData();
            this.aggregator = aggregator;
            this.dialog = dialog;





        }

        private void DelInfo(TestProcessItem parmater)
        {
            var id = parmater.Id;
            var result = TestProcessService.DelItem(id);
            if (result.ResultStatus)
            {
                aggregator.SendMessage(result.ResultMessage);
                GetInfo(ProdId);

            }
            else
            {
                System.Windows.MessageBox.Show(result.ResultMessage);
            }


        }

        private void AddInfo()
        {
            DialogParameters dialogParameters = new DialogParameters
            {
                { "ProdId",ProdId},
                {"TestProcessId" ,TestModelItem[0].TestProcessId}

            };
            dialog.ShowDialog("TestItemAddView", dialogParameters, result =>
            {
                aggregator.SendMessage("新增成功!");
                GetInfo(ProdId);

            });
        }


        private void GetInfo(string parameter)
        {
            if (parameter == null)
            {
                return;
            }
            ProdId = parameter;

            using (var context = new SicoreQMSEntities1())
            {
                var productInfo = context.ProdInfo.SingleOrDefault(b => b.Id == parameter);

                if (productInfo != null)
                {
                    this.ProdName = productInfo.ProdName;
                    this.ProdLot = productInfo.ProdLot;
                    this.ProdType = productInfo.ProdType;
                    this.ProdNumber = productInfo.ProdNumber;
                    this.Prodstandard = productInfo.Prodstandard;
                    this.TestLot = productInfo.TestLot;

                    try
                    {
                        var a = productInfo.TestType.Split(';');
                        TestTypes[0].IsCheck = true;
                        foreach (var item in a)
                        {
                            //如果item在list中,list为true
                            var result = TestTypes.Any(p => p.Label == item);
                            if (result)
                            {
                                TestTypes.Where(p => p.Label == item).FirstOrDefault().IsCheck = true;
                            }
                        }
                    }
                    catch (NullReferenceException)
                    {

                        
                    }
                    
              
                    //拆分


                    GetTemplate(parameter);
                }


                else
                {
                    aggregator.SendMessage("未查询到该产品");
                }
                var testProcess = context.TestProcess.SingleOrDefault(p => p.ProdId == parameter);
                if (testProcess.AuditStatus == true)
                {
                    CanAduit = false;
                }
                else
                {
                    CanAduit = true;
                }


            }

        }

        private void ExcuteCheckCommand(string obj)
        {
            for (int i = 0; i < TestTypes.Count; i++)
            {
                if (TestTypes[i].Label != obj)
                {
                    TestTypes[i].IsCheck = false;
                }
            }
            return;
        }

        private void UpdateTestProcess()
        {

            using (var context = new SicoreQMSEntities1())
            {
                var testProcessid = "";
                foreach (var item in TestModelItem)
                {
                    var id = item.Id;
                    var testProcessItem = context.TestProcessItem.SingleOrDefault(p => p.Id == id);
                    if (testProcessItem != null)
                    {
                        testProcessItem.ExperimentItemNo = item.ExperimentItemNo;
                        testProcessItem.ExperimentName = item.ExperimentName;
                        testProcessItem.ExperimentStandard = item.ExperimentStandard;
                        testProcessItem.ExperimentConditions = item.ExperimentConditions;
                        testProcessItem.ExperimentQty = item.ExperimentQty;
                        testProcessItem.ExperimentItemPassQty = item.ExperimentItemPassQty;
                        testProcessItem.ExperimentNo = item.ExperimentNo;
                        testProcessItem.ExperimentUser = item.ExperimentUser;
                        testProcessItem.ExperimentBasis = item.ExperimentBasis;
                        testProcessItem.ExperimentType = item.ExperimentType;
                        testProcessItem.EstimatedCompletionTime = item.EstimatedCompletionTime;
                        testProcessItem.ExperimentSatrtTime = item.ExperimentSatrtTime;
                        testProcessItem.ExperimentEndTime = item.ExperimentEndTime;
                        testProcessItem.ExperimentStatus = item.ExperimentStatus;
                        testProcessItem.Remark = item.Remark;
                        testProcessItem.IsDeleted = item.IsDeleted;
                        testProcessItem.CreateUser = item.CreateUser;
                        testProcessItem.CreateDate = item.CreateDate;
                        testProcessItem.ModifyUserNo = item.ModifyUserNo;
                        testProcessItem.ModifyTime = item.ModifyTime;
                        testProcessItem.ProdId = item.ProdId;
                        testProcessItem.AuditStatus = true;
                    }
                    else
                    {
                        item.Id = Guid.NewGuid().ToString();
                        item.AuditStatus = true;
                        context.TestProcessItem.Add(item);
                    }
                    testProcessid = item.TestProcessId;


                    //context.TestProcessItem.Add(item);
                }

                var testProcess = context.TestProcess.SingleOrDefault(p => p.Id == testProcessid);
                testProcess.AuditStatus = true;
                context.SaveChanges();

            }
            aggregator.SendMessage("审核成功!");
            GetInfo(ProdId);
            return;

        }

        private void GetTemplate(string obj)
        {
            if (obj == null)
            {
                return;
            }
            TestModelItem.Clear();
            using (var contxt = new SicoreQMSEntities1())
            {
                var items = contxt.TestProcessItem.Where(p => p.ProdId == obj && p.IsDeleted == false).OrderBy(p => p.ExperimentItemNo).ToList();

                if (items.Count == 0)
                {
                    return;
                }

                foreach (var item in items)
                {
                    TestModelItem.Add(item);

                }

            }
        }
        #region 属性

        private bool _canAduit;

        public bool CanAduit
        {
            get { return _canAduit; }
            set { SetProperty(ref _canAduit, value); }
        }

        public DelegateCommand<string> CheckCommand { get; private set; }

        public DelegateCommand OnSumbit { get; private set; }

        private ObservableCollection<SelectBasic> _productNameBasic;

        public ObservableCollection<SelectBasic> ProductNameBasic
        {
            get { return _productNameBasic; }
            set { _productNameBasic = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<string> HandleSelect { get; set; }

        public DelegateCommand<TestProcessItem> HandleDel { get; set; }

        public DelegateCommand HandleAdd { get; set; }
        private ObservableCollection<SelectBasic> _modelType;

        private ObservableCollection<TestProcessItem> _testModelItem;

        private string _prodName;


        public string ProdId { get; set; }

        public string ProdName
        {
            get { return _prodName; }
            set { SetProperty(ref _prodName, value); }
        }

        private string _prodType;

        public string ProdType
        {
            get { return _prodType; }
            set { SetProperty(ref _prodType, value); }
        }

        private string _prodlot;

        public string ProdLot
        {
            get { return _prodlot; }
            set { SetProperty(ref _prodlot, value); }
        }

        private string _Prodstandard;

        public string Prodstandard
        {
            get { return _Prodstandard; }
            set { SetProperty(ref _Prodstandard, value); }
        }

        private string _prodNmuber;

        public string ProdNumber
        {
            get { return _prodNmuber; }
            set { SetProperty(ref _prodNmuber, value); }

        }

        private string _testLot;

        public string TestLot
        {
            get { return _testLot; }
            set { SetProperty(ref _testLot, value); }
        }



        public ObservableCollection<TestProcessItem> TestModelItem
        {
            get { return _testModelItem; }
            set { _testModelItem = value; RaisePropertyChanged(); }
        }


        public ObservableCollection<SelectBasic> ModelType
        {
            get { return _modelType; }
            set { _modelType = value; }
        }

        private ObservableCollection<CheckBasic> _testTypes;

        public ObservableCollection<CheckBasic> TestTypes
        {
            get { return _testTypes; }
            set { _testTypes = value; RaisePropertyChanged(); }
        }

        public bool KeepAlive { get; set; } = false;
        #endregion

        void CreateData()
        {
            TestTypes.Add(new CheckBasic() { Label = "筛选", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "鉴定", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "质量一致性", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "研发验证", IsCheck = false });
            TestTypes.Add(new CheckBasic() { Label = "其它", IsCheck = false });




        }
    }
}
