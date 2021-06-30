using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace iSpeakWebApp.Models
{
    public class SettingsModel
    {
        [Required]
        [Display(Name = "Auto Entry For Cash Payments")]
        public Guid? AutoEntryForCashPayments { get; set; }
        public static ModelMember COL_AutoEntryForCashPayments = new ModelMember { Name = "AutoEntryForCashPayments", Id = new Guid("5c62ee59-03a9-453a-95c9-a234f537adf1") };
        public string AutoEntryForCashPayments_Notes { get; set; }
        public static ModelMember COL_AutoEntryForCashPayments_Notes = new ModelMember { Name = "AutoEntryForCashPayments_Notes" };


        [Required]
        [Display(Name = "User Set Role Allowed")]
        public Guid? UserSetRoleAllowed { get; set; }
        public static ModelMember COL_UserSetRoleAllowed = new ModelMember { Name = "UserSetRoleAllowed", Display = "User Set Role Allowed", Id = new Guid("25f53554-3b9d-4d3b-a8e5-58d921950987") };
        public string UserSetRoleAllowed_Notes { get; set; }
        public static ModelMember COL_UserSetRoleAllowed_Notes = new ModelMember { Name = "UserSetRoleAllowed_Notes" };


        [Required]
        [Display(Name = "Student Role")]
        public Guid? StudentRole { get; set; }
        public static ModelMember COL_StudentRole = new ModelMember { Name = "StudentRole", Display = "Student Role", Id = new Guid("A94B2FFC-3547-40CB-96CD-F82729768926") };
        public string StudentRole_Notes { get; set; }
        public static ModelMember COL_StudentRole_Notes = new ModelMember { Name = "StudentRole_Notes" };


        [Required]
        [Display(Name = "Tutor Role")]
        public Guid? TutorRole { get; set; }
        public static ModelMember COL_TutorRole = new ModelMember { Name = "TutorRole", Display="Tutor Role", Id = new Guid("20D2F1DA-2ACC-4E92-850C-2B260848FB8F") };
        public string TutorRole_Notes { get; set; }
        public static ModelMember COL_TutorRole_Notes = new ModelMember { Name = "TutorRole_Notes" };


        [Required]
        [Display(Name = "Reset Password")]
        public string ResetPassword { get; set; }
        public static ModelMember COL_ResetPassword = new ModelMember { Name = "ResetPassword", Id = new Guid("01f2d64d-f402-4a96-b854-128f9a9ae42f") };
        public string ResetPassword_Notes { get; set; }
        public static ModelMember COL_ResetPassword_Notes = new ModelMember { Name = "ResetPassword_Notes" };


        [Display(Name = "Full Access for Tutor Schedules")]
        public string FullAccessForTutorSchedules { get; set; }
        public static ModelMember COL_FullAccessForTutorSchedules = new ModelMember { Name = "FullAccessForTutorSchedules", Id = new Guid("9b5ab31f-ce5e-4942-9e07-0fe107058910") };
        public string FullAccessForTutorSchedules_Notes { get; set; }
        public static ModelMember COL_FullAccessForTutorSchedules_Notes = new ModelMember { Name = "FullAccessForTutorSchedules_Notes" };
        public List<string> FullAccessForTutorSchedules_List { get; set; }
        public static ModelMember COL_FullAccessForTutorSchedules_List = new ModelMember { Name = "FullAccessForTutorSchedules_List" };


        [Display(Name = "Show Only Own User Data")]
        public string ShowOnlyOwnUserData { get; set; }
        public static ModelMember COL_ShowOnlyOwnUserData = new ModelMember { Name = "ShowOnlyOwnUserData", Id = new Guid("70adb944-1917-4cd7-817d-6ca5fa789d5e") };
        public string ShowOnlyOwnUserData_Notes { get; set; }
        public static ModelMember COL_ShowOnlyOwnUserData_Notes = new ModelMember { Name = "ShowOnlyOwnUserData_Notes" };
        public List<string> ShowOnlyOwnUserData_List { get; set; }
        public static ModelMember COL_ShowOnlyOwnUserData_List = new ModelMember { Name = "ShowOnlyOwnUserData_List" };


        [Display(Name = "Payroll Rates Roles")]
        public string PayrollRatesRoles { get; set; }
        public static ModelMember COL_PayrollRatesRoles = new ModelMember { Name = "PayrollRatesRoles", Id = new Guid("65644f81-e712-4a72-966b-4ae1ee2462c2") };
        public string PayrollRatesRoles_Notes { get; set; }
        public static ModelMember COL_PayrollRatesRoles_Notes = new ModelMember { Name = "PayrollRatesRoles_Notes" };
        public List<string> PayrollRatesRoles_List { get; set; }
        public static ModelMember COL_PayrollRatesRoles_List = new ModelMember { Name = "PayrollRatesRoles_List" };


        /******************************************************************************************************************************************************/
    }
}