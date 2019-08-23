using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models
{
public class RSVP
        {
                [Key]
                [Required]
                public int RSVP_ID{get;set;}
                public int WeddingId {get;set;}
                public int UserId {get;set;}
                public User Guest {get;set;}
                 public Wedding ThisWedding {get;set;}
                public RSVP(){}
                public RSVP(int _WeddingId, int userid)
                {
                        WeddingId=_WeddingId;
                        UserId=userid;
                }
        }      

}