using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using TrackingPackages.Models;

namespace TrackingPackages.Controllers
{
    public class TrackingPackagesController : ApiController
    {
        protected ApplicationDbContext Db = new ApplicationDbContext();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}