namespace OAuth_Provider_Basics.OAuth {
    using System;
    using DevExpress.Xpo;
    using System.Collections.Generic;
    using DevExpress.Data.Filtering;
    using DevExpress.Utils.OAuth.Provider;
    using DevExpress.Utils.OAuth;
    using System.Web;

    public class AccessTokenCache : AccessTokenStore {       
        public override IToken GetToken(string token) {
            if (String.IsNullOrEmpty(token)) {
                return null;
            }
            return this.Get("ac:" + token);
        }
        
        public override void RevokeToken(string token) {
            if (String.IsNullOrEmpty(token)) {
                return;
            }
            this.Set("ac:" + token, null);
        }

        public override IToken CreateToken(IToken requestToken) {
            if (requestToken == null || requestToken.IsEmpty) {
                throw new ArgumentException("requestToken is null or empty.", "requestToken");
            }
            Token accessToken = new Token(
                requestToken.ConsumerKey,
                requestToken.ConsumerSecret,
                Token.NewToken(TokenLength.Long),
                Token.NewToken(TokenLength.Long),
                requestToken.AuthenticationTicket,
                requestToken.Verifier,
                requestToken.Callback);
            return this.Set("ac:" + accessToken.Value, accessToken);
        }
    }
}