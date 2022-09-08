using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class ApproveLeaveRequestDTO
    {
        public int[] approveListLeaveID { set; get; }
        public int[] rejectListLeaveID { set; get; }

        public string prepairApproveListLeaveID { set; get; }
        public string prepairRejectListLeaveID { set; get; }
    }
}