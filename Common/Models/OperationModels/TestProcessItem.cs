using Prism.Mvvm;
using SicoreQMS.Common.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class TestProcessItem:BindableBase
    {
        /// <summary>
        /// 状态 0为未开始，1为开始，2为完成，3为异常
        /// 
        /// </summary>
        /// 

  

        public bool IsButtonEnabled
        {
            get
            {
                if (ExperimentStatus== 0&& AuditStatus == true)
                {
                    return true;
                }
                if (ExperimentStatus == 1 && AuditStatus == true)
                {
                    return true;
                }

                else { return false; }

            }
            set { RaisePropertyChanged(); }
        }

        public bool IsEditButtonEnabled
        {
            get
            {
                if (ExperimentStatus == 0 && AuditStatus == true)
                {
                    return false;
                }
                if (ExperimentStatus == 1 && AuditStatus == true)
                {
                    return false;
                }
                if (ExperimentStatus == 2 && AuditStatus == true)
                {
                    return true;
                }

                else { return false; }

            }
            set { RaisePropertyChanged(); }
        }

        public bool IsAuditButtonEnabled
        {
            get
            {
                if (AuditStatus == false)
                {
                    return true;
                }
              
                else { return false; }

            }
            set { RaisePropertyChanged(); }
        }

        public TestProcessItem()
        {
           AuditStatus = false;
            ExperimentStatus = 0;
            IsDeleted = false;
            CreateDate = DateTime.Now;
            CreateUser=AppSession.UserID;
            
        }

    }
}
