using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.Data
{
    public interface IUploadManager
    {
        public Task<string> ReceiveFile(HttpRequest request);
        public string GetErrorMsg();
    }
}
