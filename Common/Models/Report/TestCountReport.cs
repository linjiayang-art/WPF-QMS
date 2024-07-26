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

        public string Id { get; set; }  
        public int Qty { get; set; }
        public string ProdName { get; set; }
        public string ProdType { get; set; }

        public string ProdLot { get; set; }
        public string ProdNo { get; set; }
        public string TestType { get; set; }
        public int Completeness { get; set; }

        public int OriginQty { get; set; }
        public int TotalCount { get; set; }


        public int BeforAgingTemperatureCount { get; set; }
        public int AfterAgingTemperatureCount { get; set; }
        public int AgingCount { get; set; }

        public int XlineCount { get; set; }
        //public int AgingCountOut { get; set; }
        public int UltrasonicTesting { get; set; }
        //public int UltrasonicTestingOut { get; set; }
        public int StockIn { get; set; }

        public int ScrapQty { get; set; }

        private string _processStatus;

        public string ProcessStatus
        {
            get { return _processStatus; }
            set
            {
                var a = value.ToString();
                switch (a)
                {
                    case "0":
                        _processStatus = "未开始";
                        break;
                    case "1":
                        _processStatus = "开始";
                        break;
                    case "2":
                        _processStatus = "完成";
                        break;
                    case "5":
                        _processStatus = "拆分批次无法进行";
                        break;

                    default:
                        _processStatus = "未开始";
                        break;
                }
            }
        }

   

        public List<TestCountReport> ChildItems { get; set; }
    }

 

}
