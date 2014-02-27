
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[Programs]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[Programs] (
	[Program_id] BIGINT IDENTITY (1,1) NOT NULL,
	[Dashboard_id] BIGINT NULL,
	[Program_manager_id] BIGINT NOT NULL,
	[Program_manager_ref_id] BIGINT NOT NULL,
	[Program_manager_domain_id] BIGINT NOT NULL,
	CONSTRAINT [PK_Program] PRIMARY KEY CLUSTERED
	(
		[Program_id]
	)) ON [PRIMARY]

END

GO