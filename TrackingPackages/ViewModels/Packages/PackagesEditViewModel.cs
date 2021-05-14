using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackingPackages.Models;

namespace TrackingPackages.ViewModels.Packages
{
    public class PackagesEditViewModel
    {
        public int Id { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public PacketSize? PacketSize { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string Location { get; set; }
        public string Weight { get; set; }
        public bool HasValueToPay { get; set; }
        public bool IsFragile { get; set; }
    }

    public static class PackagesEditViewModelExtensions
    {
        public static Package UpdatePackage(this PackagesEditViewModel viewModel, Package package)
        {
            package.HasValueToPay = viewModel.HasValueToPay;
            package.IsFragile = viewModel.IsFragile;
            package.PacketSize = viewModel.PacketSize.Value;
            package.Weight = viewModel.Weight;
            return package;
        }
    }
}