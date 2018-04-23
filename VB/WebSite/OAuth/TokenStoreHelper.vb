Imports Microsoft.VisualBasic
	Imports System
	Imports DevExpress.Utils.OAuth.Provider
	Imports DevExpress.Utils.OAuth
	Imports System.Web
Namespace OAuth_Provider_Basics.OAuth

	Public Module TokenStoreHelper
        <System.Runtime.CompilerServices.Extension()> _
        Public Function [Get](ByVal store As TokenStore, ByVal key As String) As IToken
            If String.IsNullOrWhiteSpace(key) OrElse store Is Nothing Then
                Return Nothing
            End If
            Dim context As HttpContext = HttpContext.Current
            If context IsNot Nothing Then
                Dim token As IToken = TryCast(context.Cache(key), IToken)
                If token IsNot Nothing AndAlso (Not token.IsEmpty) Then
                    Return token
                End If
            End If
            Return Nothing
        End Function
        <System.Runtime.CompilerServices.Extension()> _
        Public Function [Set](ByVal store As TokenStore, ByVal key As String, ByVal token As IToken) As IToken
            If String.IsNullOrWhiteSpace(key) OrElse store Is Nothing Then
                Return Nothing
            End If
            Dim context As HttpContext = HttpContext.Current
            If context IsNot Nothing Then
                context.Cache.Insert(key, token, Nothing, DateTime.UtcNow.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration)
                Return token
            End If
            Return Nothing
        End Function
	End Module
End Namespace
