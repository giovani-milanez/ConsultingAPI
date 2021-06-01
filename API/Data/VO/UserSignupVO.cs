namespace API.Data.VO
{
    public class UserSignupVO
    {
        public bool IsConsultant { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CpfCnpj { get; set; }
    }
}
