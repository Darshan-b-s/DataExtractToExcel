using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BackFill_Data_extaract
{
    public class PopulatingData
    {
        public static List<SqlQuery> GetListofQuires()
        {
            List<SqlQuery> queryList = new List<SqlQuery>();
            SqlQuery rec1 = new SqlQuery { Query = ConstantQueries.SF_Locations, QueryName = "SF_Locations" };
            queryList.Add(rec1);
            SqlQuery rec2 = new SqlQuery { Query = ConstantQueries.SR_Locations, QueryName = "SR_Locations" };
            queryList.Add(rec2);
            SqlQuery rec3 = new SqlQuery { Query = ConstantQueries.backfill_date, QueryName = "backfill_date" };
            queryList.Add(rec3);
            SqlQuery rec4 = new SqlQuery { Query = ConstantQueries.Take_quantity, QueryName = "Take_quantity" };
            queryList.Add(rec4);
            SqlQuery rec5 = new SqlQuery { Query = ConstantQueries.Clearance_flag, QueryName = "Clearance_flag" };
            queryList.Add(rec5);
            SqlQuery rec6 = new SqlQuery { Query = ConstantQueries.SmartInv_Audit, QueryName = "SmartInv_Audit" };
            queryList.Add(rec6);
            SqlQuery rec7 = new SqlQuery { Query = ConstantQueries.SmartInv_AuditLog, QueryName = "SmartInv_AuditLog" };
            queryList.Add(rec7);
            SqlQuery rec8 = new SqlQuery { Query = ConstantQueries.SmartInv_BackFill_OutstandingQTY, QueryName = "SInv_BackFill_OutstandingQTY" };
            queryList.Add(rec8);
            SqlQuery rec9 = new SqlQuery { Query = ConstantQueries.Stl_200, QueryName = "Stl_200" };
            queryList.Add(rec9);
            SqlQuery rec10 = new SqlQuery { Query = ConstantQueries.PDT_Data, QueryName = "pdt_data" };
            queryList.Add(rec10);
            return queryList;
        }
        //to get list of stores
        public static List<string> GetStoreListFromCentralServer()
        {
            try
            {
                //var Server = "kwpsql302";
                //var Database = "PDTAPP";
                //var User = "PDTAppUsr";
                //var Password = "pDT@ppU$3%r!";

                //var StoreList = new List<string>();
                //using (var connection = new SqlConnection($"Server={Server};Database={Database};User Id={User};Password={Password};"))
                //{
                //    connection.Open();
                //    using (var command = new SqlCommand("usp_Get_Store_Servers", connection))
                //    {

                //        command.CommandType = CommandType.StoredProcedure;
                //        using (var reader = command.ExecuteReader())
                //        {
                //            int i = 0;
                //            while (reader.Read())
                //            {

                //                StoreList.Add(reader[0].ToString());
                //                i++;
                //            }

                //        }
                //    }
                //    connection.Close();
                //}
                var StoreList = new List<string>();
                StoreList.Add("KV1907NA001");
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
            string DataBase = "PDTDB";
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
