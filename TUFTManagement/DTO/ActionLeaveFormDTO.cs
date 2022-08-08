using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class ActionLeaveFormDTO
    {
        public int leaveID { set; get; }
        public string rejectReason { set; get; } = "";
        public string cancelReason { set; get; } = "";
    }
}