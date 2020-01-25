using System.ServiceProcess;
using System.Timers;

namespace TorrentKillSwitch
{
    public partial class Scheduler : ServiceBase
    {
        private Timer timer1 = null;
        public Scheduler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer1 = new Timer();
            this.timer1.Interval = 5000; //30 secs
            this.timer1.Elapsed += new System.Timers.ElapsedEventHandler(this.timer1_Tick);
            timer1.Enabled = true;
            Library.WriteErrorLog("Torrent Kill Switch service started");
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
            Library.WriteErrorLog("Torrent Kill Switch service stopped");
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            //Write your code call here
            Control control = new Control();
            control.GetStarted();

            
            Library.WriteErrorLog("Timer ticket and some job has been done successfully");
        }

        


    }
}
