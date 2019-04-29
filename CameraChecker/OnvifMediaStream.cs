using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;

using CameraCheckerApp;

namespace CameraCheckerApp
{
    class OnvifMediaStream
    {
        public OnvifMediaStream()
        {

        }

        public String GetStreamUri(String uri, String login, String password)
        {
            String result = "";
            ServicePointManager.Expect100Continue = false;
            EndpointAddress address = new EndpointAddress(uri);
            var device = new OnvifDevice.DeviceClient(WSDLBinding.GetWsdlBinding(), address);
            PasswordDigestBehavior passwordDigestBehavior = new PasswordDigestBehavior(login, password);
            device.Endpoint.Behaviors.Add(passwordDigestBehavior);
            try
            {
                OnvifDevice.CapabilityCategory[] cats = { OnvifDevice.CapabilityCategory.All };

                var caps = device.GetCapabilities(cats);
                var media = new OnvifMedia.MediaClient(WSDLBinding.GetWsdlBinding(), new EndpointAddress(caps.Media.XAddr));

                if (media != null)
                {
                    media.ClientCredentials.HttpDigest.ClientCredential.UserName = login;
                    media.ClientCredentials.HttpDigest.ClientCredential.Password = password;
                    media.ClientCredentials.HttpDigest.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
                    try
                    {
                        var profiles = media.GetProfiles();
                        string profileToken = profiles[1].token;
                        String snapshotUri = media.GetSnapshotUri(profileToken).Uri;

                        OnvifMedia.StreamSetup strSetup = new OnvifMedia.StreamSetup();
                        strSetup.Stream = OnvifMedia.StreamType.RTPUnicast;
                        strSetup.Transport = new OnvifMedia.Transport();
                        strSetup.Transport.Protocol = OnvifMedia.TransportProtocol.RTSP;

                        String streamuri = media.GetStreamUri(strSetup, profileToken).Uri;
                        result = streamuri;
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.ToString());
            }

            return result;
        }
    }
}
