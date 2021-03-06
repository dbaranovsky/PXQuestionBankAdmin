-- MASTER FILE FOR CREATING TABLES AND STORED PROCEDURES FOR Platform-X

--******************************************************************************
-- Please enable SQLCMD mode (In Microsoft SQL Server Management Studio, select 
--  "Query --> SQLCMD Mode" from the menu).
--
-- MODIFY:	(1) The database name (DATABASENAME)
--			(2) The path to the SQL Scripts \Database directory (PATH)
--******************************************************************************

-- The following variables must be set properly if this script is run, BUT DO NOT CHECK THEM IN
:setvar DATABASENAME "PXData_QA"
:setvar PATH "C:\Work\PXBranchCheckouts2\PX\DbScripts\PXData"

if not exists(select * from sys.databases where name = '$(DATABASENAME)')
    :r $(PATH)"create.sql"

USE $(DATABASENAME);
GO

-- CREATE TABLES
:r $(PATH)"tables\Users.sql"
:r $(PATH)"tables\UserTypes.sql"
:r $(PATH)"tables\Highlights.sql"
:r $(PATH)"tables\Note.sql"
:r $(PATH)"tables\HighlightNotes.sql"
:r $(PATH)"tables\TopNotes.sql"
:r $(PATH)"tables\ItemHighlights.sql"
:r $(PATH)"tables\ItemNotes.sql"
:r $(PATH)"tables\ReviewHighlights.sql"
:r $(PATH)"tables\ReviewNotes.sql"
:r $(PATH)"tables\SubmissionHighlights.sql"
:r $(PATH)"tables\SubmissionNotes.sql"
:r $(PATH)"tables\UserNoteSettings.sql"
:r $(PATH)"tables\UserShares.sql"
:r $(PATH)"tables\ItemUpdates.sql"
:r $(PATH)"tables\EmailTracking.sql"
:r $(PATH)"tables\EmailValues.sql"
:r $(PATH)"tables\EmailTemplate.sql"
:r $(PATH)"tables\EmailSendHistory.sql"
:r $(PATH)"tables\EmailTemplateMapping.sql"
:r $(PATH)"tables\EmailServiceConfiguration.sql"
:r $(PATH)"tables\Programs.sql"
:r $(PATH)"tables\UserPrograms.sql"
:r $(PATH)"tables\PersonalEportfolioDashboard.sql"
:r $(PATH)"tables\SharedCourses.sql"
:r $(PATH)"tables\SharedCourseUsers.sql"
:r $(PATH)"tables\SharedCourseItems.sql"
:r $(PATH)"tables\SharedCourseEnrollments.sql"
:r $(PATH)"tables\SharedCourseEnrollmentItems.sql"
:r $(PATH)"tables\VideoNotes.sql"
:r $(PATH)"tables\Dashboard.sql"
:r $(PATH)"tables\Dashboard_Types.sql"
:r $(PATH)"tables\ProgramDashboard.sql"
:r $(PATH)"tables\GenericPrograms.sql"
:r $(PATH)"tables\tblPxWebRights.sql"
:r $(PATH)"tables\tblPxWebUserRights.sql"
:r $(PATH)"tables\QuestionLogEventTypes.sql"
:r $(PATH)"tables\QuestionLogs.sql"
:r $(PATH)"tables\QuestionNotes.sql"

-- ALTER SCRIPTS
:r $(PATH)"alter\tables\alter_highlights_add_color.sql"
:r $(PATH)"alter\tables\alter_usershares_update_highlightcolor.sql"
:r $(PATH)"alter\tables\alter_note_add_status.sql"
:r $(PATH)"alter\tables\alter_note_add_description.sql"
:r $(PATH)"alter\tables\alter_usershares_add_NotesEnabled_rename_Enabled.sql"
:r $(PATH)"alter\tables\alter_emailtracking_add_productid_remove_emaildata.sql"
:r $(PATH)"alter\tables\alter_videonotes_add_videoId.sql"
:r $(PATH)"alter\tables\alter_videonotes_add_accessType.sql"
:r $(PATH)"alter\tables\alter_dashboard_add_productcourse_Id.sql"
:r $(PATH)"alter\tables\alter_EmailValues_changed_Value_size_to_Max.sql"

:r $(PATH)"alter\tables\alter_Highlights_add_XpathInfo.sql"
:r $(PATH)"alter\tables\alter_QuestionLogs_add_changes_column.sql"
:r $(PATH)"alter\tables\alter_QuestionLogs_add_version_column.sql"

-- CREATE FUNCTIONS
:r $(PATH)"functions\NotesSearchUserIds.sql"
:r $(PATH)"functions\CommaDelimitedToTable.sql"
:r $(PATH)"functions\GetEportfolioNoteCount.sql"
--:r $(PATH)"functions\fnSplitDelimitedString.sql"
--:r $(PATH)"functions\fnSplitKeyValuePairsList.sql"

-- CREATE SPROCS
:r $(PATH)"sprocs\AddNote.sql"
:r $(PATH)"sprocs\AddHighlight.sql"
:r $(PATH)"sprocs\SetHighlightNoteRelation.sql"
:r $(PATH)"sprocs\SetTopNoteRelation.sql"
:r $(PATH)"sprocs\AddHighlightNote.sql"
:r $(PATH)"sprocs\AddNoteToHighlight.sql"
:r $(PATH)"sprocs\AddNoteToTopNote.sql"
:r $(PATH)"sprocs\GetItemNotes.sql"
:r $(PATH)"sprocs\GetReviewNotes.sql"
:r $(PATH)"sprocs\GetSubmissionNotes.sql"
:r $(PATH)"sprocs\ShareHighlight.sql"
:r $(PATH)"sprocs\ShareNote.sql"
:r $(PATH)"sprocs\UpdateHighlightStatus.sql"
:r $(PATH)"sprocs\UpdateNoteStatus.sql"
:r $(PATH)"sprocs\UpdateNote.sql"
:r $(PATH)"sprocs\StopSharingToUser.sql"
:r $(PATH)"sprocs\ShareNotes.sql"
:r $(PATH)"sprocs\GetAllSharedNotes.sql"
:r $(PATH)"sprocs\UpdateHighlightColor.sql"
:r $(PATH)"sprocs\GetNoteCount.sql"
:r $(PATH)"sprocs\InitializeUser.sql"
:r $(PATH)"sprocs\GetNoteSettings.sql"
:r $(PATH)"sprocs\UpdateNoteSettings.sql"
:r $(PATH)"sprocs\GetNotesByUser.sql"
:r $(PATH)"sprocs\GetNotesForPeerReview.sql"
:r $(PATH)"sprocs\RemoveItemUpdates.sql"
:r $(PATH)"sprocs\RemoveItemUpdate.sql"
:r $(PATH)"sprocs\ResetItemUpdatedFlag.sql"
:r $(PATH)"sprocs\FlagItemAsUpdated.sql"
:r $(PATH)"sprocs\GetItemUpdateCountByEnrollment.sql"
:r $(PATH)"sprocs\GetItemUpdates.sql"
:r $(PATH)"sprocs\AddEmailTracking.sql"
:r $(PATH)"sprocs\GetEmailTrackingInfo.sql"
:r $(PATH)"sprocs\CancelEmailTracking.sql"
:r $(PATH)"sprocs\GetAllNoteCounts.sql"
:r $(PATH)"sprocs\GetReminderEmailConfiguration.sql"
:r $(PATH)"sprocs\GetReminderEmails.sql"
:r $(PATH)"sprocs\UpdateReminderEmailStatus.sql"
:r $(PATH)"sprocs\GetEmailSentHistory.sql"
:r $(PATH)"sprocs\InsertProgram.sql"
:r $(PATH)"sprocs\InsertUserProgram.sql"
:r $(PATH)"sprocs\UpdateProgram.sql"
:r $(PATH)"sprocs\UpdateUserProgram.sql"
:r $(PATH)"sprocs\DeleteProgram.sql"
:r $(PATH)"sprocs\DeleteUserProgram.sql"
:r $(PATH)"sprocs\SelectProgram.sql"
:r $(PATH)"sprocs\GetUserProgram.sql"
:r $(PATH)"sprocs\GetUserProgramByDomain.sql"
:r $(PATH)"sprocs\InsertPersonalEportfolio.sql"
:r $(PATH)"sprocs\SelectPersonalEportfolio.sql"
:r $(PATH)"sprocs\GetProgramInstructors.sql"
:r $(PATH)"sprocs\AddPresentationAlias.sql"
:r $(PATH)"sprocs\SelectPresentationAlias.sql"
:r $(PATH)"sprocs\GetPresentationAlias.sql"
:r $(PATH)"sprocs\GetVideoNotes.sql"
:r $(PATH)"sprocs\VideoHasNotes.sql"
:r $(PATH)"sprocs\UpdateVideoNoteStatus.sql"
:r $(PATH)"sprocs\AddVideoNote.sql"
:r $(PATH)"sprocs\GetDashboardId2.sql"
:r $(PATH)"sprocs\InsertDashboardData2.sql"
:r $(PATH)"sprocs\UpdateVideoNotes.sql"
:r $(PATH)"sprocs\GetProgramDashboardId.sql"
:r $(PATH)"sprocs\InsertProgramDashboardData.sql"
:r $(PATH)"sprocs\InsertProgram2.sql"
:r $(PATH)"sprocs\GetProgram.sql"
:r $(PATH)"sprocs\spGetPxWebUserRights.sql"
:r $(PATH)"sprocs\spSaveAllPxWebUserRights.sql"
:r $(PATH)"sprocs\spSavePxWebUserRights.sql"
:r $(PATH)"sprocs\GetSharedCourseDefinition.sql"
:r $(PATH)"sprocs\GetSharedCourseDefinitionBySharingId.sql"
:r $(PATH)"sprocs\GetSharedCourseDefinitionItems.sql"
:r $(PATH)"sprocs\GetSharedCourseDefinitionUsers.sql"
:r $(PATH)"sprocs\GetSharedCourseAnonymousUsers.sql"
:r $(PATH)"sprocs\UpdateSharedCourseUser.sql"
:r $(PATH)"sprocs\UpdateSharedCourseItem.sql"
:r $(PATH)"sprocs\UpdateSharedCourse.sql"
:r $(PATH)"sprocs\AddSharedCourseItem.sql"
:r $(PATH)"sprocs\AddSharedCourseUser.sql"
:r $(PATH)"sprocs\AddSharedCourse.sql"
:r $(PATH)"sprocs\AddQuestionLog.sql"
:r $(PATH)"sprocs\AddQuestionNote.sql"
:r $(PATH)"sprocs\GetQuestionLogs.sql"
:r $(PATH)"sprocs\GetQuestionNotes.sql"
:r $(PATH)"sprocs\GetEportfolioDashboardByUserRefId.sql"


-- CREATE INDEXES
:r $(PATH)"indexes\HighlightNotes.sql"
:r $(PATH)"indexes\Highlights.sql"
:r $(PATH)"indexes\ItemHighlights.sql"
:r $(PATH)"indexes\Note.sql"
:r $(PATH)"indexes\UserNoteSettings.sql"
:r $(PATH)"indexes\UserTypes.sql"
:r $(PATH)"indexes\ItemUpdates.sql"

--INSERT VALUES
:r $(PATH)"insert\insert_email_template.sql"
:r $(PATH)"insert\insert_email_template_id_2.sql"
:r $(PATH)"insert\insert_email_template_id_3.sql"
--:r $(PATH)"insert\insert_email_service_configuration.sql"
--:r $(PATH)"insert\insert_programs_user_programs.sql"
:r $(PATH)"insert\insert_dashboard_types.sql"