using SicoreQMS.Common.Models.Basic;
using SicoreQMS.Common.Models.Operation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SicoreQMS.Service
{
    public class EquipmentService
    {


        public static ObservableCollection<SelectBasic> GetEquipmentBasic()
        {
            var results = new ObservableCollection<SelectBasic>();
            using (var context = new SicoreQMSEntities1())
            {


                var query = from e in context.Equipment
                            join eStatus in context.EquipmentStatus on e.EquipmentID equals eStatus.EquipmentID
                            where eStatus.EquipmentStatus1 == 0
                            select new
                            {
                                e.EquipmentName,
                                EquipmentNo = e.EquipmentNo,
                                EquipmentID = e.EquipmentID

                            };
                var list = query.ToList();
                foreach (var r in list)
                {
                    results.Add(new SelectBasic { Value = r.EquipmentID, Label = r.EquipmentName + r.EquipmentNo });
                }

                return results;
            }




        }


        public static ResultInfo RecordEquipmentLog(string equipmentId,string useType,string useProcess)
        {
            using (var context=new SicoreQMSEntities1())
            {
                var equipment = context.Equipment.SingleOrDefault(e => e.EquipmentID == equipmentId);
                if (equipment == null)
                {
                    return new ResultInfo { ResultStatus = false, ResultMessage = "设备不存在" };
                }
                var equipmentStatus = context.EquipmentStatus.SingleOrDefault(e => e.EquipmentID == equipmentId);
                //如果设备为0则表示设备空闲,更新为1,新增设备使用记录
                if (equipmentStatus.EquipmentStatus1 == 0)
                {
                    var usageRecord = new UsageRecord
                    {
                        Id = Guid.NewGuid().ToString(),
                        EquipmentId = equipmentId,
                        StartDate = DateTime.Now,
                        UseType = useType,
                        UseProcess = useProcess
                    };
                    context.UsageRecord.Add(usageRecord);
                    equipmentStatus.EquipmentStatus1 = 1;
                    context.SaveChanges();
                    return new ResultInfo()
                    { ResultStatus = true, ResultMessage = "设备状态更新成功" };
                }

                if (equipmentStatus.EquipmentStatus1 == 1)
                {
                    var usageRecord = context.UsageRecord.SingleOrDefault(u => u.EquipmentId == equipmentId && u.EndDate == null&&u.UseProcess==useProcess );
                    if (usageRecord == null)
                    {
                        return new ResultInfo { ResultStatus = false, ResultMessage = "设备使用记录不存在" };
                    }
                    usageRecord.EndDate = DateTime.Now;
                    equipmentStatus.EquipmentStatus1 = 0;
                    context.SaveChanges();
                }
        
            }

            return new ResultInfo()
             { ResultStatus = true, ResultMessage = "设备状态更新成功" };
        }


    }
}
