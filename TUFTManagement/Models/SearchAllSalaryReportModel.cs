using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllSalaryReportModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchAllSalaryReport> data { get; set; }
    }

    public class SearchAllSalaryReport
    {
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string departmentName { set; get; } = "";
        public string positionName { set; get; } = "";
        public string empType { set; get; } = "";
        public string empSalary { set; get; } = "";

        public void loadData(DataRow dr)
        {
            empCode = dr["emp_code"].ToString();
            empName = dr["name"].ToString();
            departmentName = dr["department"].ToString();
            positionName = dr["position"].ToString();
            empType = dr["emp_type"].ToString();
            empSalary = dr["salary"].ToString();
        }
    }
}