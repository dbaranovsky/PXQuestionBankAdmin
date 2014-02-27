
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[ReviewNotes]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[ReviewNotes] (
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[NoteId] [uniqueidentifier] NOT NULL,
		[ReviewId] [nvarchar](50) NOT NULL,
		[ItemId] [nvarchar](50) NOT NULL,
		[EnrollmentId] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_ReviewNotes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ReviewNotes]  WITH CHECK ADD  CONSTRAINT [FK_ReviewNotes_Note] FOREIGN KEY([NoteId])
	REFERENCES [dbo].[Note] ([NoteId])

	ALTER TABLE [dbo].[ReviewNotes] CHECK CONSTRAINT [FK_ReviewNotes_Note]

END

GO