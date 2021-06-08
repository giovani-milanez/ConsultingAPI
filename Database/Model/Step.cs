using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;

#nullable disable

namespace Database.Model
{
    [Table("steps")]
    public partial class Step : BaseEntity
    {
        public Step()
        {
            AppointmentSteps = new HashSet<AppointmentStep>();
            ServicesSteps = new HashSet<ServicesStep>();
        }

        [Required]
        [Column("type")]
        [StringLength(255)]
        public string Type { get; set; }
        [Required]
        [Column("display_name")]
        [StringLength(255)]
        public string DisplayName { get; set; }
        [Required]
        [Column("create_schema", TypeName = "json")]
        public string CreateSchema { get; set; }
        [Required]
        [Column("submit_schema", TypeName = "json")]
        public string SubmitSchema { get; set; }

        [InverseProperty(nameof(AppointmentStep.Step))]
        public virtual ICollection<AppointmentStep> AppointmentSteps { get; set; }
        [InverseProperty(nameof(ServicesStep.Step))]
        public virtual ICollection<ServicesStep> ServicesSteps { get; set; }
    }
}
