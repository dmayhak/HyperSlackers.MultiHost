using ClassroomForOne.Extensions;
using MultiHostDemo.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiHostDemo.Models
{
    [DebuggerDisplay("LogInModel Id: {UserId,nq}, Email: {Email,nq}, Name: {FirstName,nq} {LastName,nq}")]
    public class LogInModel : ModelBase
    {
        [Required]
        [StringLength(256)]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        private LogInModel()
        {
            // private constructor so only EF can create it
        }

        public static LogInModel Get()
        {
            return new LogInModel();
        }

        /// <summary>
        /// Gets the user for log in.
        /// </summary>
        /// <returns>AppUser matching credentials. Null if user does not exist or does not match password.</returns>
        public AppUser GetUserForLogIn()
        {
            // find user by email and password for current host (or global)
            return Repo.Users.FindByEmailAndPassword(this.Email, this.Password);
        }

        internal static void InitAutoMapper()
        {
            // not used
        }
    }
}
