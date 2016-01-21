using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	public sealed class ModelMysqlFileSpider<T> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T>(new MySqlFilePipeline<T>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2, T3> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MySqlFilePipeline<T3>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2, T3, T4> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MySqlFilePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MySqlFilePipeline<T4>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2, T3, T4, T5> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MySqlFilePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MySqlFilePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MySqlFilePipeline<T5>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2, T3, T4, T5, T6> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MySqlFilePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MySqlFilePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MySqlFilePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MySqlFilePipeline<T6>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2, T3, T4, T5, T6, T7> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MySqlFilePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MySqlFilePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MySqlFilePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MySqlFilePipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new MySqlFilePipeline<T7>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2, T3, T4, T5, T6, T7, T8> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MySqlFilePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MySqlFilePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MySqlFilePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MySqlFilePipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new MySqlFilePipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new MySqlFilePipeline<T8>()));
		}
	}

	public sealed class ModelMysqlFileSpider<T1, T2, T3, T4, T5, T6, T7, T8, T9> : BaseModelSpider
	{
		public ModelMysqlFileSpider(string identify, Site site, IScheduler scheduler = null)
			: this(identify, new ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9>(site), scheduler)
		{
		}

		public ModelMysqlFileSpider(string identify, ModelPageProcessor processor, IScheduler scheduler = null) : base(identify, processor, scheduler)
		{
			AddPipeline(new ModelPipeline<T1>(new MySqlFilePipeline<T1>()));
			AddPipeline(new ModelPipeline<T2>(new MySqlFilePipeline<T2>()));
			AddPipeline(new ModelPipeline<T3>(new MySqlFilePipeline<T3>()));
			AddPipeline(new ModelPipeline<T4>(new MySqlFilePipeline<T4>()));
			AddPipeline(new ModelPipeline<T5>(new MySqlFilePipeline<T5>()));
			AddPipeline(new ModelPipeline<T6>(new MySqlFilePipeline<T6>()));
			AddPipeline(new ModelPipeline<T7>(new MySqlFilePipeline<T7>()));
			AddPipeline(new ModelPipeline<T8>(new MySqlFilePipeline<T8>()));
			AddPipeline(new ModelPipeline<T9>(new MySqlFilePipeline<T9>()));
		}
	}
}
