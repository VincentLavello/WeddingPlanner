using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models
{
public class User
{
    [Key]
    public int UserId {get;set;}
    [Required]
    [MinLength(2, ErrorMessage="First Name requires 2 characters.")]
    public string FirstName {get;set;}
    [Required]
    [MinLength(2, ErrorMessage="Last Name requires 2 characters.")]
    public string LastName {get;set;}
    [EmailAddress]
    [Required]
    public string Email {get;set;}
    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer.")]
    public string Password {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
    // Will not be mapped to your users table.
    public List<RSVP> Attending {get;set;}    
    [NotMapped]
    [Compare("Password")]
    [MinLength(8, ErrorMessage="Passwords must match.")]
    [DataType(DataType.Password)]
    public string Confirm {get;set;}
    // public List<Transaction> Transactions {get;set;}
    public User(){}
    public User(string firstName, string lastName, string email, string pwd)
    {
        FirstName=firstName; 
        LastName=lastName; 
        Email=email; 
        Password=pwd;
    }
    public override string ToString() 
    {
        return $"{this.FirstName} {this.LastName}";
    }
    [NotMapped]
    public string FullName {
        get {return $"{FirstName} {LastName}";}
    }
  }//class
}//namespace