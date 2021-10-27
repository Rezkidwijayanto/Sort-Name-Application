using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sort_Name_Application.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Sort_Name_Application.Controllers
{
    public class HomeController : Controller
    {
       
        private IWebHostEnvironment Environment;
        static List<DataRow> sortlist = new List<DataRow>();
        public HomeController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
           
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Download()
        {
            var memStream = new MemoryStream();
            var streamWriter = new StreamWriter(memStream);
            foreach (DataRow dr in sortlist)
            { 
                streamWriter.WriteLine(dr["FullName"]);
            }
            streamWriter.Flush();                                 
            memStream.Seek(0, SeekOrigin.Begin);
            byte[] bytesInStream = memStream.ToArray(); // simpler way of converting to array
            memStream.Close();
            return File(bytesInStream, "text/plain", "SortNameByLastName.txt");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(IFormFile postedFiles)
        {

            DataTable SortName = new DataTable();
           
            SortName.Columns.Add("FullName");
            SortName.Columns.Add("LastName");
            
           
            string data;
            using (var fileStram = postedFiles.OpenReadStream())
            {
                using (var reader = new StreamReader(fileStram))
                {
                    while ((data=reader.ReadLine()) != null)
                    {
                        string[] datasplit;
                        var dr = SortName.NewRow();
                        dr["FullName"] = data;
                        datasplit = data.Split(" ");
                        dr["LastName"] = datasplit[datasplit.Length - 1];
                       SortName.Rows.Add(dr);
                    }
                }
            }

            SortName.DefaultView.Sort = "LastName ASC";
            SortName = SortName.DefaultView.ToTable();
            ViewBag.datasource = SortName;
            sortlist = SortName.AsEnumerable().ToList<DataRow>();
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
