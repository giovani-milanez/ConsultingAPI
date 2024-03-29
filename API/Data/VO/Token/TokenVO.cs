﻿namespace API.Data.VO
{
    public class TokenVO
    {
        public TokenVO(bool autheticated, string created, string expiration, string accessToken, string refreshToken, UserShortVO user)
        {
            Autheticated = autheticated;
            Created = created;
            Expiration = expiration;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            User = user;
        }

        public bool Autheticated { get; set; }
        public string Created { get; set; }
        public string Expiration { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserShortVO User { get; set; }
    }
}
