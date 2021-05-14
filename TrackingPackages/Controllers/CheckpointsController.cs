using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TrackingPackages.Models;
using TrackingPackages.Validators;
using TrackingPackages.ViewModels.Checkpoints;

namespace TrackingPackages.Controllers
{
    public class CheckpointsController : TrackingPackagesController
    {
        // api/Checkpoints/ALl
        [HttpGet]
        public IQueryable<Checkpoint> All()
        {
            return Db.Checkpoints;
        }

        // GET api/Checkpoints
        // api/Checkpoints/Checkpoint/id
        [HttpGet]
        public IHttpActionResult Checkpoint(int id)
        {
            try
            {
                Checkpoint checkpoint = Db.Checkpoints.FirstOrDefault(p => p.Id == id);
                if (checkpoint == null)
                {
                    return NotFound();
                }

                return Ok(checkpoint);
            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        // POST: api/Checkpoints/Create
        [HttpPost]
        public async Task<IHttpActionResult> Create(Checkpoint checkpoint)
        {
            try
            {
                //Validation
                Validator result = CheckpointValidator.Create(checkpoint);

                if (!result.Success)
                {
                    return BadRequest(result.ErrorMessage);
                }
                //Create Checkpoint
                Db.Checkpoints.Add(checkpoint);
                await Db.SaveChangesAsync();
                return Ok("Checkpoint created successfully");
            }
            catch
            {
                return BadRequest("An error has occurred");
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> Update(Checkpoint checkpoint)
        {
            try
            {
                //Validation
                Checkpoint checkpointInDb = Db.Checkpoints.FirstOrDefault(p => p.Id == checkpoint.Id);
                Validator result = CheckpointValidator.Edit(checkpoint, checkpointInDb);

                if (!result.Success)
                {
                    return BadRequest(result.ErrorMessage);
                }

                checkpointInDb.City = checkpoint.City;
                checkpointInDb.Country = checkpoint.Country;
                checkpointInDb.PlaceType = checkpoint.PlaceType;
                checkpointInDb.TypeOfControl = checkpoint.TypeOfControl;

                await Db.SaveChangesAsync();
                return Ok("Checkpoint updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Checkpoint was changed by another user");
            }
            catch
            {
                return BadRequest();
            }

        }

        // DELETE: api/Checkpoints/5
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            try
            {
                Checkpoint checkpoint = Db.Checkpoints.FirstOrDefault(p => p.Id == id);
                if (checkpoint == null)
                {
                    return NotFound();
                }

                Db.Checkpoints.Remove(checkpoint);
                await Db.SaveChangesAsync();

                return Ok("Checkpoint deleted successfully");

            }
            catch
            {
                return BadRequest("An error has occurred");
            }

        }

        // Scan Package
        [HttpPost]
        public async Task<IHttpActionResult> Scan(ScanPackageViewModel viewModel)
        {
            Package package = Db.Packages.FirstOrDefault(p => p.Id == viewModel.PackageId);
            Checkpoint checkpoint = Db.Checkpoints.FirstOrDefault(p => p.Id == viewModel.CheckpointId);

            if (package == null || checkpoint == null)
                return BadRequest("Invalid data");

            if (package.Status != Status.InTransit)
                return BadRequest("Package status invalid, must be in transit");

            package.LastCheckpointChecked = checkpoint.Id;
            package.Status = Status.StoppedByLegal;
            if (checkpoint.PlaceType == PlaceType.CustomsFacility)
                package.IsStoppedInCustoms = true;

            await Db.SaveChangesAsync();

            return Ok("Package is now being scanned in checkpoint");
        }

        public IHttpActionResult Release(ScanPackageViewModel viewModel)
        {
            Package package = Db.Packages.FirstOrDefault(p => p.Id == viewModel.PackageId);
            Checkpoint checkpoint = Db.Checkpoints.FirstOrDefault(p => p.Id == viewModel.CheckpointId);

            if (package == null || checkpoint == null)
                return BadRequest("Invalid data");

            if (package.LastCheckpointChecked != checkpoint.Id)
                return BadRequest("Package is not on this checkpoint");

            if (package.Status != Status.StoppedByLegal)
                return BadRequest("Package is not in a checkpoint");

            if (package.IsStoppedInCustoms)
                package.IsStoppedInCustoms = false;

            package.Status = Status.InTransit;

            return Ok("Package was released from checkpoint");
        }
    }
}
