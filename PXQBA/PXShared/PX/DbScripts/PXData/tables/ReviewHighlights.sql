
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[ReviewHighlights]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[ReviewHighlights](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[HighlightId] [uniqueidentifier] NOT NULL,
		[ReviewId] [nvarchar](50) NOT NULL,
		[ItemId] [nvarchar](50) NOT NULL,
		[EnrollmentId] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_ReviewHighlights] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ReviewHighlights]  WITH CHECK ADD  CONSTRAINT [FK_ReviewHighlights_Highlights] FOREIGN KEY([HighlightId])
	REFERENCES [dbo].[Highlights] ([HighlightId])

	ALTER TABLE [dbo].[ReviewHighlights] CHECK CONSTRAINT [FK_ReviewHighlights_Highlights]

END

GO


