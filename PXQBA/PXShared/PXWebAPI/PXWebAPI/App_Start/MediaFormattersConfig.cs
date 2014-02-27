using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace PXWebAPI.App_Start
{

	//public class MultiFormDataMediaTypeFormatter : FormUrlEncodedMediaTypeFormatter
	//{
	//    public MultiFormDataMediaTypeFormatter()
	//        : base()
	//    {
	//        this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
	//    }

	//    public override bool CanReadType(Type type)
	//    {
	//        return true;
	//    }

	//    public override bool CanWriteType(Type type)
	//    {
	//        return false;
	//    }



	//    protected Task<object> OnReadFromStreamAsync<FormatterContext>(Type type, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
	//    {

	//        //var contents = formatterContext.Request.Content.ReadAsMultipartAsync().Result;
	//        return Task.Factory.StartNew<object>(() =>
	//            {
	//                return new MultiFormKeyValueModel(contents);
	//        });
	//    }

	//    //class MultiFormKeyValueModel : IKeyValueModel
	//    //{
	//    //    IEnumerable<HttpContent> _contents;
	//    //    public MultiFormKeyValueModel(IEnumerable<HttpContent> contents)
	//    //    {
	//    //        _contents = contents;
	//    //    }


	//    //    public IEnumerable<string> Keys
	//    //    {
	//    //        get
	//    //        {
	//    //            return _contents.Cast<string>();
	//    //        }
	//    //    }

	//    //    public bool TryGetValue(string key, out object value)
	//    //    {
	//    //        value = _contents.FirstDispositionNameOrDefault(key).ReadAsStringAsync().Result;
	//    //        return true;
	//    //    }
	//    //}
	//}

	/// <summary>
	/// MediaFormattersConfig
	/// </summary>
	public class MediaFormattersConfig
	{
		/// <summary>
		/// RegisterMediaFormatters
		/// </summary>
		/// <param name="config"></param>
		public static void RegisterMediaFormatters(HttpConfiguration config)
		{


			var xmlFormatter = new System.Net.Http.Formatting.XmlMediaTypeFormatter();


			var jsonFormatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter
			{
				SerializerSettings = new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
					PreserveReferencesHandling = PreserveReferencesHandling.None,
					MaxDepth = 10000,
					NullValueHandling = NullValueHandling.Ignore,
					MissingMemberHandling = MissingMemberHandling.Ignore,
					Error = delegate(object sender, ErrorEventArgs args)
					{
						new List<string>().Add(args.ErrorContext.Error.Message);
						args.ErrorContext.Handled = true;
						//TODO: Log Error here
					}
					 ,
					Formatting = Newtonsoft.Json.Formatting.Indented
					 ,
					CheckAdditionalContent = false
					,
					Binder = new DefaultSerializationBinder()


					,
					ContractResolver = new DefaultContractResolver()
					{
						SerializeCompilerGeneratedMembers = true,
						IgnoreSerializableAttribute = false,
						IgnoreSerializableInterface = false

					},

					Converters = new List<JsonConverter> {new KeyValuePairConverter(), 
																	                                     new XmlNodeConverter(), 
																	                                     new BsonObjectIdConverter(),
																	                                     new BinaryConverter(),
																	                                     new ExpandoObjectConverter(),
																	                                     new StringEnumConverter(),
																	                                     new IsoDateTimeConverter(),
																	                                     new VersionConverter()																										 
																	                                     }

				}
			};


			config.Formatters.Clear();

			config.Formatters.Add(jsonFormatter);
			config.Formatters.Add(xmlFormatter);

			config.Formatters.Add(new JQueryMvcFormUrlEncodedFormatter());

			var formUrlEncodedMediaTypeFormatter = new FormUrlEncodedMediaTypeFormatter();

			formUrlEncodedMediaTypeFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
			config.Formatters.Add(formUrlEncodedMediaTypeFormatter);


			//ValueProviderFactories.Factories.Add(new JsonValueProviderFactory());

		}
	}
}