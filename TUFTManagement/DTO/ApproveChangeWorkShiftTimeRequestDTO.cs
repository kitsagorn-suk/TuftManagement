using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{

    public class ApproveChangeWorkShiftTimeRequestDTO
    {
        public int[] approveListEmpWorkTimeID { set; get; }
        public int[] rejectListEmpWorkTimeID { set; get; }

        public string prepairApproveListEmpWorkTimeID { set; get; }
        public string prepairRejectListEmpWorkTimeID { set; get; }


    }
}