using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("appointments")]
    [Index(nameof(ClientId), Name = "appointments_client")]
    [Index(nameof(ServiceId), Name = "appointments_service")]
    public partial class Appointment : BaseEntity
    {
        public Appointment()
        {
            AppointmentSteps = new HashSet<AppointmentStep>();
        }
       
        [Column("service_id")]
        public long ServiceId { get; set; }
        [Column("client_id")]
        public long ClientId { get; set; }
        [Column("start_date", TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column("end_date", TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [Column("is_completed")]
        public bool IsCompleted { get; set; }

        [ForeignKey(nameof(ClientId))]
        [InverseProperty(nameof(User.Appointments))]
        public virtual User Client { get; set; }
        [ForeignKey(nameof(ServiceId))]
        [InverseProperty("Appointments")]
        public virtual Service Service { get; set; }
        [InverseProperty("Appointment")]
        public virtual Rating Rating { get; set; }
        [InverseProperty(nameof(AppointmentStep.Appointment))]
        public virtual ICollection<AppointmentStep> AppointmentSteps { get; set; }
    }
}
