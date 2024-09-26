using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Models;
using Models.DTO;

namespace DbModels
{
    [Table("Users", Schema = "dbo")]
    public class csUserDbM : csUser
	{
        [Key]     
        public override Guid UserId { get; set; }

        [Required]
        public override string UserName { get; set; }

        [Required]
        public override string Password { get; set; }

        [Required]
        public override string Role { get; set; }
    }
}

