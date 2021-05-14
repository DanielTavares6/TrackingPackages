using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TrackingPackages.Models
{
    [Table("Checkpoints")]
    public class Checkpoint
    {
        [Key]
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public PlaceType PlaceType { get; set; }
        public TypeOfControl TypeOfControl { get; set; }
    }

    public enum TypeOfControl { Passage = 1, Customs = 2, FinalCheck = 3}

    public enum PlaceType { Airport = 1, Port = 2, Station = 3, CustomsFacility = 4, External = 5 }
}