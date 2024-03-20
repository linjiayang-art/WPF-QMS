using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Common;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using SicoreQMS.Extensions;

namespace SicoreQMS.ViewModels
{
    internal class TestModelMaintenanceViewModel:BindableBase, IRegionMemberLifetime
    {
        #region 属性

        private string modelId;

        public string ModelId
        {
            get { return modelId; }
            set { modelId = value;RaisePropertyChanged();}
        }


      

        private IEventAggregator aggregator;

        private ObservableCollection<TestModelItem> testModelItems;

        public ObservableCollection<TestModelItem> TestItems
        {
            get { return testModelItems; }
            set { testModelItems = value; }
        }
        private ObservableCollection<SelectBasic> tsetModel;

        public ObservableCollection<SelectBasic> TestModel
        {
            get { return  tsetModel; }
            set { tsetModel =  value; RaisePropertyChanged();}
        }

        public bool KeepAlive { get; set; } =false;
        #endregion

        #region 事件
        public DelegateCommand<SelectBasic> HandleSelectModel { get; private set;}

        public DelegateCommand<string> BtnSumbit { get; private set;}
  

        #endregion


        public TestModelMaintenanceViewModel(IEventAggregator aggregator)
        {
            HandleSelectModel = new DelegateCommand<SelectBasic>(SelectModel);
            TestModel =Service.TestProcessService.GetTestModel();
            BtnSumbit=new DelegateCommand<string>(OnSubmit);
            this.aggregator=aggregator;
            TestItems = new ObservableCollection<TestModelItem>();
        }

        private void OnSubmit(string obj)
        {
            switch (obj)
            {
                case "import":
                    ImportData();
                    break;
                case "edit":
                    UpdateItem();
                    break;
                default:
                    break;
            }
        }

        private void UpdateItem()
        {
            if (TestItems.Count == 0)
            {
                aggregator.SendMessage("请导入数据！");
                return;
            }
            using (var context =new SicoreQMSEntities1())
            {
                var items = context.TestModelItem.Where(b => b.ModelId == ModelId).ToList();
                foreach (var item in items)
                {
                    context.TestModelItem.Remove(item);
                }
                context.SaveChanges();
                foreach (var item in TestItems)
                {
                    context.TestModelItem.Add(item);
                    
                }
                context.SaveChanges();

                aggregator.SendMessage("修改成功");
            }
        }

        private void SelectModel(SelectBasic basic)
        {
            ModelId= basic.Value;
            using (var context=new SicoreQMSEntities1())
            {
                var items = context.TestModelItem.Where(b => b.ModelId == ModelId).OrderBy(b=>b.ExperimentItemRank) . ToList(); 
                TestItems.Clear();
                foreach (var item in items)
                {
                    TestItems.Add(item);
                }
            }
            
        }



        private void ImportData()
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
                var loadTable = Service.DocService.LoadDoc(filePath);
                if (loadTable == null)
                {
                    aggregator.SendMessage("数据解析失败");
                    return;
                }
                aggregator.SendMessage("选择模板");
                var rank = 0;
                TestItems.Clear();
                foreach (DataRow row in loadTable.Rows)
                {
                    var testModelItem = new TestModelItem()
                    {
                        //Id = row["Id"].ToString(),
                        //TestProcessId = row["TestProcessId"].ToString(),
                        // ...
                        Id = Guid.NewGuid().ToString(),
                        ExperimentItemRank = rank++,
                        ModelId = ModelId,
                        ExperimentItemNo = row["ExperimentItemNo"].ToString(),
                        ExperimentItemName = row["ExperimentName"].ToString(),
                        ExperimentItemStandard = row["ExperimentStandard"].ToString(),
                        ExperimentItemConditions = row["ExperimentConditions"].ToString(),
                        ExperimentItemQty = TryGetIntFromDataRow(row, "ExperimentQty"),
                        ExperimentItemNumber= row["ExperimentNo"].ToString(),
                        // ...
                        ItemDesc = row["ItemDesc"].ToString(),
                        // 假设日期和布尔列是可空的，并且使用适当的格式

                    };
                    TestItems.Add(testModelItem);
                }
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



    }
}