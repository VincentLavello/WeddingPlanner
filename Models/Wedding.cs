using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models
{
public class Wedding
    {
        [Key]
        [Required]
        public int WeddingId {get;set;}
        [ForeignKey("User")]
        public int Planner_Id {get;set;}
        [Required]
        public string Bride{get;set;}
        [Required]
        public string Groom{get;set;}
        [Required]
        public string Address {get;set;}
        [Required]
        public DateTime DateofWedding {get;set;}
        public List<RSVP> Guests {get;set;}

        [NotMapped]
        public string Party 
            {
                get {return $"{Bride} & {Groom}";}
            }
        public Wedding(){}
        public Wedding(int _Planner_Id, string _Bride, string _Groom, string _Address, DateTime _DateofWedding)
            {
                Planner_Id=_Planner_Id;
                Bride= _Bride;
                Groom= _Groom;
                Address= _Address;
                DateofWedding= _DateofWedding;
            }      
    }//class
}//namespace