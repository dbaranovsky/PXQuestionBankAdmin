SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SP_rptUserSubmission]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SP_rptUserSubmission]
END
GO


CREATE PROCEDURE [dbo].[SP_rptUserSubmission]
(
	@StudentID int = 0,
	@StartDate datetime = Null,
	@EndDate datetime =Null
) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	SELECT cs.ContentSubmissionDate,en.StudentID,sm.StudentName,cs.Score 
		FROM rpt_ContentSubmission cs
		Inner Join rpt_Enrollment en ON cs.EnrollmentID = en.EnrollmentID 
		Inner Join rpt_StudentMaster sm ON en.StudentID = sm.StudentID
		WHERE cs.ContentTypeID =1 and en.StudentID =1 and (cs.ContentSubmissionDate >=@StartDate and cs.ContentSubmissionDate <=@EndDate)

END

GO