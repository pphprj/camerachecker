using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

using CameraCheckerApp;

namespace TestOnvifApp
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            _discovery = new WSDiscovery();
            _discovery.DeviceFound += this.deviceFound;

            _login = "admin";
            _password = "admin";

            this.pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;

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

                OnvifImageSaver saver = new OnvifImageSaver();

                Stream image = new MemoryStream();
                saver.SaveImageToStream(description.Endpoint, _login, _password, ref image);

                image.Position = 0;

                if (image.Length != 0)
                {
                    this.pictureBoxImage.Image = Bitmap.FromStream(image);

                    String path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    DateTime dateTime = DateTime.Now;

                    this.pictureBoxImage.Image.Save(path + "\\" + description.Ip + " " + dateTime.ToString("yyyy-MM-dd hh-mm-ss") + ".jpg");
                }
                else
                {
                    MessageBox.Show("Can't download image");
                }
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
    }
}
