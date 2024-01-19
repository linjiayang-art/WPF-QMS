using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Report
{
    public class TestCountReport
    {
        public string ProdName { get; set; }
        public string ProdType { get; set; }

        public string ProdLot { get; set; }

        public string TestTpye { get; set; }

        public string NowProcess { get; set; }

        public int Completeness { get; set; }
    }
}
