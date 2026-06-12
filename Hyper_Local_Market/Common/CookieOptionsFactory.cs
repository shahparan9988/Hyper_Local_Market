namespace HyperLocalMarket.Api.Common
{
    public static class CookieOptionsFactory
    {
        public static CookieOptions CreateSessionCookie(DateTime expiresAtUtc)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = new DateTimeOffset(expiresAtUtc),
                Path = "/"
            };
        }

        public static CookieOptions DeleteSessionCookie()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };
        }
    }
}
