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
    
    public partial class ProdInfo
    {
        public string Id { get; set; }
        public string ProdName { get; set; }
        public string ProdType { get; set; }
        public Nullable<int> ProdStatus { get; set; }
        public string ProdLot { get; set; }
        public string CheckLot { get; set; }
        public string QualityLevel { get; set; }
        public string CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
