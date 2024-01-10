using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using BackFill_Data_extaract;
using ClosedXML.Excel;
using Microsoft.Extensions.Configuration;

namespace SQLServerToExcel
{
class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        var config = builder.Build();
        string dbcontext = config.GetConnectionString("StrPDTDBContext");

        List<string> storeslist = new List<string>();
        List<SqlQuery> queryList = new List<SqlQuery>();
        List<ServerInfo> server_Details = new List<ServerInfo>();
        int errorcount=0;
        int errorrlogginrow = 2;
        var error_workbook = new XLWorkbook();
        var Error_worksheet = error_workbook.Worksheets.Add("Error");
        Error_worksheet.FirstCell().Value = "Store_ID";
        //GetList of stores to connect with
        storeslist = PopulatingData.GetStoreListFromCentralServer();
        // Define a list of SQL server connection strings
        server_Details = PopulatingData.PrepareServerInfo(storeslist);
        // get list of queries to execute
        queryList = PopulatingData.GetListofQuires();

        foreach (var connectionInfo in server_Details)
        {
            var workbook = new XLWorkbook();
            foreach (var query in queryList)
            {
                var worksheet = workbook.Worksheets.Add(query.QueryName);

                try
                {

                    var dataTable = new DataTable();
                    try
                    {
                        //string conn = ConfigurationManager.AppSettings["ConnectionStrings: PDTAPPConnection"].ToString();
                        using (var connection = new SqlConnection($"Server={connectionInfo.ServerName}\\sqlexpress;Database={connectionInfo.Database};User Id={connectionInfo.Username};Password={connectionInfo.Password};"))
                        {
                            connection.Open();

                            using (var adapter = new SqlDataAdapter(query.Query, connection))
                            {
                                adapter.Fill(dataTable);
                            }
                            connection.Close();
                            worksheet.Cell(1, 1).InsertTable(dataTable);

                        }
                        Console.WriteLine(connectionInfo.storenum + ":- Passed");
                    }
                    catch (Exception ex)
                    {

                        Error_worksheet.Cell(errorrlogginrow, 1).Value = connectionInfo.storenum;
                        Error_worksheet.Cell(errorrlogginrow, 2).Value = query.QueryName;
                        errorrlogginrow++;
                        errorcount++;
                        error_workbook.SaveAs("/Users/dbs/Downloads/errorinbackfill.xlsx");
                        Console.WriteLine(connectionInfo.storenum + ":- Failed");
                    }
                    //error_workbook.SaveAs("/Users/dbs/Downloads/Test.xlsx");
                    //bool result = CopytoServer(dataTable);
                    //Console.WriteLine("Writing to central server " + connectionInfo.storenum + (result ? " passed" : " falied"));
                    //Console.WriteLine("Remaining: " + (storecount)+ " Success: " + (successcount)+ " Error: " + (errorcount)+" ...");

                }
                catch (Exception ex)
                {
                    continue;
                }

            }
            workbook.SaveAs($"/Users/dbs/Downloads/{connectionInfo.ServerName}.xlsx");
                

        }

        Console.WriteLine("Data exported ");
        Console.ReadKey();
    }
        
        
}
}


