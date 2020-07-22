using PartialViewInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewHealthMonitor
{
    public class Startup : IStartup
    {
        public string Name { get { return "HealthMonitor"; } }

        public int Priority { get { return 0; } }

        HealthMonitor healthMonitor = null;

        public void Exit()
        {
            if (healthMonitor != null)
                healthMonitor.Stop();
        }

        public void Start()
        {
            healthMonitor = new HealthMonitor("SmartCenter.Host", true);
            healthMonitor.Start();
        }
    }
}
