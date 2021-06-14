﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Database.Model.Base;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Database.Model
{
    [Table("users")]
    [Index(nameof(ProfilePicture), Name = "user_profile_picture")]
    public partial class User : BaseEntity
    {
        public User()
        {
            Appointments = new HashSet<Appointment>();
            Files = new HashSet<File>();
            Services = new HashSet<Service>();
            Steps = new HashSet<Step>();
        }
       
        [Required]
        [Column("type", TypeName = "enum('client','consultant','admin')")]
        public string Type { get; set; }
        [Required]
        [Column("name")]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }
        [Column("password")]
        [StringLength(255)]
        public string Password { get; set; }
        [Column("cpf_cnpj")]
        [StringLength(255)]
        public string CpfCnpj { get; set; }
        [Column("short_description")]
        [StringLength(255)]
        public string ShortDescription { get; set; }
        [Column("long_description")]
        [StringLength(255)]
        public string LongDescription { get; set; }
        [Column("profile_picture")]
        public long? ProfilePicture { get; set; }
        [Column("is_email_confirmed")]
        public bool IsEmailConfirmed { get; set; }
        [Column("email_confirmation_code")]
        [MaxLength(16)]
        public byte[] EmailConfirmationCode { get; set; }
        [Column("created_at", TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(ProfilePicture))]
        [InverseProperty(nameof(File.Users))]
        public virtual File ProfilePictureNavigation { get; set; }
        [InverseProperty(nameof(Appointment.Client))]
        public virtual ICollection<Appointment> Appointments { get; set; }
        [InverseProperty(nameof(File.Uploader))]
        public virtual ICollection<File> Files { get; set; }
        [InverseProperty(nameof(Service.User))]
        public virtual ICollection<Service> Services { get; set; }
        [InverseProperty(nameof(Step.User))]
        public virtual ICollection<Step> Steps { get; set; }
    }
}
