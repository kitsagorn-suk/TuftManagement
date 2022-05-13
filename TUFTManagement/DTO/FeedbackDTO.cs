using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class FeedbackDTO
    {
        public int EmpID { set; get; } = 0;
        public int Rate { set; get; } = 0;
        public string Comment { set; get; } = "";
        public int TranID { set; get; } = 0;
        public int CreateBy { set; get; } = 0;
    }
}