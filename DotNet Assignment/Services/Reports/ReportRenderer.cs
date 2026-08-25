using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

namespace DotNet_Assignment.Services.Reports
{
    public class ReportRenderer :IReportRenderer
    {
        public byte[] RenderReport(string path, object data)
        {
            var reportPath = HostingEnvironment.MapPath(path);

            var reportPackager = new Telerik.Reporting.ReportPackager();

            Telerik.Reporting.Report report;

            using (var stream = File.OpenRead(reportPath))
            {
                report = (Telerik.Reporting.Report)reportPackager.UnpackageDocument(stream);
            }

            var table = report.Items.Find("table1", true).FirstOrDefault() as Telerik.Reporting.Table;

            if (table != null)
            {
                table.DataSource = data;
                report.DataSource = null;
            }
            else
            {
                report.DataSource = data;
            }

            var reportSource = new InstanceReportSource { ReportDocument = report };

            var processor = new ReportProcessor();

            var result = processor.RenderReport("PDF", reportSource, null);

            return result.DocumentBytes;
        }
    }
}