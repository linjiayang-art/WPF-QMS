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
    
    public partial class LotRelation
    {
        public string Id { get; set; }
        public string ProdId { get; set; }
        public string ParentId { get; set; }
        public string ChildrenId { get; set; }
        public string RelationType { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}
