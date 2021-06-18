namespace API.Data.VO
{
    public class ServiceShortVO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsDeleted { get; set; }
    }
}
