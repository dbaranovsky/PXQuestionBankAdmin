/****** Object:  Table [dbo].[GenericPrograms]    Script Date: 11/27/2012 12:37:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.[GenericPrograms]','U') IS NULL
BEGIN

CREATE TABLE [dbo].[GenericPrograms](
	[Program_id] [bigint] IDENTITY(1,1) NOT NULL,
	[Domain_id] [bigint] NOT NULL,
 CONSTRAINT [PK_GenericProgram] PRIMARY KEY CLUSTERED 
(
	[Program_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]
END
GO