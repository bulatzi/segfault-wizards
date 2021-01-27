using System;

namespace AbetApi.Authentication
{
    public interface ILdap
    {
        public bool ValidateCredentials(string userId, string password, out bool internalErrorOccurred);
    }
}
