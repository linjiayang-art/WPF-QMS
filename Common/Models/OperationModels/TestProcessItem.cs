using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class TestProcessItem
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
                if (ExperimentStatus== 0)
                {
                    return true;
                }
                if (ExperimentStatus == 1)
                {
                    return true;
                }

                else { return false; }

            }
            set { }
        }

        public TestProcessItem()
        {
           
            ExperimentStatus = 0;
            IsDeleted = false;
            CreateDate = DateTime.Now;
        }

    }
}
