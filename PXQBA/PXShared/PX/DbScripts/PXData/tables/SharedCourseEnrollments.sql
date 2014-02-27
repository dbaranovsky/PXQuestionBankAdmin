SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[SharedCourseEnrollments]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[SharedCourseEnrollments]
	(
		SharedEnrollmentId BIGINT IDENTITY(1,1) NOT NULL,
		SharedId BIGINT NOT NULL,
		EnrollmentId NVARCHAR(50) NOT NULL,
		IsActive BIT NULL,
		DateCreated DATETIME NULL,
		DateModified DATETIME NULL,
		CONSTRAINT [PK_SharedCourseEnrollments] PRIMARY KEY CLUSTERED 
		(
			[SharedEnrollmentId] ASC
		)
	) ON [PRIMARY]

	ALTER TABLE [dbo].[SharedCourseEnrollments]  WITH CHECK ADD  CONSTRAINT [FK_SharedCourseEnrollments] FOREIGN KEY([SharedId])
	REFERENCES [dbo].[SharedCourses] ([SharedCourseId])	

END


