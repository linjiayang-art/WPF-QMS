using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Report
{
    public class UsageRecordDTO
    {
        //public string Id { get; set; }
        //public string EquipmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // 假设EndDate是可空的
        public string UseType { get; set; }
        public string UseProcess { get; set; }
        public string UseUser { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentModel { get; set; }
        public string EquipmentNo { get; set; }
    }
}
