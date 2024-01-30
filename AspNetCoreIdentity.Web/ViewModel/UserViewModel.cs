﻿using AspNetCoreIdentity.Web.Models;

namespace AspNetCoreIdentity.Web.ViewModel
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhotoUrl { get; set; }

        public List<Messages> MessagesList { get; set; }

    }
}
