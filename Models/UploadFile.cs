using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kuras.Models
{
    public class UploadFile
    {
        public bool Condition { get; set; }
        public IFormFile File { get; set; }
        public string UploadedFileName { get; set; }
    }
}
