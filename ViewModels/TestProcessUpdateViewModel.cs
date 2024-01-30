using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Common;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using SicoreQMS.Service;
using Syncfusion.UI.Xaml.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SicoreQMS.ViewModels
{
    public class TestProcessUpdateViewModel : BindableBase, IRegionMemberLifetime
    {

        #region 属性
        public DelegateCommand HandlerPrint { get; set; }
        private ObservableCollection<CheckBasic> _testTypes;

        public ObservableCollection<CheckBasic> TestTypes
        {
            get { return _testTypes; }
            set { _testTypes = value; RaisePropertyChanged(); }
        }


        public DelegateCommand<TestProcessItem> BtnCommand { get; set; }

        private readonly IDialogService dialog;
        private IEventAggregator aggregator;
        private ObservableCollection<TestProcessItem> _testItems;

        public ObservableCollection<TestProcessItem> TestItems
        {
            get { return _testItems; }
            set { _testItems = value; RaisePropertyChanged(); }
        }

        public string Id { get; set; }

        private ObservableCollection<SelectBasic> _allTestProcess;

        public ObservableCollection<SelectBasic> AllTestProcess
        {
            get { return _allTestProcess; }
            set { _allTestProcess = value; RaisePropertyChanged(); }
        }

        public DelegateCommand<string> HandleSelect { get; set; }

        private TestProcess _testProcessInfo;

        public TestProcess TestProcessInfo
        {
            get { return _testProcessInfo; }
            set { SetProperty(ref _testProcessInfo, value); }
        }

        public bool KeepAlive { get; set; } = false;

        #endregion

        public TestProcessUpdateViewModel(IDialogService dialog, IEventAggregator aggregator)
        {
            AllTestProcess = new ObservableCollection<SelectBasic>();
            HandleSelect = new DelegateCommand<string>(GetTestItem);
            TestItems = new ObservableCollection<TestProcessItem>();
            HandlerPrint = new DelegateCommand(PrintModel);
            BtnCommand = new DelegateCommand<TestProcessItem>(ShowUpdateDiaLog);
            CreateData();
            this.dialog = dialog;
            this.aggregator= aggregator;
        }

        private void PrintModel()
        {

            string folderPath = @"C:\Users\1000145\Desktop\report.docx";

            SqlParameter[] param = {
                new SqlParameter("LotNo",TestProcessInfo.ProdLot)
            };

            string sql = "proc_QAExperimentReport";
            System.Data.DataSet ds = Common.DBHelper.ExecuteProcRe(sql, param);
            DataTable head_dataTable = ds.Tables[0];
            DataTable body_dataTable = ds.Tables[1];

            PrintService.ExportReportToWord("TestProcess.rdlc", folderPath, dataSource1: body_dataTable, dataSource2:  head_dataTable, dataSourceName1: "DataSet1",dataSourceName2: "DataSet2");
            this.aggregator.SendMessage("打印成功!");
            //DataTable dataTable = new DataTable();
            //dataTable.Columns.Add("ModelSort", typeof(int));
            //dataTable.Columns.Add("ProdProcessCard", typeof(string));
            //dataTable.Columns.Add("ProcessType", typeof(string));
            //using (var context = new SicoreQMSEntities1())
            //{
            //    var allModel = context.Prod_ProcessItem.Where(b => b.ProdProcessId == "c4a3c0b6-aeb9-4f03-bee9-653d0d20171a").ToList().OrderBy(x => x.ModelSort);
            //    foreach (var item in allModel)
            //    {
            //        DataRow row = dataTable.NewRow();
            //        row["ModelSort"] = (int)item.ModelSort; // 替换为实际的属性名
            //        row["ProdProcessCard"] = item.ProdProcessCard; // 替换为实际的属性名
            //        row["ProcessType"] = item.ProcessType; // 替换为实际的属性名
            //        dataTable.Rows.Add(row);
            //    }
            //}
            //PrintService.ExportReportToWord("ProdProcess.rdlc", folderPath, dataTable, "Prod_ProcessSet");
        }

        private void ShowUpdateDiaLog(TestProcessItem obj)
        {
            if (obj is null)
            {
                return;
            }
            DialogParameters dialogParameters = new DialogParameters
            {
                { "Id",obj.Id},
            };

            dialog.ShowDialog("TestProcessItemUpdateView", dialogParameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var message = result.Parameters.GetValue<string>("key");

                    // 处理 message
                    GetTestItem(Id);
                    //显示信息
                    this.aggregator.SendMessage(message);
                }}
            );


        }

        private void GetTestItem(string obj)
        {
            Id = obj;
            if (obj == null)
            {
                return;
            }
            //TestItems.Clear();
            using (var context = new SicoreQMSEntities1())
            {

                TestProcessInfo = context.TestProcess.Find(Id);

                var result = context.TestProcessItem.Where(p => p.TestProcessId == Id&&p.IsDeleted==false).OrderBy(p=>p.ExperimentItemNo).ToList();
                if (result.Count == 0)
                {
                    return;
                }
                TestItems.Clear();
                foreach (var item in result)
                {
                    TestItems.Add(item);
                }
            }

            TestTypes= TestProcessService.GetTestTypeList(TestProcessInfo.Id);

        }

        private void CreateData()
        {

            TestTypes = ProdBasicService.GetTestType();

            using (var context = new SicoreQMSEntities1())
            {
                var result = context.TestProcess.Where(p => p.Isdeletd == false && p.CompleteStatus == false).ToList();
                foreach (var item in result)
                {
                    AllTestProcess.Add(item.GetSelection());

                }
            }
        }


    }
}
