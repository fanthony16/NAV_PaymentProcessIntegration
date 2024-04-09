

Partial Public Class NAVPayment
	Partial Class DataTable1DataTable

		Private Sub DataTable1DataTable_ColumnChanging(sender As Object, e As DataColumnChangeEventArgs) Handles Me.ColumnChanging
			If (e.Column.ColumnName = Me.PaymentDetails1Column.ColumnName) Then
				'Add user code here
			End If

		End Sub

	End Class

End Class
