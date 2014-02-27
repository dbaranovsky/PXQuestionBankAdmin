
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[SharedCourseUsers]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[SharedCourseUsers](
		[SharedCourseUserId] [bigint] IDENTITY(1,1) NOT NULL,
		[SharedCourseId] [bigint] NOT NULL,
		[UserId] [bigint] NOT NULL,
		[IsActive] [bit] NOT NULL,
		[Created] [datetime] NOT NULL,
		[Modified] [datetime] NOT NULL,
	 CONSTRAINT [PK_SharedCourseUsers] PRIMARY KEY CLUSTERED 
	(
		[SharedCourseUserId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END
