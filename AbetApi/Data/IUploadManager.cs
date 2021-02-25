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
        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
        public void ReceiveFile(HttpRequest request);
    }
}
