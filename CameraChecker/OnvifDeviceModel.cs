using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.IO;

namespace CameraCheckerApp
{
    class OnvifDeviceModel
    {
        public OnvifDeviceModel()
        {

        }

        public OnvifDeviceDescription GetModel(String uri, String login, String password)
        {
            string model;
            string firmware;
            string serial;
            string hardware;

            try
            {
                EndpointAddress address = new EndpointAddress(uri);
                
                var device = new OnvifDevice.DeviceClient(WSDLBinding.GetWsdlBinding(), address);

                PasswordDigestBehavior passwordDigestBehavior = new PasswordDigestBehavior(login, password);

                device.Endpoint.Behaviors.Add(passwordDigestBehavior);

                device.GetDeviceInformation(out model, out firmware, out serial, out hardware);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.ToString());
                return new OnvifDeviceDescription();
            }

            return new OnvifDeviceDescription(hardware, model, serial, firmware, uri);
        }


    }
}
