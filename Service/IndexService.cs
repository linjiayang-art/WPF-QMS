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


namespace SicoreQMS.Service
{
    public class IndexService : BindableBase
    {

        //public static ObservableCollection<TestCountReport> OldGetTestCountReport()
        //{
        //    var testCountReportList = new ObservableCollection<TestCountReport>();

        //    using (var context= new SicoreQMSEntities1())
        //    {


        //       var testProecess= context.TestProcess.Where(p => p.Isdeletd == false).ToList();

        //        foreach (var testProeceeItem in testProecess)
        //        {
        //            //            public string NowProcess { get; set; }

        //            //public string Completeness { get; set; }

        //            var completeness=context.TestProcessItem .Where(p => p.TestProcessId == testProeceeItem.Id && p.ExperimentStatus==1 ).OrderBy(P=>P.ExperimentItemNo).FirstOrDefault();


        //            var groupedData = context.TestProcessItem
        //                .GroupBy(item => item.TestProcessId)
        //                .Select(group => new
        //                {
        //                    TestProcessId = group.Key,
        //                    TotalCount = group.Count(),
        //                    Status1Count = group.Count(item => item.ExperimentStatus == 1),
        //                    Status1Percentage = (double)group.Count(item => item.ExperimentStatus == 2) / group.Count()
        //                }).Where(p=>p.TestProcessId== testProeceeItem.Id)
        //                .SingleOrDefault();

        //            int Completeness = (int)(groupedData.Status1Percentage * 100);

        //            var testCountReport = new TestCountReport();
        //            testCountReport.ProdName = testProeceeItem.ProdName;
        //            testCountReport.ProdType = testProeceeItem.ProdType;
        //            testCountReport.ProdLot = testProeceeItem.ProdLot;
        //            testCountReport.TestTpye = testProeceeItem.TestType;
        //            if (completeness == null)
        //            {
        //                testCountReport.NowProcess = "未开始";

        //            }
        //            else
        //            {
        //                testCountReport.NowProcess = completeness.ExperimentName;
        //            }



        //            testCountReport.Completeness = Completeness;

        //            testCountReportList.Add(testCountReport);

        //        }


        //    }

        //    return testCountReportList;
        //}


        public static ObservableCollection<TestCountReport> GetTestCountReport()
        {
            var results = new ObservableCollection<TestCountReport>();
            var cardList = new List<string> { "老炼", "超声扫描", "X光检测", "入库（筛选品）" };

            using (var context = new SicoreQMSEntities1())
            {
                var query = from pP in context.Prod_Process
                            join pPI in context.Prod_ProcessItem on pP.Id equals pPI.ProdProcessId
                            join prod in context.ProdInfo on pP.ProdId equals prod.Id
                            where cardList.Any(card => pPI.ProdProcessCard == (card) && pP.IsDeleted == false)
                            orderby pP.ProdLot, pPI.ModelSort
                            select new
                            {
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


                            };
                var resultList = query.ToList();
                var listCount = resultList.Count() / 4;
                for (int i = 0; i < listCount; i++)
                {
                    var newI = i * 4;
                    var testCountReport = new TestCountReport();

                    testCountReport.ProdName = resultList[newI].ProdName;

                    testCountReport.Qty = (int)resultList[newI].Qty;
                    testCountReport.OriginQty = (int)resultList[newI].OriginQty;
                    testCountReport.ProdType = resultList[newI].ProdType;
                    testCountReport.ProdLot = resultList[newI].ProdLot;
                    testCountReport.TestType = resultList[newI].TestType;

                    testCountReport.AgingCount = (int)resultList[newI].InputQty;
                    testCountReport.XlineCount = string.IsNullOrEmpty(resultList[newI + 1].InputQty.ToString()) ? 0 : (int)resultList[newI + 1].InputQty;
                    //testCountReport.AgingCountOut = string.IsNullOrEmpty(resultList[newI].OutQty.ToString()) ? 0 : (int)resultList[newI].OutQty;
                    testCountReport.UltrasonicTesting = string.IsNullOrEmpty(resultList[newI + 2].InputQty.ToString()) ? 0 : (int)resultList[newI + 2].InputQty;
                    //testCountReport.UltrasonicTestingOut = string.IsNullOrEmpty(resultList[newI + 1].OutQty.ToString()) ? 0 : (int)resultList[newI + 1].OutQty;
                    testCountReport.StockIn = string.IsNullOrEmpty(resultList[newI + 3].InputQty.ToString()) ? 0 : (int)resultList[newI + 3].InputQty;
                    results.Add(testCountReport);

                }
            }
            var newResults = testCountFactory(results);

            return newResults;
        }
        public static ObservableCollection<TestCountReport> GetTestCountReport(string prodType, string lot, string testType = null)
        {
            var results = new ObservableCollection<TestCountReport>();
            var cardList = new List<string> { "老炼", "超声扫描", "X光检测", "入库（筛选品）" };

            using (var context = new SicoreQMSEntities1())
            {
                var query = from pP in context.Prod_Process
                            join pPI in context.Prod_ProcessItem on pP.Id equals pPI.ProdProcessId
                            join prod in context.ProdInfo on pP.ProdId equals prod.Id
                            where cardList.Any(card => pPI.ProdProcessCard == card)
              && (!String.IsNullOrEmpty(pP.ProdType) && pP.ProdType.Contains(prodType))
              && (!String.IsNullOrEmpty(pP.ProdLot) && pP.ProdLot.Contains(lot)
              && pP.IsDeleted == false)
                            orderby pP.ProdLot, pPI.ModelSort
                            select new
                            {
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
                            };
                var resultList = query.ToList();
                var listCount = resultList.Count() / 4;
                for (int i = 0; i < listCount; i++)
                {
                    var newI = i * 4;
                    var testCountReport = new TestCountReport();
                    testCountReport.ProdName = resultList[newI].ProdName;
                    testCountReport.Qty = (int)resultList[newI].Qty;
                    testCountReport.OriginQty = (int)resultList[newI].OriginQty;
                    testCountReport.ProdType = resultList[newI].ProdType;
                    testCountReport.ProdLot = resultList[newI].ProdLot;
                    testCountReport.TestType = resultList[newI].TestType;
                    testCountReport.AgingCount = (int)resultList[newI].InputQty;
                    //testCountReport.AgingCountOut = string.IsNullOrEmpty(resultList[newI].OutQty.ToString()) ? 0 : (int)resultList[newI].OutQty;
                    testCountReport.UltrasonicTesting = string.IsNullOrEmpty(resultList[newI + 2].InputQty.ToString()) ? 0 : (int)resultList[newI + 2].InputQty;
                    testCountReport.XlineCount = string.IsNullOrEmpty(resultList[newI + 1].InputQty.ToString()) ? 0 : (int)resultList[newI + 1].InputQty;
                    //testCountReport.UltrasonicTestingOut = string.IsNullOrEmpty(resultList[newI + 1].OutQty.ToString()) ? 0 : (int)resultList[newI + 1].OutQty;
                    testCountReport.StockIn = string.IsNullOrEmpty(resultList[newI + 3].InputQty.ToString()) ? 0 : (int)resultList[newI + 3].InputQty;
                    results.Add(testCountReport);
                }
            }
            //return results;

            var newResults = testCountFactory(results);

            return newResults;
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

            var checklist = new List<string> { "1", "A", "B", "C","D","2" };
            var reslut = new ObservableCollection<TestCount>();
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
                
                    var calculateDict =new Dictionary<string, string>();

                    var testProcessItem = context.TestProcessItem.Where(p => p.TestProcessId == item.Id).ToList();
                    if (testProcessItem.Count == 0)
                    {
                        continue;
                    }
                    var expando = new TestCount();

                    expando.TestLot =item.TestLot;
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
                        testNo= dataTable.Rows[0][2].ToString();
                        prodNo = dataTable.Rows[0][3].ToString();
                        calculateDict.Add(testCount, yield);
                        //var blogs = context.proc_QAExperimentReport
                        //        .FromSql($"EXECUTE dbo.GetMostPopularBlogsForUser @filterByUser={user}")
                        //        .ToList();
                    }
                    expando.One= calculateDict["1"];
                    expando.A = calculateDict["A"];
                    expando.B = calculateDict["B"];
                    expando.C = calculateDict["C"];
                    expando.D = calculateDict["D"];
                    expando.Two = calculateDict["2"];
                    expando.TestNo = testNo;
                    expando.ProdNo = prodNo;
                    reslut.Add(expando);
                }
            }
            return reslut;
        }



        public static ObservableCollection<TestCountReport> testCountFactory(ObservableCollection<TestCountReport> testCountReports)
        {
            var resluts = new ObservableCollection<TestCountReport>();
            var testProdTypeCount = new List<string>();
            var testLotCount = new List<string>();
            foreach (var item in testCountReports)
            {

                //存在就更新子批
                if (testProdTypeCount.Contains(item.ProdType) &&
                    testLotCount.Any(i => item.ProdLot.ToLower().Contains(i.ToLower())))
                {
                    //找到父级
                    var itemToUpdate = resluts.FirstOrDefault(newItem => newItem.ProdType == item.ProdType);
                    if (itemToUpdate != null)
                    {
                        //更新下后续数量
                        itemToUpdate.AgingCount += item.AgingCount;
                        itemToUpdate.XlineCount += item.XlineCount;
                        //itemToUpdate.AgingCountOut += item.AgingCountOut;
                        itemToUpdate.UltrasonicTesting += item.UltrasonicTesting;
                        //itemToUpdate.UltrasonicTestingOut += item.UltrasonicTestingOut;
                        itemToUpdate.StockIn += item.StockIn;

                        itemToUpdate.ChildItems.Add(item);
                    }
                }
                else
                {
                    resluts.Add(item);
                    testProdTypeCount.Add(item.ProdType);
                    testLotCount.Add(item.ProdLot);
                }
            }
            return resluts;
        }

        public static void DelProd(TestCountReport testCountReport)
        {
            var prodLot = testCountReport.ProdLot;
            var prodType = testCountReport.ProdType;
            using (var context = new SicoreQMSEntities1())
            {
                var prodProcess = context.Prod_Process.Where(p => p.ProdLot.Contains(prodLot) && p.ProdType == prodType).SingleOrDefault();

                prodProcess.IsDeleted = true;

                var prodProcessItem = context.Prod_ProcessItem.Where(p => p.ProdProcessId == prodProcess.Id).ToList();
                foreach (var item in prodProcessItem)
                {
                    item.IsDeleted = true;
                }
                context.SaveChanges();
            }

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
