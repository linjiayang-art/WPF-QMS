//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SicoreQMS.Common.Models.Operation
{
    using System;
    using System.Collections.Generic;
    
    public partial class UsageRecord
    {
        public string Id { get; set; }
        public string EquipmentId { get; set; }
        public string UseType { get; set; }
        public string UseProcess { get; set; }
        public string UseUser { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Remark { get; set; }
        public string ProcessId { get; set; }
        public Nullable<int> Qty { get; set; }
    }
}
