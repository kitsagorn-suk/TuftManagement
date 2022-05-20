using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveEmpWorkTimeTransChangeRequestDTO
    {
        public int transChangeID { set; get; } = 0;
        public int empWorkTimeID { set; get; } = 0;
        public int userID { set; get; } = 0;
        public int OldEmpWorkShiftID { set; get; } = 0;
        public int NewEmpWorkShiftID { set; get; } = 0;
        public string remark { set; get; } = "";

        public int statusApprove { set; get; } = 0;
    }
}