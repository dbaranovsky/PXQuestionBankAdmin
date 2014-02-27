/****** Object:  Table [dbo].[ProgramDashboard]    Script Date: 11/27/2012 10:16:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF OBJECT_ID('dbo.[ProgramDashboard]','U') IS NULL
BEGIN

CREATE TABLE [dbo].[ProgramDashboard](
	[Program_id] [bigint] NOT NULL,
	[Dashboard_id] [bigint] NOT NULL,
	[Product_type] [varchar](100) NOT NULL
) ON [PRIMARY]
END
GO

SET ANSI_PADDING OFF
GO
