using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	public sealed class ModelDatabaseSpider<T> : BaseModelSpider where T : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T>(new DatabasePipeline<T>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2, T3> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new DatabasePipeline<T3>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2, T3, T4> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new DatabasePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new DatabasePipeline<T4>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2, T3, T4, T5> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new DatabasePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new DatabasePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new DatabasePipeline<T5>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2, T3, T4, T5, T6> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new DatabasePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new DatabasePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new DatabasePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new DatabasePipeline<T6>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2, T3, T4, T5, T6, T7> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new DatabasePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new DatabasePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new DatabasePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new DatabasePipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new DatabasePipeline<T7>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2, T3, T4, T5, T6, T7, T8> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new DatabasePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new DatabasePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new DatabasePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new DatabasePipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new DatabasePipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new DatabasePipeline<T8>()));
		}
	}

	public sealed class ModelDatabaseSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity where T9 : ISpiderEntity
	{
		public ModelDatabaseSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(site), scheduler)
		{
		}

		public ModelDatabaseSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new DatabasePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new DatabasePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new DatabasePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new DatabasePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new DatabasePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new DatabasePipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new DatabasePipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new DatabasePipeline<T8>()));
			AddPipeline(new ModelPipeline<T9>(new DatabasePipeline<T9>()));
		}
	}
}
