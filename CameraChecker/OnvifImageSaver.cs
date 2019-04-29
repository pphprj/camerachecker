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

        public void SaveImage(String uri, String filename, String login, String password)
        {
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

                        Stream fileStream = File.OpenWrite(filename);
                        DownloadRemoteImage(snapshotUri, ref fileStream);
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
        }

        public void SaveImageToStream(String uri, String login, String password, ref Stream result)
        {
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

                        DownloadRemoteImage(snapshotUri, ref result);
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
        }



        // private void DownloadRemoteImageFile(string uri, string fileName)
        private void DownloadRemoteImage(string uri, ref Stream outputStream)
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
