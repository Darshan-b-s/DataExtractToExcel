using System;
namespace BackFill_Data_extaract
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


    public class ConstantQueries
    {
        public const string SF_Locations = "select grp.Str_ID,locn.Location,item.Keycode, item.ScanDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'LocationAdd'and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location  desc";
        public const string SR_Locations = "select grp.Str_ID, locn.Location,item.Keycode, item.ScanDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'StockroomAdd' and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location desc";
        public const string backfill_date = "select grp.Str_ID,locn.Location,item.Keycode, grp.Group_EndDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'StockroomBackfill' and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location desc";
        public const string Take_quantity = "select grp.Str_ID,locn.Location,item.Keycode,item.ScanCode, item.ScanDate,item.QTY from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'StockroomBackfill' and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Str_ID , item.keycode, locn.Location desc";
        public const string Clearance_flag = "Select  C_KD_STAT, F_KD_VOLUME_LINE,  F_KD_SHELF_READY, F_KD_RAINCHECK,  F_KD_RFID,   F_KD_PR, M_KD FROM   dksls01.dbo.SLT200 where C_KD_STAT=3 order by M_KD";
        public const string SmartInv_Audit = "select SmartInv_Audit_ID,SUBSTRING(@@SERVERNAME, 3, 4) as Store_ID, M_STK_RM_LOCN, M_KD, C_Process, TS_INSRT from [SmartInv_Audit] where YEAR(TS_INSRT) = YEAR(GETDATE())";
        public const string SmartInv_AuditLog = "select SmartInv_AuditLog_ID,SUBSTRING(@@SERVERNAME, 3, 4) as Store_ID, M_STK_RM_LOCN, M_KD, C_Process, TS_INSRT from [SmartInv_AuditLog] where YEAR(TS_INSRT) = YEAR(GETDATE())";
        public const string SmartInv_BackFill_OutstandingQTY = "select SUBSTRING(@@SERVERNAME, 3, 4) as Store_ID, M_KD,Q_units, TS_INSRT from [SmartInv_BackFill_OutstandingQTY] where YEAR(TS_INSRT) = YEAR(GETDATE())";
        public const string Stl_200 = "SELECT SUBSTRING(@@SERVERNAME, 3, 4) as Store_ID, M_KD as Keycode, SLT420.C_MDEPT AS Dept_ID, Q_MIN_TAKE as MinQty FROM DKSLS01.dbo.SLT200, DKSLS01.dbo.SLT420 WHERE SLT200.C_MDEPT = SLT420.C_MDEPT";
        public const string PDT_Data = "select GRP.Str_ID, grp.Session_ID, grp.GroupSeqNum, locn.Location, locn.LocnStart_Date, locn.LocnEnd_Date, item.Keycode, item.ScanDate from PDT_DataCapture grp inner join PDT_DataCapture_LOCN locn on grp.Session_ID = locn.Session_ID and grp.GroupSeqNum = locn.GroupSeqNum inner join PDT_DataCapture_ITEM item on locn.Session_ID = item.Session_ID and locn.GroupSeqNum = item.GroupSeqNum and locn.LocnSeqNum = item.LocnSeqNum where AppName = 'StockroomBackfill'and grp.Group_EndDate is not null and grp.LegacySetDate is not null order by grp.Session_ID";
    }
    
    
}
