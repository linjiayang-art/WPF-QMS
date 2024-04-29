using DocumentFormat.OpenXml.Drawing;
using ImTools;
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
            CheckCommand = new DelegateCommand<MultiSelectBasic>(CheckEquipment);
            UnCheckCommand = new DelegateCommand<MultiSelectBasic>(CheckEquipment);
          
            BtnStart = new DelegateCommand(ProcessStart, CanStartExecute);
            BtnEnd = new DelegateCommand(ProcessEnd, CanEndExecute);
            StartTime = DateTime.Now;
            Title = "试验流程卡进度更新";
            EquipemtList = Service.EquipmentService.GetMultiEquipmentBasic();
            FilterEquipmentList = EquipemtList;
        }


        private void CheckEquipment(MultiSelectBasic obj)
        {
            CheckEquipmentNo = "";
            var checkList = EquipemtList.Where(item => item.IsCheck == true).ToList();
            foreach (var item in checkList)
            {
                CheckEquipmentNo += item.Label + ";";
            }
            //去除最后一个;
            if (CheckEquipmentNo.Length > 0)
            {
                CheckEquipmentNo = CheckEquipmentNo.Substring(0, CheckEquipmentNo.Length - 1);
            }
            FilterEquipmentList.OrderBy(x => x.IsCheck);
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

            using (var context = new SicoreQMSEntities1())
            {
                var testItem = context.TestProcessItem.SingleOrDefault(b => b.Id == Id);
                var orginEqIdList = testItem.EquipmentId;


                if (orginEqIdList != null)
                {
                    var eqList = orginEqIdList.Split(';');

                    foreach (var item in eqList)
                    {
                        if (string.IsNullOrEmpty(item))
                        {
                            continue;
                        }
                        var equipment = context.Equipment.SingleOrDefault(e => e.EquipmentID == item);

                        var eqId = equipment.EquipmentID;
                        var a = EquipmentService.RecordEquipmentLog(eqId, "试验流程卡", testItem.ExperimentName);
                    }

                }

            }


            //if (!string.IsNullOrEmpty(EquipmentId))
            //{
            //    var a = EquipmentService.RecordEquipmentLog(EquipmentId, "试验流程卡", TestItem.ExperimentName);

            //}

            var dialogResult = new Prism.Services.Dialogs.DialogResult(ButtonResult.OK, new DialogParameters { { "key", result_info.ResultMessage } });

            RaiseRequestClose(dialogResult);

        }
        private void ProcessStart()
        {

            var eqList = CheckEquipmentNo?.Split(';') ?? new string[0];

            var eqIdList = "";
            foreach (var item in eqList)
            {
                var eqId = EquipemtList.SingleOrDefault(x => x.Label == item).Value;
                eqIdList += eqId + ";";
            }


            if (eqIdList.Length > 0)
            {
                eqIdList = eqIdList.Substring(0, eqIdList.Length - 1);
            }


     
            var result_info = TestProcessService.StartTset(id: Id, passQty: PassQty, remark: Remark,   equipmentid: eqIdList, equipmentList: CheckEquipmentNo,startTime: StartTime);

            if (result_info.ResultStatus == false)
            {
                System.Windows.Forms.MessageBox.Show(result_info.ResultMessage);
                return;
            }


            if (!string.IsNullOrEmpty(CheckEquipmentNo))
            {
                foreach (var item in eqList)
                {
                    var eqId = EquipemtList.SingleOrDefault(x => x.Label == item).Value;
                    var a = EquipmentService.RecordEquipmentLog(eqId, "试验流程卡", TestItem.ExperimentName, Id);
                }
                

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



        private DateTime startTime;

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value;RaisePropertyChanged(); }
        }

        public ObservableCollection<MultiSelectBasic> _equipemtList { get; set; }
        public ObservableCollection<MultiSelectBasic> EquipemtList
        {
            get { return _equipemtList; }
            set
            {
                _equipemtList = value;


                RaisePropertyChanged();
            }
        }



        public string checkEquipmentNo;
        public string CheckEquipmentNo
        {
            get { return checkEquipmentNo; }
            set { SetProperty(ref checkEquipmentNo, value); }
        }

        public DelegateCommand<MultiSelectBasic> CheckCommand { get; set; }
        public DelegateCommand<MultiSelectBasic> UnCheckCommand { get; set; }


        private ObservableCollection<MultiSelectBasic> filterEquipmentlist;

        public ObservableCollection<MultiSelectBasic> FilterEquipmentList
        {
            get { return filterEquipmentlist; }
            set { filterEquipmentlist = value; RaisePropertyChanged(); }
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


    

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetProperty(ref searchText, value);
                PerformFiltering();
            }
        }
  


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
                var eq = TestItem.EquipmentList;
                if (eq != null)
                {
                    EquipmentNo = eq;

                }
                else
                {
                    EquipmentNo = "未使用设备";
                }

                IsStartEnabled = false;
                IsEndEnabled = true;
                return;
            }
            

        }
        private void PerformFiltering()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilterEquipmentList = EquipemtList;
            }
            else
            {
                FilterEquipmentList = new ObservableCollection<MultiSelectBasic>(
                   EquipemtList.Where(item => item.Label.ToLower().Contains(SearchText.ToLower())));

                var checklist = EquipemtList.Where(item => item.IsCheck == true).ToList();
                foreach (var item in checklist)
                {
                    var a = FilterEquipmentList.SingleOrDefault(x => x.Label == item.Label);
                    if (a == null)
                    {
                        FilterEquipmentList.Add(item);
                    }

                }


            }
            FilterEquipmentList.OrderBy(x => x.IsCheck);
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
                if (TestItem.ExperimentSatrtTime!=null)
                {
                    StartTime =TestItem.ExperimentSatrtTime.Value;

                }
                else
                {
                    StartTime = DateTime.Now;
                }
            }
        }
    }
}
