using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model.Base
{
    public class BaseEntity
    {
        [Column("id")]
        public long Id { get; set; }
    }
}
