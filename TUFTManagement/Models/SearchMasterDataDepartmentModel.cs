using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class SearchMasterDataDepartmentModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public Pagination<SearchMasterDataDepartment> data { get; set; }
    }

    public class SearchMasterDataDepartment
    {
        public int id { set; get; } = 0;
        public string nameEn { set; get; } = "";
        public string nameTh { set; get; } = "";
        //public int isActive { set; get; } = 0;

        public void loadData(DataRow dr)
        {
            id = int.Parse(dr["id"].ToString());
            nameEn = dr["name_en"].ToString();
            nameTh = dr["name_th"].ToString();
            //isActive = int.Parse(dr["is_active"].ToString());
        }
    }
}