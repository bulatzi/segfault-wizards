using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AbetApi.Data
{
    public interface IUploadManager
    {
        public string FileId { get; set; }
        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
        public string OriginalFileName { get; set; }
        public bool FileNotFound { get; set; }
        public bool StoreFile(IFormFile file, List<string> acceptableTypes);
        public bool DeleteCurrentFile();
        public bool DeleteFile(string filePath);
        public FileStream GetFile(string filePath);
    }
}
