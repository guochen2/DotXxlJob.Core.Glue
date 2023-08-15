using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotXxlJob.Core.Glue.Models
{
    public class NatashaExecModel
    {
        public DateTime LastExecDate { get; set; } = DateTime.Now;
        public Action<IJobLogger, string> action { get; set; }
    }
}
