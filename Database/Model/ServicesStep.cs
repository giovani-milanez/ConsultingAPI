using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("services_steps")]
    [Index(nameof(ServiceId), Name = "services_steps_service")]
    [Index(nameof(StepId), Name = "services_steps_steps")]
    public partial class ServicesStep : BaseEntity
    {
        [Column("step_id")]
        public long StepId { get; set; }
        [Column("service_id")]
        public long ServiceId { get; set; }
        [Column("order")]
        public int Order { get; set; }
        [Required]
        [Column("title")]
        [StringLength(255)]
        public string Title { get; set; }
        [Required]
        [Column("create_data", TypeName = "json")]
        public string CreateData { get; set; }

        [ForeignKey(nameof(ServiceId))]
        [InverseProperty("ServicesSteps")]
        public virtual Service Service { get; set; }
        [ForeignKey(nameof(StepId))]
        [InverseProperty("ServicesSteps")]
        public virtual Step Step { get; set; }
    }
}
