using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	public sealed class ModelCollectorSpider<T> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T>(new CollectorModelPipeline<T>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2, T3> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new CollectorModelPipeline<T3>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2, T3, T4> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new CollectorModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new CollectorModelPipeline<T4>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2, T3, T4, T5> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new CollectorModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new CollectorModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new CollectorModelPipeline<T5>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2, T3, T4, T5, T6> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new CollectorModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new CollectorModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new CollectorModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new CollectorModelPipeline<T6>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2, T3, T4, T5, T6, T7> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new CollectorModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new CollectorModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new CollectorModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new CollectorModelPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new CollectorModelPipeline<T7>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2, T3, T4, T5, T6, T7, T8> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new CollectorModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new CollectorModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new CollectorModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new CollectorModelPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new CollectorModelPipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new CollectorModelPipeline<T8>()));
		}
	}

	public sealed class ModelCollectorSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BaseModelSpider
	{
		public ModelCollectorSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(site), scheduler)
		{
		}

		public ModelCollectorSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new CollectorModelPipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new CollectorModelPipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new CollectorModelPipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new CollectorModelPipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new CollectorModelPipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new CollectorModelPipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new CollectorModelPipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new CollectorModelPipeline<T8>()));
			AddPipeline(new ModelPipeline<T9>(new CollectorModelPipeline<T9>()));
		}
	}
}
