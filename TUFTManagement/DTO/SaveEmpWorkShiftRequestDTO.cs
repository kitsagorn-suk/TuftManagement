using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveEmpWorkShiftRequestDTO
    {
        public string mode { set; get; } = "";
        public int empWorkTimeID { set; get; } = 0;
        public int workShiftID { set; get; } = 0;

        public string wsCode { set; get; } = "";
        public int workTypeID { set; get; } = 0;
        public string remark { set; get; } = "";
        public string timeStart { set; get; } = "";
        public string timeEnd { set; get; } = "";
        public int isCancel { set; get; } = 1;
        public int status { set; get; } = 1;

    }
}