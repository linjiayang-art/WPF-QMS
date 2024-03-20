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
    
    public partial class TestProcessItem
    {
        public string Id { get; set; }
        public string TestProcessId { get; set; }
        public string ModelId { get; set; }
        public string EquipmentNo { get; set; }
        public string TextNo { get; set; }
        public string ExperimentItemNo { get; set; }
        public string ExperimentName { get; set; }
        public string ExperimentStandard { get; set; }
        public string ExperimentConditions { get; set; }
        public Nullable<int> ExperimentQty { get; set; }
        public Nullable<int> ExperimentItemPassQty { get; set; }
        public string ExperimentNo { get; set; }
        public string ExperimentUser { get; set; }
        public string ExperimentBasis { get; set; }
        public string ExperimentType { get; set; }
        public Nullable<System.DateTime> EstimatedCompletionTime { get; set; }
        public Nullable<System.DateTime> ExperimentSatrtTime { get; set; }
        public Nullable<System.DateTime> ExperimentEndTime { get; set; }
        public Nullable<int> ExperimentStatus { get; set; }
        public string Remark { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string ModifyUserNo { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
        public string ProdId { get; set; }
        public Nullable<bool> AuditStatus { get; set; }
        public string StartUser { get; set; }
        public string EndUser { get; set; }
        public Nullable<int> ExperimentItemRank { get; set; }
        public string EquipmentId { get; set; }
        public string ItemDesc { get; set; }
        public string EquipmentList { get; set; }
    }
}
