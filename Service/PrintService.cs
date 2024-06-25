using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Reporting.WinForms;
using System.Data;
using DocumentFormat.OpenXml.Math;
using System.Data.SqlClient;
using System.Windows;
using SicoreQMS.Common;
using SicoreQMS.Common.Models.Operation;

namespace SicoreQMS.Service
{
    public class PrintService
    {

        public static void ExportReportToWord(string rdlcFilePath, string exportFilePath, object dataSource, string dataSourceName)
        {
            // 创建 ReportViewer 实例
            ReportViewer reportViewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Local
            };

            // 加载 RDLC 文件
            reportViewer.LocalReport.ReportPath = rdlcFilePath;

            // 设置数据源
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName, dataSource));

            // 准备渲染报告
            string mimeType, encoding, fileNameExtension;
            string[] streams;
            Warning[] warnings;

            // 指定 Word 导出格式
            string exportFormat = "WORDOPENXML"; // 或者使用 "WORDOPENXML" 作为 DOCX 格式
            byte[] bytes = reportViewer.LocalReport.Render(
                exportFormat,
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            // 保存导出的文件
            File.WriteAllBytes(exportFilePath, bytes);
        }

        public static void ExportReportToWord(string rdlcFilePath, string exportFilePath, object dataSource1, object dataSource2, string dataSourceName1, string dataSourceName2)
        {


            using (ReportViewer reportViewer = new ReportViewer())
            {
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = rdlcFilePath;
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName1, dataSource1));
                // 设置数据源
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName2, dataSource2));

                reportViewer.RefreshReport();


                // 准备渲染报告
                string mimeType, encoding, fileNameExtension;
                string[] streams;
                Warning[] warnings;

                // 指定 Word 导出格式
                string exportFormat = "WORDOPENXML"; // 或者使用 "WORDOPENXML" 作为 DOCX 格式
                byte[] bytes = reportViewer.LocalReport.Render(
                    exportFormat,
                    null,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                // 保存导出的文件
                File.WriteAllBytes(exportFilePath, bytes);
            }
            // 创建 ReportViewer 实例
            //ReportViewer reportViewer = new ReportViewer
            //{
            //    ProcessingMode = ProcessingMode.Local
            //};

            //// 加载 RDLC 文件
            //reportViewer.LocalReport.ReportPath = rdlcFilePath;

            //// 设置数据源
            //reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName1, dataSource1));
            //// 设置数据源
            //reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName2, dataSource2));

            //// 准备渲染报告
            //string mimeType, encoding, fileNameExtension;
            //string[] streams;
            //Warning[] warnings;

            //// 指定 Word 导出格式
            //string exportFormat = "WORDOPENXML"; // 或者使用 "WORDOPENXML" 作为 DOCX 格式
            //byte[] bytes = reportViewer.LocalReport.Render(
            //    exportFormat,
            //    null,
            //    out mimeType,
            //    out encoding,
            //    out fileNameExtension,
            //    out streams,
            //    out warnings);

            //// 保存导出的文件
            //File.WriteAllBytes(exportFilePath, bytes);
        }





        public static void ExportScreeningReportToWord(string processId,string rdlcFilePath, string exportFilePath)
        {

            
            using (ReportViewer reportViewer = new ReportViewer())
            {

                if (!File.Exists(rdlcFilePath))
                {
                    throw new FileNotFoundException($"Report file not found at {rdlcFilePath}");
                }



                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.LocalReport.ReportPath = rdlcFilePath;

                string sql = $"select top 1 *From Prod_Process where id='{processId}'";
                DataTable dt = DBHelper.GetDataTable(sql);

                Dictionary<string,string> processitemId= new Dictionary<string, string>();
                using (var context=new SicoreQMSEntities1())
                {
                    var processItem=context.Prod_ProcessItem.Where(x => x.ProdProcessId == processId).ToList();
                    foreach (var item in processItem)
                    {
                        processitemId.Add(item.ModelSort.ToString(), item.Id);
                    }
                }



                // 添加主报表数据源
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));


                reportViewer.LocalReport.SubreportProcessing += (sender, ev) =>
                {
                    // 根据子报表名称绑定对应的数据源

                    if (ev.ReportPath == "ScreeningReport")
                    {
                        string screeningInfo = $"exec Proc_ScreeningInfo  @Id='{processId}'";
                        DataTable ScreeningInfo = DBHelper.GetDataTable(screeningInfo);
                        var ScreeningReportId = processitemId["1"];
                        ev.DataSources.Add(new ReportDataSource("ScreeningHead", getTableInfo("DataSetHead",ScreeningReportId)));
                        ev.DataSources.Add(new ReportDataSource("ScreeningInfo", ScreeningInfo));
                        return;
                    }
                    //内部目检
                    if (ev.ReportPath == "InternalVisualInspection")
                    {
                        var internalVisualInspectionId = processitemId["1"];
                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", internalVisualInspectionId)));
                        ev.DataSources.Add(new ReportDataSource("SubTable", getTableInfo("TypicalFailure", internalVisualInspectionId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", internalVisualInspectionId)));
                        return;
                    }
                    //温度
                    if (ev.ReportPath == "TemperatureReport")
                    {

                        var temperatureReportId = processitemId["2"];
                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", temperatureReportId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", temperatureReportId)));

                        return;
                    }
                    //电测
                    if (ev.ReportPath == "InitialElectricalTest")
                    {
                        var initialElectricalTestId = processitemId["5"];
                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", initialElectricalTestId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", initialElectricalTestId)));
                        ev.DataSources.Add(new ReportDataSource("TypicalFailure", getTableInfo("TypicalFailure", initialElectricalTestId)));

                        return;

                    }
                    //PDA
                    if (ev.ReportPath == "AginPDA")
                    {
                        var aginPDAId = processitemId["6"];

                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", aginPDAId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", aginPDAId)));
                        ev.DataSources.Add(new ReportDataSource("TypicalFailure", getTableInfo("TypicalFailure", aginPDAId)));

                        return;

                    }
                    //最终电测
                    if (ev.ReportPath == "EndElectricalTest")
                    {
                        var endElectricalTestId = processitemId["7"];
                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", endElectricalTestId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", endElectricalTestId)));
                        ev.DataSources.Add(new ReportDataSource("TypicalFailure", getTableInfo("TypicalFailure", endElectricalTestId)));

                        return;

                    }
                    //X光
                    if (ev.ReportPath == "XLineCheck")
                    {
                        var xLineCheckId = processitemId["8"];
                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", xLineCheckId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", xLineCheckId)));

                        return;

                    }
                    //超声波
                    if (ev.ReportPath == "UltrasonicScanningDetection")
                    {
                        var ultrasonicScanningDetectionId = processitemId["9"];
                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", ultrasonicScanningDetectionId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", ultrasonicScanningDetectionId)));

                        return;

                    }
                    //外部目检
                    if (ev.ReportPath == "OutEyeCheck")
                    {
                        var outEyeCheckId = processitemId["12"];
                        ev.DataSources.Add(new ReportDataSource("DataSetHead", getTableInfo("DataSetHead", outEyeCheckId)));
                        ev.DataSources.Add(new ReportDataSource("DataSetEquipment", getTableInfo("DataSetEquipment", outEyeCheckId)));
                        ev.DataSources.Add(new ReportDataSource("TypicalFailure", getTableInfo("TypicalFailure", outEyeCheckId)));


                        return;

                    }

                };
           
                reportViewer.RefreshReport();


                // 准备渲染报告
                string mimeType, encoding, fileNameExtension;
                string[] streams;
                Warning[] warnings;

                // 指定 Word 导出格式
                string exportFormat = "WORDOPENXML"; // 或者使用 "WORDOPENXML" 作为 DOCX 格式
                byte[] bytes = reportViewer.LocalReport.Render(
                    exportFormat,
                    null,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);

                // 保存导出的文件
                File.WriteAllBytes(exportFilePath, bytes);
            }

            // 创建 ReportViewer 实例
           
        }



        private static DataTable getTableInfo(string tableType, string sqlParm = null)
        {
            string getHeadInfoSql = "Proc_GetProcessHead";
            string getEquipmentInfoSql = "Proc_GetEquipmentUsage ";
            string sqlTypicalFailure = "Proc_TypicalFailureMode";
            SqlParameter[] param = null;

            if (sqlParm != null)
            {
                SqlParameter[] lparam = { new SqlParameter("Id", sqlParm) };
                param = lparam;

            }

            switch (tableType)
            {
                case "DataSetHead":

                    DataTable dataTable11 = DBHelper.ExecuteProcRe(getHeadInfoSql, param).Tables[0];
                    return dataTable11;
                case "DataSetEquipment":
                    DataTable dataTable22 = DBHelper.ExecuteProcRe(getEquipmentInfoSql, param).Tables[0];
                    return dataTable22;
                case "TypicalFailure":
                    DataTable TypicalFailureTable = DBHelper.ExecuteProcRe(sqlTypicalFailure, param).Tables[0];
                    return TypicalFailureTable;

            }

            DataTable dataTable = DBHelper.GetDataTable(getHeadInfoSql);
            return dataTable;
        }





       

    }
}

