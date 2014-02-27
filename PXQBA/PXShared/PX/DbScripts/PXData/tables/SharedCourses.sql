

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF OBJECT_ID('dbo.[SharedCourses]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[SharedCourses](
		[SharedCourseId] [bigint] IDENTITY(1,1) NOT NULL,
		[CourseId] [bigint] NOT NULL,
		[UserId] [bigint] NOT NULL,
		[AnonymousName] [varchar](50) NULL,
		[Created] [datetime] NOT NULL,
		[Modified] [datetime] NOT NULL,
		[Note] [text] NULL,
	 CONSTRAINT [PK_SharedCourse_Join] PRIMARY KEY CLUSTERED 
	(
		[SharedCourseId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

GO


