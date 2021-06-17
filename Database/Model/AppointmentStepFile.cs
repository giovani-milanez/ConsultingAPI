using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("appointment_step_files")]
    [Index(nameof(AppointmentStepId), Name = "appointment_step_files_appointment")]
    [Index(nameof(FileId), Name = "appointment_step_files_file")]
    public partial class AppointmentStepFile : BaseEntity
    {
        [Column("appointment_step_id")]
        public long AppointmentStepId { get; set; }
        [Column("file_id")]
        public long FileId { get; set; }

        [ForeignKey(nameof(AppointmentStepId))]
        [InverseProperty("AppointmentStepFiles")]
        public virtual AppointmentStep AppointmentStep { get; set; }
        [ForeignKey(nameof(FileId))]
        [InverseProperty(nameof(FileDetail.AppointmentStepFiles))]
        public virtual FileDetail File { get; set; }
    }
}
