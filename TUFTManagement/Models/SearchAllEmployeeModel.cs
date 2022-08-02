using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllEmployeeModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchAllEmployee> data { get; set; }
    }

    public class SearchAllEmployee
    {
        public int id { set; get; } = 0;
        public string empCode { set; get; } = "";
        public string empName { set; get; } = "";
        public string departmentName { set; get; } = "";
        public string positionName { set; get; } = "";
        public string empType { set; get; } = "";
        public string phoneNum { set; get; } = "";
        public string status { set; get; } = "";

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            empCode = dr["emp_code"].ToString();

            dr["emp_nick_name"].ToString();
            if (!string.IsNullOrEmpty(dr["emp_nick_name"].ToString()))
            {
                empName = dr["emp_name"].ToString() + "(" + dr["emp_nick_name"].ToString() + ")";
            }
            else
            {
                empName = dr["emp_name"].ToString();
            }
            
            positionName = dr["position_name"].ToString();
            departmentName = dr["department_name"].ToString();
            empType = dr["emp_type"].ToString();
            phoneNum = dr["phone_number"].ToString();
            status = dr["status"].ToString();
        }
    }
}