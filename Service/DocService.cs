using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SicoreQMS.Service
{
    public class DocService
    {
        public static DataTable LoadDoc(string filePath)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {



                Dictionary<string, string> TranslateDict = new Dictionary<string, string>()
                {
                    { "序号","ExperimentItemNo"},
                    {"试验项目","ExperimentName" },
                    { "试验标准/作业指导","ExperimentStandard"},
                    { "试验条件","ExperimentConditions"},
                    { "试验数量","ExperimentQty"},
                    { "合格数量","ExperimentItemPassQty"},
                    { "试验编号","ExperimentNo"},
                    { "试验人","ExperimentUser"},
                    {"试验时间", "ExperimentSatrtTime"},
                    {"其它描述","ItemDesc" }
                };

                // 假设我们只关注文档中的第一个表格
                Table table = wordDoc.MainDocumentPart.Document.Body.Elements<Table>().FirstOrDefault();
                if (table != null)
                {
                    DataTable dt = new DataTable();
                    int rowBeginIndex = 999;
                    // 遍历表格的所有行  表头第一个为序号是确定的,
                    foreach (TableRow row in table.Elements<TableRow>())
                    {
                        
                        IEnumerable<TableCell> cells = row.Elements<TableCell>();


                        var rowIndex = table.Elements<TableRow>().ToList().IndexOf(row);

                        var headText = row.Elements<TableCell>().Select(c => c.InnerText.Trim());
                        List<string> headTextList = headText.ToList();
                        if (headTextList[0] == "序号")
                        {
                            rowBeginIndex = rowIndex;
                            foreach (var item in headTextList)
                            {
                                var teanslateItem = TranslateDict[item];
                                dt.Columns.Add(teanslateItem);
                            }


                        }
                      

                        if (rowIndex > rowBeginIndex)
                        {



                            if (headTextList[0]=="备注")
                            {
                                return dt;
                                //rowBeginIndex = 99999;
                                //continue;
                            }



                            DataRow dtRow = dt.NewRow();
                            int cellIndex = 0;
                            foreach (TableCell cell in cells)
                            {
                                // 获取单元格中的文本
                                string cellText = cell.InnerText;
                                dtRow[cellIndex] = cellText;
                                cellIndex++;
                            }


                            if (headTextList[0] == "")
                            {
                                var experimentItemNo = dt.Rows[dt.Rows.Count-1][0].ToString();
                                dtRow[0] = experimentItemNo;
                            }


                            dt.Rows.Add(dtRow);
                        }
                    }
                    return dt;

                    //int cellIndex = 0;

                    //foreach (TableCell cell in cells)
                    //{

                    //    // 获取单元格中的文本
                    //    string cellText = cell.InnerText;


                    //    // 根据资源寻找表头

                    //    if (row == table.Elements<TableRow>().First())
                    //    {
                    //        dt.Columns.Add(cellText);
                    //    }
                    //    else
                    //    {
                    //        // 确保有足够的列
                    //        if (cellIndex >= dt.Columns.Count)
                    //        {
                    //            dt.Columns.Add("Column" + cellIndex);
                    //        }

                    //        dtRow[cellIndex] = cellText;
                    //    }

                    //    cellIndex++;
                    //}



                }
            }

            return null;
        }
    }
}
