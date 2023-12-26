using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Reporting.WinForms;

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
            // 创建 ReportViewer 实例
            ReportViewer reportViewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Local
            };

            // 加载 RDLC 文件
            reportViewer.LocalReport.ReportPath = rdlcFilePath;

            // 设置数据源
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName1, dataSource1));
            // 设置数据源
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource(dataSourceName2, dataSource2));

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
    }
}
