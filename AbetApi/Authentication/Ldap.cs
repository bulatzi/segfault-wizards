using System.Net;
using System.DirectoryServices.Protocols;

namespace AbetApi.Authentication
{
    public class Ldap : ILdap
    {
        private readonly string connectionUrl = "ldaps://ldap-auth.untsystem.edu:636";
        public bool LoginSuccessful { get; set; } = false;
        public bool InternalErrorOccurred { get; set; } = false;
        public string ErrorMessage { get; set; }

        public void ValidateCredentials(string userId, string password)
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
                        LoginSuccessful = true;
                    }
                    catch
                    {
                        ErrorMessage = "Error: Username and password pair is incorrect.";
                    }
                    finally
                    {
                        options.StopTransportLayerSecurity();
                    }
                }
                catch
                {
                    InternalErrorOccurred = true;
                    ErrorMessage = "Internal Server Error: Please try again later.";
                }
            }
        }
    }
}
