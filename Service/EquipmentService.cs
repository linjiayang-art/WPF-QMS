using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Models.Report;
using SicoreQMS.Common.Server;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SicoreQMS.Service
{
    public class EquipmentService
    {


        public static ObservableCollection<string> GetEquipmentList()
        {
            var results = new ObservableCollection<string>();
            using (var context = new SicoreQMSEntities1())
            {
                var query = from e in context.Equipment
                            join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
                            where eStatus.EquipmentStatus1 == 0
                            select new
                            {
                                e.EquipmentName,
                                EquipmentNo = e.EquipmentNo,
                                EquipmentID = e.EquipmentID

                            };
                var list = query.ToList();
                foreach (var r in list)
                {
                    results.Add( r.EquipmentNo );
                }
                return results;
            }

        }

        public static ObservableCollection<SelectBasic> GetEquipmentBasic()
        {
            var results = new ObservableCollection<SelectBasic>();
            using (var context = new SicoreQMSEntities1())
            {


                var query = from e in context.Equipment
                            join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
                            where eStatus.EquipmentStatus1 == 0
                            select new
                            {
                                e.EquipmentName,
                                EquipmentNo = e.EquipmentNo,
                                EquipmentID = e.EquipmentID

                            };
                var list = query.ToList();
                foreach (var r in list)
                {
                    results.Add(new SelectBasic { Value = r.EquipmentID, Label = r.EquipmentName + r.EquipmentNo });
                }

                return results;
            }
        }

        public static ObservableCollection<MultiSelectBasic> GetMultiEquipmentBasic()
        {
            var results = new ObservableCollection<MultiSelectBasic>();
            using (var context = new SicoreQMSEntities1())
            {


                var query = from e in context.Equipment
                            join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
                            where eStatus.EquipmentStatus1 == 0
                            select new
                            {
                                e.EquipmentName,
                                EquipmentNo = e.EquipmentNo,
                                EquipmentID = e.EquipmentID

                            };
                var list = query.ToList();
                foreach (var r in list)
                {
                    results.Add(new MultiSelectBasic { Value = r.EquipmentID, Label = r.EquipmentName + r.EquipmentNo ,IsCheck=false});
                }

                return results;
            }

        }


        public static ResultInfo RecordEquipmentLog(string equipmentId,string useType,string useProcess,string processId=null)
        {
            using (var context=new SicoreQMSEntities1())
            {
                var equipment = context.Equipment.SingleOrDefault(e => e.EquipmentID == equipmentId);
                if (equipment == null)
                {
                    return new ResultInfo { ResultStatus = false, ResultMessage = "设备不存在" };
                }
                var equipmentStatus = context.EquipmentStatus.SingleOrDefault(e => e.EquipmentID == equipmentId);
                //如果设备为0则表示设备空闲,更新为1,新增设备使用记录
                if (equipmentStatus.EquipmentStatus1 == 0)
                {
                    var usageRecord = new UsageRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProcessId=processId,
                        EquipmentId = equipmentId,
                        StartDate = DateTime.Now,
                        UseType = useType,
                        UseProcess = useProcess,
                        UseUser= AppSession.UserID
                    };
                    context.UsageRecord.Add(usageRecord);
                    equipmentStatus.EquipmentStatus1 = 1;
                    equipmentStatus.StatusDesc = "运行中";
                    context.SaveChanges();
                    return new ResultInfo()
                    { ResultStatus = true, ResultMessage = "设备状态更新成功" };
                }

                if (equipmentStatus.EquipmentStatus1 == 1)
                {
                    var usageRecord = context.UsageRecord.SingleOrDefault(u => u.EquipmentId == equipmentId && u.EndDate == null&&u.UseProcess==useProcess );
                    if (usageRecord == null)
                    {
                        return new ResultInfo { ResultStatus = false, ResultMessage = "设备使用记录不存在" };
                    }
                    usageRecord.EndDate = DateTime.Now;
                    equipmentStatus.EquipmentStatus1 = 0;
                    equipmentStatus.StatusDesc = "待机";
                    context.SaveChanges();
                }
        
            }

            return new ResultInfo()
             { ResultStatus = true, ResultMessage = "设备状态更新成功" };
        }


        public static ObservableCollection<EquipemntUsageReport> GetEquipmentStatus(string equipmentNoFilter=null)
        {
            var results = new ObservableCollection<EquipemntUsageReport>();



            using (var context = new SicoreQMSEntities1())
            {
                var usages = context.UsageRecord.ToList();

                var now = DateTime.Now;
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

                var usageSummary = usages
                     .Where(u => u.StartDate >= firstDayOfMonth)
                   .GroupBy(u => u.EquipmentId)
                   .Select(group => new
                   {
                       EquipmentId = group.Key,
                       TotalDays = group.Sum(g =>
                       {
                           // 计算每个使用记录的天数
                           DateTime end = g.EndDate ?? DateTime.Now; // 如果EndDate为null，使用当前日期
                           TimeSpan duration = (TimeSpan)(end - g.StartDate);
                           return duration.Days + 1; // 包含起始日期
                       })
                   }).ToDictionary(u => u.EquipmentId, u => u.TotalDays);
                // 创建设备状态报告的查询
                var equipmentData =( from e in context.Equipment
                                     where string.IsNullOrEmpty(equipmentNoFilter) || e.EquipmentNo.Contains(equipmentNoFilter)
                                     join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
                            select new 
                            {
                                e.EquipmentName,
                                e.EquipmentModel,
                                e.EquipmentNo,
                                e.EquipmentID,
                                eStatus.EquipmentStatus1,
                                eStatus.StatusDesc

                            }).ToList();
                foreach (var e in equipmentData)
                {
                    var totalUsageDays = usageSummary.TryGetValue(e.EquipmentID, out var days) ? days : 0;
                    var report = new EquipemntUsageReport
                    {
                        EquipmentName = e.EquipmentName,
                        EquipmentModel = e.EquipmentModel,
                        EquipmentID = e.EquipmentID,
                        EquipmentNo = e.EquipmentNo,
                        EquipmentStatus1 = (int)e.EquipmentStatus1,
                        StatusDesc = e.StatusDesc,
                        TotalUsageDays = totalUsageDays
                    };
                    results.Add(report);
                }
              
            }

            return results;
        }


        public static List<UsageRecordDTO> getUseAgeRecord()
        {
            var list=new List<UsageRecordDTO>();
            using (var context=new SicoreQMSEntities1())
            {
                var usages = from UsageRecord in context.UsageRecord
                             join Equipment in context.Equipment on UsageRecord.EquipmentId equals Equipment.EquipmentID
                             join Userinfo in context.UserInfo on UsageRecord.UseUser equals Userinfo.Id
                             select new UsageRecordDTO
                             {
                                 //Id = UsageRecord.Id,
                                 //EquipmentId = UsageRecord.EquipmentId,
                                 StartDate = (DateTime)UsageRecord.StartDate,
                                 EndDate = UsageRecord.EndDate,
                                 UseType = UsageRecord.UseType,
                                 UseProcess = UsageRecord.UseProcess,
                                 UseUser = Userinfo.UserName,
                                 EquipmentName = Equipment.EquipmentName,
                                 EquipmentModel = Equipment.EquipmentModel,
                                 EquipmentNo = Equipment.EquipmentNo
                             };
                list = usages.ToList();

            }

            return list;
        }


        //public static ObservableCollection<EquipemntUsageReport> GetEquipmentStatus1()
        //{
        //    var results = new ObservableCollection<EquipemntUsageReport>();
        //    using (var context = new SicoreQMSEntities1())
        //    {
        //        var endDate = DateTime.Now;
        //        var usageSummary = context.UsageRecord
        //            .GroupBy(u => u.EquipmentId)
        //            .Select(group => new
        //            {
        //                EquipmentId = group.Key,
        //                TotalDays = group.Sum(g => (g.EndDate ?? endDate).Subtract(g.StartDate ?? default(DateTime)).Days + 1)
        //            })
        //            .ToDictionary(u => u.EquipmentId, u => u.TotalDays);//转换为字典，方便后续查询

        //        var query = from e in context.Equipment
        //                    join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
        //                    join u in usageSummary on e.EquipmentID equals u.Key into usageJoin
        //                    from uj in usageJoin.DefaultIfEmpty()
        //                    select new EquipemntUsageReport
        //                    {
        //                        EquipmentName = e.EquipmentName,
        //                        EquipmentModel = e.EquipmentModel,
        //                        EquipmentID = e.EquipmentID,
        //                        EquipmentNo = e.EquipmentNo,
        //                        EquipmentStatus1 = (int)eStatus.EquipmentStatus1,
        //                        StatusDesc = eStatus.StatusDesc,
        //                        TotalUsageDays = usageSummary.ContainsKey(e.EquipmentID) ? usageSummary[e.EquipmentID] : 0

        //                    };
        //        foreach (var equipemntUsageReport in query)
        //        {
        //            results.Add(equipemntUsageReport);
        //        }
        //    }


        //    return results;
        //}


    }
}
