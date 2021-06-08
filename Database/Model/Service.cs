using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("services")]
    [Index(nameof(UserId), Name = "user_service")]
    public partial class Service : BaseEntity
    {
        public Service()
        {
            Appointments = new HashSet<Appointment>();
            ServicesSteps = new HashSet<ServicesStep>();
        }

        [Column("user_id")]
        public long UserId { get; set; }
        [Required]
        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; }
        [Required]
        [Column("description")]
        [StringLength(255)]
        public string Description { get; set; }
        [Column("is_global")]
        public bool IsGlobal { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(UserId))]
        [InverseProperty("Services")]
        public virtual User User { get; set; }
        [InverseProperty(nameof(Appointment.Service))]
        public virtual ICollection<Appointment> Appointments { get; set; }
        [InverseProperty(nameof(ServicesStep.Service))]
        public virtual ICollection<ServicesStep> ServicesSteps { get; set; }
    }
}
