Public Class PaymentObj

	Dim txtRecordType As String	  'P
	Dim txtPaymentType As String	  'PAY
	Dim txtProcessingMode As String 'ON
	Dim txtServiceType As String ' not required
	Dim txtCustomerRef As String ' Gotten from NAV
	Dim txtCustomerMemo As String	' not required
	Dim txtDebitAccCountry As String	' NG
	Dim txtDebitAccCity As String	' LOS
	Dim txtDebitAccNo As String	' Gotten from NAV
	Dim dteValueDate As Date		' Gotten from NAV
	Dim txtPayee As String		' Gotten from NAV Max of 35 xters the rest xters should flow to add1 and add2
	Dim txtPayeeAddress1 As String
	Dim txtPayeeAddress2 As String
	Dim txtPayeeCountryCode As String	'NG
	Dim txtPayeeFaxNumber As String	'not required
	Dim txtPayeeBankCode As String	'not required
	Dim txtPayeeBankClearingCode As String	' to be provided
	Dim txtPayeeBranchClearingCode As String	' to be provided

	Dim txtPayeeBankLocalClearingCode As String
	Dim txtPayeeBranchLocalClearingCode As String


	Dim txtPayeeBranchSubCode As String	' to be provided
	Dim txtPayeeAccount As String	' Gotten from NAV
	Dim txtPaymentDetails1 As String	' Gotten from NAV
	Dim txtPaymentDetails2 As String	' Gotten from NAV
	Dim numVAT As Decimal	' not required
	Dim txtWHTPrinting As String	' not required
	Dim txtWHTFormID As String	' not required
	Dim txtWHTTaxID As String	' not required
	Dim txtWHTRefNo As String	' not required
	Dim txtWHTType1 As String	' not required
	Dim txtWHTDescription1 As String	' not required
	Dim numWHTGrossAmount1 As Decimal	' not required
	Dim numWHTAmount1 As Decimal	' not required
	Dim txtWHTType2 As String	' not required
	Dim txtWHTDescription2 As String	' not required
	Dim numWHTGrossAmount2 As Decimal	' not required
	Dim numWHTAmount2 As Decimal	' not required

	Dim numDiscountAmount As Decimal	' not required
	Dim txtInvoiceFormat As String	' not required
	Dim txtPaymentCurrency As String	' NGN
	Dim numPaymentAmount As Decimal


	Dim txtLocalChargeTo As String  ' C
	Dim txtOverseesChargeTo As String	' not required
	Dim txtIntermediateBankCode As String  ' not required
	Dim txtClearingCode As String	 ' not required

	Dim txtClearingZoneCode As String	 ' not required
	Dim txtDraweeBankCode As String	 ' not required
	Dim txtDeliveryMode As String	 ' not required
	Dim txtDeliverTo As String	 ' not required
	Dim txtCounterPickupLoc As String	 ' not required

	Dim txtFXTypes As String	 ' not required
	Dim txtLocalLanPayeeName1 As String	 ' not required
	Dim txtLocalLanPayeeName2 As String	 ' not required

	Dim txtLLPayeeAddress1 As String	 ' not required
	Dim txtLLPayeeAddress2 As String	 ' not required
	Dim txtLLPayeeAddress3 As String	 ' not required
	Dim txtLLPayeeAddress4 As String	 ' not required
	Dim txtLLPaymentDetail1 As String	 ' not required
	Dim txtLLPaymentDetail2 As String	 ' not required

	Dim txtVaTType As String	 ' not required
	Dim txtDiscountType As String	 ' not required

	Dim txtDebitCurrency As String	 ' not required

	Dim txtDebitBankID As String	 ' not required
	Dim txtBeneficiaryID As String	 ' not required
	Dim txtBeneficiaryEmailID As String	 ' not required
	Dim txtBeneficiaryType As String	 ' not required
	Dim txtBeneficiaryBankType As String	 ' not required
	Dim txtBeneficiaryBankName As String	 ' not required
	Dim txtBeneficiaryBankAddress As String	 ' not required
	Dim txtIntermediateBankType As String	 ' not required
	Dim txtIntermediateBankName As String	 ' not required
	Dim txtIntermediateBankAddress As String	 ' not required

	Dim txtRecieverBankCode As String	 ' not required
	Dim txtOrderingCustomer As String	 ' not required
	Dim txtRelatedInformation As String	 ' not required

	Dim txtSpecialInstrCode1 As String	 ' not required
	Dim txtSpecialInstrDetail1 As String	 ' not required

	Dim txtSpecialInstrCode2 As String	 ' not required
	Dim txtSpecialInstrDetail2 As String	 ' not required

	Dim txtSpecialInstrCode3 As String	 ' not required
	Dim txtSpecialInstrDetail3 As String	 ' not required

	Dim txtSpecialInstrCode4 As String	 ' not required
	Dim txtSpecialInstrDetail4 As String	 ' not required

	Dim txtSpecialInstrCode5 As String	 ' not required
	Dim txtSpecialInstrDetail5 As String	 ' not required

	Dim txtSpecialInstrCode6 As String	 ' not required
	Dim txtSpecialInstrDetail6 As String	 ' not required

	Dim txtRemittanceInfoCode1 As String	 ' not required
	Dim txtRemittanceInfoDetail1 As String	 ' not required

	Dim txtRemittanceInfoCode2 As String	 ' not required
	Dim txtRemittanceInfoDetail2 As String	 ' not required

	Dim txtRemittanceInfoCode3 As String	 ' not required
	Dim txtRemittanceInfoDetail3 As String	 ' not required

	Dim txtRemittanceInfoCode4 As String	 ' not required
	Dim txtRemittanceInfoDetail4 As String	 ' not required

	Dim txtInstructionCode1 As String	 ' not required
	Dim txtInstructionCodeDesc1 As String	 ' not required

	Dim txtInstructionCode2 As String	 ' not required
	Dim txtInstructionCodeDesc2 As String	 ' not required

	Dim txtInstructionCode3 As String	 ' not required
	Dim txtInstructionCodeDesc3 As String	 ' not required

	Dim txtInstructionCode4 As String	 ' not required
	Dim txtInstructionCodeDesc4 As String	 ' not required

	Dim txtInstructionCode5 As String	 ' not required
	Dim txtInstructionCodeDesc5 As String	 ' not required

	Dim txtInstructionCode6 As String	 ' not required
	Dim txtInstructionCodeDesc6 As String	 ' not required

	Dim txtRegReportingCode1 As String	 ' not required
	Dim txtRegReportingDesc1 As String	 ' not required

	Dim txtRegReportingCode2 As String	 ' not required
	Dim txtRegReportingDesc2 As String	 ' not required

	Dim txtRegReportingCode3 As String	 ' not required
	Dim txtRegReportingDesc3 As String	 ' not required

	Dim txtSenderCharges As String	 ' not required
	Dim txtRecieverCharges As String	 ' not required

	Dim txtChequeNo As String	 ' not required
	Dim dteChequeIssuedDate As Date	 ' not required
	Dim txtCorporateChequeNo As String	 ' not required

	Dim txtExternalMemo As String	 ' not required
	Dim txtMailingAddy1 As String	 ' not required
	Dim txtMailingAddy2 As String	 ' not required
	Dim txtMailingAddy3 As String	 ' not required
	Dim txtMailingAddy4 As String	 ' not required

	Dim txtTransactionCode As String	 ' not required
	Dim txtCustomInvoiceHeader1 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment As String	 ' not required
	Dim txtCustomInvoiceColumnLenght As String	 ' not required

	Dim txtCustomInvoiceHeader2 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment2 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght2 As String	 ' not required

	Dim txtCustomInvoiceHeader3 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment3 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght3 As String	 ' not req

	Dim txtCustomInvoiceHeader4 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment4 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght4 As String	 ' not req

	Dim txtCustomInvoiceHeader5 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment5 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght5 As String	 ' not req

	Dim txtCustomInvoiceHeader6 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment6 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght6 As String	 ' not req

	Dim txtCustomInvoiceHeader7 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment7 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght7 As String	 ' not req

	Dim txtCustomInvoiceHeader8 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment8 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght8 As String	 ' not req

	Dim txtCustomInvoiceHeader9 As String	 ' not required
	Dim txtCustomInvoiceColumnAlighment9 As String	 ' not required
	Dim txtCustomInvoiceColumnLenght9 As String	 ' not req

	Dim txtFXRateIndicator As String	 ' not req
	Dim txtDestinationCenterCountryCode As String	 ' not req
	Dim txtDestinationCenterCityCode As String	 ' not req

	Dim dtePriority As Date	 ' not req
	Dim numPriority As Date	 ' not req

	Dim txtOnBehalfName As String	 ' not req
	Dim txtOnBehalfAccount As String	 ' not req
	Dim txtOnBehalfAddress1 As String	 ' not req
	Dim txtOnBehalfAddress2 As String	 ' not req
	Dim txtOnBehalfAddress3 As String	 ' not req

	Dim txtOnBehalfNameLocalLan As String	 ' not req
	Dim txtOnBehalfAddressLocalLan1 As String	 ' not req
	Dim txtOnBehalfAddressLocalLan2 As String	 ' not req
	Dim txtOnBehalfAddressLocalLan3 As String	 ' not req

	Dim txtFXRateType As String	 ' not req
	Dim txtDeliveryOption As String	 ' not req

	Dim txtPurposeOfPmt As String	 ' not req

	Dim txtRecieverID As String	 ' not req
	Dim txtRecieverIDType As String	 ' not req
	Dim txtCustomerName As String	 ' not req
	Dim txtListedCompanyCode As String	 ' not req
	Dim txtOrderOfSelf As String	 ' not req

	Dim txtSubProductType As String	 ' not req
	Dim txtBeneficiaryAccType As String	 ' not req
	Dim txtOBOId As String	 ' not req
	Dim txtCASPaymIndicator As String	 ' not req

	Dim txtOnbehalfofType As String	 ' not req
	Dim txtOnbehalfofPartyIdentifierCode As String	 ' not req
	Dim txtOnbehalfofPartyCountryCode As String	 ' not req
	Dim txtOnbehalfofPartyIdentifierIssuer As String	 ' not req
	Dim txtOnbehalfofPartyIdentifierIssueingAuthority As String	 ' not req
	Dim txtOnbehalfofPartyIdentifierRegistrationAuthority As String	 ' not req

	Dim txtPartyIdentifierValue As String	 ' not req

	Dim txtOnbehalfofAddress1Code As String	 ' not req
	Dim txtOnbehalfofAddress1CountryCode As String	 ' not req

	Dim txtOnbehalfofAddress2Code As String	 ' not req
	Dim txtOnbehalfofAddress2CountryCode As String	 ' not req

	Dim txtOnbehalfofAddress3Code As String	 ' not req
	Dim txtOnbehalfofAddress3CountryCode As String	 ' not req

	Dim txtOnbehalfofAddressLineIssuer As String	 ' not req
	Dim txtOnbehalfofAddressLineIssuerLL As String	 ' not req

	Dim txtTaxReferenceNunmber As String	 ' not req

	Dim txtCCPurposeOfPayment As String	 ' not req
	Dim txtPurposeGroup As String	 ' not req
	Dim txtChargeAccountNo As String	 ' not req
	Dim txtDocumentCount As String	 ' not req

	Dim txtDocumentName1 As String	 ' not req
	Dim txtDocumentName2 As String	 ' not req
	Dim txtDocumentName3 As String	 ' not req
	Dim txtDocumentName4 As String	 ' not req
	Dim txtDocumentName5 As String	 ' not req

	Dim txtSourceoffund As String	 ' not req

	Dim txtRecipientRefNumber1 As String	 ' not req
	Dim txtRecipientRefNumber2 As String	 ' not req

	Dim txtBeneficiaryContactNo As String	 ' not req

	Dim txtJointPayeeName As String	 ' not req

	Dim txtOthers As String	 ' not req
	Dim txtPaymentPurpose As String	 ' not req

	Property BeneficiaryEmailID As String
		Get
			Return txtBeneficiaryEmailID
		End Get
		Set(ByVal value As String)
			txtBeneficiaryEmailID = value
		End Set
	End Property


	Property RecordType As String
		Get
			Return txtRecordType
		End Get
		Set(ByVal value As String)
			txtRecordType = value
		End Set
	End Property

	Property PaymentType As String
		Get
			Return txtPaymentType
		End Get
		Set(ByVal value As String)
			txtPaymentType = value
		End Set
	End Property

	Property ProcessingMode As String
		Get
			Return txtProcessingMode
		End Get
		Set(ByVal value As String)
			txtProcessingMode = value
		End Set
	End Property

	Property CustomerRef As String
		Get
			Return txtCustomerRef
		End Get
		Set(ByVal value As String)
			txtCustomerRef = value
		End Set
	End Property

	Property CustomerMemo As String
		Get
			Return txtCustomerMemo
		End Get
		Set(ByVal value As String)
			txtCustomerMemo = value
		End Set
	End Property

	Property DebitAccCountry As String
		Get
			Return txtDebitAccCountry
		End Get
		Set(ByVal value As String)
			txtDebitAccCountry = value
		End Set
	End Property

	Property DebitAccCity As String
		Get
			Return txtDebitAccCity
		End Get
		Set(ByVal value As String)
			txtDebitAccCity = value
		End Set
	End Property

	Property DebitAccNo As String
		Get
			Return txtDebitAccNo
		End Get
		Set(ByVal value As String)
			txtDebitAccNo = value
		End Set
	End Property

	Property ServiceType As String
		Get
			Return txtServiceType
		End Get
		Set(ByVal value As String)
			txtServiceType = value
		End Set
	End Property

	Property ValueDate As Date
		Get
			Return dteValueDate
		End Get
		Set(ByVal value As Date)
			dteValueDate = value
		End Set
	End Property

	Property Payee As String
		Get
			Return txtPayee
		End Get
		Set(ByVal value As String)
			txtPayee = value
		End Set
	End Property

	Property PayeeAddress1 As String
		Get
			Return txtPayeeAddress1
		End Get
		Set(ByVal value As String)
			txtPayeeAddress1 = value
		End Set
	End Property

	Property PayeeAddress2 As String
		Get
			Return txtPayeeAddress2
		End Get
		Set(ByVal value As String)
			txtPayeeAddress2 = value
		End Set
	End Property

	Property PayeeCountryCode As String
		Get
			Return txtPayeeCountryCode
		End Get
		Set(ByVal value As String)
			txtPayeeCountryCode = value
		End Set
	End Property

	Property PayeeFaxNumber As String
		Get
			Return txtPayeeFaxNumber
		End Get
		Set(ByVal value As String)
			txtPayeeFaxNumber = value
		End Set
	End Property

	Property PayeeBankCode As String
		Get
			Return txtPayeeBankCode
		End Get
		Set(ByVal value As String)
			txtPayeeBankCode = value
		End Set
	End Property

	Property PayeeBankClearingCode As String
		Get
			Return txtPayeeBankClearingCode
		End Get
		Set(ByVal value As String)
			txtPayeeBankClearingCode = value
		End Set
	End Property

	Property PayeeBranchClearingCode As String
		Get
			Return txtPayeeBranchClearingCode
		End Get
		Set(ByVal value As String)
			txtPayeeBranchClearingCode = value
		End Set
	End Property

	Property PayeeBankLocalClearingCode As String
		Get
			Return txtPayeeBankLocalClearingCode
		End Get
		Set(ByVal value As String)
			txtPayeeBankLocalClearingCode = value
		End Set
	End Property

	Property PayeeBranchLocalClearingCode As String
		Get
			Return txtPayeeBranchLocalClearingCode
		End Get
		Set(ByVal value As String)
			txtPayeeBranchLocalClearingCode = value
		End Set
	End Property

	Property PayeeBranchSubCode As String
		Get
			Return txtPayeeBranchSubCode
		End Get
		Set(ByVal value As String)
			txtPayeeBranchSubCode = value
		End Set
	End Property

	Property PayeeAccount As String
		Get
			Return txtPayeeAccount
		End Get
		Set(ByVal value As String)
			txtPayeeAccount = value
		End Set
	End Property

	Property PaymentDetails1 As String
		Get
			Return txtPaymentDetails1
		End Get
		Set(ByVal value As String)
			txtPaymentDetails1 = value
		End Set
	End Property

	Property PaymentDetails2 As String
		Get
			Return txtPaymentDetails2
		End Get
		Set(ByVal value As String)
			txtPaymentDetails2 = value
		End Set
	End Property

	Property VAT As Decimal
		Get
			Return numVAT
		End Get
		Set(ByVal value As Decimal)
			numVAT = value
		End Set
	End Property

	Property WHTPrinting As String
		Get
			Return txtWHTPrinting
		End Get
		Set(ByVal value As String)
			txtWHTPrinting = value
		End Set
	End Property

	Property WHTFormID As String
		Get
			Return txtWHTFormID
		End Get
		Set(ByVal value As String)
			txtWHTFormID = value
		End Set
	End Property

	Property WHTTaxID As String
		Get
			Return txtWHTTaxID
		End Get
		Set(ByVal value As String)
			txtWHTTaxID = value
		End Set
	End Property

	Property WHTRefNo As String
		Get
			Return txtWHTRefNo
		End Get
		Set(ByVal value As String)
			txtWHTRefNo = value
		End Set
	End Property

	Property WHTType1 As String
		Get
			Return txtWHTType1
		End Get
		Set(ByVal value As String)
			txtWHTType1 = value
		End Set
	End Property

	Property WHTDescription1 As String
		Get
			Return txtWHTDescription1
		End Get
		Set(ByVal value As String)
			txtWHTDescription1 = value
		End Set
	End Property

	Property WHTGrossAmount1 As Decimal
		Get
			Return numWHTGrossAmount1
		End Get
		Set(ByVal value As Decimal)
			numWHTGrossAmount1 = value
		End Set
	End Property

	Property WHTAmount1 As Decimal
		Get
			Return numWHTAmount1
		End Get
		Set(ByVal value As Decimal)
			numWHTAmount1 = value
		End Set
	End Property


	Property WHTType2 As String
		Get
			Return txtWHTType2
		End Get
		Set(ByVal value As String)
			txtWHTType2 = value
		End Set
	End Property

	Property WHTDescription2 As String
		Get
			Return txtWHTDescription2
		End Get
		Set(ByVal value As String)
			txtWHTDescription2 = value
		End Set
	End Property

	Property WHTGrossAmount2 As Decimal
		Get
			Return numWHTGrossAmount2
		End Get
		Set(ByVal value As Decimal)
			numWHTGrossAmount2 = value
		End Set
	End Property

	Property WHTAmount2 As Decimal
		Get
			Return numWHTAmount2
		End Get
		Set(ByVal value As Decimal)
			numWHTAmount2 = value
		End Set
	End Property

	Property DiscountAmount As Decimal
		Get
			Return numDiscountAmount
		End Get
		Set(ByVal value As Decimal)
			numDiscountAmount = value
		End Set
	End Property

	Property InvoiceFormat As String
		Get
			Return txtInvoiceFormat
		End Get
		Set(ByVal value As String)
			txtInvoiceFormat = value
		End Set
	End Property

	Property PaymentCurrency As String
		Get
			Return txtPaymentCurrency
		End Get
		Set(ByVal value As String)
			txtPaymentCurrency = value
		End Set
	End Property

	Property PaymentAmount As Decimal
		Get

			Return numPaymentAmount
		End Get
		Set(ByVal value As Decimal)
			numPaymentAmount = value
		End Set
	End Property

	Property Others As String
		Get

			Return txtOthers
		End Get
		Set(ByVal value As String)
			txtOthers = value
		End Set
	End Property


	Property PaymentPurpose As String
		Get

			Return txtPaymentPurpose
		End Get
		Set(ByVal value As String)
			txtPaymentPurpose = value
		End Set
	End Property

End Class


