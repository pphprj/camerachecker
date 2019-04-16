using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Discovery;

namespace CameraCheckerApp
{
    class WSDeviceDescription : Object
    {
        public WSDeviceDescription(EndpointDiscoveryMetadata metadata)
        {
            _uries = new List<Uri>();
            foreach (var listenUri in metadata.ListenUris)
            {
                _uries.Add(listenUri);
                
            }
            _metadata = metadata;
        }

        public EndpointDiscoveryMetadata Metadata
        {
            get { return _metadata; }
        }

        public String Endpoint
        {
            get
            {
                if (_uries.Count > 0)
                {
                    return _uries[0].AbsoluteUri;
                }
                else
                {
                    return "";
                }
            }
        }

        private List<Uri> _uries;
        private EndpointDiscoveryMetadata _metadata;
    }
}
