using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

using System.IO;

using CameraCheckerApp;

using Vlc.DotNet.Forms;

namespace TestOnvifApp
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            _discovery = new WSDiscovery();
            _discovery.DeviceFound += this.deviceFound;

            _login = "admin";
            _password = "admin";

            //this.pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;

            Logger.WriteLog("Form main create");
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            _discovery.Start(_login, _password);

            Logger.WriteLog("Start was clicked");
        }


        private void listBoxResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_discovery.Devices.Count <= listBoxResults.SelectedIndex || listBoxResults.SelectedIndex < 0)
            {
                Logger.WriteLog("index out of bounds");
                return;
            }

            try
            {
                OnvifDeviceDescription description = (OnvifDeviceDescription)_discovery.Devices[listBoxResults.SelectedIndex];
                Logger.WriteLog(description.ToString());

                OnvifMediaStream mediaStream = new OnvifMediaStream();

                String streamUri = mediaStream.GetStreamUri(description.Endpoint, _login, _password);

                string[] mediaOptions = new string[]
                {
                    "rtsp-user=" + _login,
                    "rtsp-pwd=" + _password
                };
                this.vlcControl.Play(streamUri, mediaOptions);
            }
            catch (Exception exp)
            {
                Logger.WriteLog(exp.ToString());
                MessageBox.Show(exp.ToString());
            }
        }

        private void deviceFound(string endpoint)
        {
            if (!listBoxResults.Items.Contains(endpoint))
            {
                listBoxResults.Items.Add(endpoint);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void searchSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearchSettings searchSettings = new FormSearchSettings();
            if (searchSettings.ShowDialog() == DialogResult.OK)
            {
                _login = searchSettings.Login;
                _password = searchSettings.Password;

                Logger.WriteLog(_login + " " + _password);
            }
        }

        private String _login;
        private String _password;
        private WSDiscovery _discovery;

        private void vlcControl1_VlcLibDirectoryNeeded(object sender, VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            e.VlcLibDirectory = libDirectory;

            if (!e.VlcLibDirectory.Exists)
            {
                var folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Select Vlc libraries folder.";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                folderBrowserDialog.ShowNewFolderButton = true;
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    e.VlcLibDirectory = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                }
            }
        }

        private void buttonSnapshot_Click(object sender, EventArgs e)
        {
            if (_discovery.Devices.Count <= listBoxResults.SelectedIndex || listBoxResults.SelectedIndex < 0)
            {
                Logger.WriteLog("index out of bounds");
                return;
            }

            if (!this.vlcControl.IsPlaying)
            {
                return;
            }

            try
            {
                OnvifDeviceDescription description = (OnvifDeviceDescription)_discovery.Devices[listBoxResults.SelectedIndex];

                String path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                DateTime dateTime = DateTime.Now;

                String snapshotPath = path + "\\" + description.Ip + " " + dateTime.ToString("yyyy-MM-dd hh-mm-ss") + ".jpg";
                this.vlcControl.TakeSnapshot(snapshotPath);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.ToString());
            }
        }
    }
}
