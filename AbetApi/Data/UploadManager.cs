using AbetApi.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static AbetApi.Models.AbetModels;

namespace AbetApi.Data
{
    public class UploadManager : IUploadManager
    {
        public string FileId { get; set; }
        public string FilePath { get; set; } = null;
        public string ErrorMessage { get; set; }
        public string OriginalFileName { get; set; }
        public bool FileNotFound { get; set; } = false;
        private readonly string FOLDER_PATH = Path.Combine(Directory.GetCurrentDirectory(), "Uploads"); //Path to the Uploads folder

        //stores a received file in the Uploads folder
        public bool StoreFile(IFormFile file, List<string> acceptableTypes)
        {
            try
            {
                //check if file type is acceptable
                if (!acceptableTypes.Contains(Path.GetExtension(file.FileName)))
                {
                    ErrorMessage = "Error: Unexpected file type.";
                    return false;
                }
                
                if (file.Length > 0)
                {
                    //create Uploads folder if it does not exist
                    if (!Directory.Exists(FOLDER_PATH))
                        Directory.CreateDirectory(FOLDER_PATH);

                    OriginalFileName = Path.GetFileName(file.FileName);
                    FileId = GenerateFileName(); //generate a unique file name
                    FilePath = Path.Combine(FOLDER_PATH, FileId + Path.GetExtension(file.FileName));

                    using (FileStream stream = new FileStream(FilePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        stream.Flush();
                    }

                    return true;
                }
                else
                {
                    ErrorMessage = "Error: File contains no data.";
                    return false;
                }
            }
            catch
            {
                ErrorMessage = "Internal Server Error: Please try again later.";
                return false;
            }
        }

        //will delete the most recently uploaded file within the same request
        public bool DeleteCurrentFile()
        {
            if (!Directory.Exists(FOLDER_PATH) || FilePath == null || !File.Exists(FilePath))
            {
                ErrorMessage = "Error: File not found.";
                FileNotFound = true;
                return false;
            }
            else
            {
                File.Delete(FilePath);
                return true;
            }
        }

        //will delete a file based on a provided path
        public bool DeleteFile(string filePath)
        {
            if (!Directory.Exists(FOLDER_PATH) || !File.Exists(filePath))
            {
                ErrorMessage = "Error: File not found.";
                FileNotFound = true;
                return false;
            }
            else
            {
                File.Delete(filePath);
                return true;
            }
        }

        public FileStream GetFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    FileStream file = File.OpenRead(filePath);

                    return file;
                }
                catch
                {
                    ErrorMessage = "Internal Server Error: Error reading the file.";
                    return null;
                }
            }
            else
            {
                ErrorMessage = "Error: File not found.";
                FileNotFound = true;
                return null;
            }
        }

        private string GenerateFileName()
        {
            string randomString = Guid.NewGuid().ToString().Replace("-", "");

            //if Uploads folder does not exist, then this is the first upload
            if (!Directory.Exists(FOLDER_PATH))
                return randomString;

            HashSet<string> fileNames = Directory.GetFiles(FOLDER_PATH).Select(file => Path.GetFileNameWithoutExtension(file)).ToHashSet();
            
            //ensure file name is not taken
            while (fileNames.Contains(randomString))
                randomString = Guid.NewGuid().ToString().Replace("-", "");

            return randomString;
        }
    }
}
