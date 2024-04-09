Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Configuration
Imports System.IO
Imports System.Text.RegularExpressions.Regex
Imports System.Security.Cryptography
Imports System.Web.Script.Serialization
Imports System.Text
Imports System.Net

Public Class Form1

	Dim lstPayments As New List(Of PaymentObj)


	' update payment request lines status on dynamics NAV
	Private Function updatePaymentLine(lineNumber As Integer) As Boolean

		Dim logerrs As New Global.Logger.Logger
		logerrs.FileSource = "H2H File Generator"
		logerrs.FilePath = "E:\WebSiteDataExchange\Log\"
		logerrs.Logger(" Updating Line Number")

		Dim pmLines As New PaymentLine.PaymentLines_Service

		Dim nc As New NetworkCredential
		nc.Domain = "****"
		nc.UserName = "****"
		nc.Password = "****"

		Dim prxy As New WebProxy("172.16.0.8:8080", True)
		prxy.Credentials = nc

		pmLines.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
		pmLines.PreAuthenticate = True

		Dim PMLFilterArray As New List(Of PaymentLine.PaymentLines_Filter)
		Dim PMLFilter As New PaymentLine.PaymentLines_Filter

		PMLFilter.Field = PaymentLine.PaymentLines_Fields.Line_No
		PMLFilter.Criteria = lineNumber
		PMLFilterArray.Add(PMLFilter)
		Dim pml() As PaymentLine.PaymentLines
		pml = pmLines.ReadMultiple(PMLFilterArray.ToArray, "", 100)

		If pml.Count > 0 Then

			pml(0).H2H_Status = PaymentLine.H2H_Status.Dispatched
			Try
				pmLines.Update(pml(0))
				Return True
			Catch ex As Exception
				Return False
			End Try

		End If

		Return False

	End Function

	'get all approved payment voucher requests to generate instruction file for the third party application
	Private Sub getPVEntries()

		Dim pv As New PaymentVoucher.PaymentVoucherList_Service
		Dim bank As New BankCard.BankCard_Service
		Dim pvc As New PaymentVoucherCard.PaymentVoucherCard_Service
		Dim bnkL As New BankList.BeneficiaryBankList_Service
		Dim nc As New NetworkCredential

		lstPayments.Clear()

		nc.Domain = "pensure-nigeria.com"
		nc.UserName = "***"
		nc.Password = "***"

		Dim prxy As New WebProxy("172.16.0.8:8080", True)
		prxy.Credentials = nc
		Try

			pv.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
			pv.PreAuthenticate = True

			bank.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
			bank.PreAuthenticate = True

			pvc.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
			pvc.PreAuthenticate = True

			bnkL.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
			bnkL.PreAuthenticate = True

		Catch ex As Exception

			MsgBox("Error Connecting to Navision", MsgBoxStyle.Information)
			Exit Sub

		End Try
		
		

		Dim PVFilterArray As New List(Of PaymentVoucher.PaymentVoucherList_Filter)
		Dim PVFilterArrayCard As New List(Of PaymentVoucherCard.PaymentVoucherCard_Filter)
		Dim BankFilterArrayCard As New List(Of BankCard.BankCard_Filter)
		Dim BankListFilterArrayCard As New List(Of BankList.BeneficiaryBankList_Filter)


		Dim PVFilter As New PaymentVoucher.PaymentVoucherList_Filter

		Dim PVFilter_H2HStatus As New PaymentVoucher.PaymentVoucherList_Filter

		Dim PVFilterCard As New PaymentVoucherCard.PaymentVoucherCard_Filter
		Dim BankFilterCard As New BankCard.BankCard_Filter
		Dim BankListFilterCard As New BankList.BeneficiaryBankList_Filter


		


		PVFilter.Field = PaymentVoucher.PaymentVoucherList_Fields.No
		PVFilter.Criteria = LTrim(RTrim(txtVoucherNo.Text))
		



		PVFilterArray.Add(PVFilter)

		PVFilter_H2HStatus.Field = PaymentVoucher.PaymentVoucherList_Fields.H2H_Status
		PVFilter_H2HStatus.Criteria = "<>" & PaymentVoucher.H2H_Status.Successful & "&" & "<>" & PaymentVoucher.H2H_Status.Dispatched
		PVFilterArray.Add(PVFilter_H2HStatus)
		

		Dim pmv() As PaymentVoucher.PaymentVoucherList
		pmv = pv.ReadMultiple(PVFilterArray.ToArray, "", 5)



		'Dim i As Integer = (pmv.Count - 1) - 3
		Dim i As Integer = 0

		'Do While i < pmv.Count - 1

		Do While i < pmv.Count


			PVFilterArrayCard = New List(Of PaymentVoucherCard.PaymentVoucherCard_Filter)
			PVFilterCard = New PaymentVoucherCard.PaymentVoucherCard_Filter
			PVFilterCard.Field = PaymentVoucherCard.PaymentVoucherCard_Fields.No
			PVFilterCard.Criteria = pmv(i).No
			PVFilterArrayCard.Add(PVFilterCard)

			Dim pmr() As PaymentVoucherCard.PaymentVoucherCard

			pmr = pvc.ReadMultiple(PVFilterArrayCard.ToArray, "", 100)

			'''''''''''''''''''''''''''''''''''''''''''''''''

			'BVPayments.DebitAccNo = pmr(0).Paying_Bank_Account


			Dim k As Integer = 0

			Do While k < pmr(0).PVLines.Length

				Dim logerr As New Global.Logger.Logger
				logerr.FileSource = "H2H File Generator"
				logerr.FilePath = System.AppDomain.CurrentDomain.BaseDirectory()
				logerr.Logger(pmr(0).No & " : " & pmr(0).PVLines.Length & "Record Fetched : With Status :" & pmr(0).PVLines(k).H2H_Status & " " & lstPayments.Count)

				Dim BVPayments As New PaymentObj

				BVPayments.RecordType = "P"
				BVPayments.PaymentType = "A"
				BVPayments.ProcessingMode = "M"
				BVPayments.CustomerRef = pmr(0).No & "_" & pmr(0).PVLines(k).Line_No
				BVPayments.RecordType = "P"
				BVPayments.PaymentType = "PAY"
				BVPayments.ProcessingMode = "ON"
				BVPayments.ServiceType = ""
				BVPayments.CustomerRef = pmr(0).No & "_" & pmr(0).PVLines(k).Line_No
				BVPayments.CustomerMemo = ""
				BVPayments.DebitAccCountry = "NG"
				BVPayments.DebitAccCity = "LOS"

				BVPayments.PayeeAddress1 = ""
				BVPayments.PayeeAddress2 = ""
				BVPayments.PayeeCountryCode = "NG"
				BVPayments.PayeeFaxNumber = ""
				BVPayments.PayeeBankCode = ""
				BVPayments.PayeeBranchSubCode = ""
				BVPayments.ValueDate = pmr(0).Date

				BVPayments.VAT = 0
				BVPayments.WHTPrinting = ""
				BVPayments.WHTFormID = ""
				BVPayments.WHTTaxID = ""
				BVPayments.WHTRefNo = ""
				BVPayments.WHTType1 = ""
				BVPayments.WHTDescription1 = ""
				BVPayments.WHTGrossAmount1 = 0
				BVPayments.WHTAmount1 = 0
				BVPayments.WHTType2 = ""
				BVPayments.WHTDescription2 = ""
				BVPayments.WHTGrossAmount2 = 0
				BVPayments.WHTAmount2 = 0
				BVPayments.DiscountAmount = 0
				BVPayments.InvoiceFormat = ""
				BVPayments.PaymentCurrency = "NGN"

				BVPayments.PaymentPurpose = "PAYR"



				'''''''''''getting bank details------------
				BankFilterCard.Field = BankCard.BankCard_Fields.No
				BankFilterCard.Criteria = pmr(0).Paying_Bank_Account
				BankFilterArrayCard.Add(BankFilterCard)
				Dim PVBank() As BankCard.BankCard
				PVBank = bank.ReadMultiple(BankFilterArrayCard.ToArray, "", 100)

				If PVBank.Count > 0 Then
					BVPayments.DebitAccNo = PVBank(0).Bank_Account_No
				Else
					BVPayments.DebitAccNo = pmr(0).Paying_Bank_Account
				End If

				BVPayments.Payee = pmr(0).PVLines(k).Payee
				BVPayments.BeneficiaryEmailID = "Shareholders_Fund@leadway-pensure.com"
				BankListFilterCard.Field = BankList.BeneficiaryBankList_Fields.Code
				BankListFilterCard.Criteria = pmr(0).PVLines(k).Bank
				BankListFilterArrayCard.Add(BankListFilterCard)

				Dim PVBankList() As BankList.BeneficiaryBankList
				PVBankList = bnkL.ReadMultiple(BankListFilterArrayCard.ToArray, "", 100)

				If PVBankList.Length > 0 Then
					BVPayments.PayeeBankClearingCode = PVBankList(0).Bank_Code
					BVPayments.PayeeBranchClearingCode = PVBankList(0).Branch_Code
				Else
					BVPayments.PayeeBankClearingCode = "A"
					BVPayments.PayeeBranchClearingCode = "A"
				End If

				If IsNothing(pmr(0).PVLines(k).Payee_Bank_Account_No) = False Then
					BVPayments.PayeeAccount = pmr(0).PVLines(k).Payee_Bank_Account_No
				Else
					BVPayments.PayeeAccount = pmr(0).PVLines(k).Account_No
				End If


				If pmr(0).PVLines(k).Payment_Narration.Length > 70 Then

					BVPayments.PaymentDetails1 = pmr(0).PVLines(k).Payment_Narration.Substring(0, 69).Replace(",", "")
					'BVPayments.PaymentDetails2 = pmr(0).PVLines(k).Payment_Narration.Substring(70, pmr(0).PVLines(k).Payment_Narration.Length).Replace(",", "")
					BVPayments.PaymentDetails2 = ""

				Else

					BVPayments.PaymentDetails1 = pmr(0).PVLines(k).Payment_Narration.Replace(",", "")
					BVPayments.PaymentDetails2 = ""



				End If

				BVPayments.PaymentAmount = pmr(0).PVLines(k).Net_Amount


				If pmr(0).PVLines(k).H2H_Status = PaymentVoucherCard.H2H_Status._blank_ Then
					lstPayments.Add(BVPayments)
					updatePaymentLine(pmr(0).PVLines(k).Line_No, "Sent")
				Else
				End If

				k = k + 1

			Loop
			'update payment voucher status on dynamics NAV
			updatePaymentVoucher(pmv(i).No, "Sent")

			i = i + 1

		Loop

		Try

			If lstPayments.Count > 0 Then

				generateCSV(lstPayments)

				Dim logerr As New Global.Logger.Logger
				logerr.FileSource = "H2H File Generator"
				logerr.FilePath = System.AppDomain.CurrentDomain.BaseDirectory()
				logerr.Logger(lstPayments.Count & " Payment(s) Request File Generated Successfully")
				MsgBox("File Generated Successfully")
				Exit Sub
			Else

				Dim logerr As New Global.Logger.Logger
				logerr.FileSource = "H2H File Generator"
				logerr.FilePath = System.AppDomain.CurrentDomain.BaseDirectory()
				logerr.Logger("No record found !!!")
				MsgBox("No record found !!!")
				Exit Sub
			End If

		Catch ex As Exception

			Dim logerr As New Global.Logger.Logger
			logerr.FileSource = "H2H File Generator"
			'logerr.FilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
			logerr.FilePath = System.AppDomain.CurrentDomain.BaseDirectory()

			logerr.Logger(ex.Message)

			Exit Sub

		End Try






	End Sub

	Private Function updatePaymentLine(lineNumber As Integer, txtStatus As String) As Boolean

		Dim pmLines As New PaymentLine.PaymentLines_Service

		Dim nc As New NetworkCredential
		nc.Domain = "pensure-nigeria.com"
		nc.UserName = "coretec"
		nc.Password = "Ibukun@lag"

		Dim prxy As New WebProxy("172.16.0.8:8080", True)
		prxy.Credentials = nc

		pmLines.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
		pmLines.PreAuthenticate = True

		Dim PMLFilterArray As New List(Of PaymentLine.PaymentLines_Filter)
		Dim PMLFilter As New PaymentLine.PaymentLines_Filter

		PMLFilter.Field = PaymentLine.PaymentLines_Fields.Line_No
		PMLFilter.Criteria = lineNumber
		PMLFilterArray.Add(PMLFilter)
		Dim pml() As PaymentLine.PaymentLines
		pml = pmLines.ReadMultiple(PMLFilterArray.ToArray, "", 100)

		Try


			If pml.Count > 0 Then

				Select Case txtStatus

					Case Is = "Sent"
						pml(0).H2H_Status = PaymentLine.H2H_Status.Dispatched
					Case Is = "Successful"
						pml(0).H2H_Status = PaymentLine.H2H_Status.Successful
					Case Is = "Fail"
						pml(0).H2H_Status = PaymentLine.H2H_Status.Failed
					Case Else

				End Select

				'Try
				pmLines.Update(pml(0))
				Return True

			Else
				Return True

			End If


		Catch ex As Exception
			Return False
		End Try
		

	End Function

	Private Function updatePaymentVoucher(BPVNo As String, txtstatus As String) As Boolean

		Dim PV As New PaymentVoucherCard.PaymentVoucherCard_Service

		Dim nc As New NetworkCredential
		nc.Domain = "*****"
		nc.UserName = "*****"
		nc.Password = "*****"

		Dim prxy As New WebProxy("172.16.0.8:8080", True)
		prxy.Credentials = nc

		PV.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
		PV.PreAuthenticate = True

		Dim PVFilterArray As New List(Of PaymentVoucherCard.PaymentVoucherCard_Filter)
		Dim PVFilter As New PaymentVoucherCard.PaymentVoucherCard_Filter

		PVFilter.Field = PaymentVoucherCard.PaymentVoucherCard_Fields.No
		PVFilter.Criteria = BPVNo
		PVFilterArray.Add(PVFilter)
		Dim pvr() As PaymentVoucherCard.PaymentVoucherCard
		pvr = PV.ReadMultiple(PVFilterArray.ToArray, "", 100)

		If pvr.Count > 0 Then

			Select Case txtstatus

				Case Is = "Sent"
					pvr(0).H2H_Status = PaymentVoucherCard.H2H_Status.Dispatched
				Case Is = "Successful"
					pvr(0).H2H_Status = PaymentVoucherCard.H2H_Status.Successful
				Case Is = "Fail"
					pvr(0).H2H_Status = PaymentVoucherCard.H2H_Status.Failed
				Case Else

			End Select

			Try

				PV.Update(pvr(0))

				Return True
			Catch ex As Exception
				Return False
			End Try


		End If


	End Function

	Private Sub testDMS()
		Dim uName As String, uPWD As String, uRI As String, DestinationFile As String = ""
		uName = "***"
		uPWD = "***"
		uRI = "http://172.16.0.32:9080/wsi/FNCEWS40MTOM/"

		Dim dms As New PaymentModuleDMSWindow.CEEntry, DocumentID As String = "", DMSDocumentPath As String
		DestinationFile = "C:\deleted\hrmgt.png"
		dms.getConnection(uName, uPWD, uRI)
		DocumentID = dms.DropDocument(DestinationFile, Path.GetFileNameWithoutExtension(DestinationFile), "LPPFA_BPD")
		DMSDocumentPath = dms.GetDocument("C:\DellisMamellaDB\", DocumentID, "LPPFA_BPD", ".png")

	End Sub

	Public Sub generateCSV(ByVal lstPayment As List(Of PaymentObj))

		Dim myStream As IO.Stream, i As Integer, invTotal As Double = 0

		' generate a name for the payment request file
		Dim fName As String = Now.ToString.Replace("/", "").Replace("\", "").Replace(":", "").Replace(" ", "") & ".csv"


		' drops the generated payment request csv file in the folder in the specified path base 
		' on agreed template by the third party application
		Using myWriter As IO.StreamWriter = New IO.StreamWriter("\\epayment\ipayment\" & fName)


			Dim str As New StringBuilder


			i = 0

			Do While i < lstPayment.Count

				If i = 0 Then


					myWriter.Write("H,")
					myWriter.Write("P")

					myWriter.Write(myWriter.NewLine)

					myWriter.Write(lstPayment(i).RecordType.ToString() & ",")
					myWriter.Write(lstPayment(i).PaymentType.ToString().ToString() & ",")
					myWriter.Write(lstPayment(i).ProcessingMode.ToString() & ",")
					myWriter.Write(lstPayment(i).ServiceType.ToString() & ",")
					myWriter.Write(lstPayment(i).CustomerRef.ToString() & ",")
					myWriter.Write(lstPayment(i).CustomerMemo.ToString() & ",")
					myWriter.Write(lstPayment(i).DebitAccCountry.ToString() & ",")
					myWriter.Write(lstPayment(i).DebitAccCity.ToString() & ",")
					myWriter.Write(lstPayment(i).DebitAccNo.ToString() & ",")
					myWriter.Write(lstPayment(i).ValueDate & ",")
					myWriter.Write(lstPayment(i).Payee.ToString() & ",")

					myWriter.Write(lstPayment(i).PayeeAddress1.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeAddress2.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeCountryCode.ToString() & ",")

					myWriter.Write(lstPayment(i).PayeeFaxNumber.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeBankCode.ToString() & ",") ''''16
					myWriter.Write(lstPayment(i).PayeeBankClearingCode.ToString() & ",") ''''17
					myWriter.Write(lstPayment(i).PayeeBranchClearingCode.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeBranchSubCode.ToString() & ",")

					myWriter.Write(lstPayment(i).PayeeAccount.ToString & ",") '''''20
					myWriter.Write(lstPayment(i).PaymentDetails1.ToString & ",")
					myWriter.Write(lstPayment(i).PaymentDetails2.ToString & ",")
					myWriter.Write(lstPayment(i).VAT.ToString & ",") ''''''23

					myWriter.Write(lstPayment(i).WHTPrinting.ToString & ",")
					myWriter.Write(lstPayment(i).WHTFormID.ToString & ",")
					myWriter.Write(lstPayment(i).WHTTaxID.ToString & ",")
					myWriter.Write(lstPayment(i).WHTRefNo.ToString & ",")
					myWriter.Write(lstPayment(i).WHTType1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTDescription1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTGrossAmount1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTAmount1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTType2.ToString & ",") '''''32
					myWriter.Write(lstPayment(i).WHTDescription2.ToString & ",")
					myWriter.Write(lstPayment(i).WHTGrossAmount2.ToString & ",")
					myWriter.Write(lstPayment(i).WHTAmount2.ToString & ",")
					myWriter.Write(lstPayment(i).DiscountAmount.ToString & ",")
					myWriter.Write(lstPayment(i).InvoiceFormat.ToString & ",")	'''''37
					myWriter.Write(lstPayment(i).PaymentCurrency.ToString & ",")
					myWriter.Write(lstPayment(i).PaymentAmount & ",")	'''''39

					''''''''''''''''changes starts here 2018-03-07----------------------------------------

					myWriter.Write(",")	'p40
					myWriter.Write(",")	    'p41
					myWriter.Write(",")	'p42
					myWriter.Write(",")	'p43
					myWriter.Write(",")	'p44
					myWriter.Write(",")	'p45
					myWriter.Write(",")	'p46
					myWriter.Write(",")	'p47
					myWriter.Write(",")	    'p48
					myWriter.Write(",")	'p49
					myWriter.Write(",")	    'p50
					myWriter.Write(",")	'p51
					myWriter.Write(",")	    'p52
					myWriter.Write(",")	    'p53
					myWriter.Write(",")	    'p54
					myWriter.Write(",")	    'p55
					myWriter.Write(",")	    'p56
					myWriter.Write(",")	    'p57
					myWriter.Write(",")	    'p58
					myWriter.Write(",")	    'p59
					myWriter.Write(",")	    'p60
					myWriter.Write(",")	    'p61
					myWriter.Write(",")	    'p62

					myWriter.Write(lstPayment(i).BeneficiaryEmailID.ToString & ",")	   'p63

					myWriter.Write(",")	    'p64
					myWriter.Write(",")	    'p65
					myWriter.Write(",")	    'p66
					myWriter.Write(",")	    'p67
					myWriter.Write(",")	    'p68
					myWriter.Write(",")	    'p69
					myWriter.Write(",")	    'p70
					myWriter.Write(",")	    'p71
					myWriter.Write(",")	    'p72
					myWriter.Write(",")	    'p73
					myWriter.Write(",")	    'p74
					myWriter.Write(",")	    'p75

					myWriter.Write(",")	    'p76
					myWriter.Write(",")	    'p77

					myWriter.Write(",")	    'p78
					myWriter.Write(",")	    'p79

					myWriter.Write(",")	    'p80
					myWriter.Write(",")	    'p81

					myWriter.Write(",")	    'p82
					myWriter.Write(",")	    'p83

					myWriter.Write(",")	    'p84
					myWriter.Write(",")	    'p85

					myWriter.Write(",")	    'p86
					myWriter.Write(",")	    'p87

					myWriter.Write(",")	    'p88
					myWriter.Write(",")	    'p89

					myWriter.Write(",")	    'p90
					myWriter.Write(",")	    'p91

					myWriter.Write(",")	    'p92
					myWriter.Write(",")	    'p93

					myWriter.Write(",")	    'p94
					myWriter.Write(",")	    'p95

					myWriter.Write(",")	    'p96
					myWriter.Write(",")	    'p97

					myWriter.Write(",")	    'p98
					myWriter.Write(",")	    'p99

					myWriter.Write(",")	    'p100
					myWriter.Write(",")	    'p101

					myWriter.Write(",")	    'p102
					myWriter.Write(",")	    'p103

					myWriter.Write(",")	    'p104
					myWriter.Write(",")	    'p105

					myWriter.Write(",")	    'p106
					myWriter.Write(",")	    'p107

					myWriter.Write(",")	    'p108
					myWriter.Write(",")	    'p109

					myWriter.Write(",")	    'p110
					myWriter.Write(",")	    'p111

					myWriter.Write(",")	    'p112
					myWriter.Write(",")	    'p113

					myWriter.Write(",")	    'p114
					myWriter.Write(",")	    'p115
					myWriter.Write(",")	    'p116
					myWriter.Write(",")	    'p117

					myWriter.Write(",")	    'p118
					myWriter.Write(",")	    'p119
					myWriter.Write(",")	    'p120
					myWriter.Write(",")	    'p121

					myWriter.Write(",")	    'p122
					myWriter.Write(",")	    'p123
					myWriter.Write(",")	    'p124
					myWriter.Write(",")	    'p125

					myWriter.Write(",")	    'p126
					myWriter.Write(",")	    'p127
					myWriter.Write(",")	    'p128

					myWriter.Write(",")	    'p129
					myWriter.Write(",")	    'p130
					myWriter.Write(",")	    'p131

					myWriter.Write(",")	    'p132
					myWriter.Write(",")	    'p133
					myWriter.Write(",")	    'p134

					myWriter.Write(",")	    'p135
					myWriter.Write(",")	    'p136
					myWriter.Write(",")	    'p137

					myWriter.Write(",")	    'p138
					myWriter.Write(",")	'p139
					myWriter.Write(",")	    'p140

					myWriter.Write(",")	    'p141
					myWriter.Write(",")	    'p142
					myWriter.Write(",")	    'p143

					myWriter.Write(",")	    'p144
					myWriter.Write(",")	    'p145
					myWriter.Write(",")	    'p146

					myWriter.Write(",")	    'p147
					myWriter.Write(",")	    'p148
					myWriter.Write(",")	'p149

					myWriter.Write(",")	    'p150
					myWriter.Write(",")	    'p151
					myWriter.Write(",")	    'p152

					myWriter.Write(",")	    'p153
					myWriter.Write(",")	    'p154
					myWriter.Write(",")	    'p155
					myWriter.Write(",")	    'p156

					myWriter.Write(",")	    'p157
					myWriter.Write(",")	    'p158
					myWriter.Write(",")	    'p159

					myWriter.Write(",")	    'p160
					myWriter.Write(",")	    'p161
					myWriter.Write(",")	    'p162
					myWriter.Write(",")	    'p163

					myWriter.Write(",")	    'p164
					myWriter.Write(",")	    'p165

					myWriter.Write(",")	    'p166

					myWriter.Write(",")	    'p167
					myWriter.Write(",")	    'p168
					myWriter.Write(",")	    'p169
					myWriter.Write(",")	    'p170
					myWriter.Write(",")	    'p171

					myWriter.Write(",")	    'p172
					myWriter.Write(",")	    'p173

					myWriter.Write(",")	    'p174
					myWriter.Write(",")	    'p175

					myWriter.Write(",")	    'p176
					myWriter.Write(",")	    'p177

					myWriter.Write(",")	    'p178
					myWriter.Write(",")	    'p179

					myWriter.Write(",")	    'p180
					myWriter.Write(",")	    'p181
					myWriter.Write(",")	    'p182
					myWriter.Write(",")	    'p183
					myWriter.Write(",")	    'p184

					myWriter.Write(",")	    'p185
					myWriter.Write(",")	    'p186

					myWriter.Write(",")	    'p187
					myWriter.Write(",")	    'p188

					myWriter.Write(",")	    'p189
					myWriter.Write(",")	    'p190

					myWriter.Write(",")	    'p191
					myWriter.Write(lstPayment(i).PaymentPurpose.ToString)		  'p192



					''''''''''''''''changes end here 2018-03-07----------------------------------------

					myWriter.Write(myWriter.NewLine)

					invTotal = invTotal + lstPayment(i).PaymentAmount

				Else

					myWriter.Write(lstPayment(i).RecordType.ToString() & ",")
					myWriter.Write(lstPayment(i).PaymentType.ToString().ToString() & ",")
					myWriter.Write(lstPayment(i).ProcessingMode.ToString() & ",")
					myWriter.Write(lstPayment(i).ServiceType.ToString() & ",")
					myWriter.Write(lstPayment(i).CustomerRef.ToString() & ",")
					myWriter.Write(lstPayment(i).CustomerMemo.ToString() & ",")
					myWriter.Write(lstPayment(i).DebitAccCountry.ToString() & ",")
					myWriter.Write(lstPayment(i).DebitAccCity.ToString() & ",")
					myWriter.Write(lstPayment(i).DebitAccNo.ToString() & ",")
					myWriter.Write(lstPayment(i).ValueDate & ",")

					If IsNothing(lstPayment(i).Payee) = True Then
						myWriter.Write("" & ",")
					Else
						myWriter.Write(lstPayment(i).Payee.ToString() & ",")
					End If


					myWriter.Write(lstPayment(i).PayeeAddress1.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeAddress2.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeCountryCode.ToString() & ",")

					myWriter.Write(lstPayment(i).PayeeFaxNumber.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeBankCode.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeBankClearingCode.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeBranchClearingCode.ToString() & ",")
					myWriter.Write(lstPayment(i).PayeeBranchSubCode.ToString() & ",")

					myWriter.Write(lstPayment(i).PayeeAccount.ToString & ",")
					myWriter.Write(lstPayment(i).PaymentDetails1.ToString & ",")
					myWriter.Write(lstPayment(i).PaymentDetails2.ToString & ",")
					myWriter.Write(lstPayment(i).VAT.ToString & ",")

					myWriter.Write(lstPayment(i).WHTPrinting.ToString & ",")
					myWriter.Write(lstPayment(i).WHTFormID.ToString & ",")
					myWriter.Write(lstPayment(i).WHTTaxID.ToString & ",")
					myWriter.Write(lstPayment(i).WHTRefNo.ToString & ",")
					myWriter.Write(lstPayment(i).WHTType1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTDescription1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTGrossAmount1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTAmount1.ToString & ",")
					myWriter.Write(lstPayment(i).WHTType2.ToString & ",")
					myWriter.Write(lstPayment(i).WHTDescription2.ToString & ",")
					myWriter.Write(lstPayment(i).WHTGrossAmount2.ToString & ",")
					myWriter.Write(lstPayment(i).WHTAmount2.ToString & ",")
					myWriter.Write(lstPayment(i).DiscountAmount.ToString & ",")
					myWriter.Write(lstPayment(i).InvoiceFormat.ToString & ",")
					myWriter.Write(lstPayment(i).PaymentCurrency.ToString & ",")
					myWriter.Write(lstPayment(i).PaymentAmount & ",")


					''''''''''''''''changes starts here 2018-03-07----------------------------------------

					myWriter.Write(",")	'p40
					myWriter.Write(",")	    'p41
					myWriter.Write(",")	'p42
					myWriter.Write(",")	'p43
					myWriter.Write(",")	'p44
					myWriter.Write(",")	'p45
					myWriter.Write(",")	'p46
					myWriter.Write(",")	'p47
					myWriter.Write(",")	    'p48
					myWriter.Write(",")	'p49
					myWriter.Write(",")	    'p50
					myWriter.Write(",")	'p51
					myWriter.Write(",")	    'p52
					myWriter.Write(",")	    'p53
					myWriter.Write(",")	    'p54
					myWriter.Write(",")	    'p55
					myWriter.Write(",")	    'p56
					myWriter.Write(",")	    'p57
					myWriter.Write(",")	    'p58
					myWriter.Write(",")	    'p59
					myWriter.Write(",")	    'p60
					myWriter.Write(",")	    'p61
					myWriter.Write(",")	    'p62
					myWriter.Write(",")	    'p63
					myWriter.Write(",")	    'p64
					myWriter.Write(",")	    'p65
					myWriter.Write(",")	    'p66
					myWriter.Write(",")	    'p67
					myWriter.Write(",")	    'p68
					myWriter.Write(",")	    'p69
					myWriter.Write(",")	    'p70
					myWriter.Write(",")	    'p71
					myWriter.Write(",")	    'p72
					myWriter.Write(",")	    'p73
					myWriter.Write(",")	    'p74
					myWriter.Write(",")	    'p75

					myWriter.Write(",")	    'p76
					myWriter.Write(",")	    'p77

					myWriter.Write(",")	    'p78
					myWriter.Write(",")	    'p79

					myWriter.Write(",")	    'p80
					myWriter.Write(",")	    'p81

					myWriter.Write(",")	    'p82
					myWriter.Write(",")	    'p83

					myWriter.Write(",")	    'p84
					myWriter.Write(",")	    'p85

					myWriter.Write(",")	    'p86
					myWriter.Write(",")	    'p87

					myWriter.Write(",")	    'p88
					myWriter.Write(",")	    'p89

					myWriter.Write(",")	    'p90
					myWriter.Write(",")	    'p91

					myWriter.Write(",")	    'p92
					myWriter.Write(",")	    'p93

					myWriter.Write(",")	    'p94
					myWriter.Write(",")	    'p95

					myWriter.Write(",")	    'p96
					myWriter.Write(",")	    'p97

					myWriter.Write(",")	    'p98
					myWriter.Write(",")	    'p99

					myWriter.Write(",")	    'p100
					myWriter.Write(",")	    'p101

					myWriter.Write(",")	    'p102
					myWriter.Write(",")	    'p103

					myWriter.Write(",")	    'p104
					myWriter.Write(",")	    'p105

					myWriter.Write(",")	    'p106
					myWriter.Write(",")	    'p107

					myWriter.Write(",")	    'p108
					myWriter.Write(",")	    'p109

					myWriter.Write(",")	    'p110
					myWriter.Write(",")	    'p111

					myWriter.Write(",")	    'p112
					myWriter.Write(",")	    'p113

					myWriter.Write(",")	    'p114
					myWriter.Write(",")	    'p115
					myWriter.Write(",")	    'p116
					myWriter.Write(",")	    'p117

					myWriter.Write(",")	    'p118
					myWriter.Write(",")	    'p119
					myWriter.Write(",")	    'p120
					myWriter.Write(",")	    'p121

					myWriter.Write(",")	    'p122
					myWriter.Write(",")	    'p123
					myWriter.Write(",")	    'p124
					myWriter.Write(",")	    'p125

					myWriter.Write(",")	    'p126
					myWriter.Write(",")	    'p127
					myWriter.Write(",")	    'p128

					myWriter.Write(",")	    'p129
					myWriter.Write(",")	    'p130
					myWriter.Write(",")	    'p131

					myWriter.Write(",")	    'p132
					myWriter.Write(",")	    'p133
					myWriter.Write(",")	    'p134

					myWriter.Write(",")	    'p135
					myWriter.Write(",")	    'p136
					myWriter.Write(",")	    'p137

					myWriter.Write(",")	    'p138
					myWriter.Write(",")	'p139
					myWriter.Write(",")	    'p140

					myWriter.Write(",")	    'p141
					myWriter.Write(",")	    'p142
					myWriter.Write(",")	    'p143

					myWriter.Write(",")	    'p144
					myWriter.Write(",")	    'p145
					myWriter.Write(",")	    'p146

					myWriter.Write(",")	    'p147
					myWriter.Write(",")	    'p148
					myWriter.Write(",")	'p149

					myWriter.Write(",")	    'p150
					myWriter.Write(",")	    'p151
					myWriter.Write(",")	    'p152

					myWriter.Write(",")	    'p153
					myWriter.Write(",")	    'p154
					myWriter.Write(",")	    'p155
					myWriter.Write(",")	    'p156

					myWriter.Write(",")	    'p157
					myWriter.Write(",")	    'p158
					myWriter.Write(",")	    'p159

					myWriter.Write(",")	    'p160
					myWriter.Write(",")	    'p161
					myWriter.Write(",")	    'p162
					myWriter.Write(",")	    'p163

					myWriter.Write(",")	    'p164
					myWriter.Write(",")	    'p165

					myWriter.Write(",")	    'p166

					myWriter.Write(",")	    'p167
					myWriter.Write(",")	    'p168
					myWriter.Write(",")	    'p169
					myWriter.Write(",")	    'p170
					myWriter.Write(",")	    'p171

					myWriter.Write(",")	    'p172
					myWriter.Write(",")	    'p173

					myWriter.Write(",")	    'p174
					myWriter.Write(",")	    'p175

					myWriter.Write(",")	    'p176
					myWriter.Write(",")	    'p177

					myWriter.Write(",")	    'p178
					myWriter.Write(",")	    'p179

					myWriter.Write(",")	    'p180
					myWriter.Write(",")	    'p181
					myWriter.Write(",")	    'p182
					myWriter.Write(",")	    'p183
					myWriter.Write(",")	    'p184

					myWriter.Write(",")	    'p185
					myWriter.Write(",")	    'p186

					myWriter.Write(",")	    'p187
					myWriter.Write(",")	    'p188

					myWriter.Write(",")	    'p189
					myWriter.Write(",")	    'p190

					myWriter.Write(",")	    'p191
					myWriter.Write(lstPayment(i).PaymentPurpose.ToString)		  'p192



					''''''''''''''''changes end here 2018-03-07----------------------------------------



					myWriter.Write(myWriter.NewLine)

					invTotal = invTotal + lstPayment(i).PaymentAmount

				End If

				i = i + 1

			Loop

			myWriter.Write("T,")
			myWriter.Write(i & ",")
			myWriter.Write(invTotal)

			myWriter.Write(myWriter.NewLine)

			myWriter.Close()
		End Using

		'End If
	End Sub

	' generate payment CSV to a user define file location
	Public Sub ExtractCSV(ByVal savedata As DataTable)


		Dim myStream As IO.Stream, i As Integer
		Dim saveFileDialog1 As New SaveFileDialog()

		saveFileDialog1.Filter = "CSV files (*.csv)|*.csv"
		saveFileDialog1.FilterIndex = 2
		saveFileDialog1.RestoreDirectory = True


		Using myWriter As IO.StreamWriter = New IO.StreamWriter("c:\Test.csv")


			Dim str As New StringBuilder

			i = 0

			Do While i < savedata.Rows.Count

				If i = 0 Then

					myWriter.Write("H,")
					myWriter.Write("P")

					myWriter.Write(myWriter.NewLine)

					myWriter.Write(savedata.Rows(i).Item("RecordType").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentType").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("ProcessingMode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("ServiceType").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("CustomerRef").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("CustomerMemo").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("DebitACCountryCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("DebitACCityCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("DebitACNo").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentValueDate").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("Payee").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeAddress1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeAddress2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeCountryCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeFaxNumber").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBankCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBankLocalClearingCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBranchLocalClearingCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBranchSubCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeACNo").ToString() & ",")


					myWriter.Write(savedata.Rows(i).Item("PaymentDetails1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentDetails2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("VatAmount").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTPrintingLoc").ToString() & ",")

					myWriter.Write(savedata.Rows(i).Item("WHTFormID").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTTaxID").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTRefNo").ToString() & ",")


					myWriter.Write(savedata.Rows(i).Item("WHTType1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTDescription1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTGrossAmount1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTAmount1").ToString() & ",")

					myWriter.Write(savedata.Rows(i).Item("WHTType2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTDescription2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTGrossAmount2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTAmount2").ToString() & ",")

					myWriter.Write(savedata.Rows(i).Item("DiscountAmount").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("InvoiceFormat").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentCurrency").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("InvoicePaymentAmount").ToString() & ",")



					''''''''''''''''changes starts here 2018-03-07----------------------------------------

					myWriter.Write(savedata.Rows(i).Item("LocalChargeTo").ToString() & ",") 'p40
					myWriter.Write(savedata.Rows(i).Item("OverseasChargeTo").ToString() & ",")	'p41
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankCode").ToString() & ",") 'p42
					myWriter.Write(savedata.Rows(i).Item("ClearingCodeTT").ToString() & ",") 'p43
					myWriter.Write(savedata.Rows(i).Item("ClearingZoneCodeLBC").ToString() & ",") 'p44
					myWriter.Write(savedata.Rows(i).Item("DraweebankCodeIBC").ToString() & ",") 'p45
					myWriter.Write(savedata.Rows(i).Item("DeliveryMethod").ToString() & ",") 'p46
					myWriter.Write(savedata.Rows(i).Item("DeliveryTo").ToString() & ",") 'p47
					myWriter.Write(savedata.Rows(i).Item("CounterPickupLoc").ToString() & ",")	'p48
					myWriter.Write(savedata.Rows(i).Item("FXType").ToString() & ",")	'p49
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryName1LL").ToString() & ",")	'p50
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryName2LL").ToString() & ",")	'p51
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress1LL").ToString() & ",")	'p52
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress2LL").ToString() & ",")	'p53
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress3LL").ToString() & ",")	'p54
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress4LL").ToString() & ",")	'p55
					myWriter.Write(savedata.Rows(i).Item("PaymentDetail1LL").ToString() & ",")	'p56
					myWriter.Write(savedata.Rows(i).Item("PaymentDetail2LL").ToString() & ",")	'p57
					myWriter.Write(savedata.Rows(i).Item("VATType").ToString() & ",")	'p58
					myWriter.Write(savedata.Rows(i).Item("DiscountType").ToString() & ",")	'p59
					myWriter.Write(savedata.Rows(i).Item("DebitCurrency").ToString() & ",")	'p60
					myWriter.Write(savedata.Rows(i).Item("DebitBankID").ToString() & ",")	'p61
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryID").ToString() & ",")	'p62
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryEmailID").ToString() & ",")	'p63
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryType").ToString() & ",")	'p64
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryBankType").ToString() & ",")	'p65
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryBankName").ToString() & ",")	'p66
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryBankAddress").ToString() & ",")	'p67
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankType").ToString() & ",")	'p68
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankName").ToString() & ",")	'p69
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankAddress").ToString() & ",")	'p70
					myWriter.Write(savedata.Rows(i).Item("RecieverCorresBankCode").ToString() & ",")	'p71
					myWriter.Write(savedata.Rows(i).Item("OrderingCustormer").ToString() & ",")	'p72
					myWriter.Write(savedata.Rows(i).Item("RelatedInformation").ToString() & ",")	'p73
					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode1").ToString() & ",")	'p74
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail1").ToString() & ",")	'p75

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode2").ToString() & ",")	'p76
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail2").ToString() & ",")	'p77

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode3").ToString() & ",")	'p78
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail3").ToString() & ",")	'p79

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode4").ToString() & ",")	'p80
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail4").ToString() & ",")	'p81

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode5").ToString() & ",")	'p82
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail5").ToString() & ",")	'p83

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode6").ToString() & ",")	'p84
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail6").ToString() & ",")	'p85

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode1").ToString() & ",")	'p86
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail1").ToString() & ",")	'p87

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode2").ToString() & ",")	'p88
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail2").ToString() & ",")	'p89

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode3").ToString() & ",")	'p90
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail3").ToString() & ",")	'p91

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode4").ToString() & ",")	'p92
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail4").ToString() & ",")	'p93

					myWriter.Write(savedata.Rows(i).Item("InstructionCode1").ToString() & ",")	'p94
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc1").ToString() & ",")	'p95

					myWriter.Write(savedata.Rows(i).Item("InstructionCode2").ToString() & ",")	'p96
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc2").ToString() & ",")	'p97

					myWriter.Write(savedata.Rows(i).Item("InstructionCode3").ToString() & ",")	'p98
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc3").ToString() & ",")	'p99

					myWriter.Write(savedata.Rows(i).Item("InstructionCode4").ToString() & ",")	'p100
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc4").ToString() & ",")	'p101

					myWriter.Write(savedata.Rows(i).Item("InstructionCode5").ToString() & ",")	'p102
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc5").ToString() & ",")	'p103

					myWriter.Write(savedata.Rows(i).Item("InstructionCode6").ToString() & ",")	'p104
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc6").ToString() & ",")	'p105

					myWriter.Write(savedata.Rows(i).Item("RegReportingCode1").ToString() & ",")	'p106
					myWriter.Write(savedata.Rows(i).Item("RegReportingDesc1").ToString() & ",")	'p107

					myWriter.Write(savedata.Rows(i).Item("RegReportingCode2").ToString() & ",")	'p108
					myWriter.Write(savedata.Rows(i).Item("RegReportingDesc2").ToString() & ",")	'p109

					myWriter.Write(savedata.Rows(i).Item("RegReportingCode3").ToString() & ",")	'p110
					myWriter.Write(savedata.Rows(i).Item("RegReportingDesc3").ToString() & ",")	'p111

					myWriter.Write(savedata.Rows(i).Item("SenderCharges").ToString() & ",")	'p112
					myWriter.Write(savedata.Rows(i).Item("RecieverCharges").ToString() & ",")	'p113

					myWriter.Write(savedata.Rows(i).Item("ChequeNo").ToString() & ",")	'p114
					myWriter.Write(savedata.Rows(i).Item("ChequeIssueDate").ToString() & ",")	'p115
					myWriter.Write(savedata.Rows(i).Item("CorporateChequeNo").ToString() & ",")	'p116
					myWriter.Write(savedata.Rows(i).Item("ExternalMemo").ToString() & ",")	'p117

					myWriter.Write(savedata.Rows(i).Item("MailingAddress1").ToString() & ",")	'p118
					myWriter.Write(savedata.Rows(i).Item("MailingAddress2").ToString() & ",")	'p119
					myWriter.Write(savedata.Rows(i).Item("MailingAddress3").ToString() & ",")	'p120
					myWriter.Write(savedata.Rows(i).Item("MailingAddress4").ToString() & ",")	'p121

					myWriter.Write(savedata.Rows(i).Item("TransactionCode").ToString() & ",")	'p122
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader1").ToString() & ",")	'p123
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment1").ToString() & ",")	'p124
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght1").ToString() & ",")	'p125

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader2").ToString() & ",")	'p126
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment2").ToString() & ",")	'p127
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght2").ToString() & ",")	'p128

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader3").ToString() & ",")	'p129
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment3").ToString() & ",")	'p130
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght3").ToString() & ",")	'p131

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader4").ToString() & ",")	'p132
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment4").ToString() & ",")	'p133
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght4").ToString() & ",")	'p134

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader5").ToString() & ",")	'p135
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment5").ToString() & ",")	'p136
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght5").ToString() & ",")	'p137

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader6").ToString() & ",")	'p138
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment6").ToString() & ",")	'p139
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght6").ToString() & ",")	'p140

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader7").ToString() & ",")	'p141
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment7").ToString() & ",")	'p142
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght7").ToString() & ",")	'p143

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader8").ToString() & ",")	'p144
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment8").ToString() & ",")	'p145
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght8").ToString() & ",")	'p146

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader9").ToString() & ",")	'p147
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment9").ToString() & ",")	'p148
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght9").ToString() & ",")	'p149

					myWriter.Write(savedata.Rows(i).Item("FxRateIndicator").ToString() & ",")	'p150
					myWriter.Write(savedata.Rows(i).Item("DestinationCCountryCode").ToString() & ",")	'p151
					myWriter.Write(savedata.Rows(i).Item("DestinationCCityCode").ToString() & ",")	'p152

					myWriter.Write(savedata.Rows(i).Item("DatePriority").ToString() & ",")	'p153
					myWriter.Write(savedata.Rows(i).Item("AmountPriority").ToString() & ",")	'p154
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfName").ToString() & ",")	'p155
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAccount").ToString() & ",")	'p156

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress1").ToString() & ",")	'p157
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress2").ToString() & ",")	'p158
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress3").ToString() & ",")	'p159

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfNameLL").ToString() & ",")	'p160
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress1LL").ToString() & ",")	'p161
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress2LL").ToString() & ",")	'p162
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress3LL").ToString() & ",")	'p163

					myWriter.Write(savedata.Rows(i).Item("FxRateType").ToString() & ",")	'p164
					myWriter.Write(savedata.Rows(i).Item("DeliveryOption").ToString() & ",")	'p165

					myWriter.Write(savedata.Rows(i).Item("PurposeOfPaymentTransID").ToString() & ",")	'p166

					myWriter.Write(savedata.Rows(i).Item("RecieverID").ToString() & ",")	'p167
					myWriter.Write(savedata.Rows(i).Item("RecieverIDType").ToString() & ",")	'p168
					myWriter.Write(savedata.Rows(i).Item("CustomerName").ToString() & ",")	'p169
					myWriter.Write(savedata.Rows(i).Item("ListedCompanyCode").ToString() & ",")	'p170
					myWriter.Write(savedata.Rows(i).Item("ByOrderOfSelf").ToString() & ",")	'p171

					myWriter.Write(savedata.Rows(i).Item("PaySubProductType").ToString() & ",")	'p172
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAccountType").ToString() & ",")	'p173

					myWriter.Write(savedata.Rows(i).Item("OBOId").ToString() & ",")	'p174
					myWriter.Write(savedata.Rows(i).Item("CASPaymtIndicator").ToString() & ",")	'p175

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfType").ToString() & ",")	'p176
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIDCode").ToString() & ",")	'p177

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyCountryCode").ToString() & ",")	'p178
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIdentifierIssuer").ToString() & ",")	'p179

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIdentifierIssuingAuthority").ToString() & ",")	'p180
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIdentifierRegAuthority").ToString() & ",")	'p181
					myWriter.Write(savedata.Rows(i).Item("PartyIdentifierValue").ToString() & ",")	'p182
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress1Code").ToString() & ",")	'p183
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress1CountryCode").ToString() & ",")	'p184

					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress2Code").ToString() & ",")	'p185
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress2CountryCode").ToString() & ",")	'p186

					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress3Code").ToString() & ",")	'p187
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress3CountryCode").ToString() & ",")	'p188

					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddressLineIssuer").ToString() & ",")	'p189
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddressLineIssuerLL").ToString() & ",")	'p190

					myWriter.Write(savedata.Rows(i).Item("TaxReferenceNo").ToString() & ",")	'p191
					myWriter.Write(savedata.Rows(i).Item("CreditCountryPaymentPurpose").ToString())	    'p192



					''''''''''''''''changes end here 2018-03-07----------------------------------------





					myWriter.Write(myWriter.NewLine)

				Else

					myWriter.Write(savedata.Rows(i).Item("RecordType").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentType").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("ProcessingMode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("ServiceType").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("CustomerRef").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("CustomerMemo").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("DebitACCountryCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("DebitACCityCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("DebitACNo").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentValueDate").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("Payee").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeAddress1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeAddress2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeCountryCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeFaxNumber").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBankCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBankLocalClearingCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBranchLocalClearingCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeBranchSubCode").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PayeeACNo").ToString() & ",")


					myWriter.Write(savedata.Rows(i).Item("PaymentDetails1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentDetails2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("VatAmount").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTPrintingLoc").ToString() & ",")

					myWriter.Write(savedata.Rows(i).Item("WHTFormID").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTTaxID").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTRefNo").ToString() & ",")


					myWriter.Write(savedata.Rows(i).Item("WHTType1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTDescription1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTGrossAmount1").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTAmount1").ToString() & ",")

					myWriter.Write(savedata.Rows(i).Item("WHTType2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTDescription2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTGrossAmount2").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("WHTAmount2").ToString() & ",")

					myWriter.Write(savedata.Rows(i).Item("DiscountAmount").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("InvoiceFormat").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("PaymentCurrency").ToString() & ",")
					myWriter.Write(savedata.Rows(i).Item("InvoicePaymentAmount").ToString() & ",")



					''''''''''''''''changes starts here 2018-03-07----------------------------------------

					myWriter.Write(savedata.Rows(i).Item("LocalChargeTo").ToString() & ",") 'p40
					myWriter.Write(savedata.Rows(i).Item("OverseasChargeTo").ToString() & ",")	'p41
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankCode").ToString() & ",") 'p42
					myWriter.Write(savedata.Rows(i).Item("ClearingCodeTT").ToString() & ",") 'p43
					myWriter.Write(savedata.Rows(i).Item("ClearingZoneCodeLBC").ToString() & ",") 'p44
					myWriter.Write(savedata.Rows(i).Item("DraweebankCodeIBC").ToString() & ",") 'p45
					myWriter.Write(savedata.Rows(i).Item("DeliveryMethod").ToString() & ",") 'p46
					myWriter.Write(savedata.Rows(i).Item("DeliveryTo").ToString() & ",") 'p47
					myWriter.Write(savedata.Rows(i).Item("CounterPickupLoc").ToString() & ",")	'p48
					myWriter.Write(savedata.Rows(i).Item("FXType").ToString() & ",")	'p49
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryName1LL").ToString() & ",")	'p50
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryName2LL").ToString() & ",")	'p51
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress1LL").ToString() & ",")	'p52
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress2LL").ToString() & ",")	'p53
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress3LL").ToString() & ",")	'p54
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAddress4LL").ToString() & ",")	'p55
					myWriter.Write(savedata.Rows(i).Item("PaymentDetail1LL").ToString() & ",")	'p56
					myWriter.Write(savedata.Rows(i).Item("PaymentDetail2LL").ToString() & ",")	'p57
					myWriter.Write(savedata.Rows(i).Item("VATType").ToString() & ",")	'p58
					myWriter.Write(savedata.Rows(i).Item("DiscountType").ToString() & ",")	'p59
					myWriter.Write(savedata.Rows(i).Item("DebitCurrency").ToString() & ",")	'p60
					myWriter.Write(savedata.Rows(i).Item("DebitBankID").ToString() & ",")	'p61
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryID").ToString() & ",")	'p62
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryEmailID").ToString() & ",")	'p63
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryType").ToString() & ",")	'p64
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryBankType").ToString() & ",")	'p65
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryBankName").ToString() & ",")	'p66
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryBankAddress").ToString() & ",")	'p67
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankType").ToString() & ",")	'p68
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankName").ToString() & ",")	'p69
					myWriter.Write(savedata.Rows(i).Item("IntermediaryBankAddress").ToString() & ",")	'p70
					myWriter.Write(savedata.Rows(i).Item("RecieverCorresBankCode").ToString() & ",")	'p71
					myWriter.Write(savedata.Rows(i).Item("OrderingCustormer").ToString() & ",")	'p72
					myWriter.Write(savedata.Rows(i).Item("RelatedInformation").ToString() & ",")	'p73
					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode1").ToString() & ",")	'p74
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail1").ToString() & ",")	'p75

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode2").ToString() & ",")	'p76
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail2").ToString() & ",")	'p77

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode3").ToString() & ",")	'p78
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail3").ToString() & ",")	'p79

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode4").ToString() & ",")	'p80
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail4").ToString() & ",")	'p81

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode5").ToString() & ",")	'p82
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail5").ToString() & ",")	'p83

					myWriter.Write(savedata.Rows(i).Item("SpecialInstCode6").ToString() & ",")	'p84
					myWriter.Write(savedata.Rows(i).Item("SpecialInstDetail6").ToString() & ",")	'p85

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode1").ToString() & ",")	'p86
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail1").ToString() & ",")	'p87

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode2").ToString() & ",")	'p88
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail2").ToString() & ",")	'p89

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode3").ToString() & ",")	'p90
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail3").ToString() & ",")	'p91

					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoCode4").ToString() & ",")	'p92
					myWriter.Write(savedata.Rows(i).Item("RemittanceInfoDetail4").ToString() & ",")	'p93

					myWriter.Write(savedata.Rows(i).Item("InstructionCode1").ToString() & ",")	'p94
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc1").ToString() & ",")	'p95

					myWriter.Write(savedata.Rows(i).Item("InstructionCode2").ToString() & ",")	'p96
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc2").ToString() & ",")	'p97

					myWriter.Write(savedata.Rows(i).Item("InstructionCode3").ToString() & ",")	'p98
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc3").ToString() & ",")	'p99

					myWriter.Write(savedata.Rows(i).Item("InstructionCode4").ToString() & ",")	'p100
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc4").ToString() & ",")	'p101

					myWriter.Write(savedata.Rows(i).Item("InstructionCode5").ToString() & ",")	'p102
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc5").ToString() & ",")	'p103

					myWriter.Write(savedata.Rows(i).Item("InstructionCode6").ToString() & ",")	'p104
					myWriter.Write(savedata.Rows(i).Item("InstructionCodeDesc6").ToString() & ",")	'p105

					myWriter.Write(savedata.Rows(i).Item("RegReportingCode1").ToString() & ",")	'p106
					myWriter.Write(savedata.Rows(i).Item("RegReportingDesc1").ToString() & ",")	'p107

					myWriter.Write(savedata.Rows(i).Item("RegReportingCode2").ToString() & ",")	'p108
					myWriter.Write(savedata.Rows(i).Item("RegReportingDesc2").ToString() & ",")	'p109

					myWriter.Write(savedata.Rows(i).Item("RegReportingCode3").ToString() & ",")	'p110
					myWriter.Write(savedata.Rows(i).Item("RegReportingDesc3").ToString() & ",")	'p111

					myWriter.Write(savedata.Rows(i).Item("SenderCharges").ToString() & ",")	'p112
					myWriter.Write(savedata.Rows(i).Item("RecieverCharges").ToString() & ",")	'p113

					myWriter.Write(savedata.Rows(i).Item("ChequeNo").ToString() & ",")	'p114
					myWriter.Write(savedata.Rows(i).Item("ChequeIssueDate").ToString() & ",")	'p115
					myWriter.Write(savedata.Rows(i).Item("CorporateChequeNo").ToString() & ",")	'p116
					myWriter.Write(savedata.Rows(i).Item("ExternalMemo").ToString() & ",")	'p117

					myWriter.Write(savedata.Rows(i).Item("MailingAddress1").ToString() & ",")	'p118
					myWriter.Write(savedata.Rows(i).Item("MailingAddress2").ToString() & ",")	'p119
					myWriter.Write(savedata.Rows(i).Item("MailingAddress3").ToString() & ",")	'p120
					myWriter.Write(savedata.Rows(i).Item("MailingAddress4").ToString() & ",")	'p121

					myWriter.Write(savedata.Rows(i).Item("TransactionCode").ToString() & ",")	'p122
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader1").ToString() & ",")	'p123
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment1").ToString() & ",")	'p124
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght1").ToString() & ",")	'p125

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader2").ToString() & ",")	'p126
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment2").ToString() & ",")	'p127
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght2").ToString() & ",")	'p128

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader3").ToString() & ",")	'p129
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment3").ToString() & ",")	'p130
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght3").ToString() & ",")	'p131

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader4").ToString() & ",")	'p132
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment4").ToString() & ",")	'p133
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght4").ToString() & ",")	'p134

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader5").ToString() & ",")	'p135
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment5").ToString() & ",")	'p136
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght5").ToString() & ",")	'p137

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader6").ToString() & ",")	'p138
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment6").ToString() & ",")	'p139
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght6").ToString() & ",")	'p140

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader7").ToString() & ",")	'p141
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment7").ToString() & ",")	'p142
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght7").ToString() & ",")	'p143

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader8").ToString() & ",")	'p144
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment8").ToString() & ",")	'p145
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght8").ToString() & ",")	'p146

					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceHeader9").ToString() & ",")	'p147
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColAlignment9").ToString() & ",")	'p148
					myWriter.Write(savedata.Rows(i).Item("CustomInvoiceColLenght9").ToString() & ",")	'p149

					myWriter.Write(savedata.Rows(i).Item("FxRateIndicator").ToString() & ",")	'p150
					myWriter.Write(savedata.Rows(i).Item("DestinationCCountryCode").ToString() & ",")	'p151
					myWriter.Write(savedata.Rows(i).Item("DestinationCCityCode").ToString() & ",")	'p152

					myWriter.Write(savedata.Rows(i).Item("DatePriority").ToString() & ",")	'p153
					myWriter.Write(savedata.Rows(i).Item("AmountPriority").ToString() & ",")	'p154
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfName").ToString() & ",")	'p155
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAccount").ToString() & ",")	'p156

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress1").ToString() & ",")	'p157
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress2").ToString() & ",")	'p158
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress3").ToString() & ",")	'p159

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfNameLL").ToString() & ",")	'p160
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress1LL").ToString() & ",")	'p161
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress2LL").ToString() & ",")	'p162
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfAddress3LL").ToString() & ",")	'p163

					myWriter.Write(savedata.Rows(i).Item("FxRateType").ToString() & ",")	'p164
					myWriter.Write(savedata.Rows(i).Item("DeliveryOption").ToString() & ",")	'p165

					myWriter.Write(savedata.Rows(i).Item("PurposeOfPaymentTransID").ToString() & ",")	'p166

					myWriter.Write(savedata.Rows(i).Item("RecieverID").ToString() & ",")	'p167
					myWriter.Write(savedata.Rows(i).Item("RecieverIDType").ToString() & ",")	'p168
					myWriter.Write(savedata.Rows(i).Item("CustomerName").ToString() & ",")	'p169
					myWriter.Write(savedata.Rows(i).Item("ListedCompanyCode").ToString() & ",")	'p170
					myWriter.Write(savedata.Rows(i).Item("ByOrderOfSelf").ToString() & ",")	'p171

					myWriter.Write(savedata.Rows(i).Item("PaySubProductType").ToString() & ",")	'p172
					myWriter.Write(savedata.Rows(i).Item("BeneficiaryAccountType").ToString() & ",")	'p173

					myWriter.Write(savedata.Rows(i).Item("OBOId").ToString() & ",")	'p174
					myWriter.Write(savedata.Rows(i).Item("CASPaymtIndicator").ToString() & ",")	'p175

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfType").ToString() & ",")	'p176
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIDCode").ToString() & ",")	'p177

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyCountryCode").ToString() & ",")	'p178
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIdentifierIssuer").ToString() & ",")	'p179

					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIdentifierIssuingAuthority").ToString() & ",")	'p180
					myWriter.Write(savedata.Rows(i).Item("OnBehalfOfPartyIdentifierRegAuthority").ToString() & ",")	'p181
					myWriter.Write(savedata.Rows(i).Item("PartyIdentifierValue").ToString() & ",")	'p182
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress1Code").ToString() & ",")	'p183
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress1CountryCode").ToString() & ",")	'p184

					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress2Code").ToString() & ",")	'p185
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress2CountryCode").ToString() & ",")	'p186

					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress3Code").ToString() & ",")	'p187
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddress3CountryCode").ToString() & ",")	'p188

					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddressLineIssuer").ToString() & ",")	'p189
					myWriter.Write(savedata.Rows(i).Item("OnbehalfOfAddressLineIssuerLL").ToString() & ",")	'p190

					myWriter.Write(savedata.Rows(i).Item("TaxReferenceNo").ToString() & ",")	'p191
					myWriter.Write(savedata.Rows(i).Item("CreditCountryPaymentPurpose").ToString())	    'p192



					''''''''''''''''changes end here 2018-03-07----------------------------------------



					myWriter.Write(myWriter.NewLine)


				End If

				i = i + 1

			Loop

			myWriter.Close()
		End Using

		'End If
	End Sub
	
	' read file processing response status from the third party application
	' using the response file dropped in a shared file location
	Public Sub ReadResponse(strfname As String)

		Dim strfilename As String = strfname
		Dim num_rows As Long
		Dim num_cols As Long
		Dim x As Integer
		Dim y As Integer
		Dim strarray(1, 1) As String
		Dim pinarray As New ArrayList
		Dim readStatus As Boolean = True

		' check if the file exist in the pathh
		If File.Exists(strfname) Then

			Dim tmpstream As StreamReader = File.OpenText(strfilename)
			Dim strlines() As String
			Dim strline() As String
			Dim i As Integer

			'read all lines in the file
			strlines = tmpstream.ReadToEnd().Split(New String() {"BPV"}, StringSplitOptions.None)

			Do While i < strlines.Length
				' check if the transaction line has a status of processed
				If strlines(i).Contains("Processed") = True Then

					strline = strlines(i).Split(",")
					Dim txttransactionRef As String, txtreason As String = ""
					' retrieve the document number from the response
					txttransactionRef = "BPV" & strline(0).Replace("""", "")

					If txttransactionRef.Split("_").Count > 1 Then
						'update payment voucher document status on dynamic NAV 
						updatePaymentLine(txttransactionRef.Split("_")(1), "Successful")
					Else
					End If


				Else

				End If

				i = i + 1
			Loop

			'Exit Sub


			Dim str(), newfileName As String
			str = strfname.Split("\")
			Array.Reverse(str)
			' generate a new filename to rename processed file
			newfileName = Path.GetDirectoryName(strfname) & "\" & "read_" & str(0).Split(".")(0) & ".dump"
			tmpstream.Close()
			tmpstream.Dispose()
			If readStatus = False Then
				' rename process file after update Dynamics NAV with the status
				renameReadFile(strfname, newfileName)
			Else
			End If

		End If

	End Sub

	' retrieve all available response file from the third party business application
	Public Sub getFiles()


		Dim files() As String = System.IO.Directory.GetFiles("C:\deleted\H2HReports", "*.csv")
		Dim i As Integer

		Do While i < files.Length

			If File.Exists(files(i)) = True Then

				ReadResponse(files(i))

			Else

			End If

			i = i + 1

		Loop

	End Sub

	' rename file in the specified file location
	Public Sub renameReadFile(filePath As String, newfilePath As String)

		File.Copy(filePath, newfilePath)
		File.Delete(filePath)

	End Sub


	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		
		Try
			
			'get all approved payment voucher requests to generate instruction file for the third party application
			getPVEntries()

		Catch ex As Exception
			MsgBox("" & ex.Message)
			Exit Sub
		End Try

		
	End Sub
End Class
