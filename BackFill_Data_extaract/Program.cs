using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        public class SqlQuery
        {
            public string QueryName { get; set; }
            public string Query { get; set; }
        }
        
        public const string SF_Locations= "select grp.Str_ID,locn.Location,item.Keycode, item.ScanDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'LocationAdd'and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location  desc";
        public const string SR_Locations = "select grp.Str_ID, locn.Location,item.Keycode, item.ScanDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'StockroomAdd' and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location desc";
        public const string backfill_date = "select grp.Str_ID,locn.Location,item.Keycode, grp.Group_EndDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'StockroomBackfill' and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location desc";
        public const string Take_quantity = "select grp.Str_ID,locn.Location,item.Keycode,item.ScanCode, item.ScanDate,item.QTY from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'StockroomBackfill' and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location desc";
        public const string Clearance_flag = "Select  C_KD_STAT, F_KD_VOLUME_LINE,  F_KD_SHELF_READY, F_KD_RAINCHECK,  F_KD_RFID,   F_KD_PR, M_KD FROM   dksls01.dbo.SLT200 where C_KD_STAT=3 order by M_KD";
        public const string SmartInv_Audit = "select SmartInv_Audit_ID,SUBSTRING(@@SERVERNAME, 3, 4) as Store_ID, M_STK_RM_LOCN, M_KD, C_Process, TS_INSRT from [SmartInv_Audit] where YEAR(TS_INSRT) = YEAR(GETDATE())";
        public const string SmartInv_AuditLog = "select SmartInv_AuditLog_ID,SUBSTRING(@@SERVERNAME, 3, 4) as Store_ID, M_STK_RM_LOCN, M_KD, C_Process, TS_INSRT from [SmartInv_AuditLog] where YEAR(TS_INSRT) = YEAR(GETDATE())";
        public const string SmartInv_BackFill_OutstandingQTY = "select SUBSTRING(@@SERVERNAME, 3, 4) as Store_ID, M_KD,Q_units, TS_INSRT from [SmartInv_BackFill_OutstandingQTY] where YEAR(TS_INSRT) = YEAR(GETDATE())";


        static void Main(string[] args)
        {

            List<string> storeslist = new List<string>();
            List<SqlQuery> queryList = new List<SqlQuery>();
            List<ServerInfo> server_Details = new List<ServerInfo>();
            int storecount=0;
            int successcount=0;
            int errorcount=0;
            int errorrlogginrow = 2;
            var error_workbook = new XLWorkbook();
            var Error_worksheet = error_workbook.Worksheets.Add("Error");
            Error_worksheet.FirstCell().Value = "Store_ID";
            //GetList of stores to connect with
            storeslist =GetStoreListFromCentralServer();
            storecount = storeslist.Count();
            // Define a list of SQL server connection strings
            server_Details = PrepareServerInfo(storeslist);
            // get list of queries to execute
            queryList = GetListofQuires();
            using (var workbook = new XLWorkbook())
            {
                foreach(var query in queryList)
                {
                    var worksheet = workbook.Worksheets.Add(query.QueryName);
                    
                    try
                    {
                        foreach (var connectionInfo in server_Details)
                        {
                            var dataTable = new DataTable();
                            try
                            {
                                using (var connection = new SqlConnection($"Server={connectionInfo.ServerName}\\sqlexpress;Database={connectionInfo.Database};User Id={connectionInfo.Username};Password={connectionInfo.Password};"))
                                {
                                    connection.Open();
                                    // Define your SQ queryselect
                                    // var query = All_keycodes_selling_floor_locations;

                                    using (var adapter = new SqlDataAdapter(query.Query, connection))
                                    {
                                        adapter.Fill(dataTable);
                                    }
                                    connection.Close();
                                    successcount++;
                                    storecount--;
                                }
                                Console.WriteLine(connectionInfo.storenum + ":- Passed");
                            }
                            catch (Exception ex)
                            {

                                Error_worksheet.Cell(errorrlogginrow, 1).Value = connectionInfo.storenum;
                                Error_worksheet.Cell(errorrlogginrow, 2).Value = query.QueryName;
                                errorrlogginrow++;
                                errorcount++;
                                storecount--;
                                error_workbook.SaveAs("/Users/dbs/Downloads/errorinbackfill.xlsx");
                                Console.WriteLine(connectionInfo.storenum +":- Failed");
                            }

                            bool result = CopytoServer(dataTable);
                            Console.WriteLine("Writing to central server " + connectionInfo.storenum + (result ? " passed" : " falied"));
                            Console.WriteLine("Remaining: " + (storecount)+ " Success: " + (successcount)+ " Error: " + (errorcount)+" ...");
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                   
                }
                // Save the Excel file
            }

            Console.WriteLine("Data exported ");
            Console.ReadKey();
        }
        //to get the set of queries
        public static List<SqlQuery> GetListofQuires()
        {
            List<SqlQuery> queryList = new List<SqlQuery>();
            //SqlQuery rec1 = new SqlQuery { Query = SF_Locations, QueryName = "SF_Locations" };
            //queryList.Add(rec1);
            //SqlQuery rec2 = new SqlQuery { Query = SR_Locations, QueryName = "SR_Locations" };
            //queryList.Add(rec2);
            //SqlQuery rec3 = new SqlQuery { Query = backfill_date, QueryName = "backfill_date" };
            //queryList.Add(rec3);
            //SqlQuery rec4 = new SqlQuery { Query = Take_quantity, QueryName = "Take_quantity" };
            //queryList.Add(rec4);
            //SqlQuery rec5 = new SqlQuery { Query = Clearance_flag, QueryName = "Clearance_flag" };
            //queryList.Add(rec5);
            //SqlQuery rec6 = new SqlQuery { Query = SmartInv_Audit, QueryName = "SmartInv_Audit" };
            //queryList.Add(rec6);
            //SqlQuery rec7 = new SqlQuery { Query = SmartInv_AuditLog, QueryName = "SmartInv_AuditLog" };
            //queryList.Add(rec7);
            SqlQuery rec8 = new SqlQuery { Query = SmartInv_BackFill_OutstandingQTY, QueryName = "SInv_BackFill_OutstandingQTY" };
            queryList.Add(rec8);
            return queryList;
        }
        //to get list of stores
        public static List<string> GetStoreListFromCentralServer()
        {
            try
            {
                var Server = "kwpsql302";
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
            string DataBase= "DKSLS01";
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

        // Bulk Copy to Central store
        public static bool CopytoServer(DataTable data)
        {
            
            var conn = new SqlConnection();
            conn.ConnectionString = "Server=kwtsql302;Database=PDTAPP;User Id=PDTAppUsr;Password=pDT@ppU$3%r!;Trusted_Connection=False;MultipleActiveResultSets=true";
            SqlCommand command = conn.CreateCommand();
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction("");
            command.Transaction = transaction;
            try
            {
                using (SqlBulkCopy sqlbc = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    sqlbc.DestinationTableName = "SmartInv_BackFill_OutstandingQTY";
                    sqlbc.ColumnMappings.Add("Store_ID", "Store_ID");
                    sqlbc.ColumnMappings.Add("M_KD", "M_KD");
                    sqlbc.ColumnMappings.Add("Q_units", "Q_units");
                    sqlbc.ColumnMappings.Add("TS_INSRT", "TS_INSRT");
                    sqlbc.BulkCopyTimeout = 800;
                    sqlbc.WriteToServer(data);
                }
                transaction.Commit();
                conn.Close();
                return true;
            }
            catch (Exception ex)

            {
                Console.WriteLine("Error in bulk copy count");
                transaction.Rollback();
                conn.Close();
                return false;
            }

        }
    }
}


