using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	public sealed class ModelSpider<T> : BaseModelSpider where T : ISpiderEntity
	{
		public ModelSpider(string identify, Site site, IPageModelPipeline<T> pipeline, IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T>(site), pipeline, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor, IPageModelPipeline<T> pipeline, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T>(pipeline));
		}
	}

	public sealed class ModelSpider<T1, T2> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site, IPageModelPipeline<T1> pipeline1, IPageModelPipeline<T2> pipeline2, IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2>(site), pipeline1, pipeline2, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor, IPageModelPipeline<T1> pipeline1, IPageModelPipeline<T2> pipeline2, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
		}
	}

	public sealed class ModelSpider<T1, T2, T3> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3>(site), pipeline1, pipeline2, pipeline3, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4>(site), pipeline1, pipeline2, pipeline3, pipeline4, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7, T8> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7, pipeline8, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
			AddPipeline(new ModelPipeline<T8>(pipeline8));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity where T9 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7, pipeline8, pipeline9, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
			AddPipeline(new ModelPipeline<T8>(pipeline8));
			AddPipeline(new ModelPipeline<T9>(pipeline9));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity where T9 : ISpiderEntity
		where T10 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7,
			  pipeline8, pipeline9, pipeline10, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
			AddPipeline(new ModelPipeline<T8>(pipeline8));
			AddPipeline(new ModelPipeline<T9>(pipeline9));
			AddPipeline(new ModelPipeline<T10>(pipeline10));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity where T9 : ISpiderEntity
		where T10 : ISpiderEntity where T11 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7,
			  pipeline8, pipeline9, pipeline10, pipeline11, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
			AddPipeline(new ModelPipeline<T8>(pipeline8));
			AddPipeline(new ModelPipeline<T9>(pipeline9));
			AddPipeline(new ModelPipeline<T10>(pipeline10));
			AddPipeline(new ModelPipeline<T11>(pipeline11));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity where T9 : ISpiderEntity
		where T10 : ISpiderEntity where T11 : ISpiderEntity where T12 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IPageModelPipeline<T12> pipeline12,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7,
			  pipeline8, pipeline9, pipeline10, pipeline11, pipeline12, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IPageModelPipeline<T12> pipeline12,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
			AddPipeline(new ModelPipeline<T8>(pipeline8));
			AddPipeline(new ModelPipeline<T9>(pipeline9));
			AddPipeline(new ModelPipeline<T10>(pipeline10));
			AddPipeline(new ModelPipeline<T11>(pipeline11));
			AddPipeline(new ModelPipeline<T12>(pipeline12));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity where T9 : ISpiderEntity
		where T10 : ISpiderEntity where T11 : ISpiderEntity where T12 : ISpiderEntity where T13 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IPageModelPipeline<T12> pipeline12,
			IPageModelPipeline<T13> pipeline13,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7,
			  pipeline8, pipeline9, pipeline10, pipeline11, pipeline12, pipeline13, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IPageModelPipeline<T12> pipeline12,
			IPageModelPipeline<T13> pipeline13,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
			AddPipeline(new ModelPipeline<T8>(pipeline8));
			AddPipeline(new ModelPipeline<T9>(pipeline9));
			AddPipeline(new ModelPipeline<T10>(pipeline10));
			AddPipeline(new ModelPipeline<T11>(pipeline11));
			AddPipeline(new ModelPipeline<T12>(pipeline12));
			AddPipeline(new ModelPipeline<T13>(pipeline13));
		}
	}

	public sealed class ModelSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : BaseModelSpider
		where T1 : ISpiderEntity where T2 : ISpiderEntity where T3 : ISpiderEntity where T4 : ISpiderEntity where T5 : ISpiderEntity where T6 : ISpiderEntity where T7 : ISpiderEntity where T8 : ISpiderEntity where T9 : ISpiderEntity
		where T10 : ISpiderEntity where T11 : ISpiderEntity where T12 : ISpiderEntity where T13 : ISpiderEntity
		where T14 : ISpiderEntity
	{
		public ModelSpider(string identify, Site site,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IPageModelPipeline<T12> pipeline12,
			IPageModelPipeline<T13> pipeline13,
			IPageModelPipeline<T14> pipeline14,
			IScheduler scheduler = null)
		: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13,T14>(site), pipeline1, pipeline2, pipeline3, pipeline4, pipeline5, pipeline6, pipeline7,
			  pipeline8, pipeline9, pipeline10, pipeline11, pipeline12, pipeline13,pipeline14, scheduler)
		{
		}

		public ModelSpider(string identify, ModelPageProcessor processor,
			IPageModelPipeline<T1> pipeline1,
			IPageModelPipeline<T2> pipeline2,
			IPageModelPipeline<T3> pipeline3,
			IPageModelPipeline<T4> pipeline4,
			IPageModelPipeline<T5> pipeline5,
			IPageModelPipeline<T6> pipeline6,
			IPageModelPipeline<T7> pipeline7,
			IPageModelPipeline<T8> pipeline8,
			IPageModelPipeline<T9> pipeline9,
			IPageModelPipeline<T10> pipeline10,
			IPageModelPipeline<T11> pipeline11,
			IPageModelPipeline<T12> pipeline12,
			IPageModelPipeline<T13> pipeline13,
			IPageModelPipeline<T14> pipeline14,
			IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(pipeline1));
			AddPipeline(new ModelPipeline<T2>(pipeline2));
			AddPipeline(new ModelPipeline<T3>(pipeline3));
			AddPipeline(new ModelPipeline<T4>(pipeline4));
			AddPipeline(new ModelPipeline<T5>(pipeline5));
			AddPipeline(new ModelPipeline<T6>(pipeline6));
			AddPipeline(new ModelPipeline<T7>(pipeline7));
			AddPipeline(new ModelPipeline<T8>(pipeline8));
			AddPipeline(new ModelPipeline<T9>(pipeline9));
			AddPipeline(new ModelPipeline<T10>(pipeline10));
			AddPipeline(new ModelPipeline<T11>(pipeline11));
			AddPipeline(new ModelPipeline<T12>(pipeline12));
			AddPipeline(new ModelPipeline<T13>(pipeline13));
			AddPipeline(new ModelPipeline<T14>(pipeline14));
		}
	}
}
