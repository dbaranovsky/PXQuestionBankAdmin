-- MASTER FILE FOR CREATING TABLES AND STORED PROCEDURES FOR Platform-X Reporting

--******************************************************************************
-- Please enable SQLCMD mode (In Microsoft SQL Server Management Studio, select 
--  "Query --> SQLCMD Mode" from the menu).
--
-- MODIFY:	(1) The database name (DATABASENAME)
--			(2) The path to the SQL Scripts \Database directory (PATH)
--******************************************************************************

:setvar DATABASENAME "PXReporting"
:setvar PATH "D:\PX\PX\DbScripts\PXReporting\"

if not exists(select * from sys.databases where name = '$(DATABASENAME)')
    :r $(PATH)"create.sql"

USE $(DATABASENAME);
GO

-- CREATE TABLES
:r $(PATH)"tables\rpt_AnswerDetail.sql"
:r $(PATH)"tables\rpt_AssignmentMaster.sql"
:r $(PATH)"tables\rpt_ContentSession.sql"
:r $(PATH)"tables\rpt_ContentSubmission.sql"
:r $(PATH)"tables\rpt_ContentTypeMaster.sql"
:r $(PATH)"tables\rpt_CourseMaster.sql"
:r $(PATH)"tables\rpt_Enrollment.sql"
:r $(PATH)"tables\rpt_FileSubmission.sql"
:r $(PATH)"tables\rpt_GroupMaster.sql"
:r $(PATH)"tables\rpt_QuestionMaster.sql"
:r $(PATH)"tables\rpt_QuestionTypeMaster.sql"
:r $(PATH)"tables\rpt_QuizMaster.sql"
:r $(PATH)"tables\rpt_QuizSubmission.sql"
:r $(PATH)"tables\rpt_StatusMaster.sql"
:r $(PATH)"tables\rpt_StudentMaster.sql"


-- CREATE FUNCTIONS

-- CREATE SPROCS
:r $(PATH)"sprocs\SP_rptAssignmentSubmission.sql"
:r $(PATH)"sprocs\SP_rptAssignmentSubmission1.sql"
:r $(PATH)"sprocs\SP_rptComments1.sql"
:r $(PATH)"sprocs\SP_rptCommentsold.sql"
:r $(PATH)"sprocs\SP_rptDiscussionForum.sql"
:r $(PATH)"sprocs\SP_rptDiscussionForum1.sql"
:r $(PATH)"sprocs\SP_rptDiscussionForumold.sql"
:r $(PATH)"sprocs\SP_rptFileSubmission.sql"
:r $(PATH)"sprocs\SP_rptFileSubmission1.sql"
:r $(PATH)"sprocs\SP_rptHomework.sql"
:r $(PATH)"sprocs\SP_rptLogins.sql"
:r $(PATH)"sprocs\SP_rptPageViews.sql"
:r $(PATH)"sprocs\SP_rptQuizSubmission.sql"
:r $(PATH)"sprocs\SP_rptQuizSubmissionDetail.sql"
:r $(PATH)"sprocs\SP_rptQuizSubmissionDetailold.sql"
:r $(PATH)"sprocs\SP_rptQuizSubmissionold.sql"
:r $(PATH)"sprocs\SP_rptStudentGroup.sql"
:r $(PATH)"sprocs\SP_rptUserSubmission.sql"
:r $(PATH)"sprocs\SP_rptUserSubmissionDetail.sql"
:r $(PATH)"sprocs\SP_rptWikiActivity.sql"
:r $(PATH)"sprocs\SP_rptWikiActivityold.sql"
