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
        public IActionResult Details()
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

            var result = new List<dynamic>();
            foreach (DataRow row in dataTable.Rows)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                foreach (DataColumn col in dataTable.Columns)
                {
                    obj.Add(col.ColumnName, row[col.ColumnName]);
                }
                result.Add(obj);
            }
            connection.Close();

            return View(dataTable);
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
    }
}
