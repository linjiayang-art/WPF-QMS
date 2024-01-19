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

namespace SicoreQMS.Service
{
    public class IndexService : BindableBase, IRegionMemberLifetime
    {
        public bool KeepAlive { get; set; }=false;

        public static ObservableCollection<TestCountReport> GetTestCountReport()
        {
            var testCountReportList = new ObservableCollection<TestCountReport>();

            using (var context= new SicoreQMSEntities1())
            {
              

               var testProecess= context.TestProcess.Where(p => p.Isdeletd == false).ToList();

                foreach (var testProeceeItem in testProecess)
                {
                    //            public string NowProcess { get; set; }

                    //public string Completeness { get; set; }

                    var completeness=context.TestProcessItem .Where(p => p.TestProcessId == testProeceeItem.Id && p.ExperimentStatus==1 ).OrderBy(P=>P.ExperimentItemNo).FirstOrDefault();


                    var groupedData = context.TestProcessItem
                        .GroupBy(item => item.TestProcessId)
                        .Select(group => new
                        {
                            TestProcessId = group.Key,
                            TotalCount = group.Count(),
                            Status1Count = group.Count(item => item.ExperimentStatus == 1),
                            Status1Percentage = (double)group.Count(item => item.ExperimentStatus == 2) / group.Count()
                        }).Where(p=>p.TestProcessId== testProeceeItem.Id)
                        .SingleOrDefault();

                    int Completeness = (int)(groupedData.Status1Percentage * 100);

                    var testCountReport = new TestCountReport();
                    testCountReport.ProdName = testProeceeItem.ProdName;
                    testCountReport.ProdType = testProeceeItem.ProdType;
                    testCountReport.ProdLot = testProeceeItem.ProdLot;
                    testCountReport.TestTpye = testProeceeItem.TestType;
                    if (completeness == null)
                    {
                        testCountReport.NowProcess = "未开始";
                        
                    }
                    else
                    {
                        testCountReport.NowProcess = completeness.ExperimentName;
                    }

                  
                    
                    testCountReport.Completeness = Completeness;

                    testCountReportList.Add(testCountReport);

                }


            }

            return testCountReportList;
        }

    }
}
