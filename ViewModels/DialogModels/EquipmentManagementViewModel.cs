using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SicoreQMS.ViewModels.DialogModels
{
    internal class EquipmentManagementViewModel : BindableBase, IDialogAware
    {
        #region
        private IEventAggregator eventAggregator;


        private string equiomentNo;

        public string EquipmentNo
        {
            get { return equiomentNo; }
            set { equiomentNo = value; RaisePropertyChanged(); }
        }

        private string equipmentName;

        public string EquipmentName
        {
            get { return equipmentName; }
            set { equipmentName = value; RaisePropertyChanged(); }
        }


        private string equipmentModel;

        public string EquipmentModel
        {
            get { return equipmentModel; }
            set { equipmentModel = value; RaisePropertyChanged(); }
        }

        private string eqRemark;

        public string Remark
        {
            get { return eqRemark; }
            set { eqRemark = value; RaisePropertyChanged(); }
        }




        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<string> BtnCommit { get; private set; }



        public string Title { get; set; } = "设备管理";


        #endregion

        public EquipmentManagementViewModel(IEventAggregator aggregator)
        {
            BtnCommit = new DelegateCommand<string>(HandelClick);
            this.eventAggregator = aggregator;

        }

        private void HandelClick(string obj)
        {
            switch (obj)
            {
                case "Add":
                    AddNewEquipment();
                    break;
                case "Cancel":
                    RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(ButtonResult.Cancel));
                    break;
            }
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }
        private void AddNewEquipment()
        {
            if (string.IsNullOrEmpty(EquipmentNo))
            {
                this.eventAggregator.SendMessage("设备编号不允许为空");
                return;
            }
            if (string.IsNullOrEmpty(EquipmentName))
            {
                this.eventAggregator.SendMessage("设备名称不允许为空");
                return;
            }
            if (string.IsNullOrEmpty(EquipmentModel))
            {
                this.eventAggregator.SendMessage("设备型号不允许为空");
                return;
            }

            using (var context = new SicoreQMSEntities1())
            {
                var eq = new Equipment() {
                    EquipmentID=Guid.NewGuid().ToString(),
                    EquipmentNo = EquipmentNo,
                    EquipmentName = EquipmentName,
                    EquipmentModel = EquipmentModel,
                    Remark = Remark
                     };
                if (EquipmentName.ToString().Contains("老炼板"))
                {
                    eq.EquipmentType = "1";
                    eq.Capacity = 100;
                }
  
                context.Equipment.Add(eq);
                var eqs = new EquipmentStatus()
                {
                    Id = Guid.NewGuid().ToString(),
                    EquipmentID = eq.EquipmentID,
                    EquipmentStatus1 = 0,
                    StatusDesc = "闲置"

                };
                context.EquipmentStatus.Add(eqs);
                context.SaveChanges();
                }
        
    
            ButtonResult btnResult = ButtonResult.None;
            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(btnResult));
            this.eventAggregator.SendMessage("添加成功!");

        }

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
