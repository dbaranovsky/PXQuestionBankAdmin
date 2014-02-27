
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[EmailSendHistory]','U') IS NULL
BEGIN

	--Email send history
	CREATE TABLE [dbo].[EmailSendHistory](
		[TrackingEmailId] [bigint] NOT NULL,
		[EmailAddress] [nvarchar](50) NOT NULL,
		[Status] [char](1) NOT NULL
	) ON [PRIMARY]

	ALTER TABLE [dbo].[EmailSendHistory]  WITH CHECK ADD  CONSTRAINT [FK_EmailSendHistory_EmailTracking] FOREIGN KEY([TrackingEmailId])
	REFERENCES [dbo].[EmailTracking] ([EmailId])

	ALTER TABLE [dbo].[EmailSendHistory] CHECK CONSTRAINT [FK_EmailSendHistory_EmailTracking]

END

GO