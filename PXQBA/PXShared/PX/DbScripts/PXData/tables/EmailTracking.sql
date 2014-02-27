
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[EmailTracking]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[EmailTracking](
		[EmailId] [bigint] NOT NULL IDENTITY(1,1),
		[CourseId] [nvarchar](50) NOT NULL,
		[InstructorId] [int] NOT NULL,
		[EmailData] [xml] NOT NULL,
		[SendOnDate] [datetime] NOT NULL,
		[Status] [varchar](15) NOT NULL,
		[NotificationType] [int] NOT NULL,
		[ItemId] [nvarchar](50) NULL,
	 CONSTRAINT [PK_EmailTracking] PRIMARY KEY CLUSTERED 
	(
		[EmailId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO