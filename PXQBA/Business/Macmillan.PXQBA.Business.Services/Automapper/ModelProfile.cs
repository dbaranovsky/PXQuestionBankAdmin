using System;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Course = Macmillan.PXQBA.Business.Models.Course;
using LearningObjective = Macmillan.PXQBA.Business.Models.LearningObjective;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Services.Automapper
{
    public class ModelProfile : Profile
    {
        private readonly IModelProfileService modelProfileService;

        public ModelProfile(IModelProfileService modelProfileService)
        {
            this.modelProfileService = modelProfileService;
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Bfw.Agilix.DataContracts.Course, Course>()
                .ForMember(dest => dest.ProductCourseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => src.LearningObjectives))
                .ForMember(dest => dest.QuestionCardLayout,
                    opt => opt.MapFrom(src => modelProfileService.GetQuestionCardLayout(src)))
                .ForMember(dest => dest.Chapters,
                    opt => opt.MapFrom(src => modelProfileService.GetHardCodedQuestionChapters(src)))
                .ForMember(dest => dest.Banks,
                    opt => opt.MapFrom(src => modelProfileService.GetHardCodedQuestionBanks(src)))
                .ForMember(dest => dest.FieldDescriptors,
                    opt => opt.MapFrom(src => modelProfileService.GetCourseMetadataFieldDescriptors(src)));


            Mapper.CreateMap<CourseMetadataFieldDescriptor, QuestionMetaField>()
                .ForMember(dest => dest.FriendlyName, opt => opt.MapFrom(src => src.Friendlyname))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TypeDescriptor,
                    opt => opt.MapFrom(src => Mapper.Map<MetaFieldTypeDescriptor>(src)));

            Mapper.CreateMap<CourseMetadataFieldDescriptor, MetaFieldTypeDescriptor>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.AvailableChoice,
                    opt => opt.MapFrom(src => src.CourseMetadataFieldValues.Select(i => i.Text).ToDictionary(t=>t)));
 

            Mapper.CreateMap<Bfw.Agilix.DataContracts.LearningObjective, LearningObjective>()
                .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Title));


            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, Question>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
                .ForMember(dto => dto.EntityId, opt => opt.MapFrom(q => q.EntityId))
                .ForMember(dto => dto.LocalMetadata, opt => opt.MapFrom(q => q))
                .ForMember(dto => dto.Type, opt => opt.Ignore())
                .ForMember(dto => dto.Preview, opt => opt.MapFrom( q => QuestionPreviewHelper.GetQuestionHtmlPreview(q)))
                .ForMember(dto => dto.QuizId, opt => opt.MapFrom(q => modelProfileService.GetQuizIdForQuestion(q.Id, q.EntityId)));

            Mapper.CreateMap<Bfw.Agilix.DataContracts.Question, QuestionStaticMetadata>()
                .ForMember(dto => dto.Title, opt => opt.MapFrom(q => q.Title))
                .ForMember(dto => dto.Chapter, opt => opt.MapFrom(q => q.eBookChapter))
                .ForMember(dto => dto.Bank, opt => opt.MapFrom(q => q.QuestionBank))
                .ForMember(dto => dto.Sequence, opt => opt.Ignore());

            Mapper.CreateMap<Question, Bfw.Agilix.DataContracts.Question>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(q => q.Id))
                //.ForMember(dto => dto.EntityId, opt => opt.MapFrom(q => q.EntityId))
                .ForMember(dto => dto.Title, opt => opt.MapFrom(q => q.LocalMetadata.Title))
                .ForMember(dto => dto.eBookChapter, opt => opt.MapFrom(q => q.LocalMetadata.Chapter))
                .ForMember(dto => dto.QuestionBank, opt => opt.MapFrom(q => q.LocalMetadata.Bank));

            Mapper.CreateMap<AgilixUser, UserInfo>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.LastName))
                .ForMember(d => d.Username, opt => opt.MapFrom(s => s.Credentials != null ? s.Credentials.Username : s.UserName))
                .ForMember(d => d.Password, opt => opt.MapFrom(s => s.Credentials != null ? s.Credentials.Password : ""))
                .ForMember(d => d.ReferenceId, opt => opt.MapFrom(s => s.Reference))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.DomainId, opt => opt.MapFrom(s => s.Domain != null ? s.Domain.Id : ""))
                .ForMember(d => d.DomainName, opt => opt.MapFrom(s => s.Domain != null ? s.Domain.Name : ""))
                .ForMember(d => d.LastLogin, opt => opt.MapFrom(s => s.LastLogin));

            Mapper.CreateMap<string, InteractionType>().ConvertUsing(modelProfileService.CreateInteractionType);

            Mapper.CreateMap<Course, TitleViewModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(c => c.ProductCourseId))
                .ForMember(vm => vm.Title, opt => opt.MapFrom(c => c.Title))
                .ForMember(vm => vm.Chapters, opt => opt.MapFrom(c => c.GetChaptersList()));

            Mapper.CreateMap<CourseMetadataFieldValue, ChapterViewModel>()
                .ForMember(vm => vm.Id, opt => opt.MapFrom(c => FirstCharacterToLower(c.Text)))
                .ForMember(vm => vm.Title, opt => opt.MapFrom(c => c.Text));

            Mapper.CreateMap<Question, QuestionViewModel>()
                .ForMember(dest => dest.ProductCourses, opt => opt.MapFrom(src => src.ProductCourses.Select(p => p.Title)));

            Mapper.CreateMap<QuestionViewModel, Question>();

            #region UI models to dummy models

            Mapper.CreateMap<DataAccess.Data.ProductCourse, Question>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Question.DlapId))
                .ForMember(dest => dest.LocalMetadata, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Question.Type))
                .ForMember(dest => dest.Preview, opt => opt.MapFrom(src => src.Question.Preview))
                .ForMember(dest => dest.ProductCourses, opt => opt.MapFrom(src => modelProfileService.GetHardCodedSharedProductCourses(src)))
                .ForMember(dest => dest.SharedMetadata, opt => opt.MapFrom(src => 
                                src.QuestionId % 2 != 0
                                    ? src
                                    : null))
                .ForMember(dest => dest.QuestionIdDuplicateFrom,
                    opt =>
                        opt.MapFrom(
                            src =>
                                src.QuestionId%2 != 0
                                    ? modelProfileService.GetHardCodedQuestionDuplicate()
                                    : String.Empty));


            Mapper.CreateMap<DataAccess.Data.ProductCourse, QuestionStaticMetadata>()
                .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.Keywords.Split('|')))
                .ForMember(dest => dest.SuggestedUse, opt => opt.MapFrom(src => src.SuggestedUse.Split('|')))
                .ForMember(dest => dest.LearningObjectives,
                    opt =>
                        opt.MapFrom(
                            src => modelProfileService.GetLOByGuid(src.ProductCourseDlapId, src.LearningObjectives)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Question.Status));

            Mapper.CreateMap<Question, DataAccess.Data.ProductCourse>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.Keywords, opt => opt.MapFrom(src => src.LocalMetadata.Keywords != null ? string.Join("|", src.LocalMetadata.Keywords) : null))
                  .ForMember(dest => dest.SuggestedUse, opt => opt.MapFrom(src => src.LocalMetadata.SuggestedUse != null ? string.Join("|", src.LocalMetadata.SuggestedUse) : null))
                  .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => modelProfileService.SetLearningObjectives(src.LocalMetadata.LearningObjectives)))
                  .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.LocalMetadata.Title))
                  .ForMember(dest => dest.ExcerciseNo, opt => opt.MapFrom(src => src.LocalMetadata.ExcerciseNo))
                  .ForMember(dest => dest.Chapter, opt => opt.MapFrom(src => src.LocalMetadata.Chapter))
                  .ForMember(dest => dest.Bank, opt => opt.MapFrom(src => src.LocalMetadata.Bank))
                  .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.LocalMetadata.Difficulty))
                  .ForMember(dest => dest.CognitiveLevel, opt => opt.MapFrom(src => src.LocalMetadata.CognitiveLevel))
                  .ForMember(dest => dest.Guidance, opt => opt.MapFrom(src => src.LocalMetadata.Guidance));
          

            Mapper.CreateMap<Question, DataAccess.Data.Question>()
                .ForMember(dest => dest.DlapId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                //Should proper mapping be here?
                .ForMember(dest => dest.ProductCourses, opt => opt.Ignore());

            Mapper.CreateMap<Question, QuestionMetadata>()
             .ForMember(dest => dest.Data, opt => opt.MapFrom(src => modelProfileService.CreateQuestionMetadata(src)));

            Mapper.CreateMap<Note, DataAccess.Data.Note>()
                  .ForMember(dest => dest.QuestionId, opt => opt.Ignore());
            Mapper.CreateMap<DataAccess.Data.Note, Note>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.Question.DlapId));



            #endregion
        }

        public static string FirstCharacterToLower(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            var stringBuilder = new StringBuilder(input);
            stringBuilder[0] = Char.ToLower(input[0]);
            return stringBuilder.ToString();
        }
    }
}
