using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Configuration;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace TorrentKillSwitchGUI
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
            DefineButton(GetServiceStatus());
            
        }

        private void DefineButton(string status)
        {
            if (status.Equals("Stopped"))
            {
                btnClickToStart.Text = "Click To Start";
            }
            else if (status.Equals("Running"))
            {
                btnClickToStart.Text = "Click To Stop";
            }
            else 
            {
                btnClickToStart.Text = "Unkown Service State";
                btnClickToStart.Enabled = false;
            }
        }

        private string GetPathToExecutableOfService()
        {
            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + Properties.Settings.Default.ServiceName + "'"))
            {
                wmiService.Get();
                string currentserviceExePath = wmiService["PathName"].ToString();
                return wmiService["PathName"].ToString();
            }
        }

        private void EditConfigFile()
        {
            string location = GetPathToExecutableOfService();
            location = location.Trim(Path.GetInvalidFileNameChars());
            Configuration config = ConfigurationManager.OpenExeConfiguration(location);
            string newIp = GetResponseFromAPI();

            dynamic json = JsonConvert.DeserializeObject(newIp);
            newIp = json["ip"];
            config.AppSettings.Settings.Remove("SafeIP");
            config.AppSettings.Settings.Add("SafeIP", newIp);
            
            System.Configuration.AppSettingsSection appSettings = (System.Configuration.AppSettingsSection)config.GetSection("appSettings");

            foreach (string key in appSettings.Settings.AllKeys)
            {
                if (key.Equals("SafeIP"))
                {
                    string value = appSettings.Settings[key].Value;
                }
            }


            config.Save(ConfigurationSaveMode.Modified);

        }

        private string GetResponseFromAPI()
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Properties.Settings.Default.APIURL);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                                
                throw;
            }
        }

        private string GetServiceStatus()
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = "TorrentKillSwitch";
            try
            {
                return sc.Status.ToString();
            }
            catch (Exception)
            {
                label1.Text = "Torrent Kill Switch service not installed";
                btnClickToStart.Text = "Torrent Kill Switch service not installed";
                btnClickToStart.Enabled = false;
                return "";
            }
        }

        private void StartService()
        {

            EditConfigFile();


            ServiceController sc = new ServiceController();
            sc.ServiceName = "TorrentKillSwitch";
            try
            {
                // Start the service, and wait until its status is "Running".
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running);
                btnClickToStart.Text = "Click To Stop";

                // Display the current service status.
            }
            catch (InvalidOperationException e)
            {
                var puaseme = "";
                throw;
            }
        }
        private void StopService()
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = "TorrentKillSwitch";
            try
            {
                // Start the service, and wait until its status is "Running".
                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped);
                btnClickToStart.Text = "Click To Start";

                // Display the current service status.
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        private void btnClickToStart_Click(object sender, EventArgs e)
        {
            string status = GetServiceStatus();
            try
            {
                if (status == "Stopped")
                {
                    StartService();
                }
                else if (status == "Running")
                {
                    StopService();
                }
            }
            catch (InvalidOperationException)
            {
                label1.Text = "Torrent Kill Switch service not installed";
                return;
            }
        }
    }
}
