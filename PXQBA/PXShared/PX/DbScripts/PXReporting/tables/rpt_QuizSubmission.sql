SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[rpt_QuizSubmission]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[rpt_QuizSubmission](
	[ContentSubmissionID] [int] IDENTITY(1,1) NOT NULL,
	[QuizID] [varchar](50) NULL,
	[QuestionID] [varchar](50) NOT NULL,
	[EnrollmentID] [int] NOT NULL,
	[StudentAnswer] [varchar](50) NOT NULL,
	[QuestionVersion] [varchar](15) NULL,
 CONSTRAINT [PK_rpt_QuizSubmission] PRIMARY KEY CLUSTERED 
(
	[ContentSubmissionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO



