using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrackingPackages.Validators
{
    public class Validator
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Validator(bool success, string msg = "")
        {
            Success = success;
            ErrorMessage = msg;
        }
    }
}