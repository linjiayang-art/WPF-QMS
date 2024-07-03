using DocumentFormat.OpenXml.Drawing.Diagrams;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Common.Models.Report
{
   public class EquipmentDateModel : BindableBase
    {
        private string _equipment;
        private string _model;
        private string _serialNumber;
        private DateTime _manufactureDate;
        private Dictionary<DateTime, string> _dailyData;
        private Dictionary<DateTime, string> _colorData = new Dictionary<DateTime, string>();
        private string equipmentNo;

        private string _status;

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);

        }

        public string EquipmentNo
                      
        {
            get => equipmentNo;
            set => SetProperty(ref equipmentNo, value);

        }

        public string Equipment
        {
            get { return _equipment; }
            set { SetProperty(ref _equipment, value); }
        }

        public string Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set { SetProperty(ref _serialNumber, value); }
        }

        public DateTime ManufactureDate
        {
            get { return _manufactureDate; }
            set { SetProperty(ref _manufactureDate, value); }
        }

        // 用于存储每一天的数据
        public Dictionary<DateTime, string> DailyData
        {
            get { return _dailyData ?? (_dailyData = new Dictionary<DateTime, string>()); }
            set { SetProperty(ref _dailyData, value); }
        }
        public Dictionary<DateTime, string> ColorData
        {
            get => _colorData;
            set => SetProperty(ref _colorData, value);
        }
    }
}
