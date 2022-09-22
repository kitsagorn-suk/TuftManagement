using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllWorkTimeReportModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchAllWorkTimeReport> data { get; set; }
    }

    public class SearchAllWorkTimeReport
    {
        public int empUserID { set; get; } = 0;
        public string empCode { set; get; } = "";
        public string name { set; get; } = "";
        public string departmentName { set; get; } = "";
        public string positionName { set; get; } = "";
        public string empType { set; get; } = "";
        public int totalTime { set; get; } = 0;
        public int absentWork { set; get; } = 0;
        public int leaveWork { set; get; } = 0;
        public int lateWork { set; get; } = 0;
        public int ot { set; get; } = 0;
        public List<WorkTimeDetail> detailList{ set; get; }

        public void loadData(DataRow dr)
        {
            empUserID = int.Parse(dr["user_id"].ToString());
            empCode = dr["emp_code"].ToString();
            name = dr["name"].ToString();
            departmentName = dr["department"].ToString();
            positionName = dr["position"].ToString();
            empType = dr["emp_type"].ToString();
            totalTime = int.Parse(dr["total_time"].ToString());
            absentWork = int.Parse(dr["absent_work"].ToString());
            leaveWork = int.Parse(dr["leave_work"].ToString());
            lateWork = int.Parse(dr["late_work"].ToString());
            ot = int.Parse(dr["ot"].ToString());
        }
    }

    public class WorkTimeDetail
    {
        public string workDate { set; get; } = "";
        public string wsCode { set; get; } = "";
        public string workTime { set; get; } = "";
        public string timeIn { set; get; } = "";
        public string timeOut { set; get; } = "";
        public string status { set; get; } = "";

        public void loadData(DataRow dr)
        {
            workDate = dr["work_date"].ToString();
            wsCode = dr["ws_code"].ToString();
            workTime = dr["work_time"].ToString();
            timeIn = dr["time_in"].ToString();
            timeOut = dr["time_out"].ToString();
            status = dr["status"].ToString();
        }
    }
}