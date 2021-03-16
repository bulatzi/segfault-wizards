using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AbetApi.Data
{
    public class UploadManager : IUploadManager
    {
        public string FilePath { get; set; } = null;
        public string ErrorMessage { get; set; }

        //stores a received file in the Uploads folder
        public void StoreFile(IFormFile file, List<string> acceptableTypes)
        {
            try
            {
                //check if file type is acceptable
                if (!acceptableTypes.Contains(Path.GetExtension(file.FileName)))
                {
                    ErrorMessage = "Error: Unexpected file type.";
                    return;
                }

                if (file.Length > 0)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                    //create Uploads folder if it does not exist
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    FilePath = Path.Combine(folderPath, file.FileName);

                    using (FileStream stream = new FileStream(FilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Flush();
                    }
                }
                else
                {
                    ErrorMessage = "Error: File contains no data.";
                }
            }
            catch
            {
                ErrorMessage = "Internal Server Error: Please try again later.";
            }
        }
    }
}
