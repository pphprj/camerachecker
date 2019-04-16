using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CameraCheckerApp
{
    class WSDLBinding
    {
        public static System.ServiceModel.Channels.Binding GetWsdlBinding()
        {
            HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
            httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12, Encoding.UTF8)    , httpTransport);   
        }
    }
}
