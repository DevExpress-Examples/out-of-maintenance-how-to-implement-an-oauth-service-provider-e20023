Imports DevExpress.Utils.OAuth
Imports DevExpress.Utils.OAuth.Provider
Imports System

Namespace OAuth_Provider_Basics.OAuth

	Public Class RequestTokenCache
		Inherits RequestTokenStore

		Public Overrides Function GetToken(ByVal token As String) As IToken
			If String.IsNullOrWhiteSpace(token) Then
				Return Nothing
			End If
			Return Me.Get("rt:" & token)
		End Function

		Public Overrides Function CreateUnauthorizeToken(ByVal consumerKey As String, ByVal consumerSecret As String, ByVal callback As String) As IToken
			If String.IsNullOrEmpty(consumerKey) Then
				Throw New ArgumentException("consumerKey is null or empty.", "consumerKey")
			End If
			If String.IsNullOrEmpty(consumerSecret) Then
				Throw New ArgumentException("consumerSecret is null or empty.", "consumerSecret")
			End If
			If String.IsNullOrEmpty(callback) Then
				Throw New ArgumentException("callback is null or empty.", "callback")
			End If

			Dim unauthorizeToken As IToken = New Token(consumerKey, consumerSecret, Token.NewToken(TokenLength.Long), Token.NewToken(TokenLength.Long), String.Empty, Token.NewToken(TokenLength.Short), callback)

			Return Me.Set("rt:" & unauthorizeToken.Value, unauthorizeToken)
		End Function

		Public Overrides Function AuthorizeToken(ByVal token As String, ByVal authenticationTicket As String) As IToken
			If String.IsNullOrEmpty(token) Then
				Throw New ArgumentException("token is null or empty.", "token")
			End If
			If String.IsNullOrEmpty(authenticationTicket) Then
				Throw New ArgumentException("authenticationTicket is null or empty.", "authenticationTicket")
			End If
			Dim unauthorizeToken As IToken = Me.Get("rt:" & token)
			If unauthorizeToken Is Nothing OrElse unauthorizeToken.IsEmpty Then
				Return Nothing
			End If
'INSTANT VB NOTE: The local variable authorizeToken was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim authorizeToken_Conflict As IToken = New Token(unauthorizeToken.ConsumerKey, unauthorizeToken.ConsumerSecret, unauthorizeToken.Value, unauthorizeToken.Secret, authenticationTicket, unauthorizeToken.Verifier, unauthorizeToken.Callback)
			Return Me.Set("rt:" & authorizeToken_Conflict.Value, authorizeToken_Conflict)
		End Function

	End Class
End Namespace