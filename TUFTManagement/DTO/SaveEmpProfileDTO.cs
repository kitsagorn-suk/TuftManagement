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

        public List<SaveEmergencyContact> emergencyContact { set; get; }

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
        
    }
    
    public class SaveEmergencyContact
    {
        public string emerFullName { set; get; } = "";
        public int emerRelationShipID { set; get; } = 0;
        public string emerContact { set; get; } = "";
    }

}