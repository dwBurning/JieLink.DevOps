using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JieShun.JieLink.DevOps.Updater.Utils
{
    public class ConsoleRedirect : TextWriter
    {
        Action<string> writeLine;
        public ConsoleRedirect(Action<string> writeLine)
        {
            this.writeLine = writeLine;
        }
        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string value)
        {
            base.WriteLine();
            writeLine?.Invoke(value);
        }

        public override void WriteLine(object value)
        {
            this.WriteLine(value?.ToString());
        }

        public override void WriteLine(string format, object arg0)
        {
            this.WriteLine(string.Format(format, arg0));
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            this.WriteLine(string.Format(format, arg0, arg1));
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            this.WriteLine(string.Format(format, arg0, arg1, arg2));
        }

        public override void WriteLine(string format, params object[] arg)
        {
            this.WriteLine(string.Format(format, arg));
        }
    }
}
