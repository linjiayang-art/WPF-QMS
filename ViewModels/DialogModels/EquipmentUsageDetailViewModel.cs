using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Report;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels
{
    public class EquipmentUsageDetailViewModel:BindableBase, IDialogAware
    {
        #region
        public ObservableCollection<EquipmentUsageDetailModel> EquipmentUsages { get; set; }

        private string _equipmentNo;

        public string EquipmentNo
        {
            get => _equipmentNo;
            set => SetProperty(ref _equipmentNo, value);

        }
        private DateTime _startDate;

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);

        }
        private DateTime _endDate;

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);

        }




        public string Title { get; set; } ="设备使用详情";
        #endregion
        public EquipmentUsageDetailViewModel()
        {
            EquipmentUsages = new ObservableCollection<EquipmentUsageDetailModel>();  
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
      
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            this.EquipmentNo= parameters.GetValue<string>("equipmentNo");
           this.StartDate = parameters.GetValue<DateTime>("startDate");
           this.EndDate = parameters.GetValue<DateTime>("endDate");
            this.EquipmentUsages = Service.EquipmentService.GetEquipmentUsageDetails(EquipmentNo,StartDate,EndDate);
        }
    }
}
