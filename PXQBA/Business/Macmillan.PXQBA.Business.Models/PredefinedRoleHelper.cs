using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public static class PredefinedRoleHelper
    {
        public static IEnumerable<Role> GetPredefinedRoles()
        {
            return new List<Role>()
            {
                new Role()
                {
                    Id = (int) PredefinedRole.SuperAdministrator,
                    Capabilities = new List<Capability>()
                    {
                        Capability.DefineRole,
                        Capability.SetRoleToTitle,
                        Capability.ViewQuestionList,
                        Capability.PreviewQuestion,
                        Capability.TestQuestion,
                        Capability.CreateQuestion,
                        Capability.DuplicateQuestion,
                        Capability.EditMetadataConfigValues,
                        Capability.EditQuestionCardTemplate,
                        Capability.ChangeStatusFromAvailableToInProgress,
                        Capability.ChangeStatusFromAvailableToDeleted,
                        Capability.ChangeStatusFromInProgressToAvailable,
                        Capability.ChangeStatusFromInProgressToDeleted,
                        Capability.ChangeStatusFromDeletedToAvailable,
                        Capability.ChangeStatusFromDeletedToInProgress,
                        Capability.EditAvailableQuestion,
                        Capability.EditInProgressQuestion,
                        Capability.EditDeletedQuestion,
                        Capability.FlagQuestion,
                        Capability.AddNoteToQuestion,
                        Capability.UnflagQuestion,
                        Capability.RemoveNoteFromQuestion,
                        Capability.SubscribeToQuestionFromAnotherTitle,
                        Capability.PublishQuestionToAnotherTitle,
                        Capability.EditSharedQuestionContent,
                        Capability.EditSharedQuestionMetadata,
                        Capability.OverrideQuestionMetadata,
                        Capability.RestoreLocalizedMetadataToSharedValue,
                        Capability.EditTitleMetadataReduced,
                        Capability.EditTitleMetadataFull,
                        Capability.ImportQuestionFromTitle,
                        Capability.ImportQuestionfromQTI,
                        Capability.ImportQuestionfromRespondus,
                        Capability.ImportQuestionfromQML,
                        Capability.ViewVersionHistory,
                        Capability.CreateDraftFromAvailableQuestion,
                        Capability.TestSpecificVersion,
                        Capability.PublishDraft,
                        Capability.ChangeDraftStatus,
                        Capability.RestoreOldVersion,
                        Capability.CreateDraftFromOldVersion,
                        Capability.ArchiveQuestionBank,
                        Capability.DownloadQuestionBankArchive,
                        Capability.DeleteExistingQuestionBank
                    },
                    CanDelete = false
                },
                new Role()
                {
                    Id = (int) PredefinedRole.Administrator,
                    Capabilities = new List<Capability>()
                    {
                        Capability.SetRoleToTitle,
                        Capability.ViewQuestionList,
                        Capability.PreviewQuestion,
                        Capability.TestQuestion,
                        Capability.CreateQuestion,
                        Capability.DuplicateQuestion,
                        Capability.EditMetadataConfigValues,
                        Capability.EditQuestionCardTemplate,
                        Capability.ChangeStatusFromAvailableToInProgress,
                        Capability.ChangeStatusFromAvailableToDeleted,
                        Capability.ChangeStatusFromInProgressToAvailable,
                        Capability.ChangeStatusFromInProgressToDeleted,
                        Capability.ChangeStatusFromDeletedToAvailable,
                        Capability.ChangeStatusFromDeletedToInProgress,
                        Capability.EditAvailableQuestion,
                        Capability.EditInProgressQuestion,
                        Capability.EditDeletedQuestion,
                        Capability.FlagQuestion,
                        Capability.AddNoteToQuestion,
                        Capability.UnflagQuestion,
                        Capability.RemoveNoteFromQuestion,
                        Capability.SubscribeToQuestionFromAnotherTitle,
                        Capability.PublishQuestionToAnotherTitle,
                        Capability.EditSharedQuestionContent,
                        Capability.EditSharedQuestionMetadata,
                        Capability.OverrideQuestionMetadata,
                        Capability.RestoreLocalizedMetadataToSharedValue,
                        Capability.EditTitleMetadataReduced,
                        Capability.EditTitleMetadataFull,
                        Capability.ImportQuestionFromTitle,
                        Capability.ImportQuestionfromQTI,
                        Capability.ImportQuestionfromRespondus,
                        Capability.ImportQuestionfromQML,
                        Capability.ViewVersionHistory,
                        Capability.CreateDraftFromAvailableQuestion,
                        Capability.TestSpecificVersion,
                        Capability.PublishDraft,
                        Capability.ChangeDraftStatus,
                        Capability.RestoreOldVersion,
                        Capability.CreateDraftFromOldVersion
                    },
                    CanDelete = false
                },
                new Role()
                {
                    Id = (int) PredefinedRole.MediaProducer,
                    Capabilities = new List<Capability>()
                    {
                        Capability.SetRoleToTitle,
                        Capability.ViewQuestionList,
                        Capability.PreviewQuestion,
                        Capability.TestQuestion,
                        Capability.CreateQuestion,
                        Capability.DuplicateQuestion,
                        Capability.EditMetadataConfigValues,
                        Capability.EditQuestionCardTemplate,
                        Capability.ChangeStatusFromAvailableToInProgress,
                        Capability.ChangeStatusFromAvailableToDeleted,
                        Capability.ChangeStatusFromInProgressToAvailable,
                        Capability.ChangeStatusFromInProgressToDeleted,
                        Capability.ChangeStatusFromDeletedToAvailable,
                        Capability.ChangeStatusFromDeletedToInProgress,
                        Capability.EditAvailableQuestion,
                        Capability.EditInProgressQuestion,
                        Capability.EditDeletedQuestion,
                        Capability.FlagQuestion,
                        Capability.AddNoteToQuestion,
                        Capability.UnflagQuestion,
                        Capability.RemoveNoteFromQuestion,
                        Capability.SubscribeToQuestionFromAnotherTitle,
                        Capability.PublishQuestionToAnotherTitle,
                        Capability.EditSharedQuestionContent,
                        Capability.EditSharedQuestionMetadata,
                        Capability.OverrideQuestionMetadata,
                        Capability.RestoreLocalizedMetadataToSharedValue,
                        Capability.EditTitleMetadataReduced,
                        Capability.EditTitleMetadataFull,
                        Capability.ImportQuestionFromTitle,
                        Capability.ImportQuestionfromQTI,
                        Capability.ImportQuestionfromRespondus,
                        Capability.ImportQuestionfromQML,
                        Capability.ViewVersionHistory,
                        Capability.CreateDraftFromAvailableQuestion,
                        Capability.TestSpecificVersion,
                        Capability.PublishDraft,
                        Capability.ChangeDraftStatus,
                        Capability.RestoreOldVersion,
                        Capability.CreateDraftFromOldVersion
                    },
                    CanDelete = false
                },
                new Role()
                {
                    Id = (int) PredefinedRole.MediaEditor,
                    Capabilities = new List<Capability>()
                    {
                        Capability.SetRoleToTitle,
                        Capability.ViewQuestionList,
                        Capability.PreviewQuestion,
                        Capability.TestQuestion,
                        Capability.CreateQuestion,
                        Capability.DuplicateQuestion,
                        Capability.EditMetadataConfigValues,
                        Capability.EditQuestionCardTemplate,
                        Capability.ChangeStatusFromAvailableToInProgress,
                        Capability.ChangeStatusFromAvailableToDeleted,
                        Capability.ChangeStatusFromInProgressToAvailable,
                        Capability.ChangeStatusFromInProgressToDeleted,
                        Capability.ChangeStatusFromDeletedToAvailable,
                        Capability.ChangeStatusFromDeletedToInProgress,
                        Capability.EditAvailableQuestion,
                        Capability.EditInProgressQuestion,
                        Capability.EditDeletedQuestion,
                        Capability.FlagQuestion,
                        Capability.AddNoteToQuestion,
                        Capability.UnflagQuestion,
                        Capability.RemoveNoteFromQuestion,
                        Capability.SubscribeToQuestionFromAnotherTitle,
                        Capability.PublishQuestionToAnotherTitle,
                        Capability.EditSharedQuestionContent,
                        Capability.EditSharedQuestionMetadata,
                        Capability.OverrideQuestionMetadata,
                        Capability.RestoreLocalizedMetadataToSharedValue,
                        Capability.EditTitleMetadataReduced,
                        Capability.EditTitleMetadataFull,
                        Capability.ImportQuestionFromTitle,
                        Capability.ImportQuestionfromQTI,
                        Capability.ImportQuestionfromRespondus,
                        Capability.ImportQuestionfromQML,
                        Capability.ViewVersionHistory,
                        Capability.CreateDraftFromAvailableQuestion,
                        Capability.TestSpecificVersion,
                        Capability.PublishDraft,
                        Capability.ChangeDraftStatus,
                        Capability.RestoreOldVersion,
                        Capability.CreateDraftFromOldVersion
                    },
                    CanDelete = false
                },
                new Role()
                {
                    Id = (int) PredefinedRole.ProductionDeveloper,
                    Capabilities = new List<Capability>()
                    {
                        Capability.ViewQuestionList,
                        Capability.PreviewQuestion,
                        Capability.TestQuestion,
                        Capability.CreateQuestion,
                        Capability.DuplicateQuestion,
                        Capability.EditMetadataConfigValues,
                        Capability.EditQuestionCardTemplate,
                        Capability.ChangeStatusFromAvailableToInProgress,
                        Capability.ChangeStatusFromAvailableToDeleted,
                        Capability.ChangeStatusFromInProgressToAvailable,
                        Capability.ChangeStatusFromInProgressToDeleted,
                        Capability.ChangeStatusFromDeletedToAvailable,
                        Capability.ChangeStatusFromDeletedToInProgress,
                        Capability.EditAvailableQuestion,
                        Capability.EditInProgressQuestion,
                        Capability.EditDeletedQuestion,
                        Capability.FlagQuestion,
                        Capability.AddNoteToQuestion,
                        Capability.UnflagQuestion,
                        Capability.RemoveNoteFromQuestion,
                        Capability.SubscribeToQuestionFromAnotherTitle,
                        Capability.PublishQuestionToAnotherTitle,
                        Capability.EditSharedQuestionContent,
                        Capability.EditSharedQuestionMetadata,
                        Capability.OverrideQuestionMetadata,
                        Capability.RestoreLocalizedMetadataToSharedValue,
                        Capability.EditTitleMetadataReduced,
                        Capability.EditTitleMetadataFull,
                        Capability.ImportQuestionFromTitle,
                        Capability.ImportQuestionfromQTI,
                        Capability.ImportQuestionfromRespondus,
                        Capability.ImportQuestionfromQML,
                        Capability.ViewVersionHistory,
                        Capability.CreateDraftFromAvailableQuestion,
                        Capability.TestSpecificVersion,
                        Capability.PublishDraft,
                        Capability.ChangeDraftStatus,
                        Capability.RestoreOldVersion,
                        Capability.CreateDraftFromOldVersion
                    },
                    CanDelete = false
                },
                new Role()
                {
                    Id = (int) PredefinedRole.Author,
                    Capabilities = new List<Capability>()
                    {
                        Capability.ViewQuestionList,
                        Capability.PreviewQuestion,
                        Capability.TestQuestion,
                        Capability.CreateQuestion,
                        Capability.DuplicateQuestion,
                        Capability.EditMetadataConfigValues,
                        Capability.ChangeStatusFromAvailableToInProgress,
                        Capability.ChangeStatusFromAvailableToDeleted,
                        Capability.ChangeStatusFromInProgressToAvailable,
                        Capability.ChangeStatusFromInProgressToDeleted,
                        Capability.ChangeStatusFromDeletedToAvailable,
                        Capability.ChangeStatusFromDeletedToInProgress,
                        Capability.EditAvailableQuestion,
                        Capability.EditInProgressQuestion,
                        Capability.EditDeletedQuestion,
                        Capability.FlagQuestion,
                        Capability.AddNoteToQuestion,
                        Capability.UnflagQuestion,
                        Capability.RemoveNoteFromQuestion,
                        Capability.EditSharedQuestionContent,
                        Capability.EditSharedQuestionMetadata,
                        Capability.OverrideQuestionMetadata,
                        Capability.RestoreLocalizedMetadataToSharedValue,
                        Capability.EditTitleMetadataReduced,
                        Capability.ViewVersionHistory,
                        Capability.CreateDraftFromAvailableQuestion,
                        Capability.TestSpecificVersion,
                        Capability.PublishDraft,
                        Capability.ChangeDraftStatus,
                        Capability.CreateDraftFromOldVersion
                    },
                    CanDelete = false
                },
                new Role()
                {
                    Id = (int) PredefinedRole.Reviewer,
                    Capabilities = new List<Capability>()
                    {
                        Capability.ViewQuestionList,
                        Capability.PreviewQuestion,
                        Capability.TestQuestion,
                        Capability.FlagQuestion,
                        Capability.AddNoteToQuestion,
                        Capability.ViewVersionHistory,
                        Capability.CreateDraftFromOldVersion
                    },
                    CanDelete = false
                }
            };
        }
    }

    /// <summary>
    /// IMPORTANT: Int values are stored in QBARole table and cannot be changed here
    /// </summary>
    public enum PredefinedRole
    {
        SuperAdministrator = 1,
        Administrator = 2,
        MediaProducer = 3,
        MediaEditor = 4,
        ProductionDeveloper = 5,
        Author = 6,
        Reviewer = 7
    }
}