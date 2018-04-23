using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Utils.OAuth.Provider;
using DevExpress.Utils.OAuth;

namespace OAuth_Provider_Basics.Controllers
{
    public class OAuthController : Controller
    {

        public ActionResult Request_Token() {
            Response response
                = ServiceProvider.GetRequestToken(
                    Request.HttpMethod,
                    Request.Url);

            Response.StatusCode = response.StatusCode;
            
            return Content(
                response.Content, 
                response.ContentType);
        }

        public ActionResult Access_Token() {
            Response response
                = ServiceProvider.GetAccessToken(
                    Request.HttpMethod,
                    Request.Url);

            Response.StatusCode = response.StatusCode;

            return Content(
                response.Content, 
                response.ContentType);
        }

        bool VerifyRequestToken(out IToken token) {
            ValidationScope scope;
            token = ServiceProvider.VerifyRequestToken(
                        Request.HttpMethod,
                        Request.Url,
                        out scope);
            if (token == null || token.IsEmpty) {
                if (scope != null) {
                    foreach (ValidationError error in scope.Errors) {
                        ModelState.AddModelError("", error.Message);
                        return false;
                    }
                }
                ModelState.AddModelError("", "Invalid / expired token");
                return false;
            }
            return true;
        }

        public ActionResult Authorize() {
            FormCollection model = new FormCollection();
            
            IToken token;
            if (!VerifyRequestToken(out token)) {
                return View(model);
            }
            
            model["Email"] = "guest@devexpress.com";
            model["Application"] = token.Callback;
            
            return View(model);
        }

        bool AuthorizeRequestToken(string credentials) {
            ValidationScope scope;
            IToken token = ServiceProvider.AuthorizeRequestToken(
                        Request.HttpMethod,
                        Request.Url,
                        credentials,
                        out scope);
            if (token == null || token.IsEmpty) {
                if (scope != null) {
                    foreach (ValidationError error in scope.Errors) {
                        ModelState.AddModelError("", error.Message);
                        return false;
                    }
                }
                ModelState.AddModelError("", "Invalid / expired token");
                return false;
            }
            return true;
        }

        [HttpPost]
        public ActionResult Authorize(FormCollection model) {
            try {
                IToken token;
                if (!VerifyRequestToken(out token)) {
                    return View(model);
                }

                if (model["Allow"] != "Allow") {
                    return Redirect(token.Callback);
                }

                if (String.Equals("guest@devexpress.com", model["Email"], StringComparison.InvariantCultureIgnoreCase)
                        && String.Equals("devexpress", model["Password"], StringComparison.InvariantCultureIgnoreCase)) {

                            if (AuthorizeRequestToken("guest@devexpress.com")) {
                                Uri returnUri = ((Url)token.Callback).ToUri(
                                    Parameter.Token(token.Value),
                                    Parameter.Verifier(token.Verifier));
                                return Redirect(returnUri.ToString());
                            }
                                    
                }
                
                ModelState.AddModelError("", "Email address or Password is incorrect.");
                return View(model);

            } catch (Exception e) {
                ModelState.AddModelError("", e);
                return View(model);
            }
        }

    }
}
