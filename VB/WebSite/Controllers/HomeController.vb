Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports System.Runtime.Serialization
Imports DevExpress.Utils.OAuth

Namespace OAuth_Provider_Basics.Controllers
	Public Class HomeController
		Inherits Controller
		Private Function CreateConsumer() As Consumer
			Dim consumer As New Consumer()

			consumer.RequestUri = New Uri(Me.Url.Action("request_token", "OAuth", Nothing, "http").ToLower())
			consumer.AccessUri = New Uri(Me.Url.Action("access_token", "OAuth", Nothing, "http").ToLower())
			consumer.AuthorizeUri = New Uri(Me.Url.Action("authorize", "OAuth", Nothing, "http").ToLower())
			consumer.CallbackUri = New Uri(Me.Url.Action("", "", Nothing, "http").ToLower())

			consumer.ConsumerKey = "anonymous"
			consumer.ConsumerSecret = "anonymous"

			consumer.HttpMethod = "GET"
			consumer.Signature = Signature.HMACSHA1

			Return consumer
		End Function

		Public Function Index() As ActionResult

			Dim request_token As IToken
			Dim model As New FormCollection()

			Dim [step] As String = Request.Form("Step")
			If String.IsNullOrEmpty([step]) Then
				Dim oauth_verifier As String = Request.QueryString.Get("oauth_verifier")
				If (Not String.IsNullOrEmpty(oauth_verifier)) Then
				   [step] = "Callback"
				End If
			End If

			Select Case [step]
				Case "Request token"
					Dim request_consumer As Consumer = CreateConsumer()
					request_consumer.GetRequestToken()

					Session("request_token") = request_consumer.RequestToken

					model("request_token") = request_consumer.RequestToken.Value
					model("request_token_secret") = request_consumer.RequestToken.Secret

					model("Step1.Disabled") = "disabled"
					model("Step3.Disabled") = "disabled"


				Case "Authorize"
					request_token = CType(Session("request_token"), IToken)
					Dim authorize_consumer As Consumer = CreateConsumer()

					authorize_consumer.RequestToken = New Token(request_token.ConsumerKey, request_token.ConsumerSecret, request_token.Value, request_token.Secret)

					Return Redirect(authorize_consumer.GetAuthorizeTokenUrl().ToString())

				Case "Callback"
					request_token = CType(Session("request_token"), IToken)

					Session("oauth_token") = Request.QueryString("oauth_token")
					Session("oauth_verifier") = Request.QueryString("oauth_verifier")

					model("request_token") = request_token.Value
					model("request_token_secret") = request_token.Secret
					model("oauth_token") = Request.QueryString("oauth_token")
					model("oauth_verifier") = Request.QueryString("oauth_verifier")

					model("Step1.Disabled") = "disabled"
					model("Step2.Disabled") = "disabled"


				Case "Access token"
					request_token = CType(Session("request_token"), IToken)
					Dim access_consumer As Consumer = CreateConsumer()

					access_consumer.RequestToken = New Token(request_token.ConsumerKey, request_token.ConsumerSecret, request_token.Value, request_token.Secret)

					Dim access_token As IToken = access_consumer.GetAccessToken(request_token, CType(Session("oauth_verifier"), String))

					model("request_token") = request_token.Value
					model("request_token_secret") = request_token.Secret
					model("oauth_token") = CType(Session("oauth_token"), String)
					model("oauth_verifier") = CType(Session("oauth_verifier"), String)
					model("access_token") = access_token.Value
					model("access_token_secret") = access_token.Secret

					model("Step1.Disabled") = "disabled"
					model("Step2.Disabled") = "disabled"
					model("Step3.Disabled") = "disabled"


				Case Else

					model("Step2.Disabled") = "disabled"
					model("Step3.Disabled") = "disabled"


			End Select

			Return View(model)
		End Function

	End Class
End Namespace
