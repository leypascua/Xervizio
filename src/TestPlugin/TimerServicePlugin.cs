using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Xervizio;
using System.ComponentModel.Composition;
using System.Configuration;    


namespace TestPlugin {    
    using System.Timers;

    public class TimerServicePlugin : ServicePlugin {
        private Timer _timer;

        public TimerServicePlugin() {
            _timer = new Timer(2000);
            _timer.Elapsed += OnTimerElapsed;
        }

        public override void Start() {
            Console.WriteLine("TimerService starting...");
            _timer.Start();
        }

        public override void Stop() {
            Console.WriteLine("TimerService stopping...");
            _timer.Stop();
            _timer = null;
            Thread.Sleep(7000);
        }

        public override ServicePluginState GetState() {
            return ServicePluginState.Idle;
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e) {
            Console.WriteLine(ConfigurationManager.AppSettings["message"] ?? "hoy");
        }
    }
}
