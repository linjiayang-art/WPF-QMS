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
            var results=new ObservableCollection<TestCountReport>();
            var cardList = new List<string> { "老炼", "超声扫描", "入库（筛选品）" };

            using(var context=new SicoreQMSEntities1())
            {
                var query = from pP in context.Prod_Process
                            join pPI in context.Prod_ProcessItem on pP.Id equals pPI.ProdProcessId
                            where cardList.Any(card => pPI.ProdProcessCard==(card)&& pP.IsDeleted==false)
                            orderby pP.ProdLot, pPI.ModelSort
                            select new
                            {
                                pP.ProdName,
                                pP.ProdType,
                                pP.ProdLot,
                                pPI.ProcessType,
                                pPI.ProdProcessCard,
                                pPI.InputQty,
                                pPI.OutQty,
                                pP.Qty,
                               pP.OriginQty,

                              
                            };
                var resultList = query.ToList();
                var listCount = resultList.Count()/3;
                for (int i = 0; i < listCount; i++)
                {
                   var newI=i*3;
                    var testCountReport = new TestCountReport();

                    testCountReport.ProdName = resultList[newI].ProdName;

                    testCountReport.Qty= (int)resultList[newI].Qty;
                    testCountReport.OriginQty = (int)resultList[newI].OriginQty;
                    testCountReport.ProdType = resultList[newI].ProdType;
                    testCountReport.ProdLot = resultList[newI].ProdLot;
                    testCountReport.TestType = resultList[newI].ProcessType;
                    testCountReport.AgingCount= (int)resultList[newI    ].InputQty;
                    testCountReport.AgingCountOut = string.IsNullOrEmpty(resultList[newI].OutQty.ToString()) ? 0 : (int)resultList[newI].OutQty;
                    testCountReport.UltrasonicTesting = string.IsNullOrEmpty(resultList[newI + 1].InputQty.ToString())? 0 : (int)resultList[newI + 1].InputQty;
                    testCountReport.UltrasonicTestingOut = string.IsNullOrEmpty(resultList[newI + 1].OutQty.ToString()) ? 0 : (int)resultList[newI + 1].OutQty;
                    testCountReport.StockIn = string.IsNullOrEmpty(resultList[newI + 2].InputQty.ToString()) ? 0: (int)resultList[newI + 2].InputQty;
                    results.Add(testCountReport);

                }
            }
            var newResults = testCountFactory(results);

            return newResults;
        }
        public static ObservableCollection<TestCountReport> GetTestCountReport(string prodType,string lot,string testType=null)
        {
            var results = new ObservableCollection<TestCountReport>();
            var cardList = new List<string> { "老炼", "超声扫描", "入库（筛选品）" };

            using (var context = new SicoreQMSEntities1())
            {
                var query = from pP in context.Prod_Process
                            join pPI in context.Prod_ProcessItem on pP.Id equals pPI.ProdProcessId
                            where cardList.Any(card => pPI.ProdProcessCard == card)
              && (!String.IsNullOrEmpty( pP.ProdType)  && pP.ProdType.Contains(prodType))
              && (!String.IsNullOrEmpty(pP.ProdLot) && pP.ProdLot.Contains(lot)
              && pP.IsDeleted == false)
                            orderby pP.ProdLot, pPI.ModelSort
                            select new
                            {
                                pP.ProdName,
                                pP.ProdType,
                                pP.ProdLot,
                                pPI.ProcessType,
                                pPI.ProdProcessCard,
                                pPI.InputQty,
                                pPI.OutQty,
                                pP.Qty,
                                pP.OriginQty,
                            };
                var resultList = query.ToList();
                var listCount = resultList.Count() / 3;
                for (int i = 0; i < listCount; i++)
                {
                    var newI = i * 3;
                    var testCountReport = new TestCountReport();
                    testCountReport.ProdName = resultList[newI].ProdName;
                    testCountReport.Qty = (int)resultList[newI].Qty;
                    testCountReport.OriginQty = (int)resultList[newI].OriginQty;
                    testCountReport.ProdType = resultList[newI].ProdType;
                    testCountReport.ProdLot = resultList[newI].ProdLot;
                    testCountReport.TestType = resultList[newI].ProcessType;
                    testCountReport.AgingCount = (int)resultList[newI].InputQty;
                    testCountReport.AgingCountOut = string.IsNullOrEmpty(resultList[newI].OutQty.ToString()) ? 0 : (int)resultList[newI].OutQty;
                    testCountReport.UltrasonicTesting = string.IsNullOrEmpty(resultList[newI + 1].InputQty.ToString()) ? 0 : (int)resultList[newI + 1].InputQty;
                    testCountReport.UltrasonicTestingOut = string.IsNullOrEmpty(resultList[newI + 1].OutQty.ToString()) ? 0 : (int)resultList[newI + 1].OutQty;
                    testCountReport.StockIn = string.IsNullOrEmpty(resultList[newI + 2].InputQty.ToString()) ? 0 : (int)resultList[newI + 2].InputQty;
                    results.Add(testCountReport);
                }
            }
            //return results;

            var newResults = testCountFactory(results);

            return newResults;
        }



        public static ObservableCollection<TestCountReport> testCountFactory(ObservableCollection<TestCountReport>  testCountReports)
        {
            var resluts=new ObservableCollection<TestCountReport>();
            var testProdTypeCount = new List<string>();
            var testLotCount = new List<string>();
            foreach (var item in testCountReports)
            {

                //存在就更新子批
                if (testProdTypeCount.Contains(item.ProdType)&&
                    testLotCount.Any(i => item.ProdLot.ToLower().Contains(i.ToLower())))
                {
                    //找到父级
                    var itemToUpdate = resluts.FirstOrDefault(newItem => newItem.ProdType == item.ProdType);
                    if (itemToUpdate != null)
                    {
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
            var prodLot= testCountReport.ProdLot;
            var prodType= testCountReport.ProdType;
           using (var context=new SicoreQMSEntities1())
            {
                var prodProcess = context.Prod_Process.Where(p => p.ProdLot.Contains(prodLot) && p.ProdType == prodType).ToList();
                foreach (var item in prodProcess)
                {
                    item.IsDeleted = true;
                }
                context.SaveChanges();
            }

        }

    }
}
