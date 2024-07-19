using Prism.Mvvm;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Interface;
using Syncfusion.Pdf.Barcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Operation
{
    public partial class TestProcess: BindableBase, ISelectItem
    {
        public TestProcess()
        {
            CompleteStatus = false;
            Isdeletd = false;
            AuditStatus = false;
            StatusDesc = 0;
        }

     
        public SelectBasic ProductSelect()
        {
            return new SelectBasic()
            {
                Label = $" {ProdType} :{ProdLot} :{TestType}",
                //Label = $" 产品型号: {ProdType} 批次号: {ProdLot} 试验类别:{TestType}",
                Value = Id,
                DisplayValue = ProdType
            };
          
        }

        private string _testNo;

        public string TestNo
        {
            get => _testNo;
            set => SetProperty(ref _testNo, value);

        }

        public SelectBasic GetSelection()
        {
           return new SelectBasic()
           {
               Label=$"{ProdType} :{ProdLot} :{Prodstandard} :{TestType} ",
               Value=Id,
               DisplayValue = ProdType
           };
        }
    }
}
