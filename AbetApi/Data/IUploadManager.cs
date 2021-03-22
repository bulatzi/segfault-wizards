using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{
    public interface IUploadManager
    {
        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
        public void StoreFile(IFormFile file, List<string> acceptableTypes);
        SqlReturn InsertAccess2SQLserver();
    }
}
