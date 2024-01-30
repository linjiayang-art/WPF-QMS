using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SicoreQMS.Common.Models.Report;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public string Search
        {
            get { return _search; }
            set { SetProperty(ref _search, value); }
        }

        public DelegateCommand<string> ExecuteCommand { get; private set; }

        #endregion
        public EquipemntUsageViewModel()
        {
            EquipmentList = Service.EquipmentService.GetEquipmentStatus();
            ExecuteCommand = new DelegateCommand<string>(Execute);
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
           
                case "查询": GetDataAsync(); break;
              
            }
        }

        private void GetDataAsync()
        {
            EquipmentList = Service.EquipmentService.GetEquipmentStatus(Search);
        }
    }
}