using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using TrackingPackages.Models;
using TrackingPackages.Validators;
using TrackingPackages.ViewModels.Packages;

namespace TrackingPackages.Controllers
{
    [RoutePrefix("api/[controller]")]
    public class PackagesController : TrackingPackagesController
    {

        // Get api/Packages/All
        [HttpGet]
        public IQueryable<Package> All()
        {
            return Db.Packages;
        }

        // GET api/Packages/Package/{id}
        [HttpGet]
        public IHttpActionResult Package(int id)
        {
            try
            {
                Package package = Db.Packages.FirstOrDefault(p => p.Id == id);
                if (package == null)
                {
                    return NotFound();
                }

                return Ok(package);
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        //Get Package by tracking code
        // GET /api/Packages/Package?code=
        [HttpGet]
        public IHttpActionResult Package(string code)
        {
            try
            {
                Package package = Db.Packages.FirstOrDefault(p => p.TrackingCode == code);

                if (package == null)
                    return BadRequest("Invalid tracking code");

                return Ok(package);
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        //Get Package by tracking code
        // GET /api/Packages/Package?code=
        [HttpPost]
        public IHttpActionResult Package(PackageSearchViewModel viewModel)
        {
            try
            {
                IQueryable<Package> packages = Db.Packages;

                if (viewModel.Status != null)
                    packages = packages.Where(p => p.Status == viewModel.Status);

                if (viewModel.CheckpointId != null)
                    packages = packages.Where(p => p.LastCheckpointChecked == viewModel.CheckpointId);

                return Ok(packages);
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }


        // POST: api/Packages/Create
        [HttpPost]
        public async Task<IHttpActionResult> Create(PackagesCreateViewModel viewModel)
        {
            try
            {
                //Validation
                Validator result = PackageValidator.Create(viewModel);

                if (!result.Success)
                {
                    return BadRequest(result.ErrorMessage);
                }
                //Create Package
                string trackingCode = GenerateTrackingCode(viewModel);
                Package package = viewModel.ToPackage(trackingCode);
                Db.Packages.Add(package);
                await Db.SaveChangesAsync();
                return Ok("Package created successfully");
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        // PUT: api/Packages/Update
        [HttpPut]
        public async Task<IHttpActionResult> Update(PackagesEditViewModel viewModel)
        {
            try
            {
                //Validation
                Package package = Db.Packages.FirstOrDefault(p => p.Id == viewModel.Id);
                Validator result = PackageValidator.Edit(viewModel, package);

                if (!result.Success)
                {
                    return BadRequest(result.ErrorMessage);
                }

                viewModel.UpdatePackage(package);

                await Db.SaveChangesAsync();
                return Ok("Package updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Package was changed by another user");
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        // DELETE: api/Packages/Delete/id
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                Package package = Db.Packages.FirstOrDefault(p => p.Id == id);
                if (package == null)
                {
                    return NotFound();
                }

                Db.Packages.Remove(package);
                await Db.SaveChangesAsync();

                return Ok("Package deleted successfully");

            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        //Send Package
        ///POST api/Packages/Send?id={id}
        [HttpPost]
        public async Task<IHttpActionResult> Send(int? id)
        {
            try
            {
                Package package = Db.Packages.FirstOrDefault(p => p.Id == id);

                Validator validator = PackageValidator.Send(package);
                if (!validator.Success)
                {
                    return BadRequest(validator.ErrorMessage);
                }

                package.Status = Status.InTransit;
                await Db.SaveChangesAsync();
                return Ok("Package was sent");
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        //Attempt Delivery
        // /api/Packages/AttemptDeliver?id=2
        [HttpPost]
        public async Task<IHttpActionResult> AttemptDeliver(int? id)
        {
            try
            {
                Package package = Db.Packages.FirstOrDefault(p => p.Id == id);
                Validator validator = PackageValidator.AttemptDeliver(package);
                if (!validator.Success)
                {
                    return BadRequest(validator.ErrorMessage);
                }

                package.Status = Status.AttemptedDelivery;

                await Db.SaveChangesAsync();
                return Ok("Attempting delivery");
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        //Deliver Delivery
        // /api/Packages/Deliver
        [HttpPost]
        public async Task<IHttpActionResult> Deliver(DeliverViewModel viewModel)
        {
            try
            {
                Package package = Db.Packages.FirstOrDefault(p => p.Id == viewModel.Id);
                Validator validator = PackageValidator.Deliver(package);
                if (!validator.Success)
                {
                    return BadRequest(validator.ErrorMessage);
                }

                package.Status = viewModel.Success ? Status.Delivered : Status.Returning;

                await Db.SaveChangesAsync();
                return Ok("Attempting delivery");
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        //Return Package
        // /api/Packages/Return?id=2
        [HttpPost]
        public async Task<IHttpActionResult> Return(int? id)
        {
            try
            {
                Package package = Db.Packages.FirstOrDefault(p => p.Id == id);
                Validator validator = PackageValidator.Return(package);
                if (!validator.Success)
                {
                    return BadRequest(validator.ErrorMessage);
                }

                package.Status = Status.Received;

                await Db.SaveChangesAsync();
                return Ok("Attempting delivery");
            }
            catch
            {
                return BadRequest("An error has occurred");
            }
        }

        private string GenerateTrackingCode(PackagesCreateViewModel viewModel)
        {
            try
            {
                string isoCode = GetIso(viewModel.Location);
                string zipCode = viewModel.ZipCode.Contains("-") ? viewModel.ZipCode.Replace("-", "") : viewModel.ZipCode;
                string identification = Guid.NewGuid().ToString().Split('-').LastOrDefault().Substring(0, 6);
                string date = viewModel.ReceiveDate.Value.ToString("dd/MM/yy");

                string[] dateSplit = date.Split('/');
                int day = int.Parse(dateSplit[0].ElementAt(0).ToString());
                int month = int.Parse(dateSplit[1].ElementAt(1).ToString());
                int controlDigitOne = (day + month + 20);
                while (controlDigitOne > 10)
                {
                    controlDigitOne = controlDigitOne.ToString().Select(digit => int.Parse(digit.ToString())).ToList().Aggregate((a, b) => a + b);
                }

                int isoNumericCode = GetIsoNumericCode(viewModel.Location);
                int controlDigitTWo = (isoNumericCode + int.Parse(dateSplit[2].ElementAt(1).ToString())).ToString().Select(digit => int.Parse(digit.ToString())).ToList().Aggregate((a, b) => a + b);
                return $"{isoCode}-{zipCode}-{identification}-{date.Replace("/", "")}-{controlDigitOne}{controlDigitTWo}";
            }
            catch
            {
                return "";
            }

        }
        private string GetIso(string location)
        {
            switch (location.ToUpper())
            {
                case "PORTUGAL":
                    return "PT";
                default:
                    return "";
            }
        }
        private int GetIsoNumericCode(string location)
        {
            switch (location.ToUpper())
            {
                case "PT":
                case "PORTUGAL":
                case "PRT":
                    return 620;
                case "United Kingdom":
                case "GB":
                case "GBR":
                    return 826;
                default:
                    return 0;
            }
        }

    }
}