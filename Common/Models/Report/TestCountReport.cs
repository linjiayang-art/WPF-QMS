using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Report
{
    public class TestCountReport
    {

        public TestCountReport()
        {
            // 初始化 ChildItems 防止空引用
            ChildItems = new List<TestCountReport>();
        }

        public int Qty { get; set; }
        public string ProdName { get; set; }
        public string ProdType { get; set; }

        public string ProdLot { get; set; }

        public string TestType { get; set; }
        public int Completeness { get; set; }

        public int OriginQty { get; set; }
        public int TotalCount { get; set; }

        public int AgingCount { get; set; }
        public int AgingCountOut { get; set; }
        public int UltrasonicTesting { get; set; }
        public int UltrasonicTestingOut { get; set; }
        public int StockIn { get; set; }
        public List<TestCountReport> ChildItems { get; set; }
    }

 

}
