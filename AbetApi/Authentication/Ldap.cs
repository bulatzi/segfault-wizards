using System.Net;
using System.DirectoryServices.Protocols;

namespace AbetApi.Authentication
{
    public class Ldap : ILdap
    {
        private readonly string connectionUrl = "ldaps://ldap-auth.untsystem.edu:636";

        public bool ValidateCredentials(string userId, string password, out bool internalErrorOccurred)
        {
            using (LdapConnection ldapConn = new LdapConnection(connectionUrl))
            {

                LdapSessionOptions options = ldapConn.SessionOptions;

                ldapConn.AuthType = AuthType.Basic;
                ldapConn.Credential = new NetworkCredential($"uid={userId}, ou=people, o=unt", password);
                options.ProtocolVersion = 3;
                options.AutoReconnect = true;
                options.HostName = "ldap-auth.untsystem.edu";
                options.VerifyServerCertificate += (conn, cert) => { return true; };

                //Start TLS
                try
                {
                    options.StartTransportLayerSecurity(null);

                    //Validate credentials
                    try
                    {
                        ldapConn.Bind();
                        internalErrorOccurred = false;
                        return true;
                    }
                    catch
                    {
                        internalErrorOccurred = false;
                        return false;
                    }
                    finally
                    {
                        options.StopTransportLayerSecurity();
                    }
                }
                catch
                {
                    internalErrorOccurred = true;
                    return false;
                }
            }
        }
    }
}
