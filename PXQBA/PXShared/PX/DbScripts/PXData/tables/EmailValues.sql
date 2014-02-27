
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[EmailValues]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[EmailValues](
	[TrackingEmailId] [bigint] NOT NULL,
	[Key] [varchar](50) NOT NULL,
	[Value] [nvarchar](4000) NOT NULL
	) ON [PRIMARY]


	ALTER TABLE [dbo].[EmailValues]  WITH CHECK ADD  CONSTRAINT [FK_EmailValues_EmailTracking] FOREIGN KEY([TrackingEmailId])
	REFERENCES [dbo].[EmailTracking] ([EmailId])

	ALTER TABLE [dbo].[EmailValues] CHECK CONSTRAINT [FK_EmailValues_EmailTracking]

END

GO