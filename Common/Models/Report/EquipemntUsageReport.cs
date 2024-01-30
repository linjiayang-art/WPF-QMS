using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Report
{
    public class EquipemntUsageReport
    {
        public string EquipmentName { get; set; }
        public string EquipmentNo { get; set; }
        public string EquipmentID { get; set; }

        public string EquipmentModel{ get; set; }
        public string StatusDesc { get; set; }

        public int EquipmentStatus1 { get; set; }

        public int TotalUsageDays { get; set; } = 0;
    }
}
