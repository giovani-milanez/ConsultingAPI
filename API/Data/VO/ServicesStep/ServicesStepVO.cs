using System.Text.Json;

namespace API.Data.VO
{
    public class ServicesStepVO
    {
        public long Id { get; set; }
        //public long ServiceId { get; set; }
        public StepVO Step { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public JsonDocument CreateData { get; set; }
    }
}
