using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackingPackages.Models;

namespace TrackingPackages.ViewModels.Packages
{
    public class PackageSearchViewModel
    {
        public Status? Status{ get; set; }

        public int?  CheckpointId { get; set; }
    }
}