using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TrackingPackages.Models;

namespace TrackingPackages.ViewModels.Packages
{
    public class PackagesCreateViewModel
    {
        public DateTime? ReceiveDate { get; set; }
        public PacketSize? PacketSize { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public string Location { get; set; }
        public string Weight { get; set; }
        public bool HasValueToPay { get; set; }
        public bool IsFragile { get; set; }

        public PackagesCreateViewModel() { }
        public PackagesCreateViewModel(string zipCode, string location, DateTime receivedDate)
        {
            ZipCode = zipCode;
            Location = location;
            ReceiveDate = receivedDate;
        }

    }

    public static class PackagesCreateViewModelExtensions
    {
        public static Package ToPackage(this PackagesCreateViewModel viewModel, string trackingCode)
        {
            return new Package()
            {
                HasValueToPay = viewModel.HasValueToPay,
                IsFragile = viewModel.IsFragile,
                PacketSize = viewModel.PacketSize.Value,
                ReceiveDate = viewModel.ReceiveDate.Value,
                Status = Status.Received,
                Weight = viewModel.Weight,
                TrackingCode = trackingCode
            };
        }
    }
}