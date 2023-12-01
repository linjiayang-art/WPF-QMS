using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
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
