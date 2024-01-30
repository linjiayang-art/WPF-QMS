﻿using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Interface;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using SicoreQMS.Service;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class TestProcessItemUpdateViewModel : BindableBase, IDialogAware
    {


        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }




        public TestProcessItemUpdateViewModel()
        {
            DialogHostName = "Root";


            BtnStart = new DelegateCommand(ProcessStart, CanStartExecute);
            BtnEnd = new DelegateCommand(ProcessEnd, CanEndExecute);

            Title = "试验流程卡进度更新";
            EquipemtList = Service.EquipmentService.GetEquipmentBasic();
        }


        /// <summary>
        /// 窗口关闭
        /// </summary>
        /// <param name="dialogResult"></param>
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }


        private void ProcessEnd()
        {
            var result_info = TestProcessService.EndTest(id: Id, passQty: PassQty, remark: Remark);
            if (result_info.ResultStatus == false)
            {
                System.Windows.Forms.MessageBox.Show(result_info.ResultMessage);
                return;
            }




            if (!string.IsNullOrEmpty(EquipmentId))
            {
                var a = EquipmentService.RecordEquipmentLog(EquipmentId, "试验流程卡", TestItem.ExperimentName);

            }

            var dialogResult = new Prism.Services.Dialogs.DialogResult(ButtonResult.OK, new DialogParameters { { "key", result_info.ResultMessage } });

            RaiseRequestClose(dialogResult);

        }
        private void ProcessStart()
        {
            var result_info = TestProcessService.StartTset(id: Id, passQty: PassQty, remark: Remark,equipmentid:EquipmentId);

            if (result_info.ResultStatus == false)
            {
                System.Windows.Forms.MessageBox.Show(result_info.ResultMessage);
                return;
            }


            if (!string.IsNullOrEmpty(EquipmentId))
            {
                var a = EquipmentService.RecordEquipmentLog(EquipmentId, "试验流程卡", TestItem.ExperimentName);

            }


            var dialogResult = new Prism.Services.Dialogs.DialogResult(ButtonResult.OK, new DialogParameters { { "key", result_info.ResultMessage } });



            RaiseRequestClose(dialogResult);

        }





        #region 属性
        /// <summary>
        /// 按钮相关
        /// </summary>
        /// 
        #region EquipemtList
        public ObservableCollection<SelectBasic> _equipemtList { get; set; }
        public ObservableCollection<SelectBasic> EquipemtList
        {
            get { return _equipemtList; }
            set
            {
                _equipemtList = value; RaisePropertyChanged();
            }
        }
        #endregion
        #region  equipmentId
        private string _equipmentId;

        public string EquipmentId
        {
            get { return _equipmentId; }
            set
            {
                SetProperty(ref _equipmentId, value);
            }
        }
        #endregion


        private string equipmentNo;

        public string EquipmentNo
        {
            get { return equipmentNo; }
            set { SetProperty(ref equipmentNo, value); }
        }


        public DelegateCommand BtnStart { get; set; }
        public DelegateCommand BtnEnd { get; set; }

        private string _canVisabile;

        public string CanVisabile
        {
            get { return _canVisabile; }
            set { SetProperty(ref _canVisabile, value); RaisePropertyChanged(); }
        }


        private bool _isStartEnabled;
        public bool IsStartEnabled
        {
            get { return _isStartEnabled; }
            set
            {
                SetProperty(ref _isStartEnabled, value);
                RaisePropertyChanged();
            }
        }



        private bool _isEndEnabled;
        public bool IsEndEnabled
        {
            get { return _isEndEnabled; }
            set
            {
                SetProperty(ref _isEndEnabled, value);
                RaisePropertyChanged();
            }
        }

        private bool CanStartExecute()
        {
            return IsStartEnabled;

        }
        private bool CanEndExecute()
        {
            return IsEndEnabled;

        }
        /// <summary>
        /// 字段属性
        /// </summary>

        public string Id { get; set; }
        private string _remark;

        public string Remark
        {
            get { return _remark; }
            set { SetProperty(ref _remark, value); }
        }

        private int _passQty;

        public int PassQty
        {
            get { return _passQty; }
            set { SetProperty(ref _passQty, value); }
        }


        private TestProcessItem _testItem;

        public TestProcessItem TestItem
        {
            get { return _testItem; }
            set { SetProperty(ref _testItem, value); }

        }

        public string Title { get; set; }

        public event Action<IDialogResult> RequestClose;

        #endregion


        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

            Id = parameters.GetValue<string>("Id");

            using (var context = new SicoreQMSEntities1())
            {
                TestItem = context.TestProcessItem.Find(Id);
            }
            if (TestItem.ExperimentStatus == 0)
            {
                IsStartEnabled = true;
                IsEndEnabled = false;
                return;
            }
            if (TestItem.ExperimentStatus == 1)
            {
                using (var context = new SicoreQMSEntities1())
                {
                    var eq = context.Equipment.SingleOrDefault(e => e.EquipmentID == TestItem.EquipmentId);
                    if (eq!=null)
                    {
                        EquipmentNo = eq.EquipmentNo;
                        EquipmentId = eq.EquipmentID;

                    }
                    else
                    {
                        EquipmentNo = "未使用设备";

                    }
                 
                }
                    
                IsStartEnabled = false;
                IsEndEnabled = true;
                return;
            }
            

        }

        public void OnDialogOpend(IDialogParameters parameters)
        {

            Id = parameters.GetValue<string>("Id");

            using (var context = new SicoreQMSEntities1())
            {
                TestItem = context.TestProcessItem.Find(Id);

                if (TestItem.ExperimentStatus == 0)
                {
                    IsStartEnabled = true;
                    IsEndEnabled = false;
                    return;
                }
                if (TestItem.ExperimentStatus == 1)
                {
                    var eq = context.Equipment.SingleOrDefault(e => e.EquipmentID == TestItem.EquipmentId);
                    EquipmentNo = eq.EquipmentNo;
                    EquipmentId= eq.EquipmentID;
                    IsStartEnabled = false;
                    IsEndEnabled = true;
                    return;
                }
            }
        }
    }
}
