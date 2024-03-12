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
using Microsoft.Win32;
using System.Data;
using SicoreQMS.Common.Server;

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

            HandleSelect = new DelegateCommand<object>(GetInfo);
            HandleDel = new DelegateCommand<TestProcessItem>(DelInfo);
            HandleAdd = new DelegateCommand(AddInfo);

            CheckCommand = new DelegateCommand<string>(ExcuteCheckCommand);
            TestModelItem = new ObservableCollection<TestProcessItem>();
            OnSumbit = new DelegateCommand<string>(SumbitBtnClick);

            ProductNameBasic = ProdBasicService.CreateProductSelection();
            SerachProductNameBasic = ProductNameBasic;
            CreateData();
            this.aggregator = aggregator;
            this.dialog = dialog;





        }


        private void SumbitBtnClick(string obj)
        {
            if (ProdType is null || string.IsNullOrEmpty(ProdType))
            {
                return;
            }

            switch (obj)
            {
                case "audit":
                    UpdateTestProcess();
                    break;
                case "import":
                    ImportInfo();
                    break;
                default:
                    break;
            }
        }


        private void ImportInfo()
        {
            //文件选择框获取文件路径
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // 设置对话框的标题
            openFileDialog.Title = "请选择文件";

            // 可以选择设置过滤器，限制用户选择特定类型的文件
            openFileDialog.Filter = "文本文件 (*.docx)|*.docx|所有文件 (*.*)|*.*";

            // 设置是否允许选择多个文件
            openFileDialog.Multiselect = false;

            // 显示对话框
            if (openFileDialog.ShowDialog() == true)
            {
                // 获取选中文件的路径
                string filePath = openFileDialog.FileName;
                var loadTable= Service.DocService.LoadDoc(filePath);
                var rank = 0;
                using (var context = new SicoreQMSEntities1())
                {
                    foreach (DataRow row in loadTable.Rows)
                    {
                        var testProcessItem = new TestProcessItem()
                        {
                            //Id = row["Id"].ToString(),
                            //TestProcessId = row["TestProcessId"].ToString(),
                            // ...
                            Id = Guid.NewGuid().ToString(),
                            ExperimentItemRank = rank++,
                            ProdId = ProdId,
                            TestProcessId=TestProcessId,
                            ModelId = "IMPORT",
                            ExperimentItemNo = row["ExperimentItemNo"].ToString(),
                            ExperimentName = row["ExperimentName"].ToString(),
                            ExperimentStandard = row["ExperimentStandard"].ToString(),
                            ExperimentConditions = row["ExperimentConditions"].ToString(),
                            ExperimentQty = TryGetIntFromDataRow(row, "ExperimentQty"),
                            ExperimentItemPassQty = TryGetIntFromDataRow(row, "ExperimentItemPassQty"),
                            ExperimentNo = row["ExperimentNo"].ToString(),
                            ExperimentUser = row["ExperimentUser"].ToString(),
                            CreateUser = AppSession.UserID,
                            // ...
                            ItemDesc = row["ItemDesc"].ToString(),
                            // 假设日期和布尔列是可空的，并且使用适当的格式

                        };
                        context.TestProcessItem.Add(testProcessItem);
                        context.SaveChanges();
                        TestModelItem.Add(testProcessItem);
                    }
               
                  
                }
                    // 此处可以根据获取到的文件路径进行后续操作
                   
            }
        }

        private static int? TryGetIntFromDataRow(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value)
                return 0;

            if (int.TryParse(row[columnName].ToString(), out int value))
                return value;
            return 0;
         
        }


        private void DelInfo(TestProcessItem parmater)
        {
        
            var id = parmater.Id;
            var result = TestProcessService.DelItem(id);
            if (result.ResultStatus)
            {
                aggregator.SendMessage(result.ResultMessage);
                GetInfo(TestProcessId);

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
                if (result.Result == ButtonResult.OK)
                {
                    aggregator.SendMessage("新增成功!");
                    GetInfo(TestProcessId);
                }
                

            });
        }


        private void GetInfo(object parameter)
        {

            if (parameter == null)
            {
                return;
            }
            if (parameter.GetType().ToString() != "System.String")
            {
                return;
            }

   
            TestProcessId=parameter.ToString(); ;



            using (var context = new SicoreQMSEntities1())
            {

                var testProcee= context.TestProcess.SingleOrDefault(p => p.Id == parameter);

                ProdId = testProcee.ProdId;
                var productInfo = context.ProdInfo.SingleOrDefault(b => b.Id == ProdId );

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
                        var a = testProcee.TestType.Split(';');

                        //TestTypes[0].IsCheck = true;

                        foreach (var testType in TestTypes)
                        {
                            testType.IsCheck = false;
                        }

                        foreach (var item in a)
                        {

                            //如果item在list中,list为true
                            //测试项从testProcess中获取
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
                    
                    GetTemplate(parameter.ToString());
                }


                else
                {
                    aggregator.SendMessage("未查询到该产品");
                }
                var testProcess = context.TestProcess.SingleOrDefault(p => p.Id == parameter);
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
            GetInfo(TestProcessId);
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
                var items = contxt.TestProcessItem.Where(p => p.ProdId ==ProdId && p.IsDeleted == false&&p.TestProcessId== obj).OrderBy(p => p.ExperimentItemRank).ToList();

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

        public DelegateCommand<string> OnSumbit { get; private set; }

        private ObservableCollection<SelectBasic> _productNameBasic;

        public ObservableCollection<SelectBasic> ProductNameBasic
        {
            get { return _productNameBasic; }
            set { _productNameBasic = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<SelectBasic> serachProductNameBasic;

        public ObservableCollection<SelectBasic> SerachProductNameBasic
        {
            get { return serachProductNameBasic; }
            set { serachProductNameBasic = value; RaisePropertyChanged(); }
        }


        public DelegateCommand<object> HandleSelect { get; set; }

        public DelegateCommand<TestProcessItem> HandleDel { get; set; }

        public DelegateCommand HandleAdd { get; set; }
        private ObservableCollection<SelectBasic> _modelType;

        private ObservableCollection<TestProcessItem> _testModelItem;

        private string _prodName;

        public string TestProcessId { get; set; }

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
        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetProperty(ref searchText, value);
                SearchTextList()
                ;
            }
        }

        private void SearchTextList()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                SerachProductNameBasic = ProductNameBasic;
                return;
            }
            var SearchTetxPar = SearchText.ToUpper();
            var searchList = ProductNameBasic.Where(b => b.Label.Contains(SearchTetxPar)).ToList();
            SerachProductNameBasic = new ObservableCollection<SelectBasic>(searchList);

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
