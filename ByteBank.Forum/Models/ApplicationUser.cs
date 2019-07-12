using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByteBank.Forum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}