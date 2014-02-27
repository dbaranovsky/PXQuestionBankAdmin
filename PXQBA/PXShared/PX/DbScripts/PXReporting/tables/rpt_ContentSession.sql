SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[rpt_ContentSession]','U') IS NULL
BEGIN

CREATE TABLE [dbo].[rpt_ContentSession](
	[ContentSubmissionID] [varchar](50) NOT NULL,
	[SessionStartDate] [datetime] NULL,
	[SessionEndDate] [datetime] NULL,
	[StatusID] [varchar](5) NOT NULL,
	[Duration] [varchar](5) NOT NULL
) ON [PRIMARY]
END
GO



