using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SearchWorktimePendingTotalDTO
    {
        public int totalEditWorkShift { set; get; } = 0;
        public int totalTradeWorkShift { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            totalEditWorkShift = int.Parse(dr["total_edit_work_shift"].ToString());
            totalTradeWorkShift = int.Parse(dr["total_trade_work_shift"].ToString());
        }
    }
}