using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Report;
using SicoreQMS.Extensions;
using SicoreQMS.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels
{
    public class EquipemntUsageViewModel : BindableBase, IRegionMemberLifetime
    {

        #region 属性

        #region 设备清单
        private ObservableCollection<EquipemntUsageReport> _equipmentList;

        public ObservableCollection<EquipemntUsageReport> EquipmentList
        {
            get { return _equipmentList; }
            set { _equipmentList = value; RaisePropertyChanged(); }
        }

        public bool KeepAlive { get; set; } = false;

        #endregion

        private string _search;

        private IEventAggregator aggregator { get; }
        private IDialogService dialogService { get; }

        public string Search
        {
            get { return _search; }
            set { SetProperty(ref _search, value); }
        }

        public DelegateCommand<string> ExecuteCommand { get; private set; }

        #endregion
        public EquipemntUsageViewModel(IEventAggregator aggregator,IDialogService dialogService)
        {
            EquipmentList = Service.EquipmentService.GetEquipmentStatus();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            this.aggregator = aggregator;
            this.dialogService = dialogService;
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
           
                case "查询": GetDataAsync(); break;
                case "导出": ExportData(); break;
                case "Add": AddNewEquipment();break;
            }
        }

        private void AddNewEquipment()
        {
            dialogService.ShowDialog("EquipmentManagementView", result =>
            {
                GetDataAsync();
            });
        }

        private void GetDataAsync()
        {
            EquipmentList = Service.EquipmentService.GetEquipmentStatus(Search);
        }
        private void ExportData()
        {
            var listResult = Service.EquipmentService.getUseAgeRecord();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 使用 Path.Combine 构建完整的文件路径
            string fullPath = Path.Combine(desktopPath, "机台使用记录.xlsx");
            ExcelService.getDataFile(fullPath, listResult, new Dictionary<string, string>
            {
                { "Id", "序号" },
                //{ "EquipmentId", "设备编号" },
                { "StartDate", "开始时间" },
                { "EndDate", "结束时间" },
                { "UseType", "使用类型" },
                { "UseProcess", "使用过程" },
                { "UseUser", "使用人" },
                { "EquipmentName", "设备名称" },
                { "EquipmentModel", "设备型号" },
                { "EquipmentNo", "设备编号" }
            });
            aggregator.SendMessage("导出成功");
        }

    }
}