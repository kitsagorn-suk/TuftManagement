using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetLeaveDetailModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetLeaveDetail data { get; set; }
    }

    public class GetLeaveDetail
    {
        public int id { set; get; } = 0;
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string leavetype { set; get; } = "";
        public string startDate { set; get; } = "";
        public string endDate { set; get; } = "";
        public int numdays { set; get; } = 0;
        public string leavereason { set; get; } = "";
        public string status { set; get; } = "";

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            empCode = dr["emp_code"].ToString();
            empName = dr["employee_name"].ToString();
            leavetype = dr["leave_type_name"].ToString();
            startDate = dr["start_date"].ToString();
            endDate = dr["end_date"].ToString();
            numdays = int.Parse(dr["number_of_days"].ToString());
            leavereason = dr["leave_reason"].ToString();
            status = dr["status"].ToString();
        }
    }
}