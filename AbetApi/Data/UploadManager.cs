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

        //receives file from the request and stores it in the Uploads folder
        public async void ReceiveFile(HttpRequest request)
        {
            try
            {
                IFormCollection formCollection = await request.ReadFormAsync();
                IFormFile file = formCollection.Files.First();

                if (file.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

                    //create Uploads folder if it does not exist
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    FilePath = Path.Combine(folderPath, fileName);

                    /* This will restrict the uploaded file type to only .accdb
                    if (Path.GetExtension(FilePath) != ".accdb")
                    {
                        errorMsg = "Error: File is not an access database file (.accdb).";
                        return;
                    }
                    */

                    using (FileStream stream = new FileStream(FilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
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
