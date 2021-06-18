using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("ratings")]
    [Index(nameof(AppointmentId), Name = "appointment_id", IsUnique = true)]
    public partial class Rating : BaseEntity
    {        
        [Column("appointment_id")]
        public long AppointmentId { get; set; }
        [Column("stars")]
        public int Stars { get; set; }
        [Column("comment")]
        [StringLength(255)]
        public string Comment { get; set; }
        [Column("created_at", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        [InverseProperty("Rating")]
        public virtual Appointment Appointment { get; set; }
    }
}
