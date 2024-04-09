﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SicoreQMSEntities1 : DbContext
    {
        public SicoreQMSEntities1()
            : base("name=SicoreQMSEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Prod_ProcessModel> Prod_ProcessModel { get; set; }
        public virtual DbSet<Prod_ProcessBasic> Prod_ProcessBasic { get; set; }
        public virtual DbSet<ProdInfo> ProdInfo { get; set; }
        public virtual DbSet<Prod_Process> Prod_Process { get; set; }
        public virtual DbSet<Prod_ProcessItem> Prod_ProcessItem { get; set; }
        public virtual DbSet<LotRelation> LotRelation { get; set; }
        public virtual DbSet<TestModelBasic> TestModelBasic { get; set; }
        public virtual DbSet<TestProcess> TestProcess { get; set; }
        public virtual DbSet<TestModelItem> TestModelItem { get; set; }
        public virtual DbSet<TestProcessItem> TestProcessItem { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<EquipmentStatus> EquipmentStatus { get; set; }
        public virtual DbSet<UsageRecord> UsageRecord { get; set; }
        public virtual DbSet<Menus> Menus { get; set; }
    
        public virtual ObjectResult<proc_QAExperimentReport_Result> proc_QAExperimentReport(string lotNo)
        {
            var lotNoParameter = lotNo != null ?
                new ObjectParameter("lotNo", lotNo) :
                new ObjectParameter("lotNo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<proc_QAExperimentReport_Result>("proc_QAExperimentReport", lotNoParameter);
        }
    }
}
