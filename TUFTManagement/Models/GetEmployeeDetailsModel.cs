using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmployeeDetailsModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public EmployeeDetails data { get; set; }
    }

    public class EmployeeDetails
    {
        public string empCode { set; get; } = "";
        public string name { set; get; } = "";
        public string nickName { set; get; } = "";
        public string phoneNumber { set; get; } = "";

        public void loadData(DataRow dr)
        {
            empCode = dr["emp_code"].ToString();
            name = dr["name"].ToString();
            nickName = dr["nickname"].ToString();
            phoneNumber = dr["phone_number"].ToString();
        }
    }
}