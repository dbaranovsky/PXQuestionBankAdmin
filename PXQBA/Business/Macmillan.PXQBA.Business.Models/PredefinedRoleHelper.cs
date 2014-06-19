using System.Collections.Generic;
using System.ComponentModel;
using Macmillan.PXQBA.Common.Helpers;

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
                    Name = EnumHelper.GetEnumDescription(PredefinedRole.SuperAdministrator),
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
                    CanEdit = false
                },
                new Role()
                {
                    Name = EnumHelper.GetEnumDescription(PredefinedRole.Administrator),
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
                    CanEdit = false
                },
                new Role()
                {
                    Name = EnumHelper.GetEnumDescription(PredefinedRole.MediaProducer),
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
                    CanEdit = false
                },
                new Role()
                {
                    Name = EnumHelper.GetEnumDescription(PredefinedRole.MediaEditor),
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
                    CanEdit = false
                },
                new Role()
                {
                    Name = EnumHelper.GetEnumDescription(PredefinedRole.ProductionDeveloper),
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
                    CanEdit = false
                },
                new Role()
                {
                    Name = EnumHelper.GetEnumDescription(PredefinedRole.Author),
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
                    CanEdit = false
                },
                new Role()
                {
                    Name = EnumHelper.GetEnumDescription(PredefinedRole.Reviewer),
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
                    CanEdit = false
                }
            };
        }
    }

    public enum PredefinedRole
    {
        [Description("Super Administrator")]
        SuperAdministrator = 1,
        [Description("Administrator")]
        Administrator = 2,
        [Description("Media Producer")]
        MediaProducer = 3,
        [Description("Media Editor")]
        MediaEditor = 4,
        [Description("Production Developer")]
        ProductionDeveloper = 5,
        [Description("Author")]
        Author = 6,
        [Description("Reviewer")]
        Reviewer = 7
    }
}