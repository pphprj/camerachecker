using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            _discovery.Start("admin", "mc1409");
        }

        private WSDiscovery _discovery;

        private void listBoxResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnvifDeviceDescription description = (OnvifDeviceDescription)_discovery.Devices[listBoxResults.SelectedIndex];
            OnvifImageSaver saver = new OnvifImageSaver();
            saver.SaveImage(description.Endpoint, "admin", "mc1409");
            //OnvifDeviceModel model = new OnvifDeviceModel();
            //model.GetModel(description.Endpoint, "admin", "mc1409"); 
        }

        private void deviceFound(string endpoint)
        {
            if (!listBoxResults.Items.Contains(endpoint))
            {
                listBoxResults.Items.Add(endpoint);
            }
        }
    }
}
