
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[ItemUpdates]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[ItemUpdates](
		[CourseId] [nvarchar](50) NOT NULL,
		[EnrollmentId] [nvarchar](50) NOT NULL,
		[ItemId] [nvarchar](50) NOT NULL,
		[DateUpdated] [datetime] NOT NULL,
		[Status] [tinyint] NOT NULL,
	CONSTRAINT [PK_ItemUpdates] PRIMARY KEY CLUSTERED 
	(
		[CourseId] ASC,
		[EnrollmentId] ASC,
		[ItemId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]


END

GO


