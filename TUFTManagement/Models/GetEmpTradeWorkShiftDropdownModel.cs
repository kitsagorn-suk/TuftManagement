using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmpTradeWorkShiftDropdownModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public List<EmpTradeWorkShift> data { get; set; }
    }

    public class EmpTradeWorkShift
    {
        public int empWorkTimeID { get; set; } = 0;
        public int userID { get; set; } = 0;
        public int workShiftID { get; set; } = 0;
        public string name { get; set; } = "";


        public void loadData(DataRow dr)
        {
            // name mapping table DB
            empWorkTimeID = int.Parse(dr["id"].ToString());
            userID = int.Parse(dr["id"].ToString());
            workShiftID = int.Parse(dr["id"].ToString());
            name = dr["emp_code"].ToString() + "" + dr["emp_name"].ToString() + "" + dr["ws_code"].ToString();
        }

    }
}