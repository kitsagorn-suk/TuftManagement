using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveLeaveDetailDTO
    {
        public string mode { set; get; } = "";
        public int leaveId { set; get; } = 0;
        public int empId { set; get; } = 0;
        public int leavetypeId { set; get; } = 0;
        public string startdate { set; get; } = "";
        public string enddate { set; get; } = "";
        public int numdays { set; get; } = 0;
        public string leavereason { set; get; } = "";
    }
}