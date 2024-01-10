using System;
using System.Data;
using System.Data.SqlClient;

namespace BackFill_Data_extaract
{
    public class BulkCopytoServer
    {
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
                    sqlbc.DestinationTableName = "StkRoomBackfillLocStartEnd";
                    sqlbc.ColumnMappings.Add("Str_ID", "StoreID");
                    sqlbc.ColumnMappings.Add("Session_ID", "SessionID");
                    sqlbc.ColumnMappings.Add("GroupSeqNum", "GroupID");
                    sqlbc.ColumnMappings.Add("Location", "Location");
                    sqlbc.ColumnMappings.Add("LocnStart_Date", "LocnStartDate");
                    sqlbc.ColumnMappings.Add("LocnEnd_Date", "LocnEndDate");
                    sqlbc.ColumnMappings.Add("Keycode", "Keycode");
                    sqlbc.ColumnMappings.Add("ScanDate", "ScanDate");
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
