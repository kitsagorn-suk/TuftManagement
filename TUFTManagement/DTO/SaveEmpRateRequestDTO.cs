﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveEmpRateRequestDTO
    {
        public int empRateID { set; get; } = 0;
        public int empID { set; get; } = 0;
        public string productCode { set; get; } = "";
        public int rateStaff { set; get; } = 0;
        public int rateManager { set; get; } = 0;
        public int rateOwner { set; get; } = 0;
        public int rateConfirm { set; get; } = 0;

    }
}