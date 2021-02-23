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
        private string errorMsg;

        //receives file from the request and stores it in the Uploads folder
        public async Task<string> ReceiveFile(HttpRequest request)
        {
            try
            {
                IFormCollection formCollection = await request.ReadFormAsync();
                IFormFile file = formCollection.Files.First();

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                    string filePath;

                    //create Uploads folder if it does not exist
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    filePath = Path.Combine(folderPath, fileName);

                    /* This will restrict the uploaded file type to only .accdb
                    if (Path.GetExtension(path) != ".accdb")
                    {
                        errorMsg = "Error: File is not an access database file (.accdb).";
                        return null;
                    }
                    */

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return filePath;
                }
                else
                {
                    errorMsg = "Error: File contains no data.";
                    return null;
                }
            }
            catch (Exception ex)
            {
                errorMsg = "Internal Error: " + ex.Message;
                return null;
            }
        }

        public string GetErrorMsg()
        {
            return errorMsg;
        }
    }
}
