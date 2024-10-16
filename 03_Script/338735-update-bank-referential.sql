UPDATE [Mandate].[RefBank]
SET IsJdcScrapable = 0, 
    IsJdcPartner = 1, 
    JdcPartnership = 3, -- Partner
    BankName = 'Shine'
WHERE BankCode = '17418'