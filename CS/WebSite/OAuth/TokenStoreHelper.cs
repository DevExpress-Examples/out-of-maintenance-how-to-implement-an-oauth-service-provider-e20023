namespace OAuth_Provider_Basics.OAuth {
    using System;
    using DevExpress.Utils.OAuth.Provider;
    using DevExpress.Utils.OAuth;
    using System.Web;
    
    public static class TokenStoreHelper {
        public static IToken Get(this TokenStore store, string key) {
            if (String.IsNullOrWhiteSpace(key) || store == null)
                return null;
            HttpContext context = HttpContext.Current;
            if (context != null) {
                IToken token = context.Cache[key] as IToken;
                if (token != null && !token.IsEmpty)
                    return token;
            }
            return null;
        }
        public static IToken Set(this TokenStore store, string key, IToken token) {
            if (String.IsNullOrWhiteSpace(key) || store == null)
                return null;
            HttpContext context = HttpContext.Current;
            if (context != null) {
                context.Cache.Insert(key, token, null,
                        DateTime.UtcNow.AddMinutes(5),
                        System.Web.Caching.Cache.NoSlidingExpiration);
                return token;
            }
            return null;
        }
    }
}
