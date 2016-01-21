using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	public sealed class ModelMongoDbSpider<T> : BaseModelSpider where T : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T>(new MongoDbPipeline<T>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2, T3> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey where T3 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MongoDbPipeline<T3>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2, T3, T4> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey where T3 : SpiderEntityUseStringKey where T4 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MongoDbPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MongoDbPipeline<T4>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2, T3, T4, T5> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey where T3 : SpiderEntityUseStringKey where T4 : SpiderEntityUseStringKey where T5 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MongoDbPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MongoDbPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MongoDbPipeline<T5>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2, T3, T4, T5, T6> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey where T3 : SpiderEntityUseStringKey where T4 : SpiderEntityUseStringKey where T5 : SpiderEntityUseStringKey
		where T6 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MongoDbPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MongoDbPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MongoDbPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MongoDbPipeline<T6>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2, T3, T4, T5, T6, T7> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey where T3 : SpiderEntityUseStringKey where T4 : SpiderEntityUseStringKey where T5 : SpiderEntityUseStringKey
		where T6 : SpiderEntityUseStringKey
		where T7 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MongoDbPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MongoDbPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MongoDbPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MongoDbPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new MongoDbPipeline<T7>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2, T3, T4, T5, T6, T7, T8> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey where T3 : SpiderEntityUseStringKey where T4 : SpiderEntityUseStringKey where T5 : SpiderEntityUseStringKey
		where T6 : SpiderEntityUseStringKey
		where T7 : SpiderEntityUseStringKey
		where T8 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MongoDbPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MongoDbPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MongoDbPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MongoDbPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new MongoDbPipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new MongoDbPipeline<T8>()));
		}
	}

	public sealed class ModelMongoDbSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BaseModelSpider
		where T1 : SpiderEntityUseStringKey where T2 : SpiderEntityUseStringKey where T3 : SpiderEntityUseStringKey where T4 : SpiderEntityUseStringKey where T5 : SpiderEntityUseStringKey
		where T6 : SpiderEntityUseStringKey
		where T7 : SpiderEntityUseStringKey
		where T8 : SpiderEntityUseStringKey
		where T9 : SpiderEntityUseStringKey
	{
		public ModelMongoDbSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(site), scheduler)
		{
		}

		public ModelMongoDbSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MongoDbPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MongoDbPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MongoDbPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MongoDbPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MongoDbPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MongoDbPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new MongoDbPipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new MongoDbPipeline<T8>()));
			AddPipeline(new ModelPipeline<T9>(new MongoDbPipeline<T9>()));
		}
	}
}
