using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class GetTransWorkShiftDTO
    {
        public int transChangeWorkShiftID { set; get; } = 0;
        public int userID { set; get; } = 0;
        public int workTimeID { set; get; } = 0;
        public int workShiftIdOld { set; get; } = 0;
        public int workShiftIdNew { set; get; } = 0;
        public int action { set; get; } = 0;
        public int tradeWorkTimeID { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            // name mapping table DB
            transChangeWorkShiftID = int.Parse(dr["id"].ToString());
            userID = int.Parse(dr["user_id"].ToString());
            workTimeID = int.Parse(dr["work_time_id"].ToString());
            workShiftIdOld = int.Parse(dr["work_shift_id_old"].ToString());
            workShiftIdNew = int.Parse(dr["work_shift_id_new"].ToString());
            action = int.Parse(dr["action"].ToString());
            tradeWorkTimeID = int.Parse(dr["new_work_time_id"].ToString());
        }
    }


}