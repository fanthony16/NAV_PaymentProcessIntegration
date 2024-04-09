Imports System.Net
Imports System.Text

Module Module1
	Dim lstPayments As New List(Of PaymentObj)
	Sub main()

		'MsgBox("" & System.IO.Path.GetDirectoryName(Application.ExecutablePath) & "\OutPutFile\Test.csv")
		'Exit Sub
		'MsgBox("Test")
		getPVEntries()

	End Sub

	Private Sub getPVEntries()

		Dim pv As New PaymentVoucher.PaymentVoucherList_Service
		Dim bank As New BankCard.BankCard_Service
		Dim pvc As New PaymentVoucherCard.PaymentVoucherCard_Service
		Dim bnkL As New BankList.BeneficiaryBankList_Service


		Dim nc As New NetworkCredential
		nc.Domain = "pensure-nigeria.com"
		nc.UserName = "coretec"
		nc.Password = "Ibukun@lag"

		Dim prxy As New WebProxy("172.16.0.8:8080", True)
		prxy.Credentials = nc

		pv.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
		pv.PreAuthenticate = True

		bank.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
		bank.PreAuthenticate = True

		pvc.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
		pvc.PreAuthenticate = True

		bnkL.Credentials = New NetworkCredential(nc.UserName, nc.Password, nc.Domain)
		bnkL.PreAuthenticate = True


		Dim PVFilterArray As New List(Of PaymentVoucher.PaymentVoucherList_Filter)
		Dim PVFilterArrayCard As New List(Of PaymentVoucherCard.PaymentVoucherCard_Filter)
		Dim BankFilterArrayCard As New List(Of BankCard.BankCard_Filter)
		Dim BankListFilterArrayCard As New List(Of BankList.BeneficiaryBankList_Filter)


		Dim PVFilter As New PaymentVoucher.PaymentVoucherList_Filter
		Dim PVFilter_H2HStatus As New PaymentVoucher.PaymentVoucherList_Filter
		Dim PVFilterCard As New PaymentVoucherCard.PaymentVoucherCard_Filter
		Dim BankFilterCard As New BankCard.BankCard_Filter
		Dim BankListFilterCard As New BankList.BeneficiaryBankList_Filter


		PVFilter.Field = PaymentVoucher.PaymentVoucherList_Fields.Status
		PVFilter.Criteria = "Approved"

		PVFilterArray.Add(PVFilter)

		PVFilter_H2HStatus.Field = PaymentVoucher.PaymentVoucherList_Fields.H2H_Status
		PVFilter_H2HStatus.Criteria = "<>" & PaymentVoucher.H2H_Status.Successful & "&" & "<>" & PaymentVoucher.H2H_Status.Dispatched

		PVFilterArray.Add(PVFilter_H2HStatus)



		' '''''''''''getting bank details------------
		'BankFilterCard.Field = BankCard.BankCard_Fields.No
		'BankFilterCard.Criteria = ""
		'BankFilterArrayCard.Add(BankFilterCard)
		'Dim PVBank() As BankCard.BankCard
		'PVBank = bank.ReadMultiple(BankFilterArrayCard.ToArray, "", 100)
		' '''''''''''''''''''''''''''''''''''''''''''''''''

		Dim pmv() As PaymentVoucher.PaymentVoucherList
		pmv = pv.ReadMultiple(PVFilterArray.ToArray, "", 5)

		Dim i As Integer = 0

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
				logerr.FilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
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

				BankListFilterCard.Field = BankList.BeneficiaryBankList_Fields.Bank_Code
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

				If IsNothing(pmr(0).PVLines(k).Payee_Account_No) = False Then
					BVPayments.PayeeAccount = pmr(0).PVLines(k).Payee_Account_No
				Else
					BVPayments.PayeeAccount = pmr(0).PVLines(k).Account_No
				End If


				If pmr(0).PVLines(k).Account_No.Length > 70 Then

					BVPayments.PaymentDetails1 = pmr(0).PVLines(k).Payment_Narration.Substring(0, 69).Replace(",", "")
					BVPayments.PaymentDetails2 = pmr(0).PVLines(k).Payment_Narration.Substring(70, pmr(0).PVLines(k).Payment_Narration.Length).Replace(",", "")

				Else

					BVPayments.PaymentDetails1 = pmr(0).PVLines(k).Payment_Narration.Replace(",", "")
					BVPayments.PaymentDetails2 = ""

				End If



				BVPayments.PaymentAmount = pmr(0).PVLines(k).Amount

				If pmr(0).PVLines(k).H2H_Status = PaymentVoucherCard.H2H_Status._blank_ Then
					lstPayments.Add(BVPayments)
					updatePaymentLine(pmr(0).PVLines(k).Line_No)
				Else
				End If

				k = k + 1




			Loop

			updatePaymentVoucher(pmv(i).No)

			i = i + 1

		Loop

		Try

			If lstPayments.Count > 0 Then

				ExtractCSV(lstPayments)

				Dim logerr As New Global.Logger.Logger
				logerr.FileSource = "H2H File Generator"
				logerr.FilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
				logerr.Logger(lstPayments.Count & " Payment(s) request generated Successfully")

			Else

				Dim logerr As New Global.Logger.Logger
				logerr.FileSource = "H2H File Generator"
				logerr.FilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
				logerr.Logger("No record found !!!")

			End If

		Catch ex As Exception

			Dim logerr As New Global.Logger.Logger
			logerr.FileSource = "H2H File Generator"
			logerr.FilePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)
			logerr.Logger(ex.Message)

			Exit Sub

		End Try

		MsgBox("File Generated Successfully")

		Exit Sub


	End Sub


	Public Sub ExtractCSV(ByVal lstPayment As List(Of PaymentObj))


		Dim myStream As IO.Stream, i As Integer, invTotal As Double = 0

		Dim fName As String = Now.ToString.Replace("/", "").Replace("\", "").Replace(":", "").Replace(" ", "") & ".csv"

		Using myWriter As IO.StreamWriter = New IO.StreamWriter(System.IO.Path.GetDirectoryName(Application.ExecutablePath) & "\OutPutFile\" & fName)



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


	Private Function updatePaymentLine(lineNumber As Integer) As Boolean

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

		If pml.Count > 0 Then

			pml(0).H2H_Status = PaymentLine.H2H_Status.Dispatched
			Try
				pmLines.Update(pml(0))
				Return True
			Catch ex As Exception
				Return False
			End Try


		End If


	End Function

	Private Function updatePaymentVoucher(BPVNo As String) As Boolean

		Dim PV As New PaymentVoucherCard.PaymentVoucherCard_Service

		Dim nc As New NetworkCredential
		nc.Domain = "pensure-nigeria.com"
		nc.UserName = "coretec"
		nc.Password = "Ibukun@lag"

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

			pvr(0).H2H_Status = PaymentVoucherCard.H2H_Status.Dispatched
			Try

				PV.Update(pvr(0))

				Return True
			Catch ex As Exception
				Return False
			End Try


		End If


	End Function

End Module
