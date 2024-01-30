using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class Prod_ProcessItem : BindableBase
    {

        /// <summary>
        /// itemStatus 0未开始 1开始 2完成 3跳过 4暂定 5拆分批次无法进行
        /// </summary>
        public Prod_ProcessItem()
        {

            IsComplete = false;
            ItemStatus = 0;
            IsDeleted = false;
            CreateDate = DateTime.Now;
            InputQty = 0;
        }

        private bool _isButtonEnabled;
        public bool IsButtonEnabled
        {
            get
            {
                if (ItemStatus == 5)
                {
                    return false;
                }

                if (IsComplete == false)
                {
                    return true;
                }
                else { return false; }
                ;
            }

            set { SetProperty(ref _isButtonEnabled, value); }
        }

        private string _nowStatus;
        public string NowStatus
        {
            /// <summary>
            /// itemStatus 0未开始 1开始 2完成 3跳过 4暂定 5拆分批次无法进行
            /// </summary>
            get
            {
                if (ItemStatus == 0)
                {
                    return "未开始";
                }
                if (ItemStatus == 1)
                {
                    return "进行中";
                }
                if (ItemStatus == 2)
                {
                    return "已完结";
                }
                if (ItemStatus == 5)
                {
                    return "批次拆分,当前批次已锁定";
                }
                return "状态未明确";
                ;
            }

            set { SetProperty(ref _nowStatus, value); }
        }

        public void CopyModelData(Prod_ProcessModel prod_Process)
        {
            ModelSort = prod_Process.ModelSort;
            ProdProcessCard = prod_Process.ProdProcessCard;
            ProcessType = prod_Process.ProcessType;
            ProdStandard = prod_Process.ProdStandard;
            CheckStandard = prod_Process.CheckStandard;

        }
    }
}
