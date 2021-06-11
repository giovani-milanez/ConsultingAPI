using System.Collections.Generic;

namespace API.Data.VO
{
    public class ServiceEditVO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ServicesStepCreateVO> Steps { get; set; }
    }
}
