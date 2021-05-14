using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackingPackages.Models;

namespace TrackingPackages.Validators
{
    public static class CheckpointValidator
    {
        public static Validator Create(Checkpoint checkpoint)
        {
            if (checkpoint == null)
                return new Validator(false, "Erro");

            if (string.IsNullOrWhiteSpace(checkpoint.City))
                return new Validator(false, "Please insert a city");

            if (string.IsNullOrWhiteSpace(checkpoint.Country))
                return new Validator(false, "Please insert a country");

            if (!Enum.IsDefined(typeof(PlaceType), checkpoint.PlaceType))
                return new Validator(false, "Please insert a valid place type");

            if (!Enum.IsDefined(typeof(TypeOfControl), checkpoint.TypeOfControl))
                return new Validator(false, "Please insert a valid type of control");

            return new Validator(true);
        }

        public static Validator Edit(Checkpoint checkpoint, Checkpoint checkpointInDb)
        {
            if (checkpoint == null)
                return new Validator(false, "Erro");

            if (checkpointInDb == null)
                return new Validator(false, "Invalid checkpoint");

            if (string.IsNullOrWhiteSpace(checkpoint.City))
                return new Validator(false, "Please insert a city");

            if (string.IsNullOrWhiteSpace(checkpoint.Country))
                return new Validator(false, "Please insert a country");

            if (!Enum.TryParse(checkpoint.PlaceType.ToString(), out PlaceType placeType))
                return new Validator(false, "Please insert a valid place type");

            return new Validator(true);
        }
    }
}