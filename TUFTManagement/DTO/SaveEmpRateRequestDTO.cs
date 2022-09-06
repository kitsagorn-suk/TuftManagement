using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveEmpRateRequestDTO
    {
        public int empRateID { set; get; } = 0;
        public int empID { set; get; } = 0;
        public int serviceNo { set; get; } = 0;
        public int productGrade { set; get; } = 0;
        public int startDrink { set; get; } = 0;
        public int fullDrink { set; get; } = 0;
        public float rateStaff { set; get; } = 0;
        public float rateManager { set; get; } = 0;
        public float rateOwner { set; get; } = 0;
        public float rateConfirm { set; get; } = 0;

    }
}