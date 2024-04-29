using ClosedXML.Excel;
using SicoreQMS.Common.Models.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SicoreQMS.Service
{
    public class ExcelService
    {
        public static void getDataFile<T>(string filePath, List<T> items, Dictionary<string, string> columnTranslations)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Test Count Report");

            
                // 获取泛型类型的属性
                PropertyInfo[] properties = typeof(T).GetProperties();
                // 创建标题行
                // 创建标题行，并使用字典进行翻译
                for (int i = 0; i < properties.Length; i++)
                {
                    string columnName = properties[i].Name;
                    string translatedName = columnTranslations.ContainsKey(columnName) ? columnTranslations[columnName] : columnName; // 如果找不到翻译，则使用原英文名
                    worksheet.Cell(1, i + 1).Value = translatedName;
                }

                // 填充数据
                for (int i = 0; i < items.Count; i++)
                {
                    for (int j = 0; j < properties.Length; j++)
                    {
                        var a = properties[j].GetValue(items[i], null);
                        if (a is null)
                        {
                            worksheet.Cell(i + 2, j + 1).Value = "";
                        }
                        else
                        {
                            worksheet.Cell(i + 2, j + 1).Value = a.ToString();
                        }
                      
                    }
                }
             
                for (int col = 1; col <= 11; col++)
                {
                    worksheet.Column(col).AdjustToContents();
                    if (worksheet.Column(col).Width <= 20)
                    {
                        worksheet.Column(col).Width = 20;
                    }
                    if (worksheet.Column(col).Width > 50) // 假设最大宽度为20
                    {
                        worksheet.Column(col).Width = 50;
                    }

                }
                //worksheet.Columns().AdjustToContents();
                worksheet.Columns().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                // 保存到文件
                workbook.SaveAs(filePath);
            }
        }


    }
}
