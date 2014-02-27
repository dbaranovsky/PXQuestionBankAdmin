SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[Environment]','U') IS NULL
BEGIN

CREATE TABLE [dbo].[Environment](
	[EnvironmentId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](255) NOT NULL,
	[Description] [varchar](255) NULL,
	[DlapServer] [varchar](255) NULL,
	[BrainHoneyServer] [varchar](255) NULL,
	[PxDocs] [varchar](255) NULL,
 CONSTRAINT [PK_Environment] PRIMARY KEY CLUSTERED 
(
	[EnvironmentId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


END
GO