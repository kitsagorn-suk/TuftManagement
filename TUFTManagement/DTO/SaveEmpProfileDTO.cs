using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TUFTManagement.DTO
{
    public class SaveEmpProfileDTO
    {
        public string mode { set; get; } = "";
        public int newUserID { set; get; } = 0;
        public int empProfileID { set; get; } = 0;
        public string shareCode { set; get; } = "";
        public string empCode { set; get; } = "";
        public string userName { set; get; } = "";
        public string password { set; get; } = "";
        public string identityCard { set; get; } = "";
        public string identityCardExpiry { set; get; } = "";
        public int titleID { set; get; } = 0;
        public string firstNameEN { set; get; } = "";
        public string lastNameEN { set; get; } = "";
        public string firstNameTH { set; get; } = "";
        public string lastNameTH { set; get; } = "";
        public string nickName { set; get; } = "";
        public string phoneNumber { set; get; } = "";
        public int positionID { set; get; } = 0;
        public int perNum { set; get; } = 0;
        public string dateOfBirth { set; get; } = "";
        public string joinDate { set; get; } = "";
        public string proPassDate { set; get; } = "";
        public decimal monthlySalary { set; get; } = 0;
        public decimal dailySalary { set; get; } = 0;
        public int employmentTypeID { set; get; } = 0;
        public int roleID { set; get; } = 0; 
        public int bodySetID { set; get; } = 0;
        public string shirtSize { set; get; } = "";

        public string emerFirstname { set; get; } = "";
        public string emerLastname { set; get; } = "";
        public string emerContact { set; get; } = "";

        public string cAddress { set; get; } = "";
        public int cSubDistrictID { set; get; } = 0;
        public int cDistrictID { set; get; } = 0;
        public int cProvinceID { set; get; } = 0;
        public string cZipcode { set; get; } = "";
        public int isSamePermanentAddress { set; get; } = 0;
        public string pAddress { set; get; } = "";
        public int pSubDistrictID { set; get; } = 0;
        public int pDistrictID { set; get; } = 0;
        public int pProvinceID { set; get; } = 0;
        public string pZipcode { set; get; } = "";
    }
}