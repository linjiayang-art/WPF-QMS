using SicoreQMS.Common.Models.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class Prod_Process
    {
        public Prod_Process()

        {
            CurrentProcess = 1;
            // 设置默认值
            ProdStatus = 0;
            IsDeleted = false;
            CreateDate = DateTime.Now;

        }


        public SelectBasic ProductSelect()
        {
            string label = $" {ProdType} :{ProdLot} : {Qty}";
            return new SelectBasic { Label = label, Value = Id };
        }


    }
}
