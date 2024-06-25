using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Basic
{
   public class ProdStructureModel
    {
        public string ProdType { get; set; }

        public string ProdNo { get; set; }

        public string TestNo { get; set; }

        public ProdStructureModel(string prodType, string prodNo,string testNo=null )
        {
            this.ProdNo= prodNo;
            this.ProdType = prodType;
            this.TestNo = testNo;
               
        }
    }
}
