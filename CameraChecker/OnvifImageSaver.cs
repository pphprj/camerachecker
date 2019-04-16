using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.IO;

using CameraCheckerApp;

namespace CameraCheckerApp
{
    class OnvifImageSaver
    {
        public OnvifImageSaver()
        {

        }

        public void SaveImage(String uri, String login, String password)
        {
            ServicePointManager.Expect100Continue = false;
            EndpointAddress address = new EndpointAddress(uri);
            var device = new OnvifDevice.DeviceClient(WSDLBinding.GetWsdlBinding(), address);
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

                        DownloadRemoteImageFile(snapshotUri, "test.jpg");
                    }
                    catch (Exception ex)
                    {
                        
                    }
                } 
            }
            catch (Exception ex)
            {
            }
        }

       /* System.ServiceModel.Channels.Binding WsdlBinding
        {
            get
            {
                HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
                httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
                return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12, Encoding.UTF8), httpTransport);
            }
        }*/

        private void DownloadRemoteImageFile(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download oit
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                    while (bytesRead != 0);
                }
            }
        }
    }
}
