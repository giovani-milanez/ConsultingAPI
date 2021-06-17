using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("file_details")]
    [Index(nameof(ContentId), Name = "files_details_content")]
    [Index(nameof(UploaderId), Name = "files_user")]
    [Index(nameof(Guid), Name = "guid_index")]
    public partial class FileDetail : BaseEntity
    {
        public FileDetail()
        {
            AppointmentStepFiles = new HashSet<AppointmentStepFile>();
            Users = new HashSet<User>();
        }

        [Column("content_id")]
        public long ContentId { get; set; }
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
        [Column("uploader_id")]
        public long UploaderId { get; set; }

        [ForeignKey(nameof(ContentId))]
        [InverseProperty(nameof(FileContent.FileDetails))]
        public virtual FileContent Content { get; set; }
        [ForeignKey(nameof(UploaderId))]
        [InverseProperty(nameof(User.FileDetails))]
        public virtual User Uploader { get; set; }
        [InverseProperty(nameof(AppointmentStepFile.File))]
        public virtual ICollection<AppointmentStepFile> AppointmentStepFiles { get; set; }
        [InverseProperty(nameof(User.ProfilePicture))]
        public virtual ICollection<User> Users { get; set; }
    }
}
