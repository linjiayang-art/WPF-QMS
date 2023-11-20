using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SicoreQMS.Common.Models.Operation;
namespace SicoreQMS.Common.Models.Operation
{
    public  partial class Prod_ProcessModel
    {
        public string SayContent()
        {
            string result = $"{Id},{ProcessType},{ProdProcessCard}";
            return result;
        }

    }
}
