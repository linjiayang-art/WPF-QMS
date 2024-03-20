using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using SicoreQMS.Common.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public class TestProcessService
    {

        public static ObservableCollection<SelectBasic> GetTestModel()
        {
            using (var context = new SicoreQMSEntities1())
            {
                var list = context.TestModelBasic.Where(b => b.Isdeleted == false).ToList();
                var result = new ObservableCollection<SelectBasic>();
                foreach (var item in list)
                {
                    result.Add(item.GetSelection());
                }
                return result;
            }
        }


   

        public static ResultInfo DelItem(string id)
        {
            var resultInfo = new ResultInfo();
            using (var context = new SicoreQMSEntities1())
            {
                var item = context.TestProcessItem.SingleOrDefault(b => b.Id == id);
                if (item.IsDeleted == true)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "该数据已删除!";
                    return resultInfo;
                }
                if (item != null)
                {
                    item.IsDeleted = true;
                    context.SaveChanges();
                }
                else
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "未获取到该数据!无法删除";
                    return resultInfo;

                }



            }
            resultInfo.ResultStatus = true;
            resultInfo.ResultMessage = "删除成功!";
            return resultInfo;
        }


        public static ResultInfo ADDItem(TestProcessItem testProcessItem)
        {
            var resultInfo = new ResultInfo();
            using (var context = new SicoreQMSEntities1())
            {
                context.TestProcessItem.Add(testProcessItem);
                context.SaveChanges();
                resultInfo.ResultStatus = true;
                resultInfo.ResultMessage = "添加成功!";
                return resultInfo;
            }
        }

        public static ResultInfo StartTset(string id, int passQty, string remark,string equipmentid=null,string equipmentList=null)
        {
            var resultInfo = new ResultInfo();
            using (var context = new SicoreQMSEntities1())
            {


                var item = context.TestProcessItem.Find(id);

                if (item.AuditStatus == false)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "该数据未审核!";
                    return resultInfo;
                }

                if (item.ExperimentQty < passQty)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "产出数大于投入数!";

                    return resultInfo;
                }
                item.ExperimentItemPassQty = passQty;
                item.EquipmentId = equipmentid;
                item.EquipmentList = equipmentList;
                item.ExperimentStatus = 1;
                item.Remark += remark;
                item.ExperimentSatrtTime = DateTime.Now;
                item.StartUser = AppSession.UserID;
                context.SaveChanges();
                resultInfo.ResultStatus = true;
                resultInfo.ResultMessage = "开始成功!";
            }
            return resultInfo;

        }

        public static ResultInfo EndTest(string id, int passQty, string remark)
        {
            var resultInfo = new ResultInfo();
            using (var context = new SicoreQMSEntities1())
            {
                var item = context.TestProcessItem.Find(id);


                if (item.AuditStatus == false)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "该数据未审核!";
                    return resultInfo;
                }

                if (item.ExperimentQty < passQty)
                {
                    resultInfo.ResultStatus = false;
                    resultInfo.ResultMessage = "产出数大于投入数!";
                    return resultInfo;

                }
                item.ExperimentItemPassQty = passQty;
                item.ExperimentStatus = 2;
                item.Remark += "完成时备注:" + remark;
                item.EndUser = AppSession.UserID;
                item.ExperimentEndTime = DateTime.Now;
                item.EstimatedCompletionTime = DateTime.Now;
                context.SaveChanges();
                resultInfo.ResultStatus = true;
                resultInfo.ResultMessage = "完成成功!";
                return resultInfo;
            }
        }


        public static ObservableCollection<CheckBasic> GetTestTypeList(string testProcessId)
        {

            var list = new ObservableCollection<CheckBasic>() { };
            list.Add(new CheckBasic() { Label = "筛选", IsCheck = false });
            list.Add(new CheckBasic() { Label = "鉴定", IsCheck = false });
            list.Add(new CheckBasic() { Label = "质量一致性", IsCheck = false });
            list.Add(new CheckBasic() { Label = "研发验证", IsCheck = false });
            list.Add(new CheckBasic() { Label = "其它", IsCheck = false });

            using (var context = new SicoreQMSEntities1())
            {
                var testProcessInfo = context.TestProcess.SingleOrDefault(b => b.Id == testProcessId);
                var a = testProcessInfo.TestType.Split(';');
                foreach (var item in a)
                {
                    //如果item在list中,list为true
                    var result = list.Any(p => p.Label == item);
                    if (result)
                    {
                        list.Where(p => p.Label == item).FirstOrDefault().IsCheck = true;
                    }
                }

            }
            return list;
        }
}
}
