using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet_Assignment.Services.Reports
{
    public interface IReportRenderer
    {
        byte[] RenderReport(string path, object data);
    }
}
