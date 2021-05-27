using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Model
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("is_consultant")]
        public bool IsConsultant { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("refresh_token")]
        public string RefreshToken { get; set; }
        [Column("refresh_token_expiry_time")]
        public DateTime RefreshTokenExpiryTime { get; set; }
        [Column("is_email_confirmed")]
        public bool IsEmailConfirmed { get; set; }
        [Column("cpf_cnpj")]
        public string CpfCnpj { get; set; }
        [Column("short_description")]
        public string ShortDescription { get; set; }
        [Column("long_description")]
        public string LongDescription { get; set; }
        [Column("profile_picture")]
        public byte[] ProfilePicture { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
