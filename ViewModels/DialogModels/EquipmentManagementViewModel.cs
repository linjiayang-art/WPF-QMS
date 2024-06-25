using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace SicoreQMS.ViewModels.DialogModels
{
    internal class EquipmentManagementViewModel : BindableBase, IDialogAware
    {
        #region
        private IEventAggregator eventAggregator;


        private string equiomentNo;
        private string equipmentName;
        private string equipmentModel;
        private string eqRemark;
        private bool showCapaCity;
        private int capaCity;



        public DelegateCommand<SelectBasic> HandleSelectModel { get; private set; }
        public ObservableCollection<SelectBasic> equipmentType { get; set; }
        public ObservableCollection<SelectBasic> EquipmentType
        {
            get
            {
                return equipmentType;
            }
            set
            { equipmentType = value; RaisePropertyChanged(); }
        }

        private string choseEquipment;

        public string ChoseEquipment
        {
            get { return choseEquipment; }
            set { choseEquipment = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 是否展示承载容量
        /// </summary>
        public bool ShowCapaCity
        {
            get { return showCapaCity; }
            set { showCapaCity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 老炼板承载容量
        /// </summary>
        public int CapaCity
        {
            get { return capaCity; }
            set { capaCity = value; RaisePropertyChanged(); }
        }


        public string EquipmentNo
        {
            get { return equiomentNo; }
            set { equiomentNo = value; RaisePropertyChanged(); }
        }



        public string EquipmentName
        {
            get { return equipmentName; }
            set { equipmentName = value; RaisePropertyChanged(); }
        }




        public string EquipmentModel
        {
            get { return equipmentModel; }
            set { equipmentModel = value; RaisePropertyChanged(); }
        }


        public string Remark
        {
            get { return eqRemark; }
            set { eqRemark = value; RaisePropertyChanged(); }
        }

        private bool _havingEdit;

        public bool HavingEdit
        {
            get => _havingEdit;
            set => SetProperty(ref _havingEdit, value);

        }

        private bool _havingAdd;

        public bool HavingAdd
        {
            get => _havingAdd;
            set => SetProperty(ref _havingAdd, value);

        }

        private string _equipmentId { get; set; }




        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<string> BtnCommit { get; private set; }



        public string Title { get; set; } = "设备管理";



        #endregion

        public EquipmentManagementViewModel(IEventAggregator aggregator)
        {
            BtnCommit = new DelegateCommand<string>(HandelClick);
            HandleSelectModel = new DelegateCommand<SelectBasic>(HandleSelect);
            EquipmentType = new ObservableCollection<SelectBasic>(
                new List<SelectBasic>
                {
                    new SelectBasic(){Label="老炼板",Value="1"},
                    new SelectBasic(){Label="其他",Value="2"}
                }
                );
            CapaCity = 0;
            this.eventAggregator = aggregator;

        }

        private void HandleSelect(SelectBasic basic)
        {
            if (basic is null)
            {
                return;
            }
            ChoseEquipment = basic.Value;
            if (basic.Value == "1")
            {

                ShowCapaCity = true;
            }
            else
            {

                showCapaCity = false;
            }

        }

        private void HandelClick(string obj)
        {
            switch (obj)
            {
                case "Add":
                    AddNewEquipment();
                    break;
                case "Edit":
                    EditEquipment();
                    break;
                case "Cancel":
                    RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(ButtonResult.Cancel));
                    break;
            }
        }

        private void EditEquipment()
        {
            var result = Service.EquipmentService.EditEquipment(equipmentid: _equipmentId, equipmentNo: EquipmentNo, equipmentName: EquipmentName, remark: Remark,equipmentModel:EquipmentModel);
            if (result.ResultStatus)
            {
                this.eventAggregator.SendMessage(result.ResultMessage.ToString());
                ButtonResult btnResult = ButtonResult.None;
                RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(btnResult));

            }
            else
            {
                this.eventAggregator.SendMessage(result.ResultMessage.ToString());
                return;

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
                var eq = new Equipment("default_create_status")
                {
                    EquipmentID = Guid.NewGuid().ToString(),
                    EquipmentNo = EquipmentNo,
                    EquipmentName = EquipmentName,
                    EquipmentModel = EquipmentModel,
                    EquipmentType = choseEquipment,
                    Capacity = CapaCity,
                    AvailableCapacity = capaCity,
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
            if (parameters.ContainsKey("equipmentNo"))
            {
                var eqNo = parameters.GetValue<string>("equipmentNo");
                var eq = Service.EquipmentService.GetSingelEquipment(eqNo);
                _equipmentId = eq.EquipmentID;
                EquipmentNo = eq.EquipmentNo;
                EquipmentName = eq.EquipmentName;
                EquipmentModel = eq.EquipmentModel;
                Remark = eq.Remark;
                CapaCity = (int)eq.Capacity;
                ChoseEquipment = eq.EquipmentType;
                if (eq.EquipmentType == "1")
                {
                    ShowCapaCity = true;
                }
                else
                {
                    ShowCapaCity = false;
                }
                HavingAdd = false;
                HavingEdit = true;
            }
            else
            {
                HavingAdd = true;
                HavingEdit = false;
            }

        }
    }
}
