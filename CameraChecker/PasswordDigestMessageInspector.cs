using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Microsoft.Web.Services3.Security.Tokens;

namespace CameraCheckerApp
{
    internal class PasswordDigestMessageInspector : IClientMessageInspector
    {
        private string password;
        private string username;

        public PasswordDigestMessageInspector(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            UsernameToken token = new UsernameToken(this.username, this.password, PasswordOption.SendHashed);
            // Serialize the token to XML
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement securityToken = token.GetXml(xmlDoc);
            // find nonce and add EncodingType attribute for BSP compliance
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsMgr.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            XmlNodeList nonces = securityToken.SelectNodes("//wsse:Nonce", nsMgr);
            XmlAttribute encodingAttr = xmlDoc.CreateAttribute("EncodingType");
            encodingAttr.Value = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary";
            if (nonces.Count > 0)
            {
                nonces[0].Attributes.Append(encodingAttr);
            }
            MessageHeader securityHeader = MessageHeader.CreateHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", securityToken, false);
            request.Headers.Add(securityHeader);
            // complete
            return Convert.DBNull;
        }

        /*public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            if (HTTPDigestAuthentication)
            {
                string digestHeader = string.Format("Digest username=\"{0}\",realm=\"{1}\",nonce=\"{2}\",uri=\"{3}\"," +
                                                    "cnonce=\"{4}\",nc={5:00000000},qop={6},response=\"{7}\",opaque=\"{8}\"",
                                                    this.username, realm, nonce, new Uri(this.URI).AbsolutePath, cnonce, counter, qop, digestResponse, opaque);

                HttpRequestMessageProperty httpRequest = new HttpRequestMessageProperty();
                httpRequest.Headers.Add("Authorization", digestHeader);
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);

                return Convert.DBNull;
            }
            else if (UsernametokenAuthorization)
            {
                string headerText = "<wsse:UsernameToken xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" +
                                    "<wsse:Username>" + this.username + "</wsse:Username>" +
                                    "<wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest\">" + digestPassword + "</wsse:Password>" +
                                    "<wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">" + Convert.ToBase64String(nonce) + "</wsse:Nonce>" +
                                    "<wsu:Created xmlns=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" + created + "</wsu:Created>" +
                                    "</wsse:UsernameToken>";

                XmlDocument MyDoc = new XmlDocument();
                MyDoc.LoadXml(headerText);

                MessageHeader myHeader = MessageHeader.CreateHeader("Security", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd", MyDoc.DocumentElement, false);

                request.Headers.Add(myHeader);

                return Convert.DBNull;
            }

            return request;
        }*/

    }
}