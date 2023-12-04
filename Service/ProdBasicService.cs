using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public class ProdBasicService
    {

        public static ObservableCollection<CheckBasic>  GetTestType()
        {
            var list =new ObservableCollection<CheckBasic>() { };
            list.Add(new CheckBasic() { Label = "筛选", IsCheck = false });
            list.Add(new CheckBasic() { Label = "鉴定", IsCheck = false });
            list.Add(new CheckBasic() { Label = "质量一致性", IsCheck = false });
            list.Add(new CheckBasic() { Label = "研发验证", IsCheck = false });
            list.Add(new CheckBasic() { Label = "其它", IsCheck = false });
            return list;

        }


        public static bool CreateProdBasic(string prodName, string prodType, int qty, string prodLot)
        {
            string lastChar = prodType.Substring(prodType.Length - 1, 1).ToUpper();
            var qualityLevel = "";
            if (lastChar == "J")
            {
                qualityLevel = "军品";
            }
            else
            {
                qualityLevel = "民品";
            }
            using (var dbContext = new SicoreQMSEntities1())
            {
                // 创建一个新的 ProdInfo 对象
                ProdInfo newProdInfo = new ProdInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    ProdName = prodName,  
                    ProdType = prodType,  
                    ProdStatus = 0,                
                    Qty = qty,
                    OrginQty =qty,
                    ProdLot = prodLot,       
                    QualityLevel = qualityLevel,               
                };
                // 将新的 ProdInfo 对象添加到数据库
                dbContext.ProdInfo.Add(newProdInfo);
                dbContext.SaveChanges();

                CreateProdProcess(newProdInfo);
            }

       
           return true;




        }


        public static void CreateProdProcess(ProdInfo prodInfo)
        {

            using (var context=new SicoreQMSEntities1())
            {
                Prod_Process newProcessInfo = new Prod_Process
                {
                    Id = Guid.NewGuid().ToString(),
                    ProdId = prodInfo.Id,
                    ProdName = prodInfo.ProdName,
                    ProdLot = prodInfo.ProdLot,
                    QualityLevel = prodInfo.QualityLevel,
                    ProdType = prodInfo.ProdType,
                    ModelName = "军工",/*ad*/
                    Qty = prodInfo.Qty,
                    OrginQty = prodInfo.Qty,
                };

                // 将新的 ProdInfo 对象添加到数据库
                context.Prod_Process.Add(newProcessInfo);
                context.SaveChanges();
                var processModel=context.Prod_ProcessModel.Where(p=>p.ModelName=="军品").ToList();

                foreach (var item in processModel)
                {
                    Prod_ProcessItem newProcessItem = new Prod_ProcessItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProdId = prodInfo.Id,
                        ProdProcessId = newProcessInfo.Id,
                        ProdName = prodInfo.ProdName,
                        ProdType = prodInfo.ProdType,
                        Lot = prodInfo.ProdLot,
                        QualityLevel = prodInfo.QualityLevel,
                        ModelName = "军工",
                    };
                    newProcessItem.CopyModelData(item);
                    context.Prod_ProcessItem.Add(newProcessItem);
                }
                context.SaveChanges();
            }
        }

    }
}
