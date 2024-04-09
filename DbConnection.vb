Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration

Public Class DbConnection
	'Opene a connection to the database and Return the connection object
	Public Function getConnection(fund As String) As SqlConnection
		Try
			Dim conn As SqlConnection

			Dim connectionString As String = ConfigurationManager.ConnectionStrings(fund).ConnectionString
			conn = New SqlConnection(connectionString)
			conn.Open()
			Return conn
		Catch ex As Exception
			Dim logerr As New Global.Logger.Logger
			logerr.FileSource = "Payment Module"
			logerr.FilePath = "c:"
			logerr.Logger(ex.Message & " : Opening Connection")
			'pnlError.Visible = True
			'Me.lblError.Text = "Error Loading Bank Branches"
		End Try
		Return Nothing


	End Function


    'close opened connection to the database
	Public Sub close(fund As String)
		Try
			Dim conn As SqlConnection
			Dim connectionString As String = ConfigurationManager.ConnectionStrings(fund).ConnectionString
			conn = New SqlConnection(connectionString)
			conn.Close()
		Catch ex As Exception
			Dim logerr As New Global.Logger.Logger
			logerr.FileSource = "Payment Module"
			logerr.FilePath = "c:"
			logerr.Logger(ex.Message & " : Closing Connection")
		End Try


	End Sub


End Class
