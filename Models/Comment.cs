using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TheWallCSharp.Models
{
    public class Comment
    {
        [Key]
        public int CommentId {get;set;}

        public string Content {get;set;}

        public int UserId {get;set;}

        public int MessageId {get;set;}

        public User CommentCreator {get;set;}

        public Message MessageCommented {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}