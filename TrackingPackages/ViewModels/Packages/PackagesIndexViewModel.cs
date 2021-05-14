using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackingPackages.Models;

namespace TrackingPackages.ViewModels.Packages
{
    public class PackagesIndexViewModel
    {
        public List<SinglePackage> Packages { get; set; } = new List<SinglePackage>();
    }

    public class SinglePackage
    {
        public int Id { get; set; }
        public string TrackingCode { get; set; }
        public DateTime ReceiveDate { get; set; }
        public string Status { get; set; }
        public SinglePackage(){ }
        public SinglePackage(Package package)
        {
            Id = package.Id;
            TrackingCode = package.TrackingCode;
            ReceiveDate = package.ReceiveDate;
            Status = package.Status.ToString();
        }
    }
}