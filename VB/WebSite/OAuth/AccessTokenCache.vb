Imports System.Web
Imports DevExpress.Utils.OAuth
Imports DevExpress.Utils.OAuth.Provider
Imports DevExpress.Data.Filtering
Imports System.Collections.Generic
Imports DevExpress.Xpo
Imports System

Namespace OAuth_Provider_Basics.OAuth

	Public Class AccessTokenCache
		Inherits AccessTokenStore

		Public Overrides Function GetToken(ByVal token As String) As IToken
			If String.IsNullOrEmpty(token) Then
				Return Nothing
			End If
			Return Me.Get("ac:" & token)
		End Function

		Public Overrides Sub RevokeToken(ByVal token As String)
			If String.IsNullOrEmpty(token) Then
				Return
			End If
			Me.Set("ac:" & token, Nothing)
		End Sub

		Public Overrides Function CreateToken(ByVal requestToken As IToken) As IToken
			If requestToken Is Nothing OrElse requestToken.IsEmpty Then
				Throw New ArgumentException("requestToken is null or empty.", "requestToken")
			End If
			Dim accessToken As New Token(requestToken.ConsumerKey, requestToken.ConsumerSecret, Token.NewToken(TokenLength.Long), Token.NewToken(TokenLength.Long), requestToken.AuthenticationTicket, requestToken.Verifier, requestToken.Callback)
			Return Me.Set("ac:" & accessToken.Value, accessToken)
		End Function
	End Class
End Namespace