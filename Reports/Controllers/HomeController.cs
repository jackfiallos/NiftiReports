using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reports.DAL;
using Reports.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Reports.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDBContext _context;
        private readonly MainContext _mainContext;

        /**
         * 
         **/
        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context, MainContext mainContext)
        {
            _context = context;
            _mainContext = mainContext;
            _logger = logger;
        }

        /**
         * 
         **/
        public async Task<IActionResult> Index(string id)
        {
            List<Item> items = await _context.Items
                .Where(m => m.parentId == id)
                .Select(l => new Item()
                {
                    id = l.id,
                    name = l.name,
                    children = _context.Items.Where(o => o.parentId == l.id).Count()
                })
                .ToListAsync();
            return View("Index", items);
        }

        public IActionResult Export()
        {
            var connection = _mainContext.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM cars WHERE CAST(created_at AS DATE) = @P0";

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@P0";
            param.Value = "2021-08-29";

            command.Parameters.Add(param);
            connection.Open();
            var dataReader = command.ExecuteReader();

            var dataTable = new DataTable();
            dataTable.Load(dataReader);

            MemoryStream stream = ToCSV(dataTable);
            return File(stream.ToArray(), "text/csv", "data.csv");
        }

        /**
         * 
         **/
        public async Task<IActionResult> Details(string id)
        {
            List<Param> items = await _context.Params
                .Where(m => m.itemId == id)
                .Select(l => new Param()
                {
                    name = l.name,
                    value = l.value
                })
                .ToListAsync();

            return View("Index", items);
        }

        /**
         * 
         **/
        public IActionResult Privacy()
        {
            return View();
        }

        /**
         * 
         **/
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static MemoryStream ToCSV(DataTable dtDataTable)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter objstreamwriter = new StreamWriter(stream);

                //headers  
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    objstreamwriter.Write(dtDataTable.Columns[i]);
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        objstreamwriter.Write(",");
                    }
                }
                objstreamwriter.Write(objstreamwriter.NewLine);
                //rows
                foreach (DataRow dr in dtDataTable.Rows)
                {
                    for (int i = 0; i < dtDataTable.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[i]))
                        {
                            string value = dr[i].ToString();
                            if (value.Contains(','))
                            {
                                value = String.Format("\"{0}\"", value);
                                objstreamwriter.Write(value);
                            }
                            else
                            {
                                objstreamwriter.Write(dr[i].ToString());
                            }
                        }
                        if (i < dtDataTable.Columns.Count - 1)
                        {
                            objstreamwriter.Write(",");
                        }
                    }
                    objstreamwriter.Write(objstreamwriter.NewLine);
                }

                objstreamwriter.Flush();
                objstreamwriter.Close();

                return stream;
            }

        }
    }
}
