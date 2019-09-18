using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrackDr.Models
{
    public partial class Parent
    {
        public Parent()
        {
            ParentDoctor = new HashSet<ParentDoctor>();
        }

        public enum StateAbbreviations
        {
            AL, AK, AZ, AR, CA, CO, CT, DE, FL, GA, HI, ID,
            IL, IN, IA, KS, KY, LA, ME, MD, MA, MI, MN, MS,
            MO, MT, NE, NV, NH, NJ, NM, NY, NC, ND, OH, OK,
            OR, PA, RI, SC, SD, TN, TX, UT, VT, VA, WA, WV,
            WI, WY
        }
        
        public string ParentId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "House Number is a required field")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Must be a number")]
        public string HouseNumber { get; set; }
        [Required(ErrorMessage = "Street is a required field")]
        public string Street { get; set; }
        public string Street2 { get; set; }
        [Required(ErrorMessage = "City is a required field")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is a required field")]
        public string State { get; set; }
        [Required (ErrorMessage="Zip Code is a required field")]
        [Range(0, 99999, ErrorMessage = "Zip Code must be 5 digits")]
        public string ZipCode { get; set; }
        public string InsuranceBaseName { get; set; }

        public virtual ICollection<ParentDoctor> ParentDoctor { get; set; }
    }
}
