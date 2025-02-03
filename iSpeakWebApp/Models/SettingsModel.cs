using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace iSpeakWebApp.Models
{
    public class SettingsModel
    {
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
        public static ModelMember COL_ResetPassword = new ModelMember { Name = "ResetPassword", Display = "Reset Password", Id = new Guid("01f2d64d-f402-4a96-b854-128f9a9ae42f") };
        public string ResetPassword_Notes { get; set; }
        public static ModelMember COL_ResetPassword_Notes = new ModelMember { Name = "ResetPassword_Notes" };


        [Display(Name = "Full Access for Tutor Schedules")]
        public string FullAccessForTutorSchedules { get; set; }
        public static ModelMember COL_FullAccessForTutorSchedules = new ModelMember { Name = "FullAccessForTutorSchedules", Display = "Full Access for Tutor Schedules", Id = new Guid("9b5ab31f-ce5e-4942-9e07-0fe107058910") };
        public string FullAccessForTutorSchedules_Notes { get; set; }
        public static ModelMember COL_FullAccessForTutorSchedules_Notes = new ModelMember { Name = "FullAccessForTutorSchedules_Notes" };
        public List<string> FullAccessForTutorSchedules_List { get; set; }
        public static ModelMember COL_FullAccessForTutorSchedules_List = new ModelMember { Name = "FullAccessForTutorSchedules_List" };


        [Display(Name = "Show Only Own User Data")]
        public string ShowOnlyOwnUserData { get; set; }
        public static ModelMember COL_ShowOnlyOwnUserData = new ModelMember { Name = "ShowOnlyOwnUserData", Display = "Show Only Own User Data", Id = new Guid("70adb944-1917-4cd7-817d-6ca5fa789d5e") };
        public string ShowOnlyOwnUserData_Notes { get; set; }
        public static ModelMember COL_ShowOnlyOwnUserData_Notes = new ModelMember { Name = "ShowOnlyOwnUserData_Notes" };
        public List<string> ShowOnlyOwnUserData_List { get; set; }
        public static ModelMember COL_ShowOnlyOwnUserData_List = new ModelMember { Name = "ShowOnlyOwnUserData_List" };


        [Display(Name = "Payroll Rates Roles")]
        public string PayrollRatesRoles { get; set; }
        public static ModelMember COL_PayrollRatesRoles = new ModelMember { Name = "PayrollRatesRoles", Display = "Rates Roles", Id = new Guid("65644f81-e712-4a72-966b-4ae1ee2462c2") };
        public string PayrollRatesRoles_Notes { get; set; }
        public static ModelMember COL_PayrollRatesRoles_Notes = new ModelMember { Name = "PayrollRatesRoles_Notes" };
        public List<string> PayrollRatesRoles_List { get; set; }
        public static ModelMember COL_PayrollRatesRoles_List = new ModelMember { Name = "PayrollRatesRoles_List" };


        [Display(Name = "Club Classroom Link")]
        public string ClubClassroomLink { get; set; }
        public static ModelMember COL_ClubClassroomLink = new ModelMember { Name = "ClubClassroomLink", Display = "Club Classroom Link", Id = new Guid("596AA212-1A9C-4FF8-B95F-3EA414D262FF") };


        public string AdvertisementBanner1 { get; set; }
        public static ModelMember COL_AdvertisementBanner1 = new ModelMember { Name = "AdvertisementBanner1", Display = "Advertisement Banner 1", Id = new Guid("40C4CB20-26C3-4196-AA04-6E03A418225B") };

        public string ClubScheduleImage1 { get; set; }
        public static ModelMember COL_ClubScheduleImage1 = new ModelMember { Name = "ClubScheduleImage1", Display = "Club Schedule Image 1", Id = new Guid("DE5D02AA-9E30-4ADA-9655-7F34A953A213") };

        public string ClubScheduleImage2 { get; set; }
        public static ModelMember COL_ClubScheduleImage2 = new ModelMember { Name = "ClubScheduleImage2", Display = "Club Schedule Image 2", Id = new Guid("000416E9-1022-43BF-AD2E-683C257F9EA7") };

        public string ClubScheduleImage3 { get; set; }
        public static ModelMember COL_ClubScheduleImage3 = new ModelMember { Name = "ClubScheduleImage3", Display = "Club Schedule Image 3", Id = new Guid("ABDC9DFB-8B07-4AA5-A550-C80ED1C40BE2") };


        /******************************************************************************************************************************************************/
    }
}