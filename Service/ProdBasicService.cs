﻿using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using Syncfusion.Windows.Shared.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SicoreQMS.Service
{
    public class ProdBasicService
    {

        public static ObservableCollection<CheckBasic> GetTestType()
        {
            var list = new ObservableCollection<CheckBasic>() { };
            list.Add(new CheckBasic() { Label = "筛选", IsCheck = false });
            list.Add(new CheckBasic() { Label = "鉴定", IsCheck = false });
            list.Add(new CheckBasic() { Label = "质量一致性", IsCheck = false });
            list.Add(new CheckBasic() { Label = "研发验证", IsCheck = false });
            list.Add(new CheckBasic() { Label = "其它", IsCheck = false });
            return list;

        }




        public static ObservableCollection<SelectBasic> CreateProductSelection()
        {

            ObservableCollection<SelectBasic> values = new ObservableCollection<SelectBasic>();
            var newProductNameBasic = new ObservableCollection<SelectBasic>();

            using (var context = new SicoreQMSEntities1())
            {
                var productItem = context.ProdInfo
                    //.Where(b => b.ProdStatus == 0 || b.ProdStatus == 5 || b.ProdStatus == 1)
                    .ToList().OrderBy(x => x.CreateDate);
                foreach (var item in productItem)
                {
                    values.Add(item.ProductSelect());
                }
            }
            return values;
            //ProductNameBasic = newProductNameBasic;
        }

        public static bool CreateProdBasic(string prodName, string prodType, int qty, string prodLot,string testLot,string prodNumber,string prodstandard)
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
                    TestLot = testLot,
                    ProdNumber = prodNumber,
                    ProdName = prodName,
                    ProdType = prodType,
                    ProdStatus = 0,
                    Qty = qty,
                    OrginQty = qty,
                    ProdLot = prodLot,
                    QualityLevel = qualityLevel,
                    Prodstandard = prodstandard
                };
                // 将新的 ProdInfo 对象添加到数据库
                dbContext.ProdInfo.Add(newProdInfo);
                dbContext.SaveChanges();

                CreateProdProcess(newProdInfo);
                CreateTestProcess(newProdInfo);
            }


            return true;




        }


        public static void CreateProdProcess(ProdInfo prodInfo)
        {

            using (var context = new SicoreQMSEntities1())
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
                var processModel = context.Prod_ProcessModel.Where(p => p.ModelName == "军品").ToList();

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

        public static void CreateTestProcess(ProdInfo prodInfo)
        {
            using (var context = new SicoreQMSEntities1())
            {
                TestProcess testProcess = new TestProcess()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProdId = prodInfo.Id,
                    ModelTypeId = "CE282394-1E58-4A38-BA00-C87D512617D7",//默认军工
                    CreateUser = AppSession.UserID,
                    ProdName = prodInfo.ProdName,
                    ProdType = prodInfo.ProdType,
                    ProdLot = prodInfo.ProdLot,
                    Prodstandard = prodInfo.Prodstandard,
                    TestLot = prodInfo.TestLot,
                    TestType = "",
                    ProdNumber = prodInfo.ProdNumber,
                };

                context.TestProcess.Add(testProcess);
                context.SaveChanges();

                var items = context.TestModelItem.Where(p => p.ModelId == "CE282394-1E58-4A38-BA00-C87D512617D7").OrderByDescending(p => p.ExperimentItemNo).ToList();
                foreach (var item in items)
                {
                    TestProcessItem testProcessitem = new TestProcessItem()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProdId = prodInfo.Id,
                        TestProcessId = testProcess.Id,
                        ModelId = item.Id,
                        ExperimentItemNo = item.ExperimentItemNo,
                        ExperimentName = item.ExperimentItemName,
                        ExperimentStandard = item.ExperimentItemStandard,
                        ExperimentConditions = item.ExperimentItemConditions,
                        ExperimentNo = item.ExperimentItemNumber,
                        ExperimentQty = item.ExperimentItemQty,
                    };
                    context.TestProcessItem.Add(testProcessitem);
                }

                context.SaveChanges();

            }


        }
    }
}
