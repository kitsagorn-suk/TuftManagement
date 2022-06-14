using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class LoginModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public LoginData data { get; set; }
    }

    public class LoginData
    {
        public int id { get; set; } = 0;
        public string username { get; set; } = "";
        public string token { get; set; } = "";
        public List<RoleIDList> role { get; set; }
        public List<ShareHolderList> shareHolder { get; set; }
        public List<AccessRole> accessList { get; set; }

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            username = dr["username"].ToString();
            //employeeName = dr["firstname"].ToString() + " " + dr["lastname"].ToString();
            //imageUrl = dr["image_name"].ToString();
        }
    }

    public class AccessRole
    {
        public string object_id { set; get; } = "";
        public string object_name { set; get; } = "";

        public void loadDataAccessRole(DataRow dr)
        {
            object_id = dr["object_id"].ToString();
            object_name = dr["object_name"].ToString();
        }
    }

    public class RoleIDList
    {
        public int roleID { set; get; } = 0;
        public string roleName { set; get; } = "";

        public void loadDataUserRole(DataRow dr)
        {
            roleID = int.Parse(dr["role_id"].ToString());
            roleName = dr["name"].ToString();
        }
    }

    public class ShareHolderList
    {
        public int shareID { set; get; } = 0;
        public string shareCode { set; get; } = "";
        public string shareName { set; get; } = "";
        public List<AgentList> agentList { get; set; }
        
        public void loadDataShareHolder(DataRow dr)
        {
            shareID = int.Parse(dr["share_id"].ToString());
            shareCode = dr["share_code"].ToString();
            shareName = dr["name"].ToString();
        }
    }

    public class AgentList
    {
        public int agentID { set; get; } = 0;
        public string agentName { set; get; } = "";
        public void loadDataAgent(DataRow dr)
        {
            agentID = int.Parse(dr["agent_id"].ToString());
            agentName = dr["name"].ToString();
        }
    }

}