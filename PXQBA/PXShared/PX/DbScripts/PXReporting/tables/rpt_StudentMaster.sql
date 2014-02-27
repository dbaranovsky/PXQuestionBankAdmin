
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[rpt_StudentMaster]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[rpt_StudentMaster](
	[StudentID] [int] NOT NULL,
	[StudentName] [nvarchar](100) NOT NULL
) ON [PRIMARY]
END
GO


