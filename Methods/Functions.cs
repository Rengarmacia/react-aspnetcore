using kuras.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace kuras.Methods
{
    public class Functions
    {
        public static bool UploadFile(ref UploadFile file, string folderName, string webroot)
        {
            // If file is not empty/was selected
            if (file.File != null)
            {
                string uniqueFileName, filePath;
                string uploadsFolder = Path.Combine(webroot, folderName);
                //Debug.WriteLine("Upload Folder: " + uploadsFolder);
                Guid unique = Guid.NewGuid();

                uniqueFileName = unique + "%%%" + file.File.FileName;
                filePath = Path.Combine(uploadsFolder, uniqueFileName);
                file.UploadedFileName = uniqueFileName;
                try
                {
                    //Create a file
                    var fileCreator = new FileStream(filePath, FileMode.Create);
                    file.File.CopyTo(fileCreator);
                    fileCreator.Close();
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
