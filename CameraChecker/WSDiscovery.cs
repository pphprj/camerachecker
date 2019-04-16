using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel.Discovery;
using System.Xml;
using System.Collections;

using CameraCheckerApp;

namespace CameraCheckerApp
{
    class WSDiscovery
    {
        public delegate void DeviceFoundHandler(String enpoint);

        public WSDiscovery()
        {
            _devices = new ArrayList();

            var endPoint = new UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscoveryApril2005);

            _discoveryClient = new DiscoveryClient(endPoint);
            _discoveryClient.FindProgressChanged += this.deviceFounded;

            _discoveryState = new object();
        }

        public event DeviceFoundHandler DeviceFound;

        public void Start(String login, String password)
        {
            _login = login;
            _password = password;

            FindCriteria findCriteria = new FindCriteria();
            findCriteria.Duration = TimeSpan.MaxValue;
            findCriteria.MaxResults = int.MaxValue;
            // Edit: optionally specify contract type, ONVIF v1.0
            findCriteria.ContractTypeNames.Add(new XmlQualifiedName("NetworkVideoTransmitter",
                "http://www.onvif.org/ver10/network/wsdl"));

            _discoveryClient.FindAsync(findCriteria, _discoveryState);
        }

        public void Stop()
        {
            _discoveryClient.CancelAsync(_discoveryState);
        }


        public ArrayList Devices
        {
            get { return _devices; }
        }

        private void deviceFounded(object sender, FindProgressChangedEventArgs e)
        {
            var wsdevice = new WSDeviceDescription(e.EndpointDiscoveryMetadata);
            OnvifDeviceModel model = new OnvifDeviceModel();
            OnvifDeviceDescription device = model.GetModel(wsdevice.Endpoint, _login, _password);
            if (!_devices.Contains(device))
            {
                _devices.Add(device);
                DeviceFound(device.ToString());
            }
        }

        private ArrayList _devices;
        private DiscoveryClient _discoveryClient;
        private object _discoveryState;
        private String _login;
        private String _password;
    }
}
