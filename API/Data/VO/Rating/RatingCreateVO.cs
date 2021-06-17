namespace API.Data.VO.Rating
{
    public class RatingCreateVO
    {
        public long AppointmentId { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
    }
}
