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
using System.Data.SqlClient;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;

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
            reportViewer.LocalReport.ReportPath = "D:\\A-QMS\\SicoreQMS\\Prints\\TestProcess.rdlc";


            var head_dataTable = new DataTable();
            var body_dataTable = new DataTable();
            using (var context = new SicoreQMSEntities1 ())
            {
                var sqlCommand = "EXEC proc_QAExperimentReport @LotNo";
                var lotNoParam = new SqlParameter("@LotNo", "测试申请批次");

                // 执行存储过程
                var results = context.Database.SqlQuery<object>(sqlCommand, lotNoParam).ToList();

                // 获取 ObjectContext
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;

                // 使用内部连接获取第二个结果集
                var entityConnection = (EntityConnection)objectContext.Connection;
                var dbConnection = entityConnection.StoreConnection;

                dbConnection.Open();
                var command = dbConnection.CreateCommand();
                command.CommandText = sqlCommand;
                
                using (var reader = command.ExecuteReader())
                {
                    // 跳过第一个结果集
                    reader.NextResult();

                    // 映射第二个结果集
                    var secondResults = objectContext.Translate<object>(reader).ToList();
                }
                dbConnection.Close();

            }

            //    DataTable dataTable = new DataTable();

            //dataTable.Columns.Add("ModelSort", typeof(int));
            //dataTable.Columns.Add("ProdProcessCard", typeof(string));
            //dataTable.Columns.Add("ProcessType", typeof(string));
            //using (var context = new SicoreQMSEntities1())
            //{
            //    var allModel = context.Prod_ProcessItem.Where(b => b.ProdProcessId == "cb9c0b39-e67d-48f8-8bb8-db8237509770").ToList().OrderBy(x => x.ModelSort);
            //    foreach (var item in allModel)
            //    {
            //        DataRow row = dataTable.NewRow();
            //        row["ModelSort"] =(int) item.ModelSort; // 替换为实际的属性名
            //        row["ProdProcessCard"] = item.ProdProcessCard; // 替换为实际的属性名
            //        row["ProcessType"] = item.ProcessType; // 替换为实际的属性名
            //        dataTable.Rows.Add(row);

            //    }
            //}
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", body_dataTable));

                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", head_dataTable));

                // 添加数据源
               // reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Prod_ProcessSet", dataTable));

                // 刷新报表
                reportViewer.RefreshReport();
            

        }
    }
}
