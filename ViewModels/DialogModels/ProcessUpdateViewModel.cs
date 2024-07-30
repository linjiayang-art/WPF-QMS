using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Extensions;
using SicoreQMS.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SicoreQMS.ViewModels.DialogModels
{
    public class ProcessUpdateViewModel : BindableBase, IDialogAware
    {

        public ProcessUpdateViewModel(IEventAggregator aggregator)
        {
            Title = "生产流程卡进度更新";
            BtnStart = new DelegateCommand(ProcessStart, CanStartExecute);
            BtnEnd = new DelegateCommand(ProcessEnd, CanEndExecute);
            CheckCommand = new DelegateCommand<MultiSelectBasic>(CheckEquipment);
            UnCheckCommand = new DelegateCommand<MultiSelectBasic>(CheckEquipment);
            SPUnCheckCommand= new DelegateCommand<MultiSelectBasic>(SPCheckEquipment);
            SPCheckCommand = new DelegateCommand<MultiSelectBasic>(SPCheckEquipment);
            IsStartEnabled = false;
            IsEndEnabled = false;
            this.aggregator = aggregator;
            EquipemtList = Service.EquipmentService.GetMultiEquipmentBasic();
            SpEquipmentList = Service.EquipmentService.GetMultiSpEquipmentBasic();
            ChoseSpEquipment=new ObservableCollection<SPEquipment>();
            FilterEquipmentList = EquipemtList;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            
        }

        private void SPCheckEquipment(MultiSelectBasic basic)
        {
            var spId = basic.Value;
            using (var context = new SicoreQMSEntities1())
            {
                var spEquipment = context.Equipment.Where(p => p.EquipmentID== spId && p.IsDeleted == false ).SingleOrDefault();

                if (spEquipment is null)
                {
                    return;
                }

                if (basic.IsCheck)
                {
                    if (ChoseSpEquipment.Any(p=>p.EquipmentID==spId) )
                    {
                        return;

                    }
                    ChoseSpEquipment.Add(new SPEquipment(spEquipment));


                }
                else
                {
                    ChoseSpEquipment.Remove(ChoseSpEquipment.SingleOrDefault(x => x.EquipmentID == spId));


                }

            }

        }

        private void getProductSPNo()
        {

            var indesx=prodType.ToLower().IndexOf("sp");
            if (indesx > 0)
            {
                SPNo = prodType.Substring(indesx,3);
                
            }
            SPEquipmentList = Service.EquipmentService.GetSPEquipment(SPNo);
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

        private void SelectEquipment(string obj)
        {

            aggregator.SendMessage(obj);
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {

            RequestClose.Invoke(dialogResult);
        }



        private bool CanStartExecute()
        {
            return IsStartEnabled;

        }
        private bool CanEndExecute()
        {
            return IsEndEnabled;

        }


        public DelegateCommand BtnStart { get; set; }
        public DelegateCommand BtnEnd { get; set; }

        #region  属性

        private DateTime staretime;
        private DateTime endTime;
        private string equipmentNo;
        private string _equipmentId;
        private string searchText;
        public string checkEquipmentNo;
        private string _beginRemark;
        private string _endRemark;
        private string _id;
        private bool _isStartEnabled;
        private string prodType;
        public string SPNo { get; set; }
        private bool isUseSP;

        public bool IsUseSP
        {
            get { return isUseSP; }
            set { isUseSP = value; RaisePropertyChanged(); }
        }



        private ObservableCollection<SPEquipment> spEquipmentList;

        public ObservableCollection<SPEquipment> SPEquipmentList
        {
            get { return spEquipmentList; }
            set { spEquipmentList = value; RaisePropertyChanged(); }
        }


        //老炼板
        private ObservableCollection<MultiSelectBasic> _spEquipmentList;

        public ObservableCollection<MultiSelectBasic> SpEquipmentList
        {
            get => _spEquipmentList;
            set => SetProperty(ref _spEquipmentList, value);

        }

       

        public ObservableCollection<SPEquipment> ChoseSpEquipment { get; set; }
     
        private string _spSerchText;

        public DelegateCommand<MultiSelectBasic> SPCheckCommand { get; set; }
        public DelegateCommand<MultiSelectBasic> SPUnCheckCommand { get; set; }

        public string SpSerchText
        {
            get => _spSerchText;
            set => SetProperty(ref _spSerchText, value);

        }





        public DateTime StartTime
        {
            get { return staretime; }
            set { SetProperty(ref staretime, value); }
        }



        public DateTime EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value); }
        }



        public DelegateCommand<MultiSelectBasic> CheckCommand { get; set; }
        public DelegateCommand<MultiSelectBasic> UnCheckCommand { get; set; }



        public string EquipmentNo
        {
            get { return equipmentNo; }
            set { SetProperty(ref equipmentNo, value); }
        }



        public string CheckEquipmentNo
        {
            get { return checkEquipmentNo; }
            set { SetProperty(ref checkEquipmentNo, value); }
        }

        public string EquipmentId
        {
            get { return _equipmentId; }
            set
            {
                SetProperty(ref _equipmentId, value);
            }
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

        private ObservableCollection<MultiSelectBasic> filterEquipmentlist;

        public ObservableCollection<MultiSelectBasic> FilterEquipmentList
        {
            get { return filterEquipmentlist; }
            set { filterEquipmentlist = value; RaisePropertyChanged(); }
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                SetProperty(ref searchText, value);
                PerformFiltering();
            }
        }

        public string BeginRemark
        {
            get { return _beginRemark; }
            set { SetProperty(ref _beginRemark, value); }
        }

        public string EndRemark
        {
            get { return _endRemark; }
            set { SetProperty(ref _endRemark, value); }
        }

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

        private IEventAggregator aggregator;

        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _prodProcessCard;
        public string ProdProcessCard
        {
            get { return _prodProcessCard; }
            set { SetProperty(ref _prodProcessCard, value); }
        }

        private string _processType;
        public string ProcessType
        {
            get { return _processType; }
            set { SetProperty(ref _processType, value); }
        }

        private int _outQty;
        public int OutQty
        {
            get { return _outQty; }
            set { SetProperty(ref _outQty, value); }
        }

        private int _inputQty;
        public int InputQty
        {
            get { return _inputQty; }
            set { SetProperty(ref _inputQty, value) ;RaisePropertyChanged(); }
        }
        private string _prodStandard;


        public string ProdStandard
        {
            get { return _prodStandard; }
            set { _prodStandard = value; }
        }
        #endregion

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
        private void ProcessEnd()
        {


            var result = ProdProcessService.EndProcess(id: Id, qty: OutQty, remark: EndRemark, endTime: EndTime);

            if (result.ResultStatus == false)
            {
                aggregator.SendMessage(result.ResultMessage);
                return;
            }

            //获取设备的ID


            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.SingleOrDefault(b => b.Id == Id);
                var orginEqIdList = prodProcessItem.EquipmentId;


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
                        var a = EquipmentService.RecordEquipmentLog(eqId, "生产流程卡", ProcessType);
                    }

                }

            }

            //更新老炼板记录
           Service.EquipmentService.EndSPEquipment(Id);

            ButtonResult btnResult = ButtonResult.None;

            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(btnResult));

            aggregator.SendMessage(result.ResultMessage);

        }
        /// <summary>
        /// 产品进度更新
        /// </summary>
        private void ProcessStart()
        {

            var eqList = CheckEquipmentNo?.Split(';') ?? new string[0];

            var eqIdList = "";

            if (IsUseSP)
            {
                var spCount = 0;
                foreach (var item in ChoseSpEquipment)
                {
                    spCount += item.UseQty;
                    if (item.Capacity < item.UseQty)
                    {
                        aggregator.SendMessage("老炼投入数量大于设备总容量!");
                        return;
                    }
                    if (item.AvailableCapacity<item.UseQty)
                    {
                        aggregator.SendMessage("老炼投入数量大于当前设备容量!");
                        return;
                    }
                }
                if (spCount != InputQty)
                {
                    aggregator.SendMessage("老炼数量不等于投入数量!");
                    return;
                }
  
                foreach (var item in ChoseSpEquipment)
                {
                    item.AddSpUsage("生产流程卡", ProcessType,Id);
                }
            }

            foreach (var item in eqList)
            {
                var eqId = EquipemtList.SingleOrDefault(x => x.Label == item).Value;
                eqIdList += eqId + ";";
            }



            if (eqIdList.Length > 0)
            {
                eqIdList = eqIdList.Substring(0, eqIdList.Length - 1);
            }


            var result = ProdProcessService.BeginProcess(id: Id, qty: InputQty, equipmentList: CheckEquipmentNo, equipmentId: eqIdList, remark: BeginRemark, startTime: StartTime);

            if (result.ResultStatus == false)
            {
                aggregator.SendMessage(result.ResultMessage);
                return;
            }

            if (!string.IsNullOrEmpty(CheckEquipmentNo))
            {


                foreach (var item in eqList)
                {
                    var eqId = EquipemtList.SingleOrDefault(x => x.Label == item).Value;
                    var a = EquipmentService.RecordEquipmentLog(eqId, "生产流程卡", ProcessType,qty:InputQty, Id);
                }

            }
            //设备记录



            ButtonResult btnResult = ButtonResult.None;

            RaiseRequestClose(new Prism.Services.Dialogs.DialogResult(btnResult));

            aggregator.SendMessage(result.ResultMessage);
        }

        public string Title { get; set; }

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
            Id = parameters.GetValue<string>("Id");

            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.SingleOrDefault(b => b.Id == Id);
                if (prodProcessItem != null)
                {
                    prodType= prodProcessItem.ProdType;
                    ProdProcessCard = prodProcessItem.ProdProcessCard;
                    ProcessType = prodProcessItem.ProcessType;
                    OutQty = prodProcessItem.OutQty ?? 0;
                    InputQty = prodProcessItem.InputQty ?? 0;
                    ProdStandard = prodProcessItem.ProdStandard;

                }

              

                else
                {
                    MessageBox.Show("未查询到该产品");
                }

                if (InputQty == 0)
                {
                    var prodprocess = context.Prod_Process.Find(prodProcessItem.ProdProcessId);
                    InputQty = (int)prodprocess.Qty;
                }

                getProductSPNo();
                if (prodProcessItem.ItemStatus == 0)
                {
                    IsStartEnabled = true;
                    IsEndEnabled = false;
                    return;
                }
                if (prodProcessItem.ItemStatus == 1)
                {
                    var eq = prodProcessItem.EquipmentList;
                    //var eq = context.Equipment.SingleOrDefault(e => e.EquipmentID == prodProcessItem.EquipmentId);

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

           
        }




    }
}
