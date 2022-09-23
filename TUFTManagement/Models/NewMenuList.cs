using System.Data;

namespace TUFTManagement.Models
{
    public class NewMenuList
    {
        public string objectID { set; get; }
        public string objectName { set; get; }
        public string parentID { set; get; }
        public string icon { set; get; }


        public void loadDataMenu(DataRow dr)
        {
            objectID = dr["object_id"].ToString();
            objectName = dr["object_name"].ToString();
            parentID = dr["parent_id"].ToString();
            icon = dr["icon"].ToString();
        }

    }
}