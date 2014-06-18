using System.Collections.Generic;
using System.ComponentModel;

namespace Macmillan.PXQBA.Business.Models
{

    public static class CapabilityHelper
    {
        public static Dictionary<string, IEnumerable<Capability>> GetCapabilityGroups()
        {
            return new Dictionary<string, IEnumerable<Capability>>()
            {
                {"Roles", new List<Capability>{Capability.DefineRole, Capability.SetRoleToTitle}},
                {"Question Navigation", new List<Capability>{Capability.ViewQuestionList, Capability.PreviewQuestion, Capability.TestQuestion}},
                {"Question Editing", new List<Capability>{Capability.CreateQuestion, Capability.DuplicateQuestion}},
                {"Metadata", new List<Capability>{Capability.EditMetadataConfigValues, Capability.EditQuestionCardTemplate}},
                {"Question Status", new List<Capability>{Capability.ChangeStatusFromAvailableToDeleted, Capability.ChangeStatusFromAvailableToInProgress, Capability.ChangeStatusFromDeletedToAvailable, Capability.ChangeStatusFromDeletedToInProgress, Capability.ChangeStatusFromInProgressToAvailable, Capability.ChangeStatusFromInProgressToDeleted}},
                {"Question Content", new List<Capability>{Capability.EditAvailableQuestion, Capability.EditDeletedQuestion, Capability.EditInProgressQuestion}},
                {"Question Flags / Notes", new List<Capability>{Capability.FlagQuestion, Capability.AddNoteToQuestion, Capability.UnflagQuestion, Capability.RemoveNoteFromQuestion}},
                {"Question Sharing / Metadata Sharing", new List<Capability>{Capability.SubscribeToQuestionFromAnotherTitle, Capability.PublishQuestionToAnotherTitle, Capability.EditSharedQuestionContent, Capability.EditSharedQuestionMetadata, Capability.OverrideQuestionMetadata, Capability.RestoreLocalizedMetadataToSharedValue}},
                {"Metadata Config", new List<Capability>{Capability.EditTitleMetadataFull, Capability.EditTitleMetadataReduced}},
                {"Import", new List<Capability>{Capability.ImportQuestionFromTitle, Capability.ImportQuestionfromQML, Capability.ImportQuestionfromQTI, Capability.ImportQuestionfromRespondus}},
                {"Versioning", new List<Capability>{Capability.ViewVersionHistory, Capability.CreateDraftFromAvailableQuestion, Capability.TestSpecificVersion, Capability.PublishDraft, Capability.ChangeDraftStatus, Capability.RestoreOldVersion, Capability.CreateDraftFromOldVersion}},
                {"Archiving", new List<Capability>{Capability.ArchiveQuestionBank, Capability.DownloadQuestionBankArchive, Capability.DeleteExistingQuestionBank}},
            };
        }
    }

    /// <summary>
    /// IMPORTANT: The values for capabilities below are stored in PxData2 database and thus cannot be changed here
    /// </summary>
    public enum Capability
    {
        [Description("Can define new roles")]
        DefineRole = 1,
        [Description("Can set roles for specific titles")]
        SetRoleToTitle = 2,
        [Description("view/search/filter question lists")]
        ViewQuestionList = 3,
        [Description("preview questions")]
        PreviewQuestion = 4,
        [Description("test questions")]
        TestQuestion = 5,
        [Description("create question")]
        CreateQuestion = 6,
        [Description("duplicate question")]
        DuplicateQuestion = 7,
        [Description("Edit metadata values for a specific title")]
        EditMetadataConfigValues = 8,
        [Description("Edit title-specific question card template")]
        EditQuestionCardTemplate = 9,
        [Description("change question status from Available to In Progress")]
        ChangeStatusFromAvailableToInProgress = 10,
        [Description("change question status from Available to Deleted")]
        ChangeStatusFromAvailableToDeleted = 11,
        [Description("change question status from In Progress to Available")]
        ChangeStatusFromInProgressToAvailable = 12,
        [Description("change question status from In Progress to Deleted")]
        ChangeStatusFromInProgressToDeleted = 13,
        [Description("change question status from Deleted to Available")]
        ChangeStatusFromDeletedToAvailable = 14,
        [Description("change question status from Deleted to In Progress")]
        ChangeStatusFromDeletedToInProgress = 15,
        [Description("edit question that has status Available")]
        EditAvailableQuestion = 16,
        [Description("edit question that has status In Progress")]
        EditInProgressQuestion = 17,
        [Description("edit question that has status Deleted")]
        EditDeletedQuestion = 18,
        [Description("flag question")]
        FlagQuestion = 19,
        [Description("add notes")]
        AddNoteToQuestion = 20,
        [Description("unflag question")]
        UnflagQuestion = 21,
        [Description("remove notes")]
        RemoveNoteFromQuestion = 22,
        [Description("subscribe to a question from another title")]
        SubscribeToQuestionFromAnotherTitle = 23,
        [Description("publish a question to another title")]
        PublishQuestionToAnotherTitle = 24,
        [Description("edit shared question content")]
        EditSharedQuestionContent = 25,
        [Description("edit shared question metadata")]
        EditSharedQuestionMetadata = 26,
        [Description("restore localized question metadata to shared value by turning off override")]
        OverrideQuestionMetadata = 27,
        [Description("restore localized question metadata to shared value by turning off override")]
        RestoreLocalizedMetadataToSharedValue = 28,
        [Description("edit title metadata (with the exception of \"item link IDs\" and \"internal name\")")]
        EditTitleMetadataReduced = 29,
        [Description("edit title metadata (all)")]
        EditTitleMetadataFull = 30,
        [Description("Import one or more questions, including question metadata, from another LaunchPad title to a specific LaunchPad title")]
        ImportQuestionFromTitle = 31,
        [Description("Import one or more questions, including question metadata, from QTI files to a specific LaunchPad title")]
        ImportQuestionfromQTI = 32,
        [Description("Import one or more questions, including question metadata, from Respondus files to a specific LaunchPad title")]
        ImportQuestionfromRespondus = 33,
        [Description("Import one or more questions, including question metadata, from QML files to a specific LaunchPad title")]
        ImportQuestionfromQML = 34,
        [Description("View version history of a question or draft")]
        ViewVersionHistory = 35,
        [Description("Create a new draft in order to edit ‘available to instructors’ question")]
        CreateDraftFromAvailableQuestion = 36,
        [Description("Test a specific version of a question or draft")]
        TestSpecificVersion = 37,
        [Description("Publish a draft")]
        PublishDraft = 38,
        [Description("Change draft status")]
        ChangeDraftStatus = 39,
        [Description("Restore an old version of a question")]
        RestoreOldVersion = 40,
        [Description("Create a new draft from an old version of a question")]
        CreateDraftFromOldVersion = 41,
        [Description("Can archive a question bank")]
        ArchiveQuestionBank = 42,
        [Description("Can download an archive of a title's question bank repository")]
        DownloadQuestionBankArchive = 43,
        [Description("Can delete an existing question bank, moving all questions inside it to another question bank")]
        DeleteExistingQuestionBank =44
    }
}