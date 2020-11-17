using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using kuras.Data;
using kuras.Models;
//For the file upload
using System.IO;
using Microsoft.AspNetCore.Hosting;
//Excel
using ClosedXML.Excel;
using kuras.Services;

namespace kuras.Controllers
{

    public class KilometersController : Controller
    {
        private readonly KilometersService _kilometersService;
        private readonly IWebHostEnvironment webhostEnvironment;

        public KilometersController(KilometersService kilometersService, IWebHostEnvironment webhostEnvironment)
        {
            _kilometersService = kilometersService;
            this.webhostEnvironment = webhostEnvironment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFileData([FromBody] KilometersInsert model)
        {
            // if (!ModelState.IsValid) return NotFound();
            // if (model == null) return NotFound();

            // if (!Int32.TryParse(model.GpsId, out int gpsId) || !Int32.TryParse(model.OdoId, out int odoId)) return NotFound();
            var odoFile = _kilometersService.Get(model.OdoId);
            var gpsFile = _kilometersService.Get(model.GpsId);

            if (gpsFile == null || odoFile == null) return NotFound();

            List<Kilometers> FileData = await GetComparedList(await MergeOdoGps(gpsFile.FileName, odoFile.FileName));

            if (FileData == null) return NotFound();

            foreach (Kilometers item in FileData)
                _context.Add(item);

            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(TableView));

        }
        private async Task<List<Kilometers>> GetComparedList(List<Kilometers> MergedList)
        {
            if (MergedList == null) return null;
            foreach (Kilometers e in MergedList)
                if (await _context.Kilometers.AnyAsync(
                    x => x.NumberPlate == e.NumberPlate && x.Month == e.Month && x.Year == e.Year)
                   )
                    MergedList.Remove(e);
            return MergedList;
        }
        private async Task<List<Kilometers>> MergeOdoGps(string fileNameGps, string fileNameOdo)
        {
            List<GpsKilometers> GpsList = GetGpsFileData(fileNameGps);
            List<Kilometers> OdoList = await GetOdoFileData(fileNameOdo);
            foreach(var odoItem in OdoList)
            {
                GpsKilometers gpsItem = GpsList.FirstOrDefault(x => x.NumberPlate == odoItem.NumberPlate);
                if (gpsItem != null)
                    odoItem.KmGps = gpsItem.km;
                
            }
            return OdoList;
        }
        private List<GpsKilometers> GetGpsFileData(string FileName)
        {
            List<GpsKilometers> FileData = new List<GpsKilometers>();

            if (FileName == null) return null;
            IXLWorksheet WorkSheet = GetWorksheet(FileName, "kilometers");
            var dataRow = WorkSheet.Row(6).RowUsed();
            int device = 1;
            int kms = 2;

            while (!dataRow.Cell(device).IsEmpty())
            {
                string dname = dataRow.Cell(device).GetString();
                bool kmb = dataRow.Cell(kms).TryGetValue(out float tempkm);
                dname = dname.Trim(' ');
                var splitd = dname.Split(' ');
                for (int i = 0; i < splitd.Length; i++)
                {
                    splitd[i] = splitd[i].Trim(' ');
                }
                string np = "";
                string name = "";
                switch (splitd.Length)
                {
                    case 1:
                        np = splitd[0];
                        break;
                    case 2:
                        if (splitd[0].Length == 3 && splitd[1].Length == 3)
                            np = splitd[0] + splitd[1];
                        else if (splitd[0].Length == 6)
                        {
                            np = splitd[0];
                            name = splitd[1];
                        }
                        break;
                    case 3:
                        if (splitd[0].Length == 3)
                        {
                            np = splitd[0] + splitd[1];
                            name = splitd[2];
                        }
                        else if (splitd[0].Length == 6)
                        {
                            np = splitd[0];
                            name = splitd[1] + "_" + splitd[2];
                        }
                        break;
                    case 4:
                        if (splitd[0].Length == 3 && splitd[1].Length == 3)
                        {
                            np = splitd[0] + splitd[1];
                            name = splitd[2] + "_" + splitd[3];
                        }
                        break;
                    default:
                        np = new System.Text.StringBuilder("").AppendFormat("length: {0} ", splitd.Length).ToString();
                        name = "n/a";
                        break;
                }

                if (kmb)
                {
                    FileData.Add(new GpsKilometers
                    {
                        NumberPlate = np,
                        km = tempkm
                    });
                }
                dataRow = dataRow.RowBelow();
            }
            
            //if it is empty, empty list is returned
            return FileData;
        }
        private IXLWorksheet GetWorksheet(string fileName, string folderName)
        {
            string uploadsFolder = Path.Combine(webhostEnvironment.WebRootPath, folderName);
            string filePath = Path.Combine(uploadsFolder, fileName);
            XLWorkbook wb = new XLWorkbook(filePath);
            IXLWorksheet ws = wb.Worksheet(1);
            return ws;
        }
        private async Task<List<Kilometers>> GetOdoFileData(
            string FileName)
        {
            var FileData = new List<Kilometers>();
            if (FileName == null) return FileData;

            // Get a specified worksheet with data
            IXLWorksheet CurrentWorkSheet = GetWorksheet(FileName, "kilometers");

            #region worksheet cell numbers
            // where data starts (worksheet cell numbers)
            var cell_numberplate = 5;
            var cell_period = 3;
            var datastart = 6;
            var dataRow = CurrentWorkSheet.Row(datastart).RowUsed();
            // odo kilometers
            var start = 44;
            var end = 45;

            // Diesel 22 / 23
            // gasoline 30 / 31
            var dieselStart = 22;
            var dieselEnd = 23;
            var gasolineStart = 30;
            var gasolineEnd = 31;
            #endregion

            var CarList = await _context.Car.ToListAsync();

            // While the cell that holds the numberPlate is not empty (until the end of the list)
            string lastNP = null;
            Car carFound = null;
            Kilometers RowObject = new Kilometers();
            while (!dataRow.Cell(cell_numberplate).IsEmpty())
            {
                // Every cycle RowObject get nullified
                RowObject = null;


                string temp_NumberPlate = dataRow.Cell(cell_numberplate).GetString();
                if (lastNP != temp_NumberPlate)
                {
                    lastNP = temp_NumberPlate;
                    carFound = CarList.Find(x => x.NumberPlate == temp_NumberPlate);
                }
                if (carFound != null)
                {
                    // benzoline and diesel
                    bool ds = dataRow.Cell(dieselStart).TryGetValue(out float dStart);
                    bool de = dataRow.Cell(dieselEnd).TryGetValue(out float dEnd);
                    bool bs = dataRow.Cell(gasolineStart).TryGetValue(out float bStart);
                    bool be = dataRow.Cell(gasolineEnd).TryGetValue(out float bEnd);
                    // date/period
                    string temp_date = dataRow.Cell(cell_period).GetString();
                    // if all values were parsed 
                    if (ds && de && bs && be)
                    {
                        string[] dateSplit = temp_date.Split('-');
                        if (dateSplit.Length == 2)
                        {
                            for (int i = 0; i < dateSplit.Length; i++)
                                dateSplit[i] = dateSplit[i].Trim(' ');

                            var yearb = Int32.TryParse(dateSplit[0], out int year);
                            var monthb = Int32.TryParse(dateSplit[1], out int month);

                            if (yearb && monthb)
                            {
                                if (dStart != 0 || dEnd != 0)
                                    RowObject = new Kilometers
                                    {
                                        Month = month,
                                        Year = year,
                                        NumberPlate = temp_NumberPlate,
                                        StartFuel = dStart,
                                        EndFuel = dEnd,
                                        Car = carFound.Id
                                    };
                                else if (bStart != 0 || bEnd != 0)
                                    RowObject = new Kilometers
                                    {
                                        Month = month,
                                        Year = year,
                                        NumberPlate = temp_NumberPlate,
                                        StartFuel = bStart,
                                        EndFuel = bEnd,
                                        Car = carFound.Id
                                    };
                                else
                                {
                                    RowObject = new Kilometers
                                    {
                                        Month = month,
                                        Year = year,
                                        NumberPlate = temp_NumberPlate,
                                        StartFuel = 0,
                                        EndFuel = 0,
                                        Car = carFound.Id
                                    };
                                }
                            }
                        }
                    }
                    // odo kilometers
                    bool startb = dataRow.Cell(start).TryGetValue(out float temp_km_start);
                    bool endb = dataRow.Cell(end).TryGetValue(out float temp_km_end);
                    if (startb && endb)
                    {
                        RowObject.StartOdometer = temp_km_start;
                        RowObject.EndOdometer   = temp_km_end;
                    }
                    FileData.Add(RowObject);
                }
                // next row
                dataRow = dataRow.RowBelow();
            }
            // end of while          
            //if it is empty, empty list is returned
            return FileData;
        }


        public async Task<IActionResult> ShowGps(int? id)
        {
            //if no such id
            if (id == null)
            {
                return NotFound();
            }

            var kilometersFile = await _context.KilometersFile
                .FirstOrDefaultAsync(m => m.Id == id);

            //if no data in that id 
            if (kilometersFile == null)
            {
                return NotFound();
            }

            if (kilometersFile.FileName != null)
            {
                var FileData = ReturnFileDataGps(kilometersFile.FileName);
                ViewBag.FileName = kilometersFile.FileName;
                //if file is retrieved, its view is returned
                return View(FileData);
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> ShowOdo(int? id)
        {
            //if no such id
            if (id == null)
            {
                return NotFound();
            }

            var kilometersFile = await _context.KilometersFile
                .FirstOrDefaultAsync(m => m.Id == id);

            //if no data in that id 
            if (kilometersFile == null)
            {
                return NotFound();
            }

            if (kilometersFile.FileName != null)
            {
                var FileData = ReturnFileDataOdo(kilometersFile.FileName);
                ViewBag.FileName = kilometersFile.FileName;
                //if file is retrieved, its view is returned
                return View(FileData);
            }
            else
            {
                return NotFound();
            }
        }
        public async Task<IActionResult> ChooseFiles()
        {
            var files = await _context.KilometersFile.ToListAsync();
            return View(files);
        }
        
        public async Task<IActionResult> TableView()
        {
            return View(await _context.Kilometers.ToListAsync()); 
        }
        public List<ViewModels.KilometersOdoFileViewModel> ReturnFileDataOdo(string FileName)
        {
            List<ViewModels.KilometersOdoFileViewModel> FileData = new List<ViewModels.KilometersOdoFileViewModel>();

            if (FileName != null)
            {
                string uploadsFolder = Path.Combine(webhostEnvironment.WebRootPath, "kilometers");
                string filePath = Path.Combine(uploadsFolder, FileName);
                var wb = new XLWorkbook(filePath);
                var ws = wb.Worksheet(1);
                var numberplate = 5;
                var period = 3;
                var start = 44;
                var end = 45;
                var datastart = 6;
                var dataRow = ws.Row(datastart).RowUsed();

                List<ViewModels.KilometersOdoFileViewModel> Devices = new List<ViewModels.KilometersOdoFileViewModel>();

                while (!dataRow.Cell(numberplate).IsEmpty())
                {
                    string tempp = dataRow.Cell(period).GetString();
                    string tempnb = dataRow.Cell(numberplate).GetString();
                    double temps = dataRow.Cell(start).GetDouble();
                    double tempe = dataRow.Cell(end).GetDouble();

                    Devices.Add(new ViewModels.KilometersOdoFileViewModel
                    {
                        Period = tempp,
                        NumberPlate = tempnb,
                        Start = temps,
                        End = tempe
                    }); 
                   

                    dataRow = dataRow.RowBelow();

                }
                FileData = Devices;
            }
            //its view is returned  
            //if it is empty, empty list is returned
            return FileData;
        }
        public List<ViewModels.KilometersGpsFileViewModel> ReturnFileDataGps(string FileName)
        {
            List<ViewModels.KilometersGpsFileViewModel> FileData = new List<ViewModels.KilometersGpsFileViewModel>();

            if (FileName != null)
            {
                string uploadsFolder = Path.Combine(webhostEnvironment.WebRootPath, "kilometers");
                string filePath = Path.Combine(uploadsFolder, FileName);
                var wb = new XLWorkbook(filePath);
                var ws = wb.Worksheet(1);
                var device = 1;
                var kms = 2;
                var dataRow = ws.Row(6).RowUsed();

                List<ViewModels.KilometersGpsFileViewModel> Devices = new List<ViewModels.KilometersGpsFileViewModel>();

                while(!dataRow.Cell(device).IsEmpty())
                {
                    string dname = dataRow.Cell(device).GetString();
                    bool kmb = dataRow.Cell(kms).TryGetValue(out double tempkm);
                    dname = dname.Trim(' ');
                    var splitd = dname.Split(' ');
                    for(int i = 0; i < splitd.Length; i++)
                    {
                        splitd[i] = splitd[i].Trim(' ');
                    }
                    string np = "";
                    string name = "";
                    switch(splitd.Length)
                    {
                        case 1:
                            np = splitd[0];
                            break;
                        case 2:
                            if (splitd[0].Length == 3 && splitd[1].Length == 3)
                                np = splitd[0] + splitd[1];
                            else if(splitd[0].Length == 6)
                            {
                                np = splitd[0];
                                name = splitd[1];
                            }
                            break;
                        case 3:
                            if(splitd[0].Length == 3)
                            {
                                np = splitd[0] + splitd[1];
                                name = splitd[2];
                            }
                            else if(splitd[0].Length == 6)
                            {
                                np = splitd[0];
                                name = splitd[1] + "_" + splitd[2];
                            }
                            break;
                        case 4:
                            if(splitd[0].Length == 3 && splitd[1].Length == 3)
                            {
                                np = splitd[0] + splitd[1];
                                name = splitd[2] + "_" + splitd[3];
                            }
                            break;
                        default:
                            np = new System.Text.StringBuilder("").AppendFormat("length: {0} ", splitd.Length).ToString();
                            name = "n/a";
                            break;
                    }

                    if(kmb)
                    {
                        Devices.Add(
                            new ViewModels.KilometersGpsFileViewModel
                            {
                                DeviceName = dname,
                                DeviceKm = tempkm,
                                Name = name,
                                NumberPlate = np
                            });
                    }
                    dataRow = dataRow.RowBelow();               
                }
                FileData = Devices;
            }
            //its view is returned  
            //if it is empty, empty list is returned
            return FileData;
        }
        
        
    }
}
