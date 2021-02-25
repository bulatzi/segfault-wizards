using System;

namespace AbetApi.Authentication
{
    public interface ILdap
    {
        public bool LoginSuccessful { get; set; }
        public bool InternalErrorOccurred { get; set; }
        public string ErrorMessage { get; set; }
        public void ValidateCredentials(string userId, string password);
    }
}
