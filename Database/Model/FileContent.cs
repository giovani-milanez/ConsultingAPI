using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("file_content")]
    [Index(nameof(FileGuid), Name = "file_guid", IsUnique = true)]
    public partial class FileContent : BaseEntity
    {
        public FileContent()
        {
            FileDetails = new HashSet<FileDetail>();
        }

        [Required]
        [Column("file_guid")]
        [MaxLength(16)]
        public byte[] FileGuid { get; set; }
        [Required]
        [Column("content", TypeName = "mediumblob")]
        public byte[] Content { get; set; }

        [InverseProperty(nameof(FileDetail.Content))]
        public virtual ICollection<FileDetail> FileDetails { get; set; }
    }
}
