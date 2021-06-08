using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("appointment_steps")]
    [Index(nameof(AppointmentId), Name = "appointment_steps_appointment")]
    [Index(nameof(StepId), Name = "appointment_steps_steps")]
    public partial class AppointmentStep : BaseEntity
    {
        [Column("appointment_id")]
        public long AppointmentId { get; set; }
        [Column("step_id")]
        public long StepId { get; set; }
        [Required]
        [Column("submit_data", TypeName = "json")]
        public string SubmitData { get; set; }
        [Column("is_completed")]
        public bool IsCompleted { get; set; }
        [Column("date_completed", TypeName = "datetime")]
        public DateTime? DateCompleted { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        [InverseProperty("AppointmentSteps")]
        public virtual Appointment Appointment { get; set; }
        [ForeignKey(nameof(StepId))]
        [InverseProperty("AppointmentSteps")]
        public virtual Step Step { get; set; }
    }
}
