﻿namespace API.Data.VO
{
    public class UserShortVO
    {
        public long Id { get; set; }
        public bool IsConsultant { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string ProfilePicUrl { get; set; }
    }
}
