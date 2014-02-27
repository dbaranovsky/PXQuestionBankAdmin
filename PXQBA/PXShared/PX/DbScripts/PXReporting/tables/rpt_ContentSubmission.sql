SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('rpt_ContentSubmission]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[rpt_ContentSubmission](
	[ContentTypeID] [int] NOT NULL,
	[ContentSubmissionID] [varchar](100) NOT NULL,
	[ContentSubmissionDate] [datetime] NULL,
	[EnrollmentID] [int] NULL,
	[Topic] [varchar](200) NULL,
	[Wordcount] [int] NULL,
	[PageCount] [int] NULL,
	[IPAddress] [varchar](15) NULL,
	[SubmissionTitle] [varchar](50) NULL,
	[CourseID] [int] NOT NULL,
	[Score] [float] NULL,
 CONSTRAINT [PK_ContentSubmission] PRIMARY KEY CLUSTERED 
(
	[ContentTypeID] ASC,
	[ContentSubmissionID] ASC,
	[CourseID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


ALTER TABLE [dbo].[rpt_ContentSubmission]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_ContentSubmission_rpt_ContentTypeMaster] FOREIGN KEY([ContentTypeID])
REFERENCES [dbo].[rpt_ContentTypeMaster] ([ContentTypeID])
GO

ALTER TABLE [dbo].[rpt_ContentSubmission] CHECK CONSTRAINT [FK_rpt_ContentSubmission_rpt_ContentTypeMaster]
GO


