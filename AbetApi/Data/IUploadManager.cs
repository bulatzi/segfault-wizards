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
        public string OriginalFileName { get; set; }
        public bool StoreFile(IFormFile file, List<string> acceptableTypes);
        public bool DeleteCurrentFile();
        public bool DeleteFile(string filePath);
    }
}
