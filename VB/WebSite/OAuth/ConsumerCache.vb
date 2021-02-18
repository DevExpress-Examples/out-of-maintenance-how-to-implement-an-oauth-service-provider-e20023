Imports DevExpress.Utils.OAuth
Imports DevExpress.Utils.OAuth.Provider
Imports System

Namespace OAuth_Provider_Basics.OAuth

	Public Class ConsumerCache
		Inherits ConsumerStore

		Public Overrides Function GetConsumer(ByVal consumerKey As String) As IConsumer
			If String.Equals(consumerKey, "anonymous", StringComparison.InvariantCulture) Then
				Dim consumer As New ConsumerBase()
				consumer.ConsumerKey = "anonymous"
				consumer.ConsumerSecret = "anonymous"
				Return consumer
			End If
			Return Nothing
		End Function
	End Class
End Namespace