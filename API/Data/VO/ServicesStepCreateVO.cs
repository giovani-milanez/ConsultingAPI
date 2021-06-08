using System.Text.Json;

namespace API.Data.VO
{
    public class ServicesStepCreateVO
    {
        public long StepId { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public JsonDocument CreateData { get; set; }
    }
}
