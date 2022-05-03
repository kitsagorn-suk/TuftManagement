using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class ReturnVerifyModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public _ReturnVerifyModel data { get; set; }
    }

    public class _ReturnVerifyModel
    {
        public string status { get; set; } = "";

        public void loadData(DataRow dr)
        {
            status = dr["status"].ToString();
        }

    }
}