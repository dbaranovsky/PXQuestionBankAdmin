SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SP_rptQuizSubmissionDetail]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SP_rptQuizSubmissionDetail]
END
GO

CREATE PROCEDURE [dbo].[SP_rptQuizSubmissionDetail]
	-- Add the parameters for the stored procedure here
		@StudentID int = null,
		@SubmissionDate datetime =null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT cs.ContentSubmissionDate,stm.StatusName,cs.Score,cs.IPAddress,sm.StudentName,cs.EnrollmentID as StudentID ,  
	CONVERT(varchar(6), cts.Duration/3600) + ' h ' + RIGHT('0' + CONVERT(varchar(2), (cts.Duration % 3600) / 60), 2) + ' m ' + RIGHT('0' + CONVERT(varchar(2), cts.Duration % 60), 2) AS SessionLength,
		(SELECT COUNT(cts.ContentSubmissionID) from rpt_ContentSession cts WHERE cts.StatusID =1 and cs.ContentSubmissionDate =@SubmissionDate and cs.EnrollmentID =@StudentID ) as TotalSubmissions,
		(SELECT COUNT(cts.ContentSubmissionID) from rpt_ContentSession cts WHERE cts.StatusID =2 and cs.ContentSubmissionDate =@SubmissionDate and cs.EnrollmentID =@StudentID ) as SavedSubmissions
	FROM rpt_ContentSubmission cs inner join
		 rpt_StudentMaster sm ON cs.EnrollmentID = sm.StudentID Inner Join
		 rpt_ContentSession cts ON cs.ContentSubmissionID = cts.ContentSubmissionID Inner Join
		 rpt_StatusMaster stm on cts.StatusID = stm.StatusID
	Where sm.StudentID =@StudentID and cs.ContentTypeID =1 and cs.ContentSubmissionDate = @SubmissionDate	 
	group by cts.StatusID,cs.ContentSubmissionDate, cts.Duration ,stm.StatusName,cs.Score,cs.IPAddress,sm.StudentName,cs.EnrollmentID 

END

GO