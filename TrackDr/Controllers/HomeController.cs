
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TrackDr.Models;
using TrackDr.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TrackDr.Controllers
{
    // this allows you to access onnly if you are logged in 
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDatabaseHelper _dbHelper;
        private readonly IGAPIHelper _gAPIHelper;
        private readonly IBDAPIHelper _bDAPIHelper;
        public HomeController(IDatabaseHelper dbHelper, IGAPIHelper gAPIHelper, IBDAPIHelper bDAPIHelper)
        {
            _dbHelper = dbHelper;
            _gAPIHelper = gAPIHelper;
            _bDAPIHelper = bDAPIHelper;
        }

        // this method checks to see if a user is registered based on the ParentId as well as the ASP Id
        // if the user does not exist in the database, they are redirected to a registration page after they register with ASP
        // if they are already registerd with the database, they will be redirected to the search/home page
        [AllowAnonymous]
        public IActionResult Index(Doctor doctor)
        {
            if (User.Identity.IsAuthenticated)
            {
                AspNetUsers currentUser = _dbHelper.GetCurrentUser(User.Identity.Name);
                if (_dbHelper.FindParentById(currentUser.Id) == null)
                {
                    if (doctor.DoctorId != null)
                    {
                        return View("RegisterUser", doctor);
                    }
                    else
                    {
                        return View("RegisterUser");
                    }
                }
            }
            return View("Search");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // this method is the view for the list doctor
        [AllowAnonymous]
        public IActionResult ListDoctor()
        {
            return View();
        }

        // this method sends the user's input to the API search method 
        // and returns a list of doctors that corrlate with the information the user entered
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Search(string userInput, string userState)
        {
            if (TempData["Doctor"] != null)
            {
                string stringDoctor = TempData["Doctor"].ToString();
                Doctor doctor = (Doctor)JsonConvert.DeserializeObject(stringDoctor);
                _dbHelper.AddNewDoctor(doctor);
            }
            Rootobject result;
            if (userInput == null && userState == null)
            {
                result = await _bDAPIHelper.GetDoctorList();
            }
            else
            {
                result = await _bDAPIHelper.GetDoctorList(userInput, userState);
            }
            return View("ListDoctors", result);
        }

        // this method adds a doctor to the database if they have not been added before
        // the doctor's UID as well as their first name is stored
        [AllowAnonymous]
        public IActionResult AddDoctor(Doctor doctor)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["Doctor"] = JsonConvert.SerializeObject(doctor);
                return RedirectToAction("SavedDoctors");
            }
            AspNetUsers thisUser = _dbHelper.GetCurrentUser(User.Identity.Name);
            ParentDoctor newParentDoctor = new ParentDoctor();
            if (ModelState.IsValid)
            {
                Doctor newDoctor = new Doctor();
                newDoctor.DoctorId = doctor.DoctorId;
                newDoctor.FirstName = doctor.FirstName;
                if (_dbHelper.CanAddDoctor(newDoctor))
                {
                    _dbHelper.AddNewDoctor(newDoctor);
                }
                newParentDoctor.ParentId = thisUser.Id;
                newParentDoctor.DoctorId = doctor.DoctorId;
                if (_dbHelper.CanAddParentDoctorRelationship(thisUser.Id, newDoctor.DoctorId))
                {
                    _dbHelper.AddNewParentDoctorRelationship(newParentDoctor);
                }
                return RedirectToAction("SavedDoctors");
            }
            return View("Search");
        }

        // this method returns a list of saved doctors by the user based on the user's ASP Id
        public IActionResult SavedDoctors()
        {
            if (TempData["Doctor"] != null)
            {
                string stringDoctor = TempData["Doctor"].ToString();
                Doctor doctor = JsonConvert.DeserializeObject<Doctor>(stringDoctor);
                if (_dbHelper.CanAddDoctor(doctor))
                {
                    _dbHelper.AddNewDoctor(doctor);
                }
                AspNetUsers thisUser = _dbHelper.GetCurrentUser(User.Identity.Name);
                ParentDoctor newParentDoctor = new ParentDoctor();
                if (_dbHelper.FindParentById(thisUser.Id) == null)
                {
                    return RedirectToAction("RegisterUser");
                }
                newParentDoctor.ParentId = thisUser.Id;
                newParentDoctor.DoctorId = doctor.DoctorId;
                if (_dbHelper.CanAddParentDoctorRelationship(thisUser.Id, doctor.DoctorId))
                {
                    _dbHelper.AddNewParentDoctorRelationship(newParentDoctor);
                }
            }
            List<Doctor> doctorList = _dbHelper.GetListOfCurrentUsersDoctors(User.Identity.Name);
            return View(doctorList);
        }

        // this method returns extra details on a doctor chosen by the user
        public async Task<IActionResult> DoctorDetails(string doctorId)
        {
            SingleDoctor doctor = await _bDAPIHelper.GetDoctor(doctorId);
            string startAddress = _gAPIHelper.GooglefyUserAddress(User.Identity.Name);
            string endAddress = _gAPIHelper.GooglefyDoctorAddress(doctor);
            doctor.DistanceInMiles = _gAPIHelper.GetDistanceInMiles(_gAPIHelper.GetTravelRoutes(startAddress, endAddress));
            doctor.DistanceInTime = _gAPIHelper.GetDistanceInTime(_gAPIHelper.GetTravelRoutes(startAddress, endAddress));
            return View(doctor);
        }

        // this method deletes a doctor based on the doctor's UID
        public IActionResult DeleteDoctor(string doctorId)
        {
            var userDelete = _dbHelper.GetCurrentUser(User.Identity.Name);
            var foundDoctor = _dbHelper.FindParentDoctorRelationship(doctorId, userDelete);
            _dbHelper.DeleteDoctor(foundDoctor);
            return RedirectToAction("SavedDoctors");
        }

        //saves the user's address to the UserDb as well as the user's Id number and their ASP Id
        [HttpPost]
        [AllowAnonymous]
        public IActionResult RegisterUser(Parent newUserInfo)
        {
            if (ModelState.IsValid)
            {
                AspNetUsers thisUser = _dbHelper.GetCurrentUser(User.Identity.Name);
                Parent newUser = new Parent();

                newUser.HouseNumber = newUserInfo.HouseNumber;
                newUser.Street = newUserInfo.Street;
                newUser.Street2 = newUserInfo.Street2;
                newUser.City = newUserInfo.City;
                newUser.State = newUserInfo.State;
                newUser.ZipCode = newUserInfo.ZipCode;
                newUser.ParentId = thisUser.Id;
                newUser.PhoneNumber = newUserInfo.PhoneNumber;
                newUser.Email = thisUser.Email;

                _dbHelper.AddNewParent(newUser);
                if (TempData.Count() != 0)

                {
                    string stringDoctor = TempData["Doctor"].ToString();
                    Doctor doctor = JsonConvert.DeserializeObject<Doctor>(stringDoctor);
                    if (_dbHelper.CanAddDoctor(doctor))
                    {
                        _dbHelper.AddNewDoctor(doctor);
                    }

                    ParentDoctor newParentDoctor = new ParentDoctor();
                    if (newParentDoctor.ParentId == null)
                    {
                        RegisterUser();
                    }
                    newParentDoctor.ParentId = thisUser.Id;
                    newParentDoctor.DoctorId = doctor.DoctorId;
                    if (_dbHelper.CanAddParentDoctorRelationship(thisUser.Id, doctor.DoctorId))
                    {
                        _dbHelper.AddNewParentDoctorRelationship(newParentDoctor);
                    }
                    List<Doctor> doctorList = _dbHelper.GetListOfCurrentUsersDoctors(User.Identity.Name);
                    return View("SavedDoctors", doctorList);
                }
                return View("Search");
            }
            else
                return View(newUserInfo);
        }

        public IActionResult RegisterUser()
        {
            return View();
        }

        // this method shows the user's information based on their ASP Id
        // if the user is not found, the user will be redirected to the search page
        public IActionResult UserInformation()
        {
            Parent foundParent = _dbHelper.FindParentById(_dbHelper.GetCurrentUser(User.Identity.Name).Id);
            if (foundParent != null)
            {
                return View("UserInformation", foundParent);
            }
            return View("Search");
        }

        // this method allows the user to edit their information 
        // which will then be saved to the database
        [HttpPost]
        public IActionResult EditUserInformation(Parent user)
        {
            if (ModelState.IsValid)
            {

                Parent updatedUser = new Parent();
                updatedUser.HouseNumber = user.HouseNumber;
                updatedUser.Street = user.Street;
                updatedUser.Street2 = user.Street2;
                updatedUser.City = user.City;
                updatedUser.State = user.State;
                updatedUser.ZipCode = user.ZipCode;
                updatedUser.ParentId = user.ParentId;
                updatedUser.Email = user.Email;
                updatedUser.PhoneNumber = user.PhoneNumber;
                updatedUser.ParentId = user.ParentId;

                _dbHelper.UpdateParent(updatedUser);
                return View("UserInformation", updatedUser);
            }
            return View("EditUserInformation", user);
        }

        // this method redirects the user to the edit user information page if that user exists in the database
        [HttpGet]
        public IActionResult EditUserInformation(string ParentId)
        {
            Parent found = _dbHelper.FindParentById(ParentId);
            if (found != null)
            {
                return View("EditUserInformation", found);
            }
            return View("Search");
        }

        // this returns a list of unique insurance base names
        [AllowAnonymous]
        public IActionResult BaseInsurance()
        {
            var insuranceBaseName = _dbHelper.GetAllBaseInsuranceNames();
            return View(insuranceBaseName);
        }

        [AllowAnonymous]
        public IActionResult SpecialtyInsuranceNames(string baseName)
        {
            var insuranceSpecialtyName = _dbHelper.GetAllSpecialtyInsuranceNames(baseName);
            return View(insuranceSpecialtyName);
        }

        // this returns a list of doctors based on the user's insurance choice
        [AllowAnonymous]
        public async Task<IActionResult> ListDoctorsBasedOnInsurance(string specialtyName)
        {
            string uid = _dbHelper.GetSpecialtyUID(specialtyName);
            Rootobject result;
            result = await _bDAPIHelper.GetDoctorsBaseOnInsurance(uid);
            return View("ListDoctors", result);
        }
    }
}
