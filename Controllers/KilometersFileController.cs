using kuras.Models;
using kuras.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using kuras.Methods;
using System.Diagnostics;
using System.IO;

namespace kuras.Controllers
{
    // accessible when logged in
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KilometersFileController : ControllerBase
    {
        private readonly KilometersFileService _carFileService;
        private readonly IWebHostEnvironment _webhostEnvironment;
        public KilometersFileController(KilometersFileService carFileService, IWebHostEnvironment webhostEnvironment)
        {
            _carFileService = carFileService;
            _webhostEnvironment = webhostEnvironment;
        }
        // Get all filenames and paths
        [HttpGet]
        public ActionResult<List<KilometersFile>> Get() =>
            _carFileService.Get();

        [HttpGet("{id:length(24)}", Name = "GetKilometersFile")]
        public ActionResult<KilometersFile> Get(string id)
        {
            var carFile = _carFileService.Get(id);

            if (carFile == null)
            {
                return NotFound();
            }

            return carFile;
        }


        // Insert a new file
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult<KilometersFile> Create([FromForm] UploadFile uploadFile)
        {
            string contentroot = _webhostEnvironment.ContentRootPath;
            contentroot = Path.Combine(contentroot, "Files");
            bool isUploaded = Functions.UploadFile(ref uploadFile, "Kilometers", contentroot);
            if(isUploaded)
            {
                KilometersFile carFile = new KilometersFile(uploadFile);
                carFile = _carFileService.Create(carFile);
                return CreatedAtRoute("GetKilometersFile", new { id = carFile.Id.ToString() }, carFile);
            }
            return BadRequest();
        }



        // public async Task DeleteCorrupted()
        // {
        //     List<KilometersFile> kmFiles = await _context.KilometersFile.ToListAsync();
        //     foreach (var kmFile in kmFiles)
        //     {
        //         string uploadsFolder = Path.Combine(webhostEnvironment.WebRootPath, "kilometers");
        //         string filePath = Path.Combine(uploadsFolder, kmFile.FileName);
                
        //         if (!System.IO.File.Exists(filePath))
        //         {
        //             // If the file does not exist, remove its record
        //             _context.KilometersFile.Remove(kmFile);
        //         }

        //     }
        //     await _context.SaveChangesAsync();
        // }



        
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    //if no such id
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kilometersFile = await _context.KilometersFile
        //        .FirstOrDefaultAsync(m => m.Id == id);

        //    //if no data in that id 
        //    if (kilometersFile == null)
        //    {
        //        return NotFound();
        //    }                  
            
        //    return View(kilometersFile);                                   
        //}
        //// POST: Kilometers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
            
        //    var kilometersFile = await _context.KilometersFile.FindAsync(id);
        //    string webroot = webhostEnvironment.WebRootPath;
        //    string folderName = "kilometers";
        //    string folderPath = Path.Combine(webroot, folderName);
        //    bool wasDeleted = kuras.Methods.GlobalMethods.DeleteFile(kilometersFile.FileName, folderPath);
        //    if(wasDeleted)
        //    {
        //        _context.KilometersFile.Remove(kilometersFile);
        //        await _context.SaveChangesAsync();
        //    }               
        //    return RedirectToAction(nameof(Index));
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadGps([Bind("Id, gsReport")] kuras.ViewModels.FileUploadViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string webroot = webhostEnvironment.WebRootPath;
        //        GasStationFile fileObj = Methods.GlobalMethods.UploadFile(model, "kilometers", webroot);
        //        KilometersFile kmFile = new KilometersFile
        //        {
        //            FileName = fileObj.FileName,
        //            Uploaded = false,
        //            Gps = true
        //        };
        //        if (kmFile != null)
        //        {
        //            _context.Add(kmFile);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("ShowGps", new { id = kmFile.Id });
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadOdo([Bind("Id, gsReport")] kuras.ViewModels.FileUploadViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string webroot = webhostEnvironment.WebRootPath;
        //        GasStationFile fileObj = Methods.GlobalMethods.UploadFile(model, "kilometers", webroot);
        //        KilometersFile kmFile = new KilometersFile
        //        {
        //            FileName = fileObj.FileName,
        //            Uploaded = false,
        //            Gps = false
        //        };
        //        if (kmFile != null)
        //        {
        //            _context.Add(kmFile);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction("ShowOdo", new { id = kmFile.Id });
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}
    }
}