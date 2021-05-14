using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TrackingPackages.Controllers;
using TrackingPackages.Models;
using TrackingPackages.ViewModels.Packages;

namespace TrackingPackages.Tests.Controllers
{
    [TestClass]
    public class PackagesControllerTests
    {
        [TestMethod]
        public void AddMultipleProducts()
        {
            for(int i = 0; i < 10000; i++)
            {
                PackagesController controller = new PackagesController();
                Random rnd = new Random();
                PackagesCreateViewModel viewModel = new PackagesCreateViewModel()
                {
                    ReceiveDate = RandomDay(),
                    PacketSize = PacketSize.XS,
                    ZipCode = "2345-230",
                    Location = "Portugal",
                    Weight = rnd.Next(1, 100).ToString(),
                    HasValueToPay = false,
                    IsFragile = false
                };
                controller.Create(viewModel);

            }
        }

        [TestMethod]
        public void EditMultipleProducts()
        {
            for (int i = 0; i < 10000; i++)
            {
                PackagesController controller = new PackagesController();
                Random rnd = new Random();
                PackagesEditViewModel viewModel = new PackagesEditViewModel()
                {
                    Id = rnd.Next(1,100),
                    ReceiveDate = RandomDay(),
                    PacketSize = PacketSize.XS,
                    ZipCode = "2345-230",
                    Location = "Portugal",
                    Weight = rnd.Next(1, 100).ToString(),
                    HasValueToPay = false,
                    IsFragile = false
                };
                controller.Update(viewModel);
            }
        }

        [TestMethod]
        public void GetHowManyPackagesOnStatus()
        {
            PackagesController controller = new PackagesController();
            PackageSearchViewModel viewModel = new PackageSearchViewModel
            {
                Status = Status.StoppedByLegal
            };

            controller.Package(viewModel);
        }

        [TestMethod]
        public void GetHowManyPackagesOnCheckpoint()
        {
            PackagesController controller = new PackagesController();
            PackageSearchViewModel viewModel = new PackageSearchViewModel
            {
                CheckpointId = 1
            };
            var result = controller.Package(viewModel);
        }

        #region Utils
        private Random gen = new Random();
        DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
        #endregion
    }
}
