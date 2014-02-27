SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[PresentationAlias]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[PresentationAlias](
		[AliasHash] [nvarchar](4000) NULL,
		[Alias] [nvarchar](4000) NULL,
		[HomeUrl] [nvarchar](4000) NULL,
		[CourseId] [bigint] NULL,
		[DateCreated] [Datetime] DEFAULT GETDATE(),
		[DateModified] [Datetime] DEFAULT GETDATE()
		CONSTRAINT AliasHash_Unique UNIQUE CLUSTERED ([AliasHash]		
		)
	) ON [PRIMARY]

END

GO