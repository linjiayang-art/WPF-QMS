using Prism.Mvvm;
using SicoreQMS.Common.Models.Operation;
using Syncfusion.XlsIO.FormatParser.FormatTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Basic
{
    public class SPEquipment:BindableBase
    {

        public SPEquipment(Equipment equipment)
        {
            this.EquipmentNo = equipment.EquipmentNo;
            this.EquipmentID = equipment.EquipmentID;
            this.EquipmentName = equipment.EquipmentName;
            this.Capacity = equipment.Capacity ?? 0;
            this.AvailableCapacity = equipment.AvailableCapacity ?? 0;

                
        }
        public string EquipmentID { get; set; }
        public string  EquipmentName { get; set; }
        public string EquipmentNo { get; set; }
        public int Capacity { get; set; }
        public int AvailableCapacity { get; set; }
        public int useQty { get; set; } = 0;
        public int UseQty
        {
            get { return useQty; } 
            set
            {
                useQty = value;
                RaisePropertyChanged();
            }
        }


        public void AddSpUsage(string useType,string process,string processid)
        {
            Service.EquipmentService.RecordSPEquipmentLog(equipmentId: EquipmentID, useType: useType, qty: UseQty,useProcess:process ,processId: processid);

        }

    }
}
