using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace test_webApiIdentity2.Models
{
    public class user_MoreDetails
    {
        [Key]
        [Required]
        [Display(Name = "User ID")]
        public string userID { get;set;}
         [Required]
         [Display(Name = "Residence City")]
        public string residenceCity { get; set; }
         [Required]
         [Display(Name = "Work Place City")]
        public string workplaceCity { get; set; }
        
         [Display(Name = "Occupation")]
        public string profession { get; set; }
         [Display(Name = "Work Frequency")]
        public string workFrequency { get; set; }
         [Display(Name = "Transport Mode")]
        public string transportMode { get; set; }
    }
}