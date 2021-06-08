using System.Collections.Generic;

namespace API.Data.VO
{
    public class ServiceCreateVO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public long UserId { get; set; }
        public virtual ICollection<ServicesStepCreateVO> Steps { get; set; }
    }
}
