using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
   public partial class Prod_ProcessItem
    {
        public Prod_ProcessItem()
        {
                IsComplete = false;
        }

        public void CopyModelData(Prod_ProcessModel  prod_Process )
        {
          ModelSort= prod_Process.ModelSort;
          ProdProcessCard= prod_Process.ProdProcessCard;
          ProcessType= prod_Process.ProcessType;
          ProdStandard= prod_Process.ProdStandard;
          CheckStandard= prod_Process.CheckStandard;

        }
    }
}
