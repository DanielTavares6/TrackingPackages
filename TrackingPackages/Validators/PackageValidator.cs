using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackingPackages.Models;
using TrackingPackages.ViewModels.Packages;

namespace TrackingPackages.Validators
{
    public static class PackageValidator
    {
        public static Validator Create(PackagesCreateViewModel viewModel)
        {
            if (viewModel == null)
                return new Validator(false, "Erro");

            if (viewModel.ReceiveDate == null)
                return new Validator(false, "Please add the package receive date");

            if (viewModel.PacketSize == null)
                return new Validator(false, "Please select a packet size");

            if (viewModel.Weight == null)
                return new Validator(false, "Please add the weight of the package");

            if (viewModel.Address == null)
                return new Validator(false, "Please add the address");

            if (viewModel.ZipCode == null)
                return new Validator(false, "Please add the zipcode of the address");

            if (viewModel.Location == null)
                return new Validator(false, "Please add the location");

            return new Validator(true);
        }

        public static Validator Edit(PackagesEditViewModel viewModel, Package package)
        {
            if (viewModel == null)
                return new Validator(false, "Erro");

            if (package == null)
                return new Validator(false, "Invalid package");

            if (viewModel.PacketSize == null)
                return new Validator(false, "Please select a packet size");

            if (viewModel.Weight == null)
                return new Validator(false, "Please add the weight of the package");

            if (viewModel.Address == null)
                return new Validator(false, "Please add the address");

            if (viewModel.ZipCode == null)
                return new Validator(false, "Please add the zipcode of the address");

            if (viewModel.Location == null)
                return new Validator(false, "Please add the location");

            return new Validator(true);
        }

        public static Validator Send(Package package)
        {
            if (package == null)
                return new Validator(false, "Invalid package");

            if (package.Status != Status.Received)
                return new Validator(false, "Package is not on the post office");

            return new Validator(true);
        }

        public static Validator AttemptDeliver(Package package)
        {
            if (package == null)
                return new Validator(false, "Invalid package");

            if (package.Status != Status.InTransit)
                return new Validator(false,"Package must be in status of In transit");

            return new Validator(true);
        }

        public static Validator Deliver(Package package)
        {
            if (package == null)
                return new Validator(false, "Invalid package");

            if (package.Status != Status.AttemptedDelivery)
                return new Validator(false,"Package must be in status of Attempted Delivery");

            return new Validator(true);
        }

        public static Validator Return(Package package)
        {
            if (package == null)
                return new Validator(false, "Invalid package");

            if (package.Status != Status.Returning)
                return new Validator(false,"Package must be in status of Returning Delivery");

            return new Validator(true);
        }
    }
}