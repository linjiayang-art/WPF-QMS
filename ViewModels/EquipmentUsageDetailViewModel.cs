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
          
        }
    }
}
