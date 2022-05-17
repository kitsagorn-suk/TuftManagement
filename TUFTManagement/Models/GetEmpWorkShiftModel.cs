using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmpWorkShiftModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetEmpWorkShift data { get; set; }
    }

    public class GetEmpWorkShift
    {
        public int empWorkShiftID { set; get; } = 0;
        public string wsCode { set; get; } = "";
        public string timeStart { set; get; } = "";
        public string timeEnd { set; get; } = "";
        public int workTypeID { set; get; } = 0;
        public int status { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            empWorkShiftID = int.Parse(dr["id"].ToString());
            wsCode = dr["ws_code"].ToString();
            timeStart = dr["time_start"].ToString();
            timeEnd = dr["time_end"].ToString();
            workTypeID = int.Parse(dr["work_type_id"].ToString());
            status = int.Parse(dr["status"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}