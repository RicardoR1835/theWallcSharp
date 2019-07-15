using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TheWallCSharp.Models
{
    public class Message
    {
        [Key]
        public int MessageId {get;set;}

        public string Content {get;set;}

        public int UserId {get;set;}

        public User Creator {get;set;}

        public List<Like> Likes {get;set;}

        public List<Comment> Comments {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;
       
    }
}