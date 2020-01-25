using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Configuration;
using System.Management;

namespace TorrentKillSwitch
{
    class Control
    {
        public void GetStarted()
        {
            string jsonString = GetResponseFromAPI();
            string ipAddress = DecodeResponse(jsonString);
            bool isSafe = TestIPAddress(ipAddress);
            if (!isSafe) ShutdownApplication();
        }

        private void ShutdownApplication()
        {
            Process[] procs = null;
            try
            {
                procs = Process.GetProcessesByName(Properties.Settings.Default.ProcessName);
                if (procs.Length > 0)
                {
                    Process qbittorrentProc = procs[0];
                    if (!qbittorrentProc.HasExited)
                    {
                        qbittorrentProc.Kill();
                    }
                    Library.WriteErrorLog("Process Shutdown");
                }
            }
            catch (Exception ex)
            {
                Library.WriteErrorLog(ex);
                throw;
            }
            finally
            {
                if (procs != null)
                {
                    foreach (Process p in procs)
                    {
                        p.Dispose();
                    }
                }
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
        private bool TestIPAddress(string ipAddress)
        {
            string location = GetPathToExecutableOfService();
            location = location.Trim(Path.GetInvalidFileNameChars());
            Configuration config = ConfigurationManager.OpenExeConfiguration(location);
            System.Configuration.AppSettingsSection appSettings = (System.Configuration.AppSettingsSection)config.GetSection("appSettings");

            foreach (string key in appSettings.Settings.AllKeys)
            {
                if (key.Equals("SafeIP"))
                {
                    if (ipAddress.Equals(appSettings.Settings[key].Value))
                        return true;
                    

                    
                }
            }
            return false;

            
        }
        private string DecodeResponse(string jsonString)
        {
            try
            {
                dynamic json = JsonConvert.DeserializeObject(jsonString);
                return json["ip"];
            }
            catch (Exception)
            {
                ShutdownApplication();
                throw;
            }
            
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
                //call shutdown cause we didn't get a response and can't trust our selves
                ShutdownApplication();
                throw;
            }
        }
    }
}
