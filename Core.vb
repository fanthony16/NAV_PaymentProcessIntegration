Imports System.Configuration
Public Class Core

	Public Sub NAVPaymentInsert(VoucherNo As String)

		Dim myPCon As New SqlClient.SqlConnection
		Dim myComm As New SqlClient.SqlCommand
		Dim daUser As New SqlClient.SqlDataAdapter
		Dim dsUser As New DataSet
		Dim dtUser As New DataTable
		Dim db As New DbConnection

		Dim mycon As New SqlClient.SqlConnection
		mycon = db.getConnection("SurePay")

		Try

			Dim MyDataAdapter As SqlClient.SqlDataAdapter
			MyDataAdapter = New SqlClient.SqlDataAdapter("insert into tmpNAVPayment (txtVoucherNo) select @VoucherNo  where not exists (select * from tmpNAVPayment where txtvoucherno = @VoucherNo)", mycon)

			MyDataAdapter.SelectCommand.CommandType = CommandType.Text

			MyDataAdapter.SelectCommand.Parameters.Add(New SqlClient.SqlParameter("@VoucherNo", _
			    SqlDbType.VarChar))
			MyDataAdapter.SelectCommand.Parameters("@VoucherNo").Value = VoucherNo

			MyDataAdapter.SelectCommand.ExecuteNonQuery()

		Catch Ex As Exception
			'MsgBox("" & Ex.Message)
		Finally

		End Try

	End Sub

	Public Function NAVPaymentGet(voucherNo As String) As Boolean

		Dim myPCon As New SqlClient.SqlConnection
		Dim myComm As New SqlClient.SqlCommand
		Dim daUser As New SqlClient.SqlDataAdapter
		Dim dsUser As New DataSet
		Dim dtUser As New DataTable
		Dim db As New DbConnection

		Dim mycon As New SqlClient.SqlConnection
		mycon = db.getConnection("SurePay")

		Try

			Dim MyDataAdapter As SqlClient.SqlDataAdapter
			MyDataAdapter = New SqlClient.SqlDataAdapter("select * from tmpNAVPayment where txtVoucherno = @voucherNo and dtefilegenerated is null", mycon)

			MyDataAdapter.SelectCommand.CommandType = CommandType.Text

			MyDataAdapter.SelectCommand.Parameters.Add(New SqlClient.SqlParameter("@VoucherNo", _
			    SqlDbType.VarChar))
			MyDataAdapter.SelectCommand.Parameters("@VoucherNo").Value = voucherNo

			dsUser = New DataSet()
			MyDataAdapter.Fill(dsUser, "NAVPayment")
			dtUser = dsUser.Tables("NAVPayment")
			mycon.Close()

			If dtUser.Rows.Count > 0 Then
				Return True
			Else
				Return False
			End If

		Catch Ex As Exception
			'MsgBox("" & Ex.Message)
		Finally

		End Try

	End Function



End Class
