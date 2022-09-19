using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static TUFTManagement.Models.EmployeeDetails;

namespace TUFTManagement.Models
{
    public class GetAllEmployeeByPositionModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public List<GetAllEmployeeNormal> data { get; set; }
    }

    public class GetAllEmployeeNormal
    {
        public int userID { set; get; } = 0;
        public string userName { set; get; } = "";
        public string empCode { set; get; } = "";
        public int status { set; get; } = 0;

        public int titleID { set; get; } = 0;
        public string titleName { set; get; } = "";
        public string firstNameTH { set; get; } = "";
        public string lastNameTH { set; get; } = "";
        public string nickNameTH { set; get; } = "";
        public string firstNameEN { set; get; } = "";
        public string lastNameEN { set; get; } = "";
        public string nickNameEN { set; get; } = "";
        public string phoneNumber { set; get; } = "";
        public int shirtSizeID { set; get; } = 0;
        public string shirtSizeName { set; get; } = "";
        public float height { set; get; } = 0;
        public float weight { set; get; } = 0;

        public string imageProfileUrl { set; get; } = "";
        public string imageIdentityUrl { set; get; } = "";
        
        public void loadData(DataRow dr)
        {
            userID = Convert.ToInt32(dr["user_id"]);
            userName = dr["username"].ToString();
            empCode = dr["emp_code"].ToString();


            titleID = Convert.ToInt32(dr["title_id"]);
            titleName = dr["title_name"].ToString();
            firstNameTH = dr["firstname_th"].ToString();
            lastNameTH = dr["lastname_th"].ToString();
            nickNameTH = dr["nickname_th"].ToString();
            firstNameEN = dr["firstname_en"].ToString();
            lastNameEN = dr["lastname_en"].ToString();
            nickNameEN = dr["nickname_en"].ToString();
            phoneNumber = dr["phone_number"].ToString();
            shirtSizeID = Convert.ToInt32(dr["shirt_size_id"]);
            shirtSizeName = dr["shirt_size_name"].ToString();

            height = Convert.ToInt32(dr["height"]);
            weight = Convert.ToInt32(dr["weight"]);
            

            imageProfileUrl = dr["image_profile_url"].ToString();
            imageIdentityUrl = dr["image_iden_url"].ToString();

        }
    }
}