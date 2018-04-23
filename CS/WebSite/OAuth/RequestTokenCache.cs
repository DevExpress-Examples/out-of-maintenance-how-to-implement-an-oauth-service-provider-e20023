namespace OAuth_Provider_Basics.OAuth {
    using System;
    using DevExpress.Utils.OAuth.Provider;
    using DevExpress.Utils.OAuth;

    public class RequestTokenCache : RequestTokenStore {
        public override IToken GetToken(string token) {
            if (String.IsNullOrWhiteSpace(token))
                return null;
            return this.Get("rt:" + token);
        }

        public override IToken CreateUnauthorizeToken(string consumerKey, 
            string consumerSecret, string callback) {
            if (String.IsNullOrEmpty(consumerKey)) {
                throw new ArgumentException("consumerKey is null or empty.", "consumerKey");
            }
            if (String.IsNullOrEmpty(consumerSecret)) {
                throw new ArgumentException("consumerSecret is null or empty.", "consumerSecret");
            }
            if (String.IsNullOrEmpty(callback)) {
                throw new ArgumentException("callback is null or empty.", "callback");
            }
            
            IToken unauthorizeToken = new Token(
                    consumerKey,
                    consumerSecret,
                    Token.NewToken(TokenLength.Long),
                    Token.NewToken(TokenLength.Long),
                    String.Empty, /* unauthorized */
                    Token.NewToken(TokenLength.Short),
                    callback);
            
            return this.Set("rt:" + unauthorizeToken.Value, unauthorizeToken);
        }

        public override IToken AuthorizeToken(string token, string authenticationTicket) {
            if (String.IsNullOrEmpty(token)) {
                throw new ArgumentException("token is null or empty.", "token");
            }
            if (String.IsNullOrEmpty(authenticationTicket)) {
                throw new ArgumentException("authenticationTicket is null or empty.", "authenticationTicket");
            }
            IToken unauthorizeToken = this.Get("rt:" + token);
            if (unauthorizeToken == null
                            || unauthorizeToken.IsEmpty) {
                return null;
            }
            IToken authorizeToken = new Token(
                unauthorizeToken.ConsumerKey,
                unauthorizeToken.ConsumerSecret,
                unauthorizeToken.Value,
                unauthorizeToken.Secret,
                authenticationTicket,
                unauthorizeToken.Verifier, /* authorized */
                unauthorizeToken.Callback);
            return this.Set("rt:" + authorizeToken.Value, authorizeToken);
        }

    }
}