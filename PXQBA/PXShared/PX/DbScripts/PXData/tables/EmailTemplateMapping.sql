
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[EmailTemplateMapping]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[EmailTemplateMapping](
	[TemplateId] [int] NOT NULL,
	[ProductId] [nvarchar](50) NOT NULL,
	[EventType] [int] NOT NULL
	) ON [PRIMARY]

	ALTER TABLE [dbo].[EmailTemplateMapping]  WITH CHECK ADD  CONSTRAINT [FK_EmailTemplateMapping_EmailTemplate] FOREIGN KEY([TemplateId])
	REFERENCES [dbo].[EmailTemplate] ([Id])

	ALTER TABLE [dbo].[EmailTemplateMapping] CHECK CONSTRAINT [FK_EmailTemplateMapping_EmailTemplate]

END

GO