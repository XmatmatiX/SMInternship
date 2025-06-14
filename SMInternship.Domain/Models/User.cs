﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMInternship.Domain.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
        [MaxLength(500)]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Wrong email address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Wrong phone number")]
        public string? PhoneNumber { get; set; }
    }
}
