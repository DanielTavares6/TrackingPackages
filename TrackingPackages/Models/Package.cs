using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TrackingPackages.Models
{
    [Table("Packages")]
    public class Package
    {
        [Key]
        public int Id { get; set; }
        public DateTime ReceiveDate{ get; set; }

        [ForeignKey("Checkpoint")]
        public int? LastCheckpointChecked { get; set; }
        public Checkpoint Checkpoint{ get; set; }
        public bool IsStoppedInCustoms{ get; set; }
        public bool HasValueToPay { get; set; }
        public string TrackingCode { get; set; }
        public string Weight { get; set; }
        public PacketSize PacketSize{ get; set; }
        public bool IsFragile { get; set; }
        public Status Status { get; set; }  

    }
    public enum PacketSize { XS = 1, S = 2, M = 3, L = 4, XL = 5, XXL = 6 }

    public enum Status { Received= 1, InTransit = 2, StoppedByLegal = 3, AttemptedDelivery = 4, Returning = 5, Delivered = 6}

}