using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetWorkShiftTimeHeaderModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public HeaderDetail data { get; set; }
    }

    public class HeaderDetail
    {
        public int ID { set; get; } = 0;
        
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string empNickName { set; get; } = "";
        public string deptName { set; get; } = "";
        public int workShiftID { set; get; } = 0;
        public string wsCode { set; get; } = "";
        public int userID { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            ID = int.Parse(dr["id"].ToString());
            empCode = dr["emp_code"].ToString();
            empName = dr["emp_name"].ToString();
            empNickName = dr["emp_nickname"].ToString();
            deptName = dr["department_name"].ToString();
            workShiftID = int.Parse(dr["ws_id"].ToString());
            wsCode = dr["ws_code"].ToString();
            userID = int.Parse(dr["user_id"].ToString());
        }
    }
}