using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.ViewModels.DialogModels.ProdProcessModels
{
    public class MaterialRequisitionViewModel : BindableBase, IRegionMemberLifetime, INavigationAware
    {

        #region

        private string _id;




        private string _serachText;



        private Prod_ProcessItem _processItem;
        /// <summary>
        /// 筛选后的设备列表
        /// </summary>
        public ObservableCollection<MultiSelectBasic> FilterEquipmentList { get; set; }
        public ObservableCollection<MultiSelectBasic> EquipemtList { get; set; }

        private string _checkEquipmentNo;

        public string CheckEquipmentNo
        {
            get =>  _checkEquipmentNo;
            set => SetProperty(ref  _checkEquipmentNo, value);

        }


        public string SearchText
        {
            get => _serachText;
            set => SetProperty(ref _serachText, value);

        }

        public Prod_ProcessItem ProcessItem
        {
            get => _processItem;
            set => SetProperty(ref _processItem, value);

        }

        public bool KeepAlive { get; set; }=false;
       


        #endregion

        public MaterialRequisitionViewModel()
        {
            CheckCommand = new DelegateCommand<MultiSelectBasic>(CheckEquipment);
            UnCheckCommand = new DelegateCommand<MultiSelectBasic>(CheckEquipment);
            EquipemtList = Service.EquipmentService.GetMultiEquipmentBasic();
            FilterEquipmentList = EquipemtList;


        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //判断是否有这个Key
            if (navigationContext.Parameters.ContainsKey("Id"))
            {
                //取出传过来的值
                _id = navigationContext.Parameters.GetValue<string>("Id");
            }

            using (var context = new SicoreQMSEntities1())
            {
                this.ProcessItem = context.Prod_ProcessItem.Where(p => p.Id == _id).SingleOrDefault();

            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        ///设备
        ///
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

        public DelegateCommand<MultiSelectBasic> CheckCommand { get; set; }
        public DelegateCommand<MultiSelectBasic> UnCheckCommand { get; set; }
        
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
    }
}
