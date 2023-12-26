using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using Syncfusion.Pdf.Barcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class TestProcess: ISelectItem
    {
        public TestProcess()
        {
            CompleteStatus = false;
            Isdeletd = false;
            AuditStatus = false;
        }

     
        public SelectBasic ProductSelect()
        {
            return new SelectBasic()
            {
                Label = $" 产品型号: {ProdType} 批次号: {ProdLot}",
                Value = ProdId
            };
          
        }

        public SelectBasic GetSelection()
        {
           return new SelectBasic()
           {
               Label=$"{ProdName}|{ProdType}|{ProdLot}|{Prodstandard} ",
               Value=Id
           };
        }
    }
}
