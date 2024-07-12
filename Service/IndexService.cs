using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SicoreQMS.Common.Models.Report;
using System.Collections.ObjectModel;
using SicoreQMS.Common.Models.Operation;
using System.Data.Entity;
using Prism.Regions;
using System.Windows.Forms;
using ImTools;
using ClosedXML.Excel;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using ClosedXML;
using System.Data.SqlClient;
using System.Data;
using System.Dynamic;
using DocumentFormat.OpenXml.Spreadsheet;
using SicoreQMS.Common.Models.Basic;


namespace SicoreQMS.Service
{
    public class IndexService : BindableBase
    {



        public static List<TestCountReport> GetTestReportData(List<object> list)
        {

         


            var result = new List<TestCountReport>();
            return result;
        }

        public static ObservableCollection<TestCountReport> GetTestCountReport()
        {
            var results = new ObservableCollection<TestCountReport>();
            var cardList = new List<string> { "老炼", "超声扫描", "X光检测", "入库（筛选品）","老炼后常温电测","电性能测试"};

            using (var context = new SicoreQMSEntities1())
            {
                var query =( from pP in context.Prod_Process
                            join pPI in context.Prod_ProcessItem on pP.Id equals pPI.ProdProcessId
                            join prod in context.ProdInfo on pP.ProdId equals prod.Id
                            where cardList.Any(card => pPI.ProdProcessCard == (card) && pP.IsDeleted == false && prod.TestType.Contains("筛选"))
                            orderby pP.ProdLot, pPI.ModelSort
                  
                            select new
                            {
                                pP.Id,
                                pP.ProdName,
                                pP.ProdType,
                                pP.ProdLot,
                                pPI.ProcessType,
                                prod.TestType,
                                pPI.ProdProcessCard,
                                pPI.InputQty,
                                pPI.OutQty,
                                pP.Qty,
                                pP.OriginQty,
                                pPI.IsComplete,
                                prod.ProdNo,
                                pP.ProdStatus,
                                pPI.ModelSort

                            }).Take(100);

                var oldresultList = query.ToList();
                //重排序   4个一组
                var resultList = oldresultList
                .OrderBy(item => item.ProdLot)
                .ThenBy(item => item.ModelSort)
                .ToList();

                var singerList = resultList.Where(p => p.ProdProcessCard == "老炼").OrderBy(p => p.ProdLot).ToList();
                foreach (var singerItem in singerList)
                {
                    var testCountReport = new TestCountReport();
                    //获取该产品的所有工序
                    var singerResult = resultList.Where(p => p.Id == singerItem.Id).ToList();
                    testCountReport.ProdName = singerItem.ProdName;
                    testCountReport.ProdType = singerItem.ProdType;
                    testCountReport.ProdLot = singerItem.ProdLot;
                    testCountReport.TestType = singerItem.TestType;
                    testCountReport.Qty = (int)singerItem.Qty;
                    testCountReport.OriginQty = (int)singerItem.OriginQty;
                    testCountReport.AgingCount = (int)singerItem.InputQty;
                    testCountReport.ProdNo = singerItem.ProdNo;
                    testCountReport.ProcessStatus = singerItem.ProdStatus.ToString();
                    var id = singerItem.Id;
                    string sqlQuery = $"SELECT MAX(InputQty)-MIN( OutQty)  FROM Prod_ProcessItem  WHERE ProdProcessId='{id}' AND IsComplete=1";

                    var scrapQty = context.Database.SqlQuery<int?>(sqlQuery, id).FirstOrDefault();
                    if (scrapQty is null)
                    {
                        testCountReport.ScrapQty = 0;

                    }
                    else
                    {
                        testCountReport.ScrapQty = (int)scrapQty;
                    }

                    //根据cardList 加工其它项
                    foreach (var singerChilditem in cardList)
                    {
                        var itemResult = singerResult.Where(p => p.ProdProcessCard == singerChilditem).FirstOrDefault();
                        if (itemResult != null)
                        {
                            switch (singerChilditem)
                            {
                                case "老炼":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.AgingCount = 0;
                                        break;
                                    }
                                    testCountReport.AgingCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "X光检测":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.XlineCount = 0;
                                        break;
                                    }
                                    testCountReport.XlineCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;

                                    break;
                                case "超声扫描":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.UltrasonicTesting = 0;
                                        break;
                                    }
                                    testCountReport.UltrasonicTesting = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "入库（筛选品）":
                                    //if (itemResult.IsComplete == true)
                                    //{
                                    //    testCountReport.AgingCount = 0;
                                    //    break;
                                    //}
                                    testCountReport.StockIn = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "老炼后常温电测":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.BeforAgingTemperatureCount = 0;
                                        break;
                                    }
                                    testCountReport.BeforAgingTemperatureCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "电性能测试":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.AfterAgingTemperatureCount = 0;
                                        break;
                                    }
                                    testCountReport.AfterAgingTemperatureCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    results.Add(testCountReport);

                }
                var newResults = testCountFactory(results);

                return newResults;


            }
        }


        public static ObservableCollection<TestCountReport> GetTestCountReport(string prodType, string lot, string prodNo = null)
        {
            var results = new ObservableCollection<TestCountReport>();
            var cardList = new List<string> { "老炼", "超声扫描", "X光检测", "入库（筛选品）", "老炼后常温电测", "电性能测试" };

            using (var context = new SicoreQMSEntities1())
            {
                var query = (from pP in context.Prod_Process
                             join pPI in context.Prod_ProcessItem on pP.Id equals pPI.ProdProcessId
                             join prod in context.ProdInfo on pP.ProdId equals prod.Id
                             where cardList.Any(card => pPI.ProdProcessCard == card)
               && (!String.IsNullOrEmpty(pP.ProdType) && pP.ProdType.Contains(prodType))
                   && (!String.IsNullOrEmpty(prod.ProdNo) && prod.ProdNo.Contains(prodNo))
               && (!String.IsNullOrEmpty(pP.ProdLot) && pP.ProdLot.Contains(lot)
               && prod.TestType.Contains("筛选")
               && pP.IsDeleted == false)
                             orderby prod.ProdNo, pPI.ModelSort, pPI.Lot
                             select new
                             {
                                 pP.Id,
                                 pP.ProdName,
                                 pP.ProdType,
                                 pP.ProdLot,
                                 pPI.ProcessType,
                                 prod.TestType,
                                 pPI.ProdProcessCard,
                                 pPI.InputQty,
                                 pPI.OutQty,
                                 pP.Qty,
                                 pP.OriginQty,
                                 pPI.IsComplete,
                                 prod.ProdNo,
                                 pP.ProdStatus,
                                 pPI.ModelSort
                             }).Take(100);
                var resultList = query.ToList();
                var singerList = resultList.Where(p => p.ProdProcessCard == "老炼").OrderBy(p => p.ProdLot).ToList();
                
                foreach (var singerItem in singerList)
                {
                    var testCountReport = new TestCountReport();
                    //获取该产品的所有工序
                    var singerResult = resultList.Where(p => p.Id == singerItem.Id).ToList();
                    testCountReport.ProdName = singerItem.ProdName;
                    testCountReport.ProdType = singerItem.ProdType;
                    testCountReport.ProdLot = singerItem.ProdLot;
                    testCountReport.TestType = singerItem.TestType;
                    testCountReport.Qty = (int)singerItem.Qty;
                    testCountReport.OriginQty = (int)singerItem.OriginQty;
                    testCountReport.AgingCount = (int)singerItem.InputQty;
                    testCountReport.ProdNo = singerItem.ProdNo;
                    testCountReport.ProcessStatus = singerItem.ProdStatus.ToString();
                    var id = singerItem.Id;
                    string sqlQuery = $"SELECT Max(InputQty)-min( OutQty)  FROM Prod_ProcessItem  WHERE ProdProcessId='{id}' AND IsComplete=1";

                    var scrapQty = context.Database.SqlQuery<int?>(sqlQuery, id).FirstOrDefault();
                    if (scrapQty is null)
                    {
                        testCountReport.ScrapQty = 0;

                    }
                    else
                    {
                        testCountReport.ScrapQty = (int)scrapQty;
                    }

                    //根据cardList 加工其它项
                    foreach (var singerChilditem in cardList)
                    {
                        var itemResult = singerResult.Where(p => p.ProdProcessCard == singerChilditem).FirstOrDefault();
                        if (itemResult != null)
                        {
                            switch (singerChilditem)
                            {
                                case "老炼":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.AgingCount = 0;
                                        break;
                                    }
                                    testCountReport.AgingCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "X光检测":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.XlineCount = 0;
                                        break;
                                    }
                                    testCountReport.XlineCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;

                                    break;
                                case "超声扫描":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.UltrasonicTesting = 0;
                                        break;
                                    }
                                    testCountReport.UltrasonicTesting = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "入库（筛选品）":
                                    //if (itemResult.IsComplete == true)
                                    //{
                                    //    testCountReport.AgingCount = 0;
                                    //    break;
                                    //}
                                    testCountReport.StockIn = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "老炼后常温电测":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.BeforAgingTemperatureCount = 0;
                                        break;
                                    }
                                    testCountReport.BeforAgingTemperatureCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                case "电性能测试":
                                    if (itemResult.IsComplete == true)
                                    {
                                        testCountReport.AfterAgingTemperatureCount = 0;
                                        break;
                                    }
                                    testCountReport.AfterAgingTemperatureCount = string.IsNullOrEmpty(itemResult.InputQty.ToString()) ? 0 : (int)itemResult.InputQty;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    results.Add(testCountReport);

                }
                var newResults = testCountFactory(results);

                return newResults;


            }
        }


        public static ObservableCollection<TestProcessItem> GetTestItems(string prodType, string lot)
        {


            var reslut = new ObservableCollection<TestProcessItem>();
            using (var context = new SicoreQMSEntities1())
            {

                var testProcess = context.TestProcess.Where(p =>
                (!String.IsNullOrEmpty(p.ProdType) && p.ProdType.Contains(prodType))
                && (!String.IsNullOrEmpty(p.ProdLot) && p.ProdLot.Contains(lot))
                                                        ).ToList();
                if (testProcess.Count == 0)
                {
                    return reslut;
                }
                foreach (var item in testProcess)
                {
                    Dictionary<string, string> TestHead = new Dictionary<string, string>();

                    var testProcessItem = context.TestProcessItem.Where(p => p.TestProcessId == item.Id).ToList();
                    if (testProcessItem.Count == 0)
                    {
                        continue;
                    }
                    foreach (var testItem in testProcessItem)
                    {
                        reslut.Add(testItem);

                    }

                }
            }
            return reslut;
        }


        public static ObservableCollection<TestCount> GetTestCounts(string prodType, string lot)
        {

            var checklist = new List<string> { "1", "A", "B", "C", "D", "2" };
            var reslut = new ObservableCollection<TestCount>();
            using (var context = new SicoreQMSEntities1())
            {

                var testProcess = context.TestProcess.Where(p =>
                (!String.IsNullOrEmpty(p.ProdType) && p.ProdType.Contains(prodType))
                && (!String.IsNullOrEmpty(p.ProdLot) && p.ProdLot.Contains(lot))
                &&(p.Isdeletd==false)
                                                        ). OrderBy(p=>p.ProdId).Take(20). ToList();
                if (testProcess.Count == 0)
                {
                    return reslut;
                }
                foreach (var item in testProcess)
                {

                    var calculateDict = new Dictionary<string, string>();

                    var testProcessItem = context.TestProcessItem.Where(p => p.TestProcessId == item.Id&&p.IsDeleted==false ).ToList();
                    if (testProcessItem.Count == 0)
                    {
                        continue;
                    }
                    var expando = new TestCount();

                    expando.TestLot = item.TestLot;
                    string testNo = "";
                    string prodNo = "";

                    foreach (var i in checklist)
                    {
                        var ExperimentItemNoType = i;
                        var TestProcessId = item.Id;
                        SqlParameter[] param = {
                         new SqlParameter("ExperimentItemNoType",i),
                          new SqlParameter("TestProcessId",item.Id )
                          };

                        string sql = "getTestYield";
                        System.Data.DataSet ds = Common.DBHelper.ExecuteProcRe(sql, param);
                        DataTable dataTable = ds.Tables[0];
                        var testCount = dataTable.Rows[0][1].ToString();
                        var yield = dataTable.Rows[0][0].ToString();
                        testNo = dataTable.Rows[0][2].ToString();
                        prodNo = dataTable.Rows[0][3].ToString();
                        calculateDict.Add(testCount, yield);
                        //var blogs = context.proc_QAExperimentReport
                        //        .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser={user}")
                        //        .ToList();
                    }
                    expando.One = calculateDict["1"];
                    expando.A = calculateDict["A"];
                    expando.B = calculateDict["B"];
                    expando.C = calculateDict["C"];
                    expando.D = calculateDict["D"];
                    expando.Two = calculateDict["2"];
                    expando.TestNo = testNo;
                    expando.ProdNo = prodNo;
                    expando.Id= item.Id;
                    expando.Remark = item.Remark;
                    switch (item.StatusDesc)
                    {
                        case 0:
                            expando.StatusDescColer = "Red";
                            expando.StatusDesc= "未开始";
                            break;
                        case 1:
                            expando.StatusDescColer = "Yellow";
                            expando.StatusDesc = "进行中";
                            break;
                        case 2:
                            expando.StatusDescColer = "Green";
                            expando.StatusDesc = "已完成";
                            break;
                        case 3:
                            expando.StatusDescColer = "Gray";
                            expando.StatusDesc = "中止";
                            break;
                        default:
                            break;
                    };
                    reslut.Add(expando);
                }
            }
            return reslut;
        }



        public static ObservableCollection<TestCountReport> testCountFactory(ObservableCollection<TestCountReport> testCountReports)
        {
            var resluts = new ObservableCollection<TestCountReport>();
            var testProdTypeCount = new List<ProdStructureModel>();
            var testLotCount = new List<string>();

            foreach (var item in testCountReports)
            {
                //if (testProdTypeCount.Contains(item.ProdType) &&
                //    testLotCount.Any(i => item.ProdLot.ToLower().Contains(i.ToLower()))
                //                        )


                //存在就更新子批
                if (testProdTypeCount.Any(i=>i.ProdType== item.ProdType&& i.ProdNo==item.ProdNo))
              
                {
                    //找到父级
                    var itemToUpdate = resluts.FirstOrDefault(newItem => newItem.ProdType == item.ProdType&&newItem.ProdNo==item.ProdNo);
                    if (itemToUpdate != null)
                    {
                        //更新下后续数量
                        itemToUpdate.AgingCount += item.AgingCount;
                        itemToUpdate.XlineCount += item.XlineCount;
                        //itemToUpdate.AgingCountOut += item.AgingCountOut;
                        itemToUpdate.UltrasonicTesting += item.UltrasonicTesting;
                        itemToUpdate.ScrapQty += item.ScrapQty;
                        //itemToUpdate.UltrasonicTestingOut += item.UltrasonicTestingOut;
                        itemToUpdate.StockIn += item.StockIn;
                        //新增的电测
                        itemToUpdate.BeforAgingTemperatureCount += item.BeforAgingTemperatureCount;
                        itemToUpdate.AfterAgingTemperatureCount += item.AfterAgingTemperatureCount;
                        itemToUpdate.ChildItems.Add(item);
                    }
                }
                else
                {
                    resluts.Add(item);
                    var prodStructureModel = new ProdStructureModel(item.ProdType, item.ProdNo);
                    testProdTypeCount.Add(prodStructureModel);
                    testLotCount.Add(item.ProdLot);
                }
            }
            return resluts;
        }

        public static void DelProd(TestCountReport testCountReport)
        {
            var prodLot = testCountReport.ProdLot;
            var prodType = testCountReport.ProdType;
            var prodNo = testCountReport.ProdNo;
            using (var context = new SicoreQMSEntities1())
            {
                //通产品同批次可能多个,后续修复
                //var prodProcess = context.Prod_Process.Where(p => p.ProdLot.Contains(prodLot) && p.ProdType == prodType && p.IsDeleted == false&&p.ProdNo==prodNo).SingleOrDefault();
                var prodProcess = context.Prod_Process.Where(p => p.ProdLot.Contains(prodLot) && p.ProdType == prodType && p.IsDeleted == false && p.ProdNo == prodNo).ToList();
                //批次号带.则为子批次
                if (prodLot.Contains("."))
                {
                    //var prodProcessObject = prodProcess.Where(p => p.ProdLot == prodLot && prodNo == p.ProdNo).SingleOrDefault();
                    //if (prodProcessObject is null)
                    //{
                    //    return;
                    //}

                    //prodProcessObject.IsDeleted = true;
                    //
                    prodProcess= prodProcess.Where(p => p.ProdLot == prodLot && prodNo == p.ProdNo).ToList();

                }
                foreach (var ProcessItem in prodProcess)
                {
                    ProcessItem.IsDeleted = true;

                    var prodProcessItem = context.Prod_ProcessItem.Where(p => p.ProdProcessId == ProcessItem.Id && p.IsDeleted == false).ToList();
                    foreach (var item in prodProcessItem)
                    {
                        item.IsDeleted = true;
                    }
                    //删除试验流程
                    var testProcess = context.TestProcess.Where(p => p.ProdLot.Contains(prodLot) && p.ProdType == prodType).SingleOrDefault();

                    if (testProcess == null)
                    {
                        return;

                    }
                    testProcess.Isdeletd = true;

                    var testProcessItem = context.TestProcessItem.Where(p => p.TestProcessId == testProcess.Id).ToList();
                    foreach (var item in testProcessItem)
                    {
                        item.IsDeleted = true;
                    }
                    context.SaveChanges();
                }

            
               

            }

        }

        public static bool DelTestProcess(string testid)
        {
            try
            {
                using (var context = new SicoreQMSEntities1())
                {
                    var testProcess = context.TestProcess.Where(p => p.Id == testid).SingleOrDefault();
                    if (testProcess == null)
                    {
                        return false;

                    }
                    testProcess.Isdeletd = true;
                    var testProcessItem = context.TestProcessItem.Where(p => p.TestProcessId == testid).ToList();
                    if (testProcessItem.Count == 0)
                    {
                        return false;
                    }
                    foreach (var item in testProcessItem)
                    {
                        item.IsDeleted = true;
                    }
                    context.SaveChanges();
                }
                return true;

            }
            catch (Exception)
            {

                return false;
            }
            return true;
           
        }

        public static void ExportTestCountReportListToExcel(IEnumerable<TestCountReport> oldreports, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Test Count Report");

                // 添加标题行
                var currentRow = 1;
                List<TestCountReport> reports = new List<TestCountReport>();
                foreach (var item in oldreports)
                {
                    var child = item.ChildItems;
                    if (child.Count > 0)
                    {
                        foreach (var childItem in child)
                        {
                            reports.Add(childItem);
                        }
                    }
                    reports.Add(item);

                }

                reports = reports.OrderBy(p => p.ProdType).OrderBy(p => p.ProdLot).ThenBy(p => p.ProdLot).ToList();

                worksheet.Cell(currentRow, 1).Value = "产品名称";
                worksheet.Cell(currentRow, 2).Value = "产品型号";
                worksheet.Cell(currentRow, 3).Value = "生产批次";
                worksheet.Cell(currentRow, 4).Value = "试验类别";
                worksheet.Cell(currentRow, 5).Value = "原始总数量";
                worksheet.Cell(currentRow, 6).Value = "总数量";
                worksheet.Cell(currentRow, 7).Value = "老炼投入数量";
                worksheet.Cell(currentRow, 8).Value = "X光投入数量";
                worksheet.Cell(currentRow, 9).Value = "超声波检投入数量";

                worksheet.Cell(currentRow, 10).Value = "入库总数";
                // 继续添加其他标题...

                // 填充数据
                foreach (var report in reports)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = report.ProdName;
                    worksheet.Cell(currentRow, 2).Value = report.ProdType;
                    worksheet.Cell(currentRow, 3).Value = report.ProdLot;
                    worksheet.Cell(currentRow, 4).Value = report.TestType;
                    worksheet.Cell(currentRow, 5).Value = report.OriginQty;
                    worksheet.Cell(currentRow, 6).Value = report.Qty;
                    worksheet.Cell(currentRow, 7).Value = report.AgingCount;
                    worksheet.Cell(currentRow, 8).Value = report.XlineCount;
                    worksheet.Cell(currentRow, 9).Value = report.UltrasonicTesting;

                    worksheet.Cell(currentRow, 10).Value = report.StockIn;

                    // 继续填充其他字段...
                }
                for (int col = 1; col <= 11; col++)
                {
                    worksheet.Column(col).AdjustToContents();
                    if (worksheet.Column(col).Width <= 20)
                    {
                        worksheet.Column(col).Width = 20;
                    }
                    if (worksheet.Column(col).Width > 50) // 假设最大宽度为20
                    {
                        worksheet.Column(col).Width = 50;
                    }

                }

                // 保存到文件
                workbook.SaveAs(filePath);
            }
        }

    }
}
