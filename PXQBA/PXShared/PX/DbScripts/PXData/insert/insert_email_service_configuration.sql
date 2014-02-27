SET NOCOUNT ON
GO

IF NOT EXISTS (SELECT 1 FROM dbo.EmailServiceConfiguration)
BEGIN
	INSERT INTO dbo.EmailServiceConfiguration
		(TrackingDbConnection, SendInterval, SMTPAddress, UserName, [Password]) VALUES
		('Data Source=db27.hbpna.com;Initial Catalog=PXData;User ID=pxuser;Password=m@gikM1ssl3',
		3600000, 'smtp.sendgrid.net', 'macmillanhighered', 'MHE41Mad')	
END
ELSE
BEGIN
	UPDATE dbo.EmailServiceConfiguration SET
		TrackingDbConnection = 'Data Source=db27.hbpna.com;Initial Catalog=PXData;User ID=pxuser;Password=m@gikM1ssl3',
		SendInterval = 3600000,
		SMTPAddress = 'smtp.sendgrid.net',
		UserName = 'macmillanhighered',
		[Password] = 'MHE41Mad'
END
GO