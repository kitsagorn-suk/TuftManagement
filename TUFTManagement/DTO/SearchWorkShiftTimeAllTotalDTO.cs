using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SearchWorkShiftTimeAllTotalDTO
    {
        public int shiftID { set; get; } = 0;
        public string shiftName { set; get; } = "";
        public int total { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            shiftID = int.Parse(dr["id"].ToString());
            shiftName = dr["ws_code"].ToString();
            total = int.Parse(dr["total"].ToString());
        }
    }
}