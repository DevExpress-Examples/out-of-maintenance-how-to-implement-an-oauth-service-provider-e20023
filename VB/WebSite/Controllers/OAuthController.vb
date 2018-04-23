Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.Mvc
Imports DevExpress.Utils.OAuth.Provider
Imports DevExpress.Utils.OAuth

Namespace OAuth_Provider_Basics.Controllers
	Public Class OAuthController
		Inherits Controller

		Public Function Request_Token() As ActionResult
			Dim response As Response = ServiceProvider.GetRequestToken(Request.HttpMethod, Request.Url)

            Me.Response.StatusCode = response.StatusCode

			Return Content(response.Content, response.ContentType)
		End Function

		Public Function Access_Token() As ActionResult
			Dim response As Response = ServiceProvider.GetAccessToken(Request.HttpMethod, Request.Url)

            Me.Response.StatusCode = response.StatusCode

			Return Content(response.Content, response.ContentType)
		End Function

		Private Function VerifyRequestToken(<System.Runtime.InteropServices.Out()> ByRef token As IToken) As Boolean
			Dim scope As ValidationScope
			token = ServiceProvider.VerifyRequestToken(Request.HttpMethod, Request.Url, scope)
			If token Is Nothing OrElse token.IsEmpty Then
				If scope IsNot Nothing Then
					For Each [error] As ValidationError In scope.Errors
						ModelState.AddModelError("", [error].Message)
						Return False
					Next [error]
				End If
				ModelState.AddModelError("", "Invalid / expired token")
				Return False
			End If
			Return True
		End Function

		Public Function Authorize() As ActionResult
			Dim model As New FormCollection()

			Dim token As IToken
			If (Not VerifyRequestToken(token)) Then
				Return View(model)
			End If

			model("Email") = "guest@devexpress.com"
			model("Application") = token.Callback

			Return View(model)
		End Function

		Private Function AuthorizeRequestToken(ByVal credentials As String) As Boolean
			Dim scope As ValidationScope
			Dim token As IToken = ServiceProvider.AuthorizeRequestToken(Request.HttpMethod, Request.Url, credentials, scope)
			If token Is Nothing OrElse token.IsEmpty Then
				If scope IsNot Nothing Then
					For Each [error] As ValidationError In scope.Errors
						ModelState.AddModelError("", [error].Message)
						Return False
					Next [error]
				End If
				ModelState.AddModelError("", "Invalid / expired token")
				Return False
			End If
			Return True
		End Function

		<HttpPost> _
		Public Function Authorize(ByVal model As FormCollection) As ActionResult
			Try
				Dim token As IToken
				If (Not VerifyRequestToken(token)) Then
					Return View(model)
				End If

				If model("Allow") <> "Allow" Then
					Return Redirect(token.Callback)
				End If

				If String.Equals("guest@devexpress.com", model("Email"), StringComparison.InvariantCultureIgnoreCase) AndAlso String.Equals("devexpress", model("Password"), StringComparison.InvariantCultureIgnoreCase) Then

							If AuthorizeRequestToken("guest@devexpress.com") Then
								Dim returnUri As Uri = (CType(token.Callback, Url)).ToUri(Parameter.Token(token.Value), Parameter.Verifier(token.Verifier))
								Return Redirect(returnUri.ToString())
							End If

				End If

				ModelState.AddModelError("", "Email address or Password is incorrect.")
				Return View(model)

			Catch e As Exception
				ModelState.AddModelError("", e)
				Return View(model)
			End Try
		End Function

	End Class
End Namespace
