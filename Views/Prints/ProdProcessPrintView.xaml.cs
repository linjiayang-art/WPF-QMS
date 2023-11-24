using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using SicoreQMS.Common.Models.Operation;
using System.Data;
using Microsoft.Reporting.WinForms;
using MaterialDesignThemes.Wpf;
using System.Windows.Forms.Integration;

namespace SicoreQMS.Views.Prints
{
    /// <summary>
    /// ProdProcessPrintView.xaml 的交互逻辑
    /// </summary>
    public partial class ProdProcessPrintView : System.Windows.Controls.UserControl
    {
        public ProdProcessPrintView()
        {
            InitializeComponent();

           
            btnLoadForm.Click += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    LoadReport();
                });

            };
           
        }

        private void LoadReport()
        {
            var reportViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            windowsFormsHost.Child = reportViewer;
            // 创建 ReportViewer 对象
            //var reportViewer = (Microsoft.Reporting.WinForms.ReportViewer)windowsFormsHost.Child;

            // 设置报表文件路径或数据源等
             reportViewer.LocalReport.ReportPath = "D:\\A-QMS\\SicoreQMS\\Prints\\ProdProcess.rdlc";

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("ModelSort", typeof(int));
            dataTable.Columns.Add("ProdProcessCard", typeof(string));
            dataTable.Columns.Add("ProcessType", typeof(string));
            using (var context = new SicoreQMSEntities1())
            {
                var allModel = context.Prod_ProcessItem.Where(b => b.ProdProcessId == "cb9c0b39-e67d-48f8-8bb8-db8237509770").ToList().OrderBy(x => x.ModelSort);
                foreach (var item in allModel)
                {
                    DataRow row = dataTable.NewRow();
                    row["ModelSort"] =(int) item.ModelSort; // 替换为实际的属性名
                    row["ProdProcessCard"] = item.ProdProcessCard; // 替换为实际的属性名
                    row["ProcessType"] = item.ProcessType; // 替换为实际的属性名
                    dataTable.Rows.Add(row);

                }
            }

            // 添加数据源
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Prod_ProcessSet", dataTable));

            // 刷新报表
            reportViewer.RefreshReport();
        }


    }
}
