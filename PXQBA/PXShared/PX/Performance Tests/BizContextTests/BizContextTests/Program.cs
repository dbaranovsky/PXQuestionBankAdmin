using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;

namespace BizContextTests
{
    class Program
    {
        protected static ISession Session { get; set; }

        /// <summary>
        /// All test must conform to this delegate. It represents one
        /// test run, which will be timed externally.
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="productId"></param>
        /// <param name="referenceId"></param>
        /// <param name="domainId"></param>
        delegate TestInformation TestRun(string courseId, string productId, string referenceId, string domainId);

        static void Main(string[] args)
        {
            Configure();

            int runs = 50;
            int t = 1;
            string courseId = "21519";
            string productId = "11169";
            string referenceId = "9";
            string domainId = "8";

            var tests = new List<TestRun>
            {
                TimeNonBatched,
                TimeBatched,
                RefactorOne,
                RefactorTwo
            };

            Console.WriteLine("... Will run {0} tests, {1} runs each ...", tests.Count, runs);
            foreach (var test in tests)
            {
                Console.WriteLine("... Running Test {0} of {1} ...", t, tests.Count);
                RunTest(test, runs, courseId, productId, referenceId, domainId);
                Console.WriteLine("\n\n");
                ++t;
            }
            
            Console.ReadKey();
        }

        /// <summary>
        /// Configures the session to communicate with DLAP.
        /// </summary>
        static void Configure()
        {
            var sm = new ThreadSessionManager(new Bfw.Common.Logging.NullLogger(), null);
            Session = sm.StartAnnonymousSession();
        }

        /// <summary>
        /// Runs the given test and outputs the anlysis results.
        /// </summary>
        /// <param name="test">test to run.</param>
        /// <param name="runs">number of runs to measure.</param>
        /// <param name="courseId">id of the course.</param>
        /// <param name="productId">id of the product.</param>
        /// <param name="referenceId">external id of the user.</param>
        /// <param name="domainId">id of the domain</param>
        static void RunTest(TestRun test, int runs, string courseId, string productId, string referenceId, string domainId)
        {
            var timings = new List<long>();
            var watch = new Stopwatch();
            var info = test(courseId, productId, referenceId, domainId);

            for (int i = 0; i < runs; ++i)
            {
                watch.Restart();

                info.Run();

                watch.Stop();
                timings.Add(watch.Elapsed.Ticks);
            }

            var min = timings.Min();
            var max = timings.Max();
            var med = timings.Median();
            var avg = (long)timings.Average();

            Console.WriteLine("{0}:\n\"{1}\"\n\tmin -> {2:hh\\:mm\\:ss\\:ffff}\n\tmax -> {3:hh\\:mm\\:ss\\:ffff}\n\tmed -> {4:hh\\:mm\\:ss\\:ffff}\n\tavg -> {5:hh\\:mm\\:ss\\:ffff}", info.Name, info.Description, new TimeSpan(min), new TimeSpan(max), new TimeSpan(med), new TimeSpan(avg));
        }

        /// <summary>
        /// Loads the standard BusinessContext data a given number of times, keeping track of
        /// how long each run took.
        /// </summary>
        /// <param name="runs">Number of times to run the test.</param>
        /// <param name="courseId">Id of the course.</param>
        /// <param name="productId">Id of the product course.</param>
        /// <param name="referenceId">Reference id of the user.</param>
        /// <returns>Timing information for each run.</returns>
        static TestInformation TimeNonBatched(string courseId, string productId, string referenceId, string domainId)
        {
            var info = new TestInformation
            {
                Name = "Non-Batched",
                Description = "Loads Course, Product, Domain, User, and Enrollments without using a batch"
            };

            var getProductCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };
            var getCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };
            var getDomain = new GetDomain();
            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch()
            };
            var getEnrollments = new GetEntityEnrollmentList
            {
                SearchParameters = new EntitySearch()
            };

            info.Run = () =>
            {
                Session.ExecuteAsAdmin(getProductCourse);
                Session.ExecuteAsAdmin(getCourse);

                getDomain.DomainId = getCourse.Courses.First().Domain.Id;
                Session.ExecuteAsAdmin(getDomain);

                getUser.SearchParameters.DomainId = getDomain.Domain.Id;
                getUser.SearchParameters.ExternalId = referenceId;
                Session.ExecuteAsAdmin(getUser);

                getEnrollments.SearchParameters.CourseId = getCourse.Courses.First().Id;
                getEnrollments.SearchParameters.UserId = getUser.Users.First().Id;
                Session.ExecuteAsAdmin(getEnrollments);
            };

            return info;
        }

        /// <summary>
        /// Loads the standard BusinessContext data in a batch a given number of times, keeping track of
        /// how long each run took. This assumes we are given the domain id.
        /// </summary>
        /// <param name="runs">Number of times to run the test.</param>
        /// <param name="courseId">Id of the course.</param>
        /// <param name="productId">Id of the product course.</param>
        /// <param name="referenceId">Reference id of the user.</param>
        /// <returns>Timing information for each run.</returns>
        static TestInformation TimeBatched(string courseId, string productId, string referenceId, string domainId)
        {
            var info = new TestInformation
            {
                Name = "Batched",
                Description = "Loads Course, Product, Domain, User, and Enrollments using a batch"
            };

            var getProductCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };
            var getCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };
            var getDomain = new GetDomain
            {
                DomainId = domainId
            };

            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    DomainId = domainId,
                    ExternalId = referenceId
                }
            };
            var getEnrollments = new GetEntityEnrollmentList
            {
                SearchParameters = new EntitySearch
                {
                    CourseId = courseId,
                    UserId = string.Format("{0}/{1}", domainId, referenceId)
                }
            };

            var batch = new Batch();
            batch.Add(getCourse);
            batch.Add(getProductCourse);
            batch.Add(getDomain);
            batch.Add(getUser);
            batch.Add(getEnrollments);
            
            info.Run = () => Session.ExecuteAsAdmin(batch);

            return info;
        }

        /// <summary>
        /// This tests what the performance impact of eliminating the Product course load is.
        /// </summary>
        /// <param name="courseId">id of the course.</param>
        /// <param name="productId">id of the product.</param>
        /// <param name="referenceId">external id of the user.</param>
        /// <param name="domainId">id of the domain.</param>
        /// <returns>information about the test</returns>
        static TestInformation RefactorOne(string courseId, string productId, string referenceId, string domainId)
        {
            var info = new TestInformation
            {
                Name = "RefactorOne",
                Description = "Simulates timing for a non-batched run that does not load the product"
            };

            var getCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };
            var getDomain = new GetDomain();
            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch()
            };
            var getEnrollments = new GetEntityEnrollmentList
            {
                SearchParameters = new EntitySearch()
            };            

            info.Run = () =>
            {
                Session.ExecuteAsAdmin(getCourse);

                getDomain.DomainId = getCourse.Courses.First().Domain.Id;
                Session.ExecuteAsAdmin(getDomain);

                getUser.SearchParameters.DomainId = getDomain.Domain.Id;
                getUser.SearchParameters.ExternalId = referenceId;
                Session.ExecuteAsAdmin(getUser);

                getEnrollments.SearchParameters.CourseId = getCourse.Courses.First().Id;
                getEnrollments.SearchParameters.UserId = getUser.Users.First().Id;
                Session.ExecuteAsAdmin(getEnrollments);
            };

            return info;
        }

        /// <summary>
        /// This tests what the performance impact of eliminating the Product course load is.
        /// </summary>
        /// <param name="courseId">id of the course.</param>
        /// <param name="productId">id of the product.</param>
        /// <param name="referenceId">external id of the user.</param>
        /// <param name="domainId">id of the domain.</param>
        /// <returns>information about the test</returns>
        static TestInformation RefactorTwo(string courseId, string productId, string referenceId, string domainId)
        {
            var info = new TestInformation
            {
                Name = "RefactorTwo",
                Description = "Simulates not loading the product course in a batch"
            };

            var getCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };
            var getDomain = new GetDomain
            {
                DomainId = domainId
            };

            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    DomainId = domainId,
                    ExternalId = referenceId
                }
            };
            var getEnrollments = new GetEntityEnrollmentList
            {
                SearchParameters = new EntitySearch
                {
                    CourseId = courseId,
                    UserId = string.Format("{0}/{1}", domainId, referenceId)
                }
            };

            var batch = new Batch();
            batch.Add(getCourse);
            batch.Add(getDomain);
            batch.Add(getUser);
            batch.Add(getEnrollments);

            info.Run = () => Session.ExecuteAsAdmin(batch);

            return info;
        }

        static TestInformation RefactorThree(string courseId, string productId, string referenceId, string domainId)
        {
            var info = new TestInformation
            {
                Name = "RefactorThree",
                Description = "Simulates timing for a non-batched run that does not load the product or domain"
            };

            var getCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };

            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch()
            };
            var getEnrollments = new GetEntityEnrollmentList
            {
                SearchParameters = new EntitySearch()
            };            

            info.Run = () =>
            {
                Session.ExecuteAsAdmin(getCourse);

                getUser.SearchParameters.DomainId = domainId;
                getUser.SearchParameters.ExternalId = referenceId;
                Session.ExecuteAsAdmin(getUser);

                getEnrollments.SearchParameters.CourseId = getCourse.Courses.First().Id;
                getEnrollments.SearchParameters.UserId = getUser.Users.First().Id;
                Session.ExecuteAsAdmin(getEnrollments);
            };

            return info;
        }

        static TestInformation RefactorFour(string courseId, string productId, string referenceId, string domainId)
        {
            var info = new TestInformation
            {
                Name = "RefactorFour",
                Description = "Simulates not loading the product course or domain in a batch"
            };

            var getCourse = new GetCourse
            {
                SearchParameters = new CourseSearch
                {
                    CourseId = courseId
                }
            };

            var getUser = new GetUsers
            {
                SearchParameters = new UserSearch
                {
                    DomainId = domainId,
                    ExternalId = referenceId
                }
            };
            var getEnrollments = new GetEntityEnrollmentList
            {
                SearchParameters = new EntitySearch
                {
                    CourseId = courseId,
                    UserId = string.Format("{0}/{1}", domainId, referenceId)
                }
            };

            var batch = new Batch();
            batch.Add(getCourse);
            batch.Add(getUser);
            batch.Add(getEnrollments);

            info.Run = () => Session.ExecuteAsAdmin(batch);

            return info;
        }
    }    

    /// <summary>
    /// Represents information about a test.
    /// </summary>
    class TestInformation
    {
        /// <summary>
        /// Name of the test.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the test.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Represents the operation that will be timed.
        /// </summary>
        public Action Run { get; set; }
    }

    public static class IEnumerableExtensions
    {
        public static long Median(this IEnumerable<long> list)
        {
            long median;
            var sorted = list.OrderBy(x => x).ToArray();

            if (sorted.Length % 2 == 0)
            {
                //even                
                var highIndex = (sorted.Length / 2);
                var lowIndex = highIndex - 1;

                median = (long)Math.Round(((double)sorted[lowIndex] + (double)sorted[highIndex]) / 2.0);
            }
            else
            {
                //odd
                var midPoint = Math.Round((double)sorted.Length / 2.0) - 1;

                if (midPoint < 0)
                    midPoint = 0;

                median = sorted[(int)midPoint];
            }

            return median;
        }
    }
}
