SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[rpt_FileSubmission]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[rpt_FileSubmission](
	[FileSubmissionID] [varchar](50) NOT NULL,
	[FileSubmissionDate] [datetime] NULL,
	[AssignmentID] [varchar](50) NOT NULL,
	[EnrollmentID] [int] NOT NULL,
	[FileName] [varchar](500) NULL,
	[FileType] [varchar](20) NULL,
	[CourseID] [int] NULL
) ON [PRIMARY]
END
GO


