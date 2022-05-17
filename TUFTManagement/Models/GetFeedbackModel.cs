using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TUFTManagement.Core;

namespace TUFTManagement.Models
{
    public class GetFeedbackModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public GetFeedback data { get; set; }
    }

    public class GetFeedback
    {
        public int feedbackID { set; get; } = 0;
        public int empID { set; get; } = 0;
        public float rate { set; get; } = 0;
        public string comment { set; get; } = "";
        public int tranID { set; get; } = 0;
        public string feedbackDate { set; get; } = "";
        public int status { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            feedbackID = int.Parse(dr["id"].ToString());
            empID = int.Parse(dr["user_id"].ToString());
            rate = float.Parse(dr["rate"].ToString());
            comment = dr["comment"].ToString();
            tranID = int.Parse(dr["tran_id"].ToString());
            feedbackDate = Utility.convertToDateTimeServiceFormatString2(dr["feedback_date"].ToString());
            status = int.Parse(dr["status"].ToString().ToLower().Equals("true") ? "1" : "0");

        }
    }
}