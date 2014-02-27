
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[ItemHighlights]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[ItemHighlights](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[HighlightId] [uniqueidentifier] NOT NULL,
		[ItemId] [nvarchar](50) NOT NULL,
		[CourseId] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_ItemHighlights] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ItemHighlights]  WITH CHECK ADD  CONSTRAINT [FK_ItemHighlights_Highlights] FOREIGN KEY([HighlightId])
	REFERENCES [dbo].[Highlights] ([HighlightId])

	ALTER TABLE [dbo].[ItemHighlights] CHECK CONSTRAINT [FK_ItemHighlights_Highlights]

END

GO