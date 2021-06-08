using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("ratings")]
    [Index(nameof(AppointmentId), Name = "ratings_appointment")]
    public partial class Rating : BaseEntity
    {
        [Column("appointment_id")]
        public long AppointmentId { get; set; }
        [Column("stars")]
        public int Stars { get; set; }
        [Column("comment")]
        [StringLength(255)]
        public string Comment { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        [InverseProperty("Ratings")]
        public virtual Appointment Appointment { get; set; }
    }
}
