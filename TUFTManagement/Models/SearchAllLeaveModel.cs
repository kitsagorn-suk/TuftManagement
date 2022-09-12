using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllLeaveModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public SearchAllLeaveDetail data { get; set; }
    }

    public class SearchAllLeaveDetail
    {

        //public List<AllLeaveShift> dataShift { get; set; }
        public List<AllLeave> dataLeave { get; set; }
        public Pagination<SearchAllLeave> dataSearch { get; set; }
    }

    public class SearchAllLeave
    {
        public int id { set; get; } = 0;
        public int empID { set; get; } = 0;
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string leavetype { set; get; } = "";
        public string startDate { set; get; } = "";
        public string endDate { set; get; } = "";
        public int numdays { set; get; } = 0;
        public int remaindays { set; get; } = 0;
        public string leavereason { set; get; } = "";
        public string status { set; get; } = "";

        public void LoadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            empID = int.Parse(dr["user_id"].ToString());
            empCode = dr["emp_code"].ToString();
            empName = dr["employee_name"].ToString();
            leavetype = dr["leave_type_name"].ToString();
            startDate = dr["start_date"].ToString();
            endDate = dr["end_date"].ToString();
            numdays = int.Parse(dr["number_of_days"].ToString());
            remaindays = int.Parse(dr["remaining_leave"].ToString());
            leavereason = dr["leave_reason"].ToString();
            status = dr["status"].ToString();
        }
    }

    public class AllLeaveShift
    {
        public string name { set; get; } = "";
        public int total { set; get; } = 0;
        

        public void LoadData(DataRow dr)
        {
            name = dr["ws_code"].ToString();
            total = int.Parse(dr["total"].ToString());
        }
    }

    public class AllLeave
    {
        public string name { set; get; } = "";
        public int total { set; get; } = 0;

        public void LoadData(DataRow dr)
        {
            name = dr["leave_name"].ToString();
            total = int.Parse(dr["total"].ToString());
        }
    }
}