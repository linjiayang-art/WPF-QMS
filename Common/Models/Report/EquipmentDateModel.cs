using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private string _equipmentYield;

        public string EquipmentYield
        {
            get => _equipmentYield;
            set => SetProperty(ref _equipmentYield, value);

        }



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

    public class ExcelExporter:BindableBase
    {
        public static void ExportToExcel(ObservableCollection<EquipmentDateModel> data, ObservableCollection<EquipmentUsageDetailModel> usageDetails,string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("设备运行情况");

                // 添加表头
                worksheet.Cell(1, 1).Value = "设备名";
                worksheet.Cell(1, 2).Value = "设备类型";
                worksheet.Cell(1, 3).Value = "利用率";
                worksheet.Cell(1, 4).Value = "有效期";
                worksheet.Cell(1, 5).Value = "设备编号";
                worksheet.Cell(1, 6).Value = "设备状态";


      
                // 动态添加 DailyData 和 ColorData 的表头
                var allDates = data.SelectMany(d => d.DailyData.Keys).Union(data.SelectMany(d => d.ColorData.Keys)).Distinct().OrderBy(d => d).ToList();
                for (int i = 0; i < allDates.Count; i++)
                {
                    worksheet.Cell(1, 7 + i).Value = allDates[i].ToString("yyyy-MM-dd");
                }

                // 添加数据
                for (int i = 0; i < data.Count; i++)
                {
                    var equipment = data[i];
                    worksheet.Cell(i + 2, 1).Value = equipment.Equipment;
                    worksheet.Cell(i + 2, 2).Value = equipment.Model;
                    worksheet.Cell(i + 2, 3).Value = equipment.EquipmentYield;
                    worksheet.Cell(i + 2, 4).Value = equipment.ManufactureDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(i + 2, 5).Value = equipment.EquipmentNo;
                    worksheet.Cell(i + 2, 6).Value = equipment.Status;

                    for (int j = 0; j < allDates.Count; j++)
                    {
                        equipment.DailyData.TryGetValue(allDates[j], out string dailyValue);
                        equipment.ColorData.TryGetValue(allDates[j], out string colorValue);
                        var cell = worksheet.Cell(i + 2, 7 + j);
                        cell.Value = dailyValue;
                        //worksheet.Cell(i + 2, 8 + j * 2).Value = colorValue;
                        switch (colorValue)
                        {   case "Gray"
                        :
                                colorValue = "#808080";
                                break;
                            case "LightCoral":
                                colorValue = "#F08080";
                                break;
                            default:
                                break;
                        }
                        // 设置单元格背景颜色
                        if (!string.IsNullOrEmpty(colorValue))
                        {
                            var color = XLColor.FromHtml(colorValue);
                            cell.Style.Fill.BackgroundColor = color;
                        }
                        // 设置单元格样式
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    }
                }
                var worksheet2 = workbook.Worksheets.Add("设备运行明细");

                // 添加表头
                worksheet2.Cell(1, 1).Value = "排序";
                worksheet2.Cell(1, 2).Value = "设备名称";
                worksheet2.Cell(1, 3).Value = "设备类型";
                worksheet2.Cell(1, 4).Value = "设备名称";
                worksheet2.Cell(1, 5).Value = "使用类型";
                worksheet2.Cell(1, 6).Value = "相关节点";
                worksheet2.Cell(1, 7).Value = "使用人";
                worksheet2.Cell(1, 8).Value = "产品批次";
                worksheet2.Cell(1, 9).Value = "产品类型";
                worksheet2.Cell(1, 10).Value = "使用时间";
                worksheet2.Cell(1, 11).Value = "数量";
                worksheet2.Cell(1, 12).Value = "开始时间";
                worksheet2.Cell(1, 13).Value = "结束时间";

                // 设置表头样式
                var headerRange2 = worksheet2.Range(1, 1, 1, 13);
                headerRange2.Style.Font.Bold = true;
                headerRange2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                headerRange2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                headerRange2.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // 添加数据
                for (int i = 0; i < usageDetails.Count; i++)
                {
                    var detail = usageDetails[i];
                    worksheet2.Cell(i + 2, 1).Value = detail.Sort;
                    worksheet2.Cell(i + 2, 2).Value = detail.EquipmentName;
                    worksheet2.Cell(i + 2, 3).Value = detail.EquipmentType;
                    worksheet2.Cell(i + 2, 4).Value = detail.EquipmentNo;
                    worksheet2.Cell(i + 2, 5).Value = detail.UseType;
                    worksheet2.Cell(i + 2, 6).Value = detail.UseProcess;
                    worksheet2.Cell(i + 2, 7).Value = detail.UseUser;
                    worksheet2.Cell(i + 2, 8).Value = detail.ProdLot;
                    worksheet2.Cell(i + 2, 9).Value = detail.ProdType;
                    worksheet2.Cell(i + 2, 10).Value = detail.UseCount;
                    worksheet2.Cell(i + 2, 11).Value = detail.Qty;
                    worksheet2.Cell(i + 2, 12).Value = detail.StartDate?.ToString("yyyy-MM-dd");
                    worksheet2.Cell(i + 2, 13).Value = detail.EndDate?.ToString("yyyy-MM-dd");

                    // 设置单元格样式
                    var rowRange = worksheet2.Range(i + 2, 1, i + 2, 13);
                    rowRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rowRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                // 自动调整列宽
                foreach (var column in worksheet2.ColumnsUsed())
                {
                    if (column.Width < 20)
                    {
                        column.Width = 20;
                    }
                }
                foreach (var column in worksheet.ColumnsUsed())
                {
                    if (column.Width < 20)
                    {
                        column.Width = 20;
                    }
                }
                //worksheet2.Columns().AdjustToContents();

                //// 自动调整列宽
                //worksheet.Columns().AdjustToContents();

                // 保存文件
                workbook.SaveAs(filePath);
            }
        }
    }
    }
