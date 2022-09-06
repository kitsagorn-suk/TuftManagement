using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetLeaveModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetLeave data { get; set; }
    }

    public class GetLeave
    {
        public int leaveTypeID { set; get; } = 0;


        public void LoadData(DataRow dr)
        {
            leaveTypeID = int.Parse(dr["id"].ToString());
        }
    }
}