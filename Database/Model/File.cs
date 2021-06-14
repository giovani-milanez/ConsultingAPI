using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("files")]
    [Index(nameof(UploaderId), Name = "files_user")]
    public partial class File : BaseEntity
    {
        public File()
        {
            AppointmentStepFiles = new HashSet<AppointmentStepFile>();
            Users = new HashSet<User>();
        }

        [Required]
        [Column("guid")]
        [MaxLength(16)]
        public byte[] Guid { get; set; }
        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [Column("type")]
        [StringLength(255)]
        public string Type { get; set; }
        [Column("size")]
        public long Size { get; set; }
        [Required]
        [Column("content", TypeName = "blob")]
        public byte[] Content { get; set; }
        [Column("uploader_id")]
        public long UploaderId { get; set; }

        [ForeignKey(nameof(UploaderId))]
        [InverseProperty(nameof(User.Files))]
        public virtual User Uploader { get; set; }
        [InverseProperty(nameof(AppointmentStepFile.File))]
        public virtual ICollection<AppointmentStepFile> AppointmentStepFiles { get; set; }
        [InverseProperty(nameof(User.ProfilePictureNavigation))]
        public virtual ICollection<User> Users { get; set; }
    }
}
