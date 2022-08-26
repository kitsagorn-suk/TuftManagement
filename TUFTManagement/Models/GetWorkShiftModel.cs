using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetWorkShiftModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetWorkShift data { get; set; }
    }

    public class GetWorkShift
    {
        public int workShiftID { set; get; } = 0;
       

        public void loadData(DataRow dr)
        {
            workShiftID = int.Parse(dr["id"].ToString());
        }
    }
}