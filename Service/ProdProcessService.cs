using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using ImTools;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public class ProdProcessService
    {

        public static ResultInfo BeginProcess(string id, int qty, string equipmentList, string equipmentId = null, string remark = "", DateTime? startTime = null)
        {
            if (startTime == null)
            {
                startTime = DateTime.Now;
            }
            var resultInfo = new ResultInfo();

            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.Find(id);

                if (prodProcessItem == null)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "未获取到该进程数据!";
                    return resultInfo;
                }

                if (prodProcessItem.ItemStatus != 0)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"当前流程实际状态为{prodProcessItem.NowStatus}!不允许重复开始";
                    return resultInfo;

                }

                var prodprocess = context.Prod_Process.Find(prodProcessItem.ProdProcessId);

                if (prodprocess.Qty < qty)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"投入数量为{qty} 大于总数量{prodprocess.Qty}!";
                    return resultInfo;
                }

                //更新流程卡
                prodprocess.ProdStatus = 1;
                prodprocess.CurrentProcess = prodProcessItem.ModelSort;

                //更新流程卡进程
                prodProcessItem.Operator = AppSession.UserID;
                prodProcessItem.OperatorName = AppSession.UserName;
                prodProcessItem.EquipmentId = equipmentId;
                prodProcessItem.EquipmentList = equipmentList;
                prodProcessItem.BeginRemark = remark;
                //投入不计数
                //prodProcessItem.InputQty = qty;
                prodProcessItem.InputQty = prodprocess.Qty;

                prodProcessItem.StartDate = startTime;
                prodProcessItem.ItemStatus = 1;//状态变更为正在进行
                context.SaveChanges();

            }

            resultInfo.ResultStatus = true;
            resultInfo.ResultMessage = "任务开始!";
            return resultInfo;
        }




        public static ResultInfo EndProcess(string id, int qty, string remark = "", DateTime? endTime = null)
        {
            if (endTime == null)
            {
                endTime = DateTime.Now;
            }

            var resultInfo = new ResultInfo();

            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.Find(id);

                if (prodProcessItem == null)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "未获取到该进程数据!";
                    return resultInfo;
                }

                if (prodProcessItem.ItemStatus != 1)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"当前流程实际状态为{prodProcessItem.NowStatus}!不允许结束!";
                    return resultInfo;

                }

                var prodprocess = context.Prod_Process.Find(prodProcessItem.ProdProcessId);

                if (qty == 0)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"产出数量为0!";
                    return resultInfo;
                }


                if (prodProcessItem.InputQty < qty)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"产出数量为{qty} 大于投入数量{prodProcessItem.InputQty}!";
                    return resultInfo;
                }

                //更新流程卡
                prodprocess.CurrentProcess = prodprocess.CurrentProcess++;
                prodprocess.Qty = qty;

                //更新流程卡进程
                prodProcessItem.EndRemark = remark;
                prodProcessItem.OutQty = qty;
                prodProcessItem.ItemStatus = 2;//状态变更为正在进行
                prodProcessItem.EndDate = endTime;
                prodProcessItem.IsComplete = true;
                context.SaveChanges();

            }

            resultInfo.ResultStatus = true;
            resultInfo.ResultMessage = "任务结束!";
            return resultInfo;
        }

        public static string GetProcessInfo(string id)
        {
            var prodId = "";

            using (var context = new SicoreQMSEntities1())
            {
                var prodProcess = context.Prod_Process.Find(id);
                if (prodProcess != null)
                {
                    prodId = prodProcess.ProdId;
                }
            }

            var parentProdId = GetBeginProdID(prodid: prodId);
            var prodIds = GetAllProdId(parentProdId);
            return ExportDate(prodIds);
        }


        public static string ExportDate(List<string> strings)
        {
    

            List<List<Prod_ProcessItem>> result = new List<List<Prod_ProcessItem>>();
            if (strings == null)
            {
                return "无数据导出";
            }
            using (var context = new SicoreQMSEntities1())
            {
                //获取主流程
                foreach (var item in strings)
                {
                    var process = context.Prod_Process.SingleOrDefault(x => x.ProdId == item);
                    if (process is null)
                    {
                        continue;
                    }
                    //获取流程卡
                    var processItems = context.Prod_ProcessItem.Where(x => x.ProdProcessId == process.Id).OrderBy(p=>p.ModelSort).ToList();
                    if (processItems.Count > 0)
                    {
                        result.Add(processItems);
                    }
                }
            }
            if (result.Count == 0)
                return "无数据导出";

            //生产excel

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 使用 Path.Combine 构建完整的文件路径
            string fullPath = Path.Combine(desktopPath, $"{result[0][0].ProdType}生产流程卡.xlsx");
            using (var workbook = new XLWorkbook())
            {
                foreach (var item in result)
                {
                    var worksheet = workbook.Worksheets.Add(item[0].Lot);
                    worksheet.Cell(1, 1).Value = "序号";
                    worksheet.Cell(1, 2).Value = "生产流程卡";
                    worksheet.Cell(1, 3).Value = "流程类型";
                    worksheet.Cell(1, 4).Value = "生产依据";
                    worksheet.Cell(1, 5).Value = "检验/判定标准";
                    worksheet.Cell(1, 6).Value = "投入数";
                    worksheet.Cell(1, 7).Value = "产出数";
                    worksheet.Cell(1, 8).Value = "操作人";
                    worksheet.Cell(1, 9).Value = "开始时间";
                    worksheet.Cell(1, 10).Value = "结束时间";
                    worksheet.Cell(1, 11).Value = "开始备注";
                    worksheet.Cell(1, 12).Value = "结束备注";

                    for (int i = 0; i < item.Count - 1; i++)
                    {

                        var newItem = item[i];
                        worksheet.Cell(i + 2, 1).Value = i + 1;
                        worksheet.Cell(i + 2, 2).Value = newItem.ProdProcessCard;
                        worksheet.Cell(i + 2, 3).Value = newItem.ProcessType;
                        worksheet.Cell(i + 2, 4).Value = newItem.ProdStandard;
                        worksheet.Cell(i + 2, 5).Value = newItem.CheckStandard;
                        worksheet.Cell(i + 2, 6).Value = newItem.InputQty;
                        worksheet.Cell(i + 2, 7).Value = newItem.OutQty;
                        worksheet.Cell(i + 2, 8).Value = newItem.OperatorName;
                        worksheet.Cell(i + 2, 9).Value = newItem.StartDate;
                        worksheet.Cell(i + 2, 10).Value = newItem.EndDate;
                        worksheet.Cell(i + 2, 11).Value = newItem.BeginRemark;
                        worksheet.Cell(i + 2, 12).Value = newItem.EndRemark;
                    }
                    for (int col = 2; col <= 12; col++)
                    {
                        worksheet.Column(col).AdjustToContents();
                        if (worksheet.Column(col).Width <= 20)
                        {
                            worksheet.Column(col).Width = 20;
                        }
                        if (worksheet.Column(col).Width > 30) // 假设最大宽度为20
                        {
                            worksheet.Column(col).Width = 80;
                        }

                    }
                    //worksheet.Columns().AdjustToContents();
                    worksheet.Columns().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
                workbook.SaveAs(fullPath);

                return "导出成功";
            }


        }


        private static string GetBeginProdID(string prodid)
        {
            using (var context = new SicoreQMSEntities1())
            {
                var prod = context.LotRelation.FirstOrDefault(x => x.ProdId == prodid);
                if (prod != null)
                {
                    GetBeginProdID(prod.ParentId);
                }
                else
                {
                    return prodid;
                }

            }
            return prodid;
        }

        private static List<string> GetAllProdId(string prodId, List<string> strings = null)
        {
            var newStrings = strings;
            if (newStrings == null)
            {
                newStrings = new List<string>
                {
                    prodId
                };
            }
            using (var context = new SicoreQMSEntities1())
            {
                var prod = context.LotRelation.Where(x => x.ParentId == prodId).OrderBy(p=>p.CreateDate).ToList();
                if (prod != null)
                {
                    foreach (var item in prod)
                    {
                        newStrings.Add(item.ProdId);
                        GetAllProdId(item.ProdId, newStrings);
                    }
                }
                else
                {
                    return newStrings;
                }
            }
            return newStrings;
        }

    }
}
