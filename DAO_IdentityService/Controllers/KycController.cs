using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.KYCModels;
using Helpers.Models.SharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_IdentityService.Controllers
{
    /// <summary>
    ///  KycController contains Kyc operation methods
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class KycController : ControllerBase
    {
        /// <summary>
        ///  Submits form data for the KYC verification
        /// </summary>
        /// <param>User information</param>
        /// <returns>Generic Simple Response class</returns>
        [HttpPost("SubmitKYCFile", Name = "SubmitKYCFile")]
        public SimpleResponse SubmitKYCFile([FromQuery] string Type, string Name, string Surname, string DoB, string Email, string Country, string DocumentNumber, string IssueDate, string ExpiryDate, string UserID)
        {
            try
            {
                var userModel = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/UserKYC/GetUserId?id=" + UserID));

                if (userModel == null || userModel.UserKYCID <= 0)
                {
                    //Create KYC applicant
                    var Person = new KYCPerson() { type = "PERSON", first_name = Name, last_name = Surname, dob = DoB, residence_country = Country, email = Email };
                    var model = Helpers.Serializers.DeserializeJson<KYCPersonResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/applicants", Helpers.Serializers.SerializeJson(Person), Program._settings.KYCID));

                    if (model != null)
                    {
                        if (Request.HasFormContentType)
                        {
                            Guid g = Guid.NewGuid();

                            var form = Request.Form;

                            if (form.Files.Count > 0)
                            {
                                //Create File Request
                                var model2 = Helpers.Request.UploadFiletoKYCAID(Program._settings.KYCURL + "/files", form.Files[0], Program._settings.KYCID);

                                //Create applicant document request
                                var Doc = new KYCDocument() { applicant_id = model.applicant_id, type = Type, document_number = DocumentNumber, issue_date = IssueDate, expiry_date = ExpiryDate, front_side_id = model2.file_id };


                                KYCFileResponse model3 = new KYCFileResponse();
                                if (form.Files.Count > 1)
                                {
                                    model3 = Helpers.Request.UploadFiletoKYCAID(Program._settings.KYCURL + "/files", form.Files[1], Program._settings.KYCID);

                                    Doc.back_side_id = model3.file_id;
                                }


                                var model4 = Helpers.Serializers.DeserializeJson<KYCDocumentResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/documents", Helpers.Serializers.SerializeJson(Doc), Program._settings.KYCID));

                                //Create verification request

                                var Verify = new KYCVerification() { applicant_id = model.applicant_id, types = new List<string>() { "DOCUMENT" }, callback_url = Program._settings.WebURLforKYCResponse };
                                var model5 = Helpers.Serializers.DeserializeJson<KYCVerificationResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/verifications", Helpers.Serializers.SerializeJson(Verify), Program._settings.KYCID));

                                //New KYC request for Db record
                                var NewKYCObj = new UserKYCDto() { UserID = Convert.ToInt32(UserID), ApplicantId = model.applicant_id, VerificationId = model5.verification_id, FileId1 = model2.file_id, KYCStatus = "pending", DocumentId = model4.document_id };

                                if (form.Files.Count > 1)
                                    NewKYCObj.FileId2 = model3.file_id;

                                var NewKYC = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Post(Program._settings.Service_Db_Url + "/UserKYC/Post", Helpers.Serializers.SerializeJson(NewKYCObj)));

                                if (NewKYC.UserKYCID <= 0)
                                {
                                    return new SimpleResponse() { Success = false, Message = "KYC error" };
                                }
                            }
                            else
                            {
                                return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                            }
                        }
                        else
                        {
                            return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                        }
                    }
                    else
                    {
                        Program.monitizer.AddConsole("KYCAID connection error.");
                        return new SimpleResponse() { Success = false, Message = "An error occurred during operations. Please try again later." };
                    }

                }
                else
                {
                    //update applicant

                    var Person = new KYCPerson() { type = "PERSON", first_name = Name, last_name = Surname, dob = DoB, residence_country = Country, email = Email };
                    var model = Helpers.Serializers.DeserializeJson<KYCPersonResponse>(Helpers.Request.KYCPatch(Program._settings.KYCURL + "/applicants/" + userModel.ApplicantId, Helpers.Serializers.SerializeJson(Person), Program._settings.KYCID));
                    if (Request.HasFormContentType)
                    {
                        Guid g = Guid.NewGuid();

                        var form = Request.Form;

                        if (form.Files.Count > 0)
                        {
                            //Update File 
                            var model2 = Helpers.Request.PutFiletoKYCAID(Program._settings.KYCURL + "/files/" + userModel.FileId1, form.Files[0], Program._settings.KYCID);

                            //Update applicant document request
                            var Doc = new KYCDocument() { applicant_id = model.applicant_id, type = Type, document_number = DocumentNumber, issue_date = IssueDate, expiry_date = ExpiryDate, front_side_id = model2.file_id };


                            KYCFileResponse model3 = new KYCFileResponse();
                            if (form.Files.Count > 1)
                            {
                                if (userModel.FileId2 == null)
                                {
                                    model3 = Helpers.Request.UploadFiletoKYCAID(Program._settings.KYCURL + "/files", form.Files[1], Program._settings.KYCID);
                                }
                                else
                                {
                                    model3 = Helpers.Request.PutFiletoKYCAID(Program._settings.KYCURL + "/files/" + userModel.FileId2, form.Files[0], Program._settings.KYCID);
                                }

                                Doc.back_side_id = model3.file_id;
                            }

                            var model4 = Helpers.Serializers.DeserializeJson<KYCDocumentResponse>(Helpers.Request.KYCPatch(Program._settings.KYCURL + "/documents/" + userModel.DocumentId, Helpers.Serializers.SerializeJson(Doc), Program._settings.KYCID));

                            var Verify = new KYCVerification() { applicant_id = model.applicant_id, types = new List<string>() { "DOCUMENT" }, callback_url = Program._settings.WebURLforKYCResponse };
                            var model5 = Helpers.Serializers.DeserializeJson<KYCVerificationResponse>(Helpers.Request.KYCPost(Program._settings.KYCURL + "/verifications", Helpers.Serializers.SerializeJson(Verify), Program._settings.KYCID));

                            //Update KYC Db record
                            var NewKYCObj = new UserKYCDto() { UserID = Convert.ToInt32(UserID), ApplicantId = model.applicant_id, VerificationId = model5.verification_id, FileId1 = model2.file_id, KYCStatus = "pending", DocumentId = model4.document_id, UserKYCID = userModel.UserKYCID };

                            if (form.Files.Count > 1)
                                NewKYCObj.FileId2 = model3.file_id;

                            var NewKYC = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/UserKYC/Update", Helpers.Serializers.SerializeJson(NewKYCObj)));

                            if (NewKYC.UserKYCID <= 0)
                            {
                                return new SimpleResponse() { Success = false, Message = "KYC update error" };
                            }
                        }
                        else
                        {
                            return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                        }
                    }
                    else
                    {
                        return new SimpleResponse() { Success = false, Message = "Please upload KYC Document." };
                    }


                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new SimpleResponse() { Success = false, Message = "Unexpected error" };
            }
            return new SimpleResponse() { Success = true, Message = "KYC completed successfully." };
        }

        /// <summary>
        ///  Receives calllback from the KYC verification site.
        /// </summary>
        /// <param name="Response">KYC information of the user</param>
        /// <returns>KYCCallBack class</returns>
        [HttpPost("KycCallBack", Name = "KycCallBack")]
        public SimpleResponse KycCallBack(KYCCallBack Response)
        {
            Program.monitizer.AddConsole(Response.applicant_id);

            try
            {
                var userModel = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/UserKYC/GetApplicantId?id=" + Response.applicant_id));
                if (userModel != null)
                {
                    userModel.Comment = Response.verifications.document.comment;
                    userModel.Verified = Response.verified;
                    userModel.KYCStatus = Response.status;

                    var KYCModel = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/UserKYC/Update", Helpers.Serializers.SerializeJson(userModel)));
                    if (KYCModel.UserKYCID <= 0)
                    {
                        Program.monitizer.AddConsole("KYC Update error");
                        return new SimpleResponse() { Success = false, Message = "KYC Update error" };
                    }

                    if (Response.verified)
                    {
                        var User = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/Users/GetId?Id=" + userModel.UserID));
                        if (User == null)
                        {
                            Program.monitizer.AddConsole("User not found");
                            return new SimpleResponse() { Success = false, Message = "User not found" };
                        }
                        else
                        {
                            User.KYCStatus = true;
                            var UserUpdate = Helpers.Serializers.DeserializeJson<UserDto>(Helpers.Request.Put(Program._settings.Service_Db_Url + "/Users/Update", Helpers.Serializers.SerializeJson(User)));
                            if (UserUpdate != null && UserUpdate.UserId > 0)
                            {
                                return new SimpleResponse() { Success = true };
                            }
                            else
                            {
                                return new SimpleResponse() { Success = false, Message = "User failed to update" };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new SimpleResponse() { Success = false, Message = "Unexpected error" };
            }
            return new SimpleResponse() { Success = true };

        }

        /// <summary>
        /// Brings countries for KYC form 
        /// </summary>
        /// <returns>List of KYCCountries class</returns>
        [HttpPost("GetKycCountries", Name = "GetKycCountries")]
        public List<KYCCountries> GetKycCountries()
        {
            List<KYCCountries> CountryList = new List<KYCCountries>();
            try
            {
                CountryList = Helpers.Serializers.DeserializeJson<List<KYCCountries>>(Helpers.Request.KYCGet(Program._settings.KYCURL + "/countries", Program._settings.KYCID));
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new List<KYCCountries>();
            }
            return CountryList;

        }

        /// <summary>
        /// Brings KYC status of user 
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>UserKYCDto class</returns>
        [HttpGet("GetKycStatus", Name = "GetKycStatus")]
        public UserKYCDto GetKycStatus(int id)
        {

            UserKYCDto model = new UserKYCDto();
            try
            {
                model = Helpers.Serializers.DeserializeJson<UserKYCDto>(Helpers.Request.Get(Program._settings.Service_Db_Url + "/UserKYC/GetUserId?id=" + id, Helpers.Serializers.SerializeJson(User)));
                if (model == null)
                    model = new UserKYCDto();
            }
            catch (Exception ex)
            {
                Program.monitizer.AddException(ex, LogTypes.ApplicationError);
                return new UserKYCDto();
            }
            return model;
        }

    }
}
