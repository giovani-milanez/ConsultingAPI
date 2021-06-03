using Database.Model.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Model
{
    public class Step : BaseEntity
    {
        [Column("type")]
        public string Type { get; set; }
        [Column("display_name")]
        public string DisplayName { get; set; }
        [Column("create_schema")]
        public string CreateSchema { get; set; }
        [Column("submit_schema")]
        public string SubmitSchema { get; set; }
    }
}
