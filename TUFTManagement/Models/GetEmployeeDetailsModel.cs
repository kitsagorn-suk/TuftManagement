using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TUFTManagement.Models
{
    public class GetEmployeeDetailsModel
    {
        public bool success { get; set; }
        public MsgModel msg { get; set; }
        public EmployeeDetails data { get; set; }
    }

    public class EmployeeDetails
    {
        public int userId { set; get; } = 0;
        public string empCode { set; get; } = "";

        public string userName { set; get; } = "";
        public string joinDate { set; get; } = "";
        public decimal monthlySalary { set; get; } = 0;
        public decimal dailySalary { set; get; } = 0;
        public int departmentID { set; get; } = 0;
        public int positionID { set; get; } = 0;
        public int employmentTypeID { set; get; } = 0;

        public int titleID { set; get; } = 0;
        public string firstNameTH { set; get; } = "";
        public string lastNameTH { set; get; } = "";
        public string nickNameTH { set; get; } = "";
        public string firstNameEN { set; get; } = "";
        public string lastNameEN { set; get; } = "";
        public string nickNameEN { set; get; } = "";
        public int nationalityID { set; get; } = 0;
        public int citizenshipID { set; get; } = 0;
        public int religionID { set; get; } = 0;
        public string dateOfBirth { set; get; } = "";
        public string identityCard { set; get; } = "";
        public string identityCardExpiry { set; get; } = "";
        public float height { set; get; } = 0;
        public float weight { set; get; } = 0;
        public int shirtSizeID { set; get; } = 0;
        public int bloodTypeID { set; get; } = 0;
        public string phoneNumber { set; get; } = "";

        public int cCountryID { set; get; } = 0;
        public string cAddress { set; get; } = "";
        public int cProvinceID { set; get; } = 0;
        public int cDistrictID { set; get; } = 0;
        public int cSubDistrictID { set; get; } = 0;
        public string cZipcode { set; get; } = "";
        public string cPhoneContact { set; get; } = "";
        public int isSamePermanentAddress { set; get; } = 0;
        public int pCountryID { set; get; } = 0;
        public string pAddress { set; get; } = "";
        public int pProvinceID { set; get; } = 0;
        public int pDistrictID { set; get; } = 0;
        public int pSubDistrictID { set; get; } = 0;
        public string pZipcode { set; get; } = "";
        public string pPhoneContact { set; get; } = "";

        public string bankAccountName { set; get; } = "";
        public string bankAccountNumber { set; get; } = "";
        public int bankID { set; get; } = 0;

        public List<EmergencyContact> emergencyContact { set; get; }

        public int bodySetID { set; get; } = 0;
        public int chest { set; get; } = 0;
        public int waist { set; get; } = 0;
        public int hip { set; get; } = 0;

        public int empRateID { set; get; } = 0;
        public string productCode { set; get; } = "";
        public int rateStaff { set; get; } = 0;
        public int rateManager { set; get; } = 0;
        public int rateOwner { set; get; } = 0;
        public int rateConfirm { set; get; } = 0;

        public string profileUrl { set; get; } = "";

        public List<ImageGallary> imageGallary { set; get; }

        public class EmergencyContact
        {
            public int emerContactID { set; get; } = 0;
            public string emerFullName { set; get; } = "";
            public int emerRelationShipID { set; get; } = 0;
            public string emerContact { set; get; } = "";

            public void loadData(DataRow dr)
            {
                emerContactID = Convert.ToInt32(dr["id"]);
                emerFullName = dr["emer_full_name"].ToString();
                emerRelationShipID = Convert.ToInt32(dr["emer_relationship_id"]);
                emerContact = dr["emer_contact"].ToString();
            }
        }

        public class ImageGallary
        {
            public string imgUrl { set; get; } = "";
            public void loadData(DataRow dr)
            {
               
                imgUrl = dr["url"].ToString();
            }
        }

        public void loadData(DataRow dr)
        {
            userId = Convert.ToInt32(dr["user_id"]);
            empCode = dr["emp_code"].ToString();
            userName = dr["username"].ToString();
            joinDate = dr["join_date"].ToString();
            monthlySalary = Convert.ToDecimal(dr["monthly_salary"]);
            dailySalary = Convert.ToDecimal(dr["daily_salary"]);
            departmentID = int.Parse(dr["department_id"].ToString());
            positionID = Convert.ToInt32(dr["position_id"]);
            employmentTypeID = Convert.ToInt32(dr["employment_type_id"]);
            titleID = Convert.ToInt32(dr["title_id"]);
            firstNameTH = dr["firstname_th"].ToString();
            lastNameTH = dr["lastname_th"].ToString();
            nickNameTH = dr["nickname_th"].ToString();
            firstNameEN = dr["firstname_en"].ToString();
            lastNameEN = dr["lastname_en"].ToString();
            nickNameEN = dr["nickname_en"].ToString();
            nationalityID = Convert.ToInt32(dr["nationality_id"]);
            citizenshipID = Convert.ToInt32(dr["citizenship_id"]);
            religionID = Convert.ToInt32(dr["religion_id"]);
            dateOfBirth = dr["date_of_Birth"].ToString();
            identityCard = dr["identity_card"].ToString();
            identityCardExpiry = dr["identity_card_expiry"].ToString();
            height = Convert.ToInt32(dr["height"]);
            weight = Convert.ToInt32(dr["weight"]);
            shirtSizeID = Convert.ToInt32(dr["shirt_size_id"]);
            bloodTypeID = Convert.ToInt32(dr["blood_type_id"]);
            phoneNumber = dr["phone_number"].ToString();

            cCountryID = Convert.ToInt32(dr["c_country_id"]);
            cAddress = dr["c_address"].ToString();
            cProvinceID = Convert.ToInt32(dr["c_province_id"]);
            cDistrictID = Convert.ToInt32(dr["c_district_id"]);
            cSubDistrictID = Convert.ToInt32(dr["c_sub_district_id"]);
            cZipcode = dr["c_zipcode"].ToString();
            cPhoneContact = dr["c_phone_contact"].ToString();
            isSamePermanentAddress = Convert.ToInt32(dr["is_same_permanent_address"]);

            pCountryID = Convert.ToInt32(dr["p_country_id"]);
            pAddress = dr["p_address"].ToString();
            pProvinceID = Convert.ToInt32(dr["p_province_id"]);
            pDistrictID = Convert.ToInt32(dr["p_district_id"]);
            pSubDistrictID = Convert.ToInt32(dr["p_sub_district_id"]);
            pZipcode = dr["p_zipcode"].ToString();
            pPhoneContact = dr["p_phone_contact"].ToString();

            bankID = Convert.ToInt32(dr["bank_id"]);
            bankAccountName = dr["account_name"].ToString();
            bankAccountNumber = dr["account_no"].ToString();

            bodySetID = Convert.ToInt32(dr["body_set_id"]);
            chest = Convert.ToInt32(dr["chest"]);
            waist = Convert.ToInt32(dr["waist"]);
            hip = Convert.ToInt32(dr["hip"]);

            empRateID = Convert.ToInt32(dr["emp_rate_id"]);
            productCode = dr["product_code"].ToString();
            rateStaff = Convert.ToInt32(dr["rate_staff"]);
            rateManager = Convert.ToInt32(dr["rate_manager"]);
            rateOwner = Convert.ToInt32(dr["rate_owner"]);
            rateConfirm = Convert.ToInt32(dr["rate_confirm"]);

            profileUrl = productCode = dr["url"].ToString();

        }

        
    }
}