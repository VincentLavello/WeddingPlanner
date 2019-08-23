using System.Collections.Generic;

using System;
using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models
{
    public class Login : IValidatableObject
    {

        [Required]
        [EmailAddress]
        public string Email{ get; set; }
        [Required]
        [MinLength(8)] 
        [DataType(DataType.Password)]
        public string Password{ get; set; }

        public Login(){}
        public Login(string email, string pwd)
        {
            Email=email; 
            Password=pwd;
        }

//@@@@@@@@@@@@@@@@@@@@@@@ ERROR
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                List<ValidationResult> results = new List<ValidationResult>();

                    if (!(Password.Equals(Password))) //temp dummy for later
                    {
                        results.Add(new ValidationResult("Password must match.", new []{"Confirm Password"}));
                    }

                    return results;
            }     
    }
}
