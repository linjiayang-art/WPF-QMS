using Prism.Mvvm;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using System.Collections.ObjectModel;
using LiveCharts.Configurations;
using System.Data.Entity;
using SicoreQMS.Common.Models.Report;
using System.Collections.Generic;
using SicoreQMS.Common.Models.Operation;
using Prism.Regions;
using System.Diagnostics;

namespace SicoreQMS.ViewModels
{
    public class EquipmentChartViewModel :BindableBase, IRegionMemberLifetime
    {
        #region 属性

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public bool KeepAlive { get; set; } = false;
        #endregion

        public EquipmentChartViewModel()
        {
  
            var start = DateTime.Today.AddDays(-15);
            var end = DateTime.Today;
            var equipments = Service.EquipmentService.GetEquipmentCharts(start, end);
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "使用次数",
                    Values = new ChartValues<int>(equipments.Select(x => x.UsageCount))
                },
                new LineSeries
                {
                    Title = "产量",
                    Values = new ChartValues<double>(equipments.Select(x => double.Parse(x.Yield.TrimEnd('%')) / 100))
                }
            };
            Labels = equipments.Select(x => x.EquipementName).ToList();
            Formatter = value => value.ToString("N");
           
        }

    }

    
}
