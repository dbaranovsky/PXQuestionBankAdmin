
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[ItemNotes]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[ItemNotes](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[ItemId] [nvarchar](50) NOT NULL,
		[NoteId] [uniqueidentifier] NOT NULL,
		[CourseId] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_ItemNotes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ItemNotes]  WITH CHECK ADD  CONSTRAINT [FK_ItemNotes_Note] FOREIGN KEY([NoteId])
	REFERENCES [dbo].[Note] ([NoteId])

	ALTER TABLE [dbo].[ItemNotes] CHECK CONSTRAINT [FK_ItemNotes_Note]

END

GO

