SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[rpt_AnswerDetail]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[rpt_AnswerDetail](
	[AnswerID] [int] IDENTITY(1,1) NOT NULL,
	[Questionid] [varchar](50) NOT NULL,
	[AnswerText] [varchar](750) NOT NULL,
	[CorrectAnswer] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Report_Answer] PRIMARY KEY CLUSTERED 
(
	[AnswerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


