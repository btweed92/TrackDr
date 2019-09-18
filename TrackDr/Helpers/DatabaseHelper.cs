using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TrackDr.Models;

namespace TrackDr.Helpers
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly TrackDrDbContext _context;
        private readonly IBDAPIHelper _bDAPIHelper;
        public DatabaseHelper(TrackDrDbContext context, IBDAPIHelper bDAPIHelper)
        {
            _context = context;
            _bDAPIHelper = bDAPIHelper;
        }

        // this method checks to see if a new doctor can be added to the database
        public bool CanAddDoctor(Doctor doctor)
        {
            Doctor foundDoctor = FindDoctorById(doctor.DoctorId);
            if (foundDoctor == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // this method checks to see if a new parent-doctor relationship can be added
        public bool CanAddParentDoctorRelationship(string parent, string doctor)
        {
            bool isValid = false;
            List<ParentDoctor> relationshipList = _context.ParentDoctor.ToList();
            List<ParentDoctor> currentParentDoctors = new List<ParentDoctor>();
            foreach (ParentDoctor relationship in relationshipList)
            {
                if (relationship.ParentId == parent)
                {
                    currentParentDoctors.Add(relationship);
                }
            }
            if(currentParentDoctors.Count == 0)
            {
                isValid = true;
            }
            foreach (ParentDoctor relationship in currentParentDoctors)
            {
                if (relationship.DoctorId == doctor)
                {
                    isValid = false;
                    break;
                }
                else
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        // this method gets the current user based on ASP registration information
        public AspNetUsers GetCurrentUser(string userName)
        {
                return _context.AspNetUsers.Where(u => u.Email == userName).First();
        }

        // this method gets the current user based on the current user's ParentId and the current ASP Id
        public Parent GetCurrentParent(AspNetUsers currentUser)
        {
            return _context.Parent.Find(currentUser.Id);
        }

        // this method adds a new doctor to the Doctor database
        public void AddNewDoctor(Doctor newDoctor)
        {
            _context.Doctor.Add(newDoctor);
            _context.SaveChanges();
        }

        // this method adds a new parent doctor relationship if it hasnt been added yet
        public void AddNewParentDoctorRelationship(ParentDoctor newParentDoctor)
        {
            _context.ParentDoctor.Add(newParentDoctor);
            _context.SaveChanges();
        }

        // this method adds a new parent to the database
        public void AddNewParent(Parent newUser)
        {
            _context.Parent.Add(newUser);
            _context.SaveChanges();
        }

        // this puts doctor's UIDs in a list based on the user's ID
        // ie.  puts the UID's of doctors affiliated with the user into a list
        // this will return a list of strings that hold the doctor's UIDs 
        public List<Doctor> GetListOfCurrentUsersDoctors(string userName)
        {
            AspNetUsers thisUser = GetCurrentUser(userName);
            List<Doctor> doctorIdList = new List<Doctor>();
            var doctor = new Doctor();
            List<ParentDoctor> savedList = _context.ParentDoctor.Where(u => u.ParentId == thisUser.Id).ToList();
            foreach (ParentDoctor relationship in savedList)
            {
                if (relationship.ParentId == thisUser.Id)
                {
                    doctor = GetDoctorFromDatabase(relationship.DoctorId);
                    doctorIdList.Add(doctor);
                }
            }
            return doctorIdList;
        }

        // this method gets a doctor from the databse based on the doctor's UID
        public Doctor GetDoctorFromDatabase(string doctorUID)
        {
            return _context.Doctor.Find(doctorUID);
        }

        // this method finds the parent in the database that is the same as the ASP user's Id
        public Parent FindParentById(string userId)
        {
            return _context.Parent.Find(userId);
        }

        // this finds a doctor in the database based on their Id
        public Doctor FindDoctorById(string doctorId)
        {
           return _context.Doctor.Find(doctorId);
        }

        // this updates the parent based on any edits the user has made to items such as their state
        public void UpdateParent(Parent updatedUser)
        {
            _context.Entry(updatedUser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.Update(updatedUser);
            _context.SaveChanges();
        }

        // this method deletes a doctor
        public void DeleteDoctor(ParentDoctor parentDoctor)
        {
            _context.ParentDoctor.Remove(parentDoctor);
            _context.SaveChanges();
        }

        // tthis method finds a doctor-parent relationship based on the ASP user's Id and the doctor's UID
        public ParentDoctor FindParentDoctorRelationship(string doctorId, AspNetUsers currentUser)
        {
            List<ParentDoctor> parentDoctorRelationship = _context.ParentDoctor.ToList();
            foreach (var relationship in parentDoctorRelationship)
            {
                if (relationship.DoctorId == doctorId)
                {
                    if(relationship.ParentId == currentUser.Id)
                    {
                        return relationship;
                    }
                }
            }
            return _context.ParentDoctor.Find(doctorId);
        }

        // this method returns a list of all base insurance names, without repeats
        public List<string> GetAllBaseInsuranceNames()
        {
            List<SavedInsurance> insuranceList = _context.SavedInsurance.ToList();
            List<string> insuranceBaseNameList = new List<string>();
            foreach (var insurance in insuranceList)
            {
                foreach (var baseName in insurance.InsuranceBaseName)
                {
                    if (!insuranceBaseNameList.Contains(insurance.InsuranceBaseName))
                    {
                        insuranceBaseNameList.Add(insurance.InsuranceBaseName);
                    }
                }
            }
            return insuranceBaseNameList;
        }

        // this method returns a list of specialty insurances based on which base name was chosen by the user
        public List<string> GetAllSpecialtyInsuranceNames(string baseName)
        {
            List<SavedInsurance> insuranceList = _context.SavedInsurance.ToList();
            List<string> specialtyNames = new List<string>();
            foreach (var insurance in insuranceList)
            {
                if (insurance.InsuranceBaseName == baseName)
                {
                    specialtyNames.Add(insurance.InsuranceSpecialtyName);
                }
            }
            return specialtyNames;
        }

        // this method returns the specialty insurance UID based on which specialty insurance the user chose
        public string GetSpecialtyUID(string specialtyName)
        {
            List<SavedInsurance> insuranceList = _context.SavedInsurance.ToList();
            foreach (var insurance in insuranceList)
            {
                if (insurance.InsuranceSpecialtyName.Contains(specialtyName))
                {
                     return insurance.InsuranceUid;
                }
            }
            return null;
        }
    }
}