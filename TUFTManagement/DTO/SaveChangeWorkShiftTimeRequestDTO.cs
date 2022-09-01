using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{

    public class SaveChangeWorkShiftTimeRequestDTO
    {
        public int empWorkTimeID { set; get; } = 0;
        public int userID { set; get; } = 0;
        public int workShiftID { set; get; } = 0;
       
        public int newEmpWorkTimeID { set; get; } = 0;
        public int newUserID { set; get; } = 0;
        public int newWorkShiftID { set; get; } = 0;

        public string remark { set; get; } = "";

    }
}