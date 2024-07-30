using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Irony;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Models.Report;
using SicoreQMS.Common.Server;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SicoreQMS.Service
{
    public class EquipmentService
    {
        public static Equipment GetSingelEquipment(string equipmentNo)
        {
            using(var context=new SicoreQMSEntities1())
            {
                var eq=context.Equipment.SingleOrDefault(e => e.EquipmentNo == equipmentNo);
                if (eq == null)
                {
                    return new Equipment();
                }
                return eq;
            }
            
        }

        public static ResultInfo EditEquipment(string equipmentid,string equipmentNo=null, string equipmentName=null,string remark=null,string equipmentModel=null)
        {
            if (string.IsNullOrEmpty(equipmentid))
            {
                return new ResultInfo { ResultStatus = false, ResultMessage = "设备ID不能为空" };
            }
            if (string.IsNullOrEmpty(equipmentNo))
            {
                return new ResultInfo { ResultStatus = false, ResultMessage = "设备编号不能为空" };
            }
            if (string.IsNullOrEmpty(equipmentName))
            {
                return new ResultInfo { ResultStatus = false, ResultMessage = "设备名称不能为空" };
            }
           if (string.IsNullOrEmpty(equipmentModel))
            {
                return new ResultInfo { ResultStatus = false, ResultMessage = "设备型号不能为空" };
            }

            using (var context=new SicoreQMSEntities1())
            {
                var eq = context.Equipment.SingleOrDefault(e => e.EquipmentID == equipmentid);
                if (eq == null)
                {
                    return new ResultInfo { ResultStatus = false, ResultMessage = "设备不存在" };
                }
                eq.EquipmentNo = equipmentNo;
                eq.EquipmentName = equipmentName;
                eq.Remark = remark;
                eq.EquipmentModel=equipmentModel;
                context.SaveChanges();
                return new ResultInfo { ResultStatus = true, ResultMessage = "设备信息更新成功" };

            }

        }



        public static ObservableCollection<string> GetEquipmentList()
        {
            var results = new ObservableCollection<string>();
            using (var context = new SicoreQMSEntities1())
            {
                var query = from e in context.Equipment
                            join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
                            ///设备可通用，不需要判断设备状态
                            //where eStatus.EquipmentStatus1 == 0
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
                            ///设备可通用，不需要判断设备状态
                            //where eStatus.EquipmentStatus1 == 0
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
                            ///设备可通用，不需要判断设备状态
                            //where eStatus.EquipmentStatus1 == 0
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
        public static ObservableCollection<MultiSelectBasic> GetMultiSpEquipmentBasic()
        {
            var results = new ObservableCollection<MultiSelectBasic>();
            using (var context = new SicoreQMSEntities1())
            {


                var query = from e in context.Equipment
                            join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
                            where eStatus.EquipmentStatus1 == 0 && e.EquipmentType=="1"
                            select new
                            {
                                e.EquipmentName,
                                EquipmentNo = e.EquipmentNo,
                                EquipmentID = e.EquipmentID

                            };
                var list = query.ToList();
                foreach (var r in list)
                {
                    results.Add(new MultiSelectBasic { Value = r.EquipmentID, Label = r.EquipmentName + r.EquipmentNo, IsCheck = false });
                }

                return results;
            }

        }


        public static ResultInfo RecordEquipmentLog(string equipmentId, string useType, string useProcess, int qty=0,string processId = null)
        {
            using (var context = new SicoreQMSEntities1())
            {


                var equipment = context.Equipment.SingleOrDefault(e => e.EquipmentID == equipmentId);
                if (equipment == null)
                {
                    return new ResultInfo { ResultStatus = false, ResultMessage = "设备不存在" };
                }
                var equipmentStatus = context.EquipmentStatus.SingleOrDefault(e => e.EquipmentID == equipmentId);
                var havingUsageRecord = context.UsageRecord.SingleOrDefault(u => u.EquipmentId == equipmentId && u.EndDate == null && u.UseProcess == useProcess&&u.ProcessId==processId);
                //没有使用记录则新增使用记录
                if (havingUsageRecord == null)
                {
                    var usageRecord = new UsageRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProcessId = processId,
                        EquipmentId = equipmentId,
                        StartDate = DateTime.Now,
                        UseType = useType,
                        UseProcess = useProcess,
                        UseUser = AppSession.UserID,
                        Qty= qty
                    };
                    context.UsageRecord.Add(usageRecord);
                    equipmentStatus.EquipmentStatus1 = 1;
                    equipmentStatus.StatusDesc = "运行中";
                    context.SaveChanges();
                    return new ResultInfo()
                    { ResultStatus = true, ResultMessage = "设备状态更新成功" };
                }
                //有使用记录则更新使用记录
                else
                {
                    var usageRecord = context.UsageRecord.SingleOrDefault(u => u.EquipmentId == equipmentId && u.EndDate == null && u.UseProcess == useProcess);
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
        //public static ResultInfo RecordEquipmentLog(string equipmentId,string useType,string useProcess,string processId=null)
        //{
        //    using (var context=new SicoreQMSEntities1())
        //    {
        //        var equipment = context.Equipment.SingleOrDefault(e => e.EquipmentID == equipmentId);
        //        if (equipment == null)
        //        {
        //            return new ResultInfo { ResultStatus = false, ResultMessage = "设备不存在" };
        //        }
        //        var equipmentStatus = context.EquipmentStatus.SingleOrDefault(e => e.EquipmentID == equipmentId);
        //        //如果设备为0则表示设备空闲,更新为1,新增设备使用记录
        //        if (equipmentStatus.EquipmentStatus1 == 0)
        //        {
        //            var usageRecord = new UsageRecord
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                ProcessId=processId,
        //                EquipmentId = equipmentId,
        //                StartDate = DateTime.Now,
        //                UseType = useType,
        //                UseProcess = useProcess,
        //                UseUser= AppSession.UserID
        //            };
        //            context.UsageRecord.Add(usageRecord);
        //            equipmentStatus.EquipmentStatus1 = 1;
        //            equipmentStatus.StatusDesc = "运行中";
        //            context.SaveChanges();
        //            return new ResultInfo()
        //            { ResultStatus = true, ResultMessage = "设备状态更新成功" };
        //        }

        //        if (equipmentStatus.EquipmentStatus1 == 1)
        //        {
        //            var usageRecord = context.UsageRecord.SingleOrDefault(u => u.EquipmentId == equipmentId && u.EndDate == null&&u.UseProcess==useProcess );
        //            if (usageRecord == null)
        //            {
        //                return new ResultInfo { ResultStatus = false, ResultMessage = "设备使用记录不存在" };
        //            }
        //            usageRecord.EndDate = DateTime.Now;
        //            equipmentStatus.EquipmentStatus1 = 0;
        //            equipmentStatus.StatusDesc = "待机";
        //            context.SaveChanges();
        //        }

        //    }

        //    return new ResultInfo()
        //     { ResultStatus = true, ResultMessage = "设备状态更新成功" };
        //}

        public static ResultInfo RecordSPEquipmentLog(string equipmentId, string useType, string useProcess, int qty=0,string processId = null)
        {
            using (var context = new SicoreQMSEntities1())
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
                        ProcessId = processId,
                        EquipmentId = equipmentId,
                        StartDate = DateTime.Now,
                        Qty = qty,
                        UseType = useType,
                        UseProcess = useProcess,
                        UseUser = AppSession.UserID
                    };
                    context.UsageRecord.Add(usageRecord);
                    equipmentStatus.EquipmentStatus1 = 1;
                    equipmentStatus.StatusDesc = "运行中";
                    //更新设备可用数量
                    equipment.AvailableCapacity -= qty;
       
                    context.SaveChanges();
                    return new ResultInfo()
                    { ResultStatus = true, ResultMessage = "设备状态更新成功" };
                }

                //if (equipmentStatus.EquipmentStatus1 == 1)
                //{
                //    var usageRecord = context.UsageRecord.SingleOrDefault(u => u.EquipmentId == equipmentId && u.EndDate == null && u.UseProcess == useProcess);
                //    if (usageRecord == null)
                //    {
                //        return new ResultInfo { ResultStatus = false, ResultMessage = "设备使用记录不存在" };
                //    }
                //    usageRecord.EndDate = DateTime.Now;
                //    equipmentStatus.EquipmentStatus1 = 0;
                //    equipmentStatus.StatusDesc = "待机";
                //    //容量返回
                //    equipment.AvailableCapacity += usageRecord.Qty;
                //    context.SaveChanges();
                //}
              
            }

            return new ResultInfo()
            { ResultStatus = true, ResultMessage = "设备状态更新成功" };
        }

        public static void EndSPEquipment(string processid)
        {
            using (var context=new SicoreQMSEntities1())
            {
                var usageRecord = context.UsageRecord.Where(u => u.ProcessId == processid).ToList();
                foreach (var u in usageRecord)
                {
                    u.EndDate = DateTime.Now;
                    var equipmentStatus = context.EquipmentStatus.SingleOrDefault(e => e.EquipmentID == u.EquipmentId);
                    var equipment = context.Equipment.SingleOrDefault(e => e.EquipmentID == u.EquipmentId);
                    equipment.AvailableCapacity += u.Qty;
                }
                context.SaveChanges();
            }

            
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

        public static ObservableCollection<SPEquipment> GetSPEquipment(string spNo)
        {

            var result = new ObservableCollection<SPEquipment> ();
            using (var context=new SicoreQMSEntities1())
            {
                var spEquipment = context.Equipment.Where(p => p.EquipmentType == "1" && p.IsDeleted == false&&p.EquipmentNo.Contains(spNo)).ToList();

                if (spEquipment == null || spEquipment.Count == 0)
                {
                    return result;
                }

                foreach (var item in spEquipment)
                {
                   
                    var spItem= new SPEquipment(item);

                    result.Add(spItem);
                }

            }
            return result;
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


        public static ObservableCollection<EquipmentDateModel> GetEquipmentReport(DateTime startDate, DateTime endDate,string equipmentName="",string equipmentNo="")
        {

            
            var results = new ObservableCollection<EquipmentDateModel>();
            using (var context = new SicoreQMSEntities1())
            {

                var equipmentListQuery = from Equipment in context.Equipment
                                         join EquipmentStatus in context.EquipmentStatus on Equipment.EquipmentID equals EquipmentStatus.EquipmentID
                                         where   (!String.IsNullOrEmpty(Equipment.EquipmentName) && Equipment.EquipmentName.Contains(equipmentName))
                                         && (!String.IsNullOrEmpty(Equipment.EquipmentNo) && Equipment.EquipmentNo.Contains(equipmentNo))
                                         select new
                                         {
                                             Equipment.EquipmentID,
                                             Equipment.EquipmentName,
                                             Equipment.EquipmentNo,
                                             Equipment.EquipmentModel,
                                             EquipmentStatus.EquipmentStatus1

                                         };
                var equipmentList = equipmentListQuery.ToList();
                foreach (var equipmentItem in equipmentList)
                {


             

                    var model = new EquipmentDateModel
                    {
                        Equipment = equipmentItem.EquipmentName,
                        Model = equipmentItem.EquipmentModel,
                        EquipmentNo = equipmentItem.EquipmentNo,

                    };
                    if (equipmentItem.EquipmentStatus1 == 0)
                    {
                        model.Status = "Active";

                    }
                    else
                    {
                        model.Status = "Inactive";
                    }
                    var useageList = context.UsageRecord.Where(e => e.EquipmentId == equipmentItem.EquipmentID&&e.StartDate>=startDate).OrderBy(p => p.StartDate).ToList();
                    //如果没有使用记录则默认为stop
                    if (useageList.Count == 0)
                    {
                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                                model.DailyData.Add(date, "stop");
                            model.ColorData.Add(date, "Gray");
                        }
                        model.EquipmentYield = "0%";
                    }
                    else
                    {
                        var useCount = 0;
                        var equipmentTotalDays = (endDate - startDate).Days + 1;
                        foreach (var usageItem in useageList)
                        {
                            
                            var usageEnddate= usageItem.EndDate ?? DateTime.Now;
                   
                            //剩余时间默认为stop
                            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                            {
                                if (model.DailyData.ContainsKey(date))
                                {
                                    continue;
                                }
                                model.DailyData.Add(date, "stop");
                                model.ColorData.Add(date, "Gray");
                                //model.ColorData.Add(date, "LightCoral");
                            }
                            //先把时间设置为0点0分
                            var usageStartDate = (DateTime)usageItem.StartDate;
                            usageStartDate = new DateTime(usageStartDate.Year, usageStartDate.Month, usageStartDate.Day, 0, 0, 0);
                            usageEnddate =  new DateTime(usageEnddate.Year, usageEnddate.Month, usageEnddate.Day, 23, 59, 59);
                           

                            //反向更新
                            for (DateTime date = usageStartDate; date <= usageEnddate; date = date.AddDays(1))
                            {
                                if (model.DailyData.ContainsKey(date))
                                {
                                    string value = "";
                                    if (model.DailyData.TryGetValue(date, out value ))
                                    {
                                        if (value=="run")
                                        {
                                            continue;
                                        }
                                    }
                                    useCount++;
                                    //更新DailyData
                                    model.DailyData[date]= "run";
                                    model.ColorData[date] = "LightCoral";
                                   
                                    continue;
                                }
                                useCount++;
                                //比较日期在同一天即可,不对比具体时刻
                                model.DailyData.Add(date, "run");
                                model.ColorData.Add(date, "LightCoral");
                            }
                        }

                        model.EquipmentYield = ((double)useCount / equipmentTotalDays).ToString("P");
                    }
                  
                    results.Add(model);
              
                }

                return results;
            }
        }


        public static ObservableCollection<EquipmentUsageDetailModel> GetEquipmentUsageDetails(string equipmentNo,DateTime startDate,DateTime endDate)
        {
            ///设备结束时间可能为空值，需要处理
            var result=new ObservableCollection<EquipmentUsageDetailModel>();
            var count = 0;
            using ( var context =new SicoreQMSEntities1())
            {
                var usageRecords = from UsageRecord in context.UsageRecord
                                   join Equipment in context.Equipment on UsageRecord.EquipmentId equals Equipment.EquipmentID
                                   join Userinfo in context.UserInfo on UsageRecord.UseUser equals Userinfo.Id
                                   where Equipment.EquipmentNo.Contains(equipmentNo) && UsageRecord.StartDate >= startDate && UsageRecord.StartDate <= endDate
                                   select new EquipmentUsageDetailModel
                                   {
                                       StartDate = (DateTime)UsageRecord.StartDate,
                                       EndDate = UsageRecord.EndDate ??DateTime.Today, //加一天
                                       UseType = UsageRecord.UseType,
                                       UseProcess = UsageRecord.UseProcess,
                                       UseUser = Userinfo.UserName,
                                       ProcessId=UsageRecord.ProcessId
                                   };
                var records = usageRecords.ToList();
                if (records.Count==0)
                {
                    return result;
                }
                foreach (var item in records)
                {

                    //当天则+1
                    if (item.EndDate==DateTime.Today)
                    {
                        item.EndDate = DateTime.Today.AddDays(1);

                    }
                    //if (item.EndDate is null)
                    //{
                    //    i
                    //}
                    var useType=item.UseType;
                    if (useType=="试验流程卡")
                    {
                        var testItem =from TestProcessItem in context.TestProcessItem
                                      join TestProcess in context.TestProcess on TestProcessItem.TestProcessId equals TestProcess.Id
                                      where TestProcessItem.Id==item.ProcessId
                                      select new
                                      {
                                       TestProcessItem.Id,
                                       TestProcess.ProdType,
                                        TestProcess.ProdLot,
                                        };

                        var testInfo = testItem.FirstOrDefault();
                        if (testInfo is null)
                        {
                            continue;
                        }
                        item.ProdLot = testInfo.ProdLot;
                        item.ProdType= testInfo.ProdType;

                    }
                    if (useType == "生产流程卡")
                    {
                        var testItem = context.Prod_ProcessItem.Where(p => p.Id == item.ProcessId).SingleOrDefault();

                     
                        if (testItem is null)
                        {
                            continue;
                        }
                        item.ProdLot = testItem.Lot;
                        item.ProdType = testItem.ProdType;

                    }
                   
                    TimeSpan timeDifference = (TimeSpan)((DateTime)item.EndDate - item.StartDate);

                    item.Sort=count++;
                    item.UseCount= timeDifference.Hours;
                    result.Add(item);
                }
            }
                return result;
        }

        public static ObservableCollection<EquipmentUsageDetailModel> GetEquipmentUsageDetails( DateTime startDate, DateTime endDate)
        {
            ///设备结束时间可能为空值，需要处理
            var result = new ObservableCollection<EquipmentUsageDetailModel>();
            var count = 0;
            using (var context = new SicoreQMSEntities1())
            {
                var usageRecords = from UsageRecord in context.UsageRecord
                                   join Equipment in context.Equipment on UsageRecord.EquipmentId equals Equipment.EquipmentID
                                   join Userinfo in context.UserInfo on UsageRecord.UseUser equals Userinfo.Id
                                   where UsageRecord.StartDate >= startDate && UsageRecord.StartDate <= endDate
                                   select new EquipmentUsageDetailModel
                                   {
                                       StartDate = (DateTime)UsageRecord.StartDate,
                                       EndDate = UsageRecord.EndDate ?? DateTime.Today,
                                       UseType = UsageRecord.UseType,
                                       UseProcess = UsageRecord.UseProcess,
                                       UseUser = Userinfo.UserName,
                                       ProcessId = UsageRecord.ProcessId,
                                        EquipmentName=Equipment.EquipmentName,
                                        EquipmentType=Equipment.EquipmentType,
                                        EquipmentNo=Equipment.EquipmentNo
                                   };
                var records = usageRecords.ToList();
                if (records.Count == 0)
                {
                    return result;
                }
                foreach (var item in records)
                {

                    //当天则+1
                    if (item.EndDate == DateTime.Today)
                    {
                        item.EndDate = DateTime.Today.AddDays(1);

                    }
                    //if (item.EndDate is null)
                    //{
                    //    i
                    //}
                    var useType = item.UseType;
                    if (useType == "试验流程卡")
                    {
                        var testItem = from TestProcessItem in context.TestProcessItem
                                       join TestProcess in context.TestProcess on TestProcessItem.TestProcessId equals TestProcess.Id
                                       where TestProcessItem.Id == item.ProcessId
                                       select new
                                       {
                                           TestProcessItem.Id,
                                           TestProcess.ProdType,
                                           TestProcess.ProdLot,
                                       };

                        var testInfo = testItem.FirstOrDefault();
                        if (testInfo is null)
                        {
                            continue;
                        }
                        item.ProdLot = testInfo.ProdLot;
                        item.ProdType = testInfo.ProdType;

                    }
                    if (useType == "生产流程卡")
                    {
                        var testItem = context.Prod_ProcessItem.Where(p => p.Id == item.ProcessId).SingleOrDefault();


                        if (testItem is null)
                        {
                            continue;
                        }
                        item.ProdLot = testItem.Lot;
                        item.ProdType = testItem.ProdType;

                    }

                    TimeSpan timeDifference = (TimeSpan)((DateTime)item.EndDate - item.StartDate);

                    item.Sort = count++;
                    item.UseCount = timeDifference.Hours;
                    result.Add(item);
                }
            }
            return result;
        }

    }
}
