using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveEmpWorkTimeRequestDTO
    {
        public int empWorkTimeID { set; get; } = 0;
        public int empID { set; get; } = 0;
        public int empWorkShiftID { set; get; } = 0;
        public string workDate { set; get; } = "";
        public string workIn { set; get; } = "";
        public string workOut { set; get; } = "";
        public string floorIn { set; get; } = "";
        public string floorOut { set; get; } = "";
        public int isFix { set; get; } = 0;
        public string reason { set; get; } = "";
    }
}