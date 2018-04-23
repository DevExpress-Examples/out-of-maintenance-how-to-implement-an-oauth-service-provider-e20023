using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization;
using DevExpress.Utils.OAuth;

namespace OAuth_Provider_Basics.Controllers
{
    public class HomeController : Controller
    {
        Consumer CreateConsumer() {
            Consumer consumer = new Consumer();

            consumer.RequestUri = new Uri(this.Url.Action("request_token", "OAuth", null, "http").ToLower());
            consumer.AccessUri = new Uri(this.Url.Action("access_token", "OAuth", null, "http").ToLower());
            consumer.AuthorizeUri = new Uri(this.Url.Action("authorize", "OAuth", null, "http").ToLower());
            consumer.CallbackUri = new Uri(this.Url.Action("", "", null, "http").ToLower());

            consumer.ConsumerKey = "anonymous";
            consumer.ConsumerSecret = "anonymous";

            consumer.HttpMethod = "GET";
            consumer.Signature = Signature.HMACSHA1;

            return consumer;
        }

        public ActionResult Index()
        {

            IToken request_token;
            FormCollection model = new FormCollection();
            
            String step = Request.Form["Step"];
            if (String.IsNullOrEmpty(step)) {
                string oauth_verifier = Request.QueryString.Get("oauth_verifier");
                if (!String.IsNullOrEmpty(oauth_verifier)) {
                   step = "Callback";
                }
            }           

            switch (step) {
                case "Request token":
                    Consumer request_consumer = CreateConsumer();
                    request_consumer.GetRequestToken();

                    Session["request_token"] = request_consumer.RequestToken;

                    model["request_token"] = request_consumer.RequestToken.Value;
                    model["request_token_secret"] = request_consumer.RequestToken.Secret;

                    model["Step1.Disabled"] = "disabled";
                    model["Step3.Disabled"] = "disabled";

                    break;

                case "Authorize":
                    request_token = (IToken)Session["request_token"];
                    Consumer authorize_consumer = CreateConsumer();

                    authorize_consumer.RequestToken = new Token(
                        request_token.ConsumerKey,
                        request_token.ConsumerSecret,
                        request_token.Value,
                        request_token.Secret);

                    return Redirect(authorize_consumer.GetAuthorizeTokenUrl().ToString());

                case "Callback":
                    request_token = (IToken)Session["request_token"];

                    Session["oauth_token"] = Request.QueryString["oauth_token"];
                    Session["oauth_verifier"] = Request.QueryString["oauth_verifier"];
                    
                    model["request_token"] = request_token.Value;
                    model["request_token_secret"] = request_token.Secret;
                    model["oauth_token"] = Request.QueryString["oauth_token"];
                    model["oauth_verifier"] = Request.QueryString["oauth_verifier"];

                    model["Step1.Disabled"] = "disabled";
                    model["Step2.Disabled"] = "disabled";

                    break;

                case "Access token":
                    request_token = (IToken)Session["request_token"];
                    Consumer access_consumer = CreateConsumer();

                    access_consumer.RequestToken = new Token(
                        request_token.ConsumerKey,
                        request_token.ConsumerSecret,
                        request_token.Value,
                        request_token.Secret);

                    IToken access_token = access_consumer.GetAccessToken(
                        request_token,
                        (String)Session["oauth_verifier"]);

                    model["request_token"] = request_token.Value;
                    model["request_token_secret"] = request_token.Secret;
                    model["oauth_token"] = (String)Session["oauth_token"];
                    model["oauth_verifier"] = (String)Session["oauth_verifier"];
                    model["access_token"] = access_token.Value;
                    model["access_token_secret"] = access_token.Secret;

                    model["Step1.Disabled"] = "disabled";
                    model["Step2.Disabled"] = "disabled";
                    model["Step3.Disabled"] = "disabled";

                    break;

                default:

                    model["Step2.Disabled"] = "disabled";
                    model["Step3.Disabled"] = "disabled";

                    break;

            }
            
            return View(model);
        }

    }
}
