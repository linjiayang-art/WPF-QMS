using DocumentFormat.OpenXml.Bibliography;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using SicoreQMS.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SicoreQMS.ViewModels
{
    public class ProdProcessUpdateViewModel : BindableBase, IRegionMemberLifetime
    {

        #region 属性
        private ObservableCollection<SelectBasic> _productNameBasic;
        private readonly IDialogService dialog;
        private ObservableCollection<Prod_ProcessItem> _processItem;

        public DelegateCommand<Prod_ProcessItem> UpdateProgressCommand { get; set; }
        public DelegateCommand<Prod_ProcessItem> EditProgressCommand { get; set; }
        public DelegateCommand<SelectBasic> SplitLotCommand { get; set; }

        public DelegateCommand<SelectBasic> ExportCommand { get; set; }
        public DelegateCommand<SelectBasic> ReportCommand{ get; set; }
        public ObservableCollection<SelectBasic> SelectBasicItem { get; set; }

        private string _prodLot;
        private string _prodName;
        private string _qualitylevel;
        private string searchText;
        private string _prodType;
        private string processId;
        private string testNo;
        private string prodNo;


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

        public string ProdProcessId
        {
            get { return processId; }
            set { SetProperty(ref processId, value); }
        }


        public string QualityLevel
        {
            get { return _qualitylevel; }
            set { SetProperty(ref _qualitylevel, value); }
        }


        public string ProdLot
        {
            get { return _prodLot; }
            set { SetProperty(ref _prodLot, value); }
        }

        public string ProdType
        {
            get { return _prodType; }
            set { SetProperty(ref _prodType, value); }
        }

        public string ProdName
        {
            get { return _prodName; }
            set { SetProperty(ref _prodName, value); }
        }

        public string TestNo
        {
            get { return testNo; }
            set { SetProperty(ref testNo, value); }
        }


        public string ProdNo
        {
            get { return prodNo; }
            set { SetProperty(ref prodNo, value); }
        }


        #endregion

        public ProdProcessUpdateViewModel(IDialogService dialog, IEventAggregator aggregator)
        {
            ProductNameBasic = new ObservableCollection<SelectBasic>();

            UpdateProgressCommand = new DelegateCommand<Prod_ProcessItem>(UpdateProgress);
            EditProgressCommand = new DelegateCommand<Prod_ProcessItem>(EditProgress);
            SplitLotCommand = new DelegateCommand<SelectBasic>(SpiltLot);
            SelectBasicItem = new ObservableCollection<SelectBasic>();
            ExportCommand = new DelegateCommand<SelectBasic>(ExportFile);
            ReportCommand= new DelegateCommand<SelectBasic>(ExportReportFile);
            CreateProductSelection();
            ProductNameBasic = SelectBasicItem;
            ProcessItem = new ObservableCollection<Prod_ProcessItem>();

            HandelSelect = new DelegateCommand<object>(GetInfo);
            QualityLevel = "军品";
            this.dialog = dialog;
            //var a = Service.EquipmentService.GetEquipmentBasic();
            Aggregator = aggregator;
        }

        private void ExportFile(SelectBasic basic)
        {
            if (basic is null || string.IsNullOrEmpty(basic.Value))
            {
                Aggregator.SendMessage("请选择批次后再进行导出!");
                return;
            }
            var result = Service.ProdProcessService.GetProcessInfo(basic.Value);

            Aggregator.SendMessage(result);

        }

        private void ExportReportFile(SelectBasic basic)
        {
            if (basic is null || string.IsNullOrEmpty(basic.Value))
            {
                Aggregator.SendMessage("请选择批次后再进行导出!");
                return;
            }
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //增加个时间轴
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");

  
           // 使用 Path.Combine 构建完整的文件路径

           var prodName = basic.Label.Split(':')[0];


            string folderPath = Path.Combine(desktopPath, $"{prodName}-筛选报告{time}.docx");

            var rdlcPath = @"\\Sx-fj-221\共享文件\IT部\PrintModel\Scanning\ProdProcessMain.rdlc";
            try
            {
                PrintService.ExportScreeningReportToWord(basic.Value, rdlcPath, folderPath);
            }
            catch (FileNotFoundException f)
            {
                Aggregator.SendMessage(f.Message.ToString());
                return;
            }
         
    
            Aggregator.SendMessage("打印成功");

        }

        private void SearchTextList()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                ProductNameBasic = SelectBasicItem;
                return;
            }
            if (getInfoInto==false)
            {
                getInfoInto = true;
                return;
            }
            ProcessItem.Clear();
            //转大写会造成逻辑重复调用，明确区分
            //var SearchTetxPar=SearchText.ToUpper();
            var searchList = SelectBasicItem.Where(b => b.Label.Contains(SearchText)).ToList();
            ProductNameBasic = new ObservableCollection<SelectBasic>(searchList);

        }

        private void SpiltLot(SelectBasic obj)
        {

            if (obj is null || string.IsNullOrEmpty(obj.Value))
            {
                return;
            }

            using (var context = new SicoreQMSEntities1())
            {
                var prodprocee = context.Prod_Process.Find(obj.Value);
                //查看明细中有无正在进行的步骤，如果有则不允许拆批
                var prodprocessItem = context.Prod_ProcessItem.Where(b => b.ProdProcessId == obj.Value && b.ItemStatus == 1).ToList();
                if (prodprocessItem.Count > 0)
                {
                    Aggregator.SendMessage("该产品有正在进行的步骤，不允许拆批.请完成当前步骤后再进行拆批");
                    //System.Windows.MessageBox.Show("该产品有正在进行的步骤，不允许拆批");
                    return;
                }
            }


            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id",obj.Value},

            };

            dialog.ShowDialog("LotSplitView", dialogParameters, result =>
            {

                var newid = result.Parameters.GetValue<string>("NewId");
                if (!string.IsNullOrEmpty(newid))
                {
                    using (var context = new SicoreQMSEntities1())
                    {
                        var productItem = context.Prod_Process.SingleOrDefault(p => p.Id == newid);
                        SelectBasicItem.Add(productItem.ProductSelect());
                    }
                }
                SearchTextList();
                GetProcessList();
            });
        }

        private void UpdateProgress(Prod_ProcessItem obj)
        {

            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id",obj.Id},

            };
            dialog.ShowDialog("ProcessUpdateView", dialogParameters, result =>
            {
                GetProcessList();

            });

        }


        private void EditProgress(Prod_ProcessItem obj)
        {

            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id",obj.Id},

            };
            dialog.ShowDialog("ProcessEditView", dialogParameters, result =>
            {
                GetProcessList();

            });

        }

        public bool KeepAlive => false;


        public bool getInfoInto=true;


        private void GetInfo(object parameter)
        {

            getInfoInto = false;
            if (parameter == null)
            {
                return;
            }
            if (parameter.GetType().ToString() != "System.String")
            {
                return;
            }

            ProdProcessId = parameter.ToString();

            using (var context = new SicoreQMSEntities1())
            {
                var productInfo = (from pd in context.ProdInfo
                                   join pp in context.Prod_Process on pd.Id equals pp.ProdId
                                   where pp.Id == ProdProcessId
                                   select new
                                   {
                                       pd.ProdNo,
                                       pd.TestNo,
                                       pp.ProdName,
                                       pp.ProdLot,
                                       pp.QualityLevel

                                   }).FirstOrDefault();

                context.Prod_Process.SingleOrDefault(b => b.Id == ProdProcessId);
                if (productInfo != null)
                {
                    this.ProdName = productInfo.ProdName;
                    this.ProdLot = productInfo.ProdLot;
                    this.QualityLevel = productInfo.QualityLevel;
                    this.ProdNo = productInfo.ProdNo;
                    this.TestNo = productInfo.TestNo;
                }
                else
                {
                    Aggregator.SendMessage("未查询到该产品");
                    //System.Windows.MessageBox.Show("未查询到该产品");
                }

            }
            GetProcessList();

        }

        public ObservableCollection<SelectBasic> ProductNameBasic
        {
            get { return _productNameBasic; }
            set { _productNameBasic = value; RaisePropertyChanged(); }
        }



        public ObservableCollection<Prod_ProcessItem> ProcessItem
        {
            get { return _processItem; }
            private set { _processItem = value; RaisePropertyChanged(); }

        }

        public void CreateProductSelection()
        {

            var newProductNameBasic = new ObservableCollection<SelectBasic>();

            using (var context = new SicoreQMSEntities1())
            {
                var productItem = context.Prod_Process
                    .Where(b => (b.ProdStatus == 0 || b.ProdStatus == 5 || b.ProdStatus == 1) && b.IsDeleted == false)
                    .OrderBy(x => x.CreateDate)
                    .ToList();
                foreach (var item in productItem)
                {
                    SelectBasicItem.Add(item.ProductSelect());
                }
            }
            //ProductNameBasic = newProductNameBasic;
        }

        void GetProcessList()
        {
            ProcessItem.Clear();
            using (var context = new SicoreQMSEntities1())
            {
                var allModel = context.Prod_ProcessItem.Where(b => b.ProdProcessId == ProdProcessId).ToList().OrderBy(x => x.ModelSort);
                foreach (var item in allModel)
                {
                    ProcessItem.Add(item);

                }
            }
        }

        public DelegateCommand CommitBtnCommand { get; private set; }


        public DelegateCommand<object> HandelSelect
        {
            get;
            private set;
        }
        public IEventAggregator Aggregator { get; }
    }
}
