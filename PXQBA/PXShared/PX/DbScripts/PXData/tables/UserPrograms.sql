
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[UserPrograms]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[UserPrograms] (
	[Program_id] BIGINT NOT NULL,
	[User_dashboard_id] BIGINT NULL,
	[User_id] BIGINT NOT NULL,
	[User_ref_id] BIGINT NOT NULL,
	[User_domain_id] BIGINT NOT NULL
	) ON [PRIMARY]


	ALTER TABLE [dbo].[UserPrograms] WITH CHECK ADD CONSTRAINT [FK_Programs_UserPrograms]
	FOREIGN KEY ([Program_id]) REFERENCES [dbo].[Programs]([Program_id])

	ALTER TABLE [dbo].[UserPrograms] CHECK CONSTRAINT [FK_Programs_UserPrograms]

END

GO