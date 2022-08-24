using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{

    public class SaveEmpWorkTimeRequestDTO_V1_1
    {
        public List<EmpWorkTimeRequestDTO> empWorkTimeRequestDTO { set; get; }
        
        public class EmpWorkTimeRequestDTO
        {
            public int empWorkTimeID { set; get; } = 0;
            public int userID { set; get; } = 0;
            public int workShiftID { set; get; } = 0;
            public string workDate { set; get; } = "";
            public bool isFix { set; get; } = false;
        }
    }
}