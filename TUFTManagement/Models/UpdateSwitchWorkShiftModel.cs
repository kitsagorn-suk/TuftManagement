using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class UpdateSwitchWorkShiftModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public SwitchWorkShift data { get; set; }
    }

    public class SwitchWorkShift
    {
        public int workShiftID { set; get; } = 0;
        public int status { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            workShiftID = int.Parse(dr["id"].ToString());
            status = int.Parse(dr["status"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}