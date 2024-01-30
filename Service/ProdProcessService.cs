using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Prism.Services.Dialogs;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public class ProdProcessService
    {

        public static ResultInfo BeginProcess(string id, int qty, string equipmentId, string remark = "")
        {
            var resultInfo = new ResultInfo();

            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.Find(id);

                if (prodProcessItem==null)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "未获取到该进程数据!";
                    return resultInfo;
                }

                if (prodProcessItem.ItemStatus!=0)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage =$"当前流程实际状态为{prodProcessItem.NowStatus}!不允许重复开始";
                    return resultInfo;

                }

                var prodprocess=context.Prod_Process.Find(prodProcessItem.ProdProcessId);

                if (prodprocess.Qty<qty )
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"投入数量为{qty} 大于总数量{prodprocess.Qty}!";
                    return resultInfo;
                }

                //更新流程卡
                prodprocess.ProdStatus = 1;
                prodprocess.CurrentProcess = prodProcessItem.ModelSort;

                //更新流程卡进程
                prodProcessItem.Operator=AppSession.UserID;
                prodProcessItem.OperatorName = AppSession.UserName;
                prodProcessItem.EquipmentId= equipmentId;
                prodProcessItem.BeginRemark = remark;
                //投入不计数
                //prodProcessItem.InputQty = qty;
                prodProcessItem.InputQty = prodprocess.Qty;

                prodProcessItem.StartDate = DateTime.Now;
                prodProcessItem.ItemStatus = 1;//状态变更为正在进行
                context.SaveChanges();

            }

            resultInfo.ResultStatus =true;
            resultInfo.ResultMessage = "任务开始!";
            return resultInfo;
        }




        public static ResultInfo EndProcess(string id,int qty,string remark="")
        {

            var resultInfo = new ResultInfo();

            using (var context = new SicoreQMSEntities1())
            {
                var prodProcessItem = context.Prod_ProcessItem.Find(id);

                if (prodProcessItem == null)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "未获取到该进程数据!";
                    return resultInfo;
                }

                if (prodProcessItem.ItemStatus != 1)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"当前流程实际状态为{prodProcessItem.NowStatus}!不允许结束!";
                    return resultInfo;

                }

                var prodprocess = context.Prod_Process.Find(prodProcessItem.ProdProcessId);

                if ( qty==0)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"产出数量为0!";
                    return resultInfo;
                }


                if (prodProcessItem.InputQty < qty)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = $"产出数量为{qty} 大于投入数量{prodProcessItem.InputQty}!";
                    return resultInfo;
                }

                //更新流程卡
                prodprocess.CurrentProcess = prodprocess.CurrentProcess++;
                prodprocess.Qty = qty;

                //更新流程卡进程
                prodProcessItem.EndRemark = remark;
                prodProcessItem.OutQty = qty;
                prodProcessItem.ItemStatus = 2;//状态变更为正在进行
                prodProcessItem.EndDate = DateTime.Now;
                prodProcessItem.IsComplete = true;
                context.SaveChanges();

            }

            resultInfo.ResultStatus = true;
            resultInfo.ResultMessage = "任务结束!";
            return resultInfo;
        }


    }
}
