using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static TUFTManagement.Models.EmployeeDetails;

namespace TUFTManagement.Models
{
    public class GetAllEmployeePrettyModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public List<GetAllEmployee> data { get; set; }
    }

    public class GetAllEmployee
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
        public int chest { set; get; } = 0;
        public int waist { set; get; } = 0;
        public int hip { set; get; } = 0;

        public string imageProfileUrl { set; get; } = "";
        public string imageIdentityUrl { set; get; } = "";

        public List<ImageGallery> imageGallery { set; get; }
        public List<GetEmployeeRate> employeeRate { set; get; }

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

            chest = Convert.ToInt32(dr["chest"]);
            waist = Convert.ToInt32(dr["waist"]);
            hip = Convert.ToInt32(dr["hip"]);

            imageProfileUrl = dr["image_profile_url"].ToString();
            imageIdentityUrl = dr["image_iden_url"].ToString();

        }
        public class GetEmployeeRate
        {
            public int empRateID { set; get; } = 0;
            public int serviceNoID { set; get; } = 0;
            public string serviceNoName { set; get; } = "";
            public int productGradeID { set; get; } = 0;
            public string productGradeName { set; get; } = "";

            public float rateStaff { set; get; } = 0;
            public float rateManager { set; get; } = 0;
            public float rateOwner { set; get; } = 0;
            public float rateConfirm { set; get; } = 0;


            public void loadData(DataRow dr)
            {
                empRateID = Convert.ToInt32(dr["emp_rate_id"]);
                serviceNoID = Convert.ToInt32(dr["service_no"]);
                serviceNoName = dr["service_no_name"].ToString();

                productGradeID = Convert.ToInt32(dr["product_grade"]);
                productGradeName = dr["product_grade_name"].ToString();

                rateStaff = float.Parse(dr["rate_staff"].ToString());
                rateManager = float.Parse(dr["rate_manager"].ToString());
                rateOwner = float.Parse(dr["rate_owner"].ToString());
                rateConfirm = float.Parse(dr["rate_confirm"].ToString());

            }

        }
    }
}