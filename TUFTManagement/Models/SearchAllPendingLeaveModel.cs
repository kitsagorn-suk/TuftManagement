using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchAllPendingLeaveModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public SearchAllPendingLeaveDetail data { get; set; }
    }

    public class SearchAllPendingLeaveDetail
    {

        //public List<AllLeaveShift> dataShift { get; set; }
        public List<AllLeave> dataLeave { get; set; }
        public Pagination<SearchAllLeave> dataSearch { get; set; }
    }
  
}