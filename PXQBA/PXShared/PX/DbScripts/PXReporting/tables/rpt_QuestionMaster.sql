SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[rpt_QuestionMaster]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[rpt_QuestionMaster](
	[QuestionID] [varchar](50) NOT NULL,
	[QuestionText] [nvarchar](750) NULL,
	[QuestionType] [varchar](25) NULL,
	[QuestionTitle] [varchar](50) NULL
) ON [PRIMARY]
END
GO


