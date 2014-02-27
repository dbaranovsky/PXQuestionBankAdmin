

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF OBJECT_ID('dbo.[SharedCourseEnrollmentItems]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[SharedCourseEnrollmentItems](
		[SharedItemId] BIGINT IDENTITY(1,1) NOT NULL,
		[SharedEnrollmentId] BIGINT NOT NULL,		
		[ItemId] NVARCHAR(50) NOT NULL,
		[IsActive] BIT NULL,
		[DateCreated] DATETIME NULL,
		[DateModified] DATETIME NULL,
		 CONSTRAINT [PK_SharedCourseEnrollmentItems] PRIMARY KEY CLUSTERED 
		 (
			[SharedItemId] ASC
		 )
	) ON [PRIMARY]
	
	ALTER TABLE [dbo].[SharedCourseEnrollmentItems]  WITH CHECK ADD CONSTRAINT [FK_SharedCourseEnrollmentItems] FOREIGN KEY([SharedEnrollmentId])
	REFERENCES [dbo].[SharedCourseEnrollments] ([SharedEnrollmentId])	

END

GO


