using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Report
{
    public class EquipmentUsageDetailModel
    {
        public int Sort { get; set; }
        public string UseType { get; set; }
        public string UseProcess { get; set; }
        public  string UseUser { get; set; }
        public string ProdLot { get;set; }
        public string ProdType { get; set; }
        public int UseCount { get; set; }
        public int Qty { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProcessId { get; set; }

    }
}


