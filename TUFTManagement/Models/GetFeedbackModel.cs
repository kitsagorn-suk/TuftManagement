using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

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
        public int rate { set; get; } = 0;
        public string comment { set; get; } = "";
        public int tranID { set; get; } = 0;
        public int feedbackDate { set; get; } = 0;
        public int status { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            feedbackID = int.Parse(dr["id"].ToString());
            empID = int.Parse(dr["emp_id"].ToString());
            rate = int.Parse(dr["rate"].ToString());
            comment = dr["comment"].ToString();
            tranID = int.Parse(dr["tran_id"].ToString());
            feedbackDate = int.Parse(dr["feedback_date"].ToString());
            status = int.Parse(dr["status"].ToString());
            
        }
    }
}