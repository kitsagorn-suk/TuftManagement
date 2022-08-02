using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmpProfileModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public EmpProfile data { get; set; }
    }

    public class EmpProfile
    {
        public string empCode { set; get; } = "";
        public string name { set; get; } = "";
        public string nickName { set; get; } = "";
        public string phoneNumber { set; get; } = "";

        public List<RoleIDList> role { get; set; }
        public List<ShareHolderList> shareHolder { get; set; }
        public List<AccessRole> accessList { get; set; }

        public void loadData(DataRow dr)
        {
            empCode = dr["emp_code"].ToString();
            name = dr["name"].ToString();
            nickName = dr["nickname"].ToString();
            phoneNumber = dr["phone_number"].ToString();
        }
    }
}