using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	public sealed class ModelJsonFileSpider<T> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T>(new JsonFileModelPipeline<T>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2, T3> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new JsonFileModelPipeline<T3>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2, T3, T4> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new JsonFileModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new JsonFileModelPipeline<T4>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2, T3, T4, T5> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new JsonFileModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new JsonFileModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new JsonFileModelPipeline<T5>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2, T3, T4, T5, T6> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new JsonFileModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new JsonFileModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new JsonFileModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new JsonFileModelPipeline<T6>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2, T3, T4, T5, T6, T7> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new JsonFileModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new JsonFileModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new JsonFileModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new JsonFileModelPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new JsonFileModelPipeline<T7>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2, T3, T4, T5, T6, T7, T8> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new JsonFileModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new JsonFileModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new JsonFileModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new JsonFileModelPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new JsonFileModelPipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new JsonFileModelPipeline<T8>()));
		}
	}

	public sealed class ModelJsonFileSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BaseModelSpider
	{
		public ModelJsonFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(site), scheduler)
		{
		}

		public ModelJsonFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new JsonFileModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new JsonFileModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new JsonFileModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new JsonFileModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new JsonFileModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new JsonFileModelPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new JsonFileModelPipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new JsonFileModelPipeline<T8>()));
			AddPipeline(new ModelPipeline<T9>(new JsonFileModelPipeline<T9>()));
		}
	}
}
