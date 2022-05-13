using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmpRateModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetEmpRate data { get; set; }
    }

    public class GetEmpRate
    {
        public int empRateID { set; get; } = 0;
        public int empProfileID { set; get; } = 0;
        public string productCode { set; get; } = "";
        public int rateStaff { set; get; } = 0;
        public int rateManager { set; get; } = 0;
        public int rateOwner { set; get; } = 0;
        public int rateConfirm { set; get; } = 0;
        public int status { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            empRateID = int.Parse(dr["id"].ToString());
            empProfileID = int.Parse(dr["emp_id"].ToString());
            productCode = dr["product_code"].ToString();
            rateStaff = int.Parse(dr["rate_staff"].ToString());
            rateManager = int.Parse(dr["rate_manager"].ToString());
            rateOwner = int.Parse(dr["rate_owner"].ToString());
            rateConfirm = int.Parse(dr["rate_confirm"].ToString());
            status = int.Parse(dr["status"].ToString().ToLower().Equals("true") ? "1" : "0");
        }
    }
}