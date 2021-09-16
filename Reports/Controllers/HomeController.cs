using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reports.DAL;
using Reports.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        /**
         * 
         **/
        public async Task<IActionResult> Details(string id)
        {
            // build the params for input fields
            List<Param> items = await _context.Params
                .Where(m => m.itemId == id)
                .Select(l => new Param()
                {
                    type = l.type,
                    value = l.value,
                    defaults = l.defaults
                })
                .ToListAsync();
            
            // in case id query param does not exists, we will use 0 as default
            ViewBag.queryId = !String.IsNullOrEmpty(id) ? id : "0";

            return View("Details", items);
        }

        /**
         * 
         **/
        [HttpPost]
        public IActionResult Export(IFormCollection form)
        {
            // get the query from sqlite
            Item result = _context.Items
                 .Where(m => m.id == form["itemId"].ToString())
                 .Select(l => new Item()
                 {
                     query = l.query
                 })
                 .FirstOrDefault();

            // from external db, create a conection and execute the query given from sqlite
            var connection = _mainContext.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandText = result.query.ToString();

            // map parameters from inputs
            foreach (var item in form)
            {
                SqlParameter param = new SqlParameter();
                param.ParameterName = item.Key;
                param.Value = item.Value[0].ToString();
                command.Parameters.Add(param);
            }

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
