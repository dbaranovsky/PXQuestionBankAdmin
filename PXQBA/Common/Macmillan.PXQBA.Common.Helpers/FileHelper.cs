using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;

namespace Macmillan.PXQBA.Common.Helpers
{
    /// <summary>
    /// Helper to work with files
    /// </summary>
   public static class FileHelper
    {
       private const string xmlFileFormat = ".xml";

       /// <summary>
       /// Saves XML element to files
       /// </summary>
       /// <param name="paths">Paths with file enames, where XML file should be saved</param>
       /// <param name="xmlFile">Xml file content</param>
       public static void SaveXmlFileToPaths(IEnumerable<string> paths, XElement xmlFile)
       {
           if (!paths.Any() || xmlFile == null)
           {
               return;
           }

           foreach (var path in paths.Where(path => !string.IsNullOrEmpty(path)))
           {
               Directory.CreateDirectory(Path.GetDirectoryName(path));
               xmlFile.Save(path);
           }
       }

       /// <summary>
       /// Saves XML elements to files
       /// </summary>
       /// <param name="paths">Paths with file enames, where XML files should be saved</param>
       /// <param name="xmlFiles">Collection of Xml files content</param>
        public static void SaveXmlFilesToPaths(IEnumerable<string> paths, IEnumerable<XElement> xmlFiles)
       {
           if (!xmlFiles.Any())
           {
               return;
           }

           foreach (var xmlFile in xmlFiles)
           {
               SaveXmlFileToPaths(paths, xmlFile);
           }
           
       }

       /// <summary>
       /// Saves Agilix Questions to selected paths
       /// </summary>
       /// <param name="questions">Collection of questions to save</param>
       /// <param name="repositories">List of repositories paths where Questions XML should be stored</param>
        public static void SaveQuestionsXmlsToPath(IEnumerable<Question> questions, IEnumerable<string> repositories)
        {
            foreach (var question in questions)
            {
                SaveQuestionXmlsToPath(question, repositories);
            }
        }

       /// <summary>
       ///  Saves single Agilix Question to selected paths
       /// </summary>
       /// <param name="question">Question to save</param>
       /// <param name="repositories">List of repositories paths where Questions XML should be stored</param>
        public static void SaveQuestionXmlsToPath(Question question, IEnumerable<string> repositories)
        {
            var questionPaths = repositories.Select(repository => Path.Combine(repository, question.EntityId, question.Id + xmlFileFormat));
            SaveXmlFileToPaths(questionPaths, question.ToEntity());
        }

       /// <summary>
       /// Saves single Agilix Course to selected paths
       /// </summary>
       /// <param name="agilixCourse">Question to save</param>
       /// <param name="repositories">List of repositories paths where Course XML should be stored</param>
       public static void SaveCourseXmlToPaths(Course agilixCourse, IEnumerable<string> repositories)
        {
            if (!repositories.Any())
            {
                return;
            }

            var coursePaths = repositories.Select(repository => Path.Combine(repository, agilixCourse.Id + xmlFileFormat));
            SaveXmlFileToPaths(coursePaths, agilixCourse.ToEntity());
        }
    }
}
