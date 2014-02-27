
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[HighlightNotes]','U') IS NULL
BEGIN

CREATE TABLE [dbo].[HighlightNotes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[HighlightId] [uniqueidentifier] NOT NULL,
	[NoteId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_HighlightNotes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[HighlightNotes]  WITH CHECK ADD  CONSTRAINT [FK_HighlightNotes_Highlights] FOREIGN KEY([HighlightId])
REFERENCES [dbo].[Highlights] ([HighlightId])

ALTER TABLE [dbo].[HighlightNotes] CHECK CONSTRAINT [FK_HighlightNotes_Highlights]

ALTER TABLE [dbo].[HighlightNotes]  WITH CHECK ADD  CONSTRAINT [FK_HighlightNotes_Note] FOREIGN KEY([NoteId])
REFERENCES [dbo].[Note] ([NoteId])

ALTER TABLE [dbo].[HighlightNotes] CHECK CONSTRAINT [FK_HighlightNotes_Note]

END

GO