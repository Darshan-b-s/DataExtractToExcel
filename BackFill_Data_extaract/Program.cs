using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;

namespace SQLServerToExcel
{
    class Program
    {
        public class ServerInfo
        {
            
            public string Username { get; set; }
            public string Password { get; set; }
            public string Database { get; set; }
            public string storenum { get; set; }
            public string ServerName { get; set; }
        }

        static void Main(string[] args)
        {
            List<string> storeslist = new List<string>();
            storeslist =GetStoreListFromCentralServer();

            // Define a list of SQL server connection strings
            List<ServerInfo> server_Details = new List<ServerInfo>();
            server_Details = PrepareServerInfo(storeslist);
            using (var workbook = new XLWorkbook())
            {
                foreach (var connectionInfo in server_Details)
                {
                    try
                    {
                        using (var connection = new SqlConnection($"Server={connectionInfo.ServerName}\\sqlexpress;Database={connectionInfo.Database};User Id={connectionInfo.Username};Password={connectionInfo.Password};"))
                        {
                            connection.Open();

                            // Define your SQ queryselect
                            var query = "select locn.Location,item.Keycode, item.ScanDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'LocationAdd'and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by keycode, Location desc";

                            using (var adapter = new SqlDataAdapter(query, connection))
                            {
                                var dataTable = new DataTable();
                                adapter.Fill(dataTable);

                                // Create a worksheet for this server's data
                                var worksheet = workbook.Worksheets.Add(connectionInfo.storenum);
                                worksheet.Cell(1, 1).InsertTable(dataTable);
                            }
                        }
                        workbook.SaveAs("/Users/dbs/Downloads/backfilltest1.xlsx");

                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    
                }

                // Save the Excel file

            }

            Console.WriteLine("Data exported to output.xlsx");
        }
        //to get list of stores
        public static List<string> GetStoreListFromCentralServer()
        {
            try
            {
                var Server = "kwtsql302";
                var Database = "PDTAPP";
                var User = "PDTAppUsr";
                var Password = "pDT@ppU$3%r!";

                var StoreList = new List<string>();
                using (var connection = new SqlConnection($"Server={Server};Database={Database};User Id={User};Password={Password};"))
                {
                    connection.Open();
                    using (var command = new SqlCommand("usp_Get_Store_Servers", connection))
                    {

                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            int i = 0;
                            while (reader.Read())
                            {

                                StoreList.Add(reader[0].ToString());
                                i++;
                            }

                        }
                    }
                    connection.Close();
                }

                return StoreList;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in getting Store List");
                return null;
            }
        }
            
        //to make server list
        public static List<ServerInfo> PrepareServerInfo(List<string> storesList)
        {
            string Username = "KMTStoreSql";
            string Password = "9@meS4Cc*";
            string DataBase= "PDTDB";
            List<ServerInfo> serverInfos = new List<ServerInfo>();

            foreach (var serverName in storesList)
            {
                var serverInfo = new ServerInfo
                {
                    Username = Username,
                    Password = Password,
                    Database = DataBase,
                    storenum = serverName.Substring(0, 6),
                    ServerName = serverName
                };

                serverInfos.Add(serverInfo);
            }
            return serverInfos;
        }
    }
}


