using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Joint.Common
{
    public class JeasuException : ApplicationException
    {
        public bool needLog = false;
        public JeasuException()
        {
            //Log.Info(this.Message, this);
        }

        public JeasuException(string message, bool needLog = false)
            : base(message)
        {
            //if (needLog)
            //{
            //    Log.Info(message, this);
            //}
            this.needLog = needLog;

        }

        public JeasuException(string message, Exception inner, bool needLog = false)
            : base(message, inner)
        {
            //if (needLog)
            //{
            //    Log.Info(message, inner);
            //}
            this.needLog = needLog;
        }
    }
}
