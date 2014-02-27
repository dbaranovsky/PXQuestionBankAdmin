
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[SubmissionNotes]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[SubmissionNotes](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[NoteId] [uniqueidentifier] NOT NULL,
		[ItemId] [nvarchar](50) NOT NULL,
		[EnrollmentId] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_SubmissionNotes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[SubmissionNotes]  WITH CHECK ADD  CONSTRAINT [FK_SubmissionNotes_Note] FOREIGN KEY([NoteId])
	REFERENCES [dbo].[Note] ([NoteId])

	ALTER TABLE [dbo].[SubmissionNotes] CHECK CONSTRAINT [FK_SubmissionNotes_Note]

END

GO


