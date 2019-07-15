using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TheWallCSharp.Models
{
    public class Like
    {
        [Key]
        public int LikeId {get;set;}

        public int UserId {get;set;}

        public int MessageId {get;set;}

        public User UserLiking {get;set;}

        public Message MessageLiked {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;

        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}