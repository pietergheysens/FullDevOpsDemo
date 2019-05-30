﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Text;

namespace aspnet_core_dotnet_core.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration config)
        {
            this.configuration = config;
        }

        public IActionResult Index()
        {
            List<Class1> ObjCustomer = new List<Class1>();
            try
            {
                String constring = configuration.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(constring))
                {
                    connection.Open();

                    Submit_Tsql_NonQuery(connection, "3 - Inserts",
                       Build_3_Tsql_Inserts("Home"));


                    ObjCustomer = Submit_6_Tsql_SelectEmployees(connection);


                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            return View(ObjCustomer.ToList());

        }

        private List<Class1> Submit_6_Tsql_SelectEmployees(SqlConnection connection)
        {

            string tsql = Build_6_Tsql_SelectEmployees();
            List<Class1> ObjCustomer = new List<Class1>();

            using (var command = new SqlCommand(tsql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String[] formats = { @"MM\/dd\/yyyy HH:mm:ss" };

                        string PageName = (string)reader["PageName"];
                        DateTime AccessDate = ((DateTime)reader["AccessDate"]);
                        int count = (int)reader["ID_column"];
                        String date = AccessDate.ToString(formats[0]);

                        Class1 Obj = new Class1();
                        Obj.PageName = PageName;
                        Obj.AccessDate = date;
                        Obj.Visits = count;
                        ObjCustomer.Add(Obj);


                    }
                }
            }
            return ObjCustomer;
        }

        private string Build_6_Tsql_SelectEmployees()
        {
            return @"
                               
                    SELECT TOP 1 * 
                    FROM(
                        SELECT TOP 2 * 
                        FROM accessLogs 
                        ORDER BY AccessDate DESC -- here you need to pass column which will provide expected sorting
                        ) t                     
                    ORDER BY AccessDate  ;
                ";
        }

        private string Build_3_Tsql_Inserts(String page)
        {

            return @"
                    
                    INSERT INTO accessLogs
                       (PageName)
                          VALUES
                       ('" + page + "');";
        }

        private void Submit_Tsql_NonQuery(SqlConnection connection,
         string tsqlPurpose,
         string tsqlSourceCode,
         string parameterName = null,
         string parameterValue = null)
        {

            {
                Console.WriteLine();
                Console.WriteLine("=================================");
                Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

                using (var command = new SqlCommand(tsqlSourceCode, connection))
                {
                    if (parameterName != null)
                    {
                        command.Parameters.AddWithValue(  // Or, use SqlParameter class.
                           parameterName,
                           parameterValue);
                    }
                    int rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine(rowsAffected + " = rows affected.");
                }
            }
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            try
            {

                string constring = configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(constring))
                {
                    connection.Open();

                    Submit_Tsql_NonQuery(connection, "3 - Inserts",
                       Build_3_Tsql_Inserts("about"));
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }



            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            try
            {

                string constring = configuration.GetConnectionString("DefaultConnection");

                using (var connection = new SqlConnection(constring))
                {
                    connection.Open();

                    Submit_Tsql_NonQuery(connection, "3 - Inserts",
                       Build_3_Tsql_Inserts("contact"));
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }


            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
