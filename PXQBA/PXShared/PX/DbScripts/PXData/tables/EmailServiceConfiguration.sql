
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[EmailServiceConfiguration]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[EmailServiceConfiguration](
	[TrackingDbConnection] [varchar](250) NULL,
	[SendInterval] [int] NULL,
	[SMTPAddress] [varchar](50) NULL,
	[UserName] [varchar](50) NULL,
	[Password] [varchar](50) NULL
	) ON [PRIMARY]

END

GO