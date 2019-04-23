using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CameraCheckerApp
{
    class OnvifDeviceDescription
    {
        public OnvifDeviceDescription()
        {
            _brand = "";
            _model = "";
            _serial = "";
            _firmware = "";
            _endpoint = "";
        }

        public OnvifDeviceDescription(String brand, String model, String serial, String firmware, String endpoint)
        {
            _brand = brand;
            _model = model;
            _serial = serial;
            _firmware = firmware;
            _endpoint = endpoint;
        }

        public bool IsEmpty()
        {
            return (_brand.Length == 0) &&
                (_model.Length == 0) &&
                (_serial.Length == 0) &&
                (_firmware.Length == 0) &&
                (_endpoint.Length == 0);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(OnvifDeviceDescription))
            {
                return false;
            }
            OnvifDeviceDescription other = (OnvifDeviceDescription)obj;
            return _endpoint.Equals(other.Endpoint);
        }

        public override int GetHashCode()
        {
            return _endpoint.GetHashCode();
        }

        public override string ToString()
        {
            return Ip + " " + _brand + " " + _model;
        }

        public String Brand
        {
            get { return _brand; }
        }

        public String Model
        {
            get { return _model; }
        }

        public String Serial
        {
            get { return _serial; }
        }

        public String Firmware
        {
            get { return _firmware; }
        }

        public String Endpoint
        {
            get { return _endpoint; }
        }

        public String Ip
        {
            get {
                Uri uri = new Uri(_endpoint);
                var ip = Dns.GetHostAddresses(uri.Host)[0];
                return ip.ToString();
            }
        }

        String _brand;
        String _model;
        String _serial;
        String _firmware;
        String _endpoint;

    }
}
