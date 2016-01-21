using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Monitor;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Core.Proxy;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Utils;
using log4net;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// A spider contains four modules: Downloader, Scheduler, PageProcessor and Pipeline. 
	/// </summary>
	public class Spider : ISpider
	{
		public int ThreadNum { get; set; } = 1;
		public int Deep { get; set; } = int.MaxValue;
		public AutomicLong FinishedPageCount { get; set; } = new AutomicLong(0);
		public bool SpawnUrl { get; set; } = true;
		public DateTime StartTime { get; private set; }
		public DateTime FinishedTime { get; private set; } = DateTime.MinValue;
		public Site Site { get; protected set; }
		public Func<string, string> SubDownloadedHtml;
		public bool ShowControl { get; set; }
		public bool SaveStatusToRedis { get; set; }
		public string Identify { get; }
		public bool ShowConsoleStatus { get; set; } = true;
		public List<IPipeline> Pipelines { get; private set; } = new List<IPipeline>();
		public IDownloader Downloader { get; private set; }
		public bool IsExitWhenComplete { get; set; } = true;
		public Status StatusCode => Stat;
		public IScheduler Scheduler { get; }
		//public IList<ISpiderListener> SpiderListeners { get; set; } = new List<ISpiderListener>();
		public event RequestSuccessed RequestSuccessedEvent;
		public event RequestFailed RequestFailedEvent;
		public event SpiderClosing SpiderClosingEvent;
		public int ThreadAliveCount => ThreadPool.ThreadAlive;
		public Dictionary<string, dynamic> Settings { get; } = new Dictionary<string, dynamic>();

		protected readonly string DataRootDirectory;
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(Spider));
		protected IPageProcessor PageProcessor { get; set; }
		protected List<Request> StartRequests { get; set; }
		protected static readonly int WaitInterval = 8;
		protected Status Stat = Status.Init;
		protected CountableThreadPool ThreadPool { get; set; }
		//protected bool DestroyWhenExit { get; set; } = true;

		private int _waitCountLimit = 20;
		private int _waitCount;
		private bool _init;
		private bool _runningExit;
		private static readonly Regex IdentifyRegex = new Regex(@"^[\d\w\s-/]+$");

		/// <summary>
		/// Create a spider with pageProcessor.
		/// </summary>
		/// <param name="pageProcessor"></param>
		/// <returns></returns>
		public static Spider Create(IPageProcessor pageProcessor)
		{
			return new Spider(Guid.NewGuid().ToString(), pageProcessor, new QueueDuplicateRemovedScheduler());
		}

		/// <summary>
		/// Create a spider with pageProcessor and scheduler
		/// </summary>
		/// <param name="pageProcessor"></param>
		/// <param name="scheduler"></param>
		/// <returns></returns>
		public static Spider Create(IPageProcessor pageProcessor, IScheduler scheduler)
		{
			return new Spider(Guid.NewGuid().ToString(), pageProcessor, scheduler);
		}

		/// <summary>
		/// Create a spider with indentify, pageProcessor, scheduler.
		/// </summary>
		/// <param name="identify"></param>
		/// <param name="pageProcessor"></param>
		/// <param name="scheduler"></param>
		/// <returns></returns>
		public static Spider Create(string identify, IPageProcessor pageProcessor, IScheduler scheduler)
		{
			return new Spider(identify, pageProcessor, scheduler);
		}

		/// <summary>
		/// Create a spider with pageProcessor.
		/// </summary>
		/// <param name="identify"></param>
		/// <param name="pageProcessor"></param>
		/// <param name="scheduler"></param>
		protected Spider(string identify, IPageProcessor pageProcessor, IScheduler scheduler)
		{
			_waitCount = 0;
			PageProcessor = pageProcessor;
			Site = pageProcessor.Site;
			StartRequests = Site.StartRequests;

			Scheduler = scheduler ?? new QueueDuplicateRemovedScheduler();
			if (string.IsNullOrWhiteSpace(identify))
			{
				Identify = string.IsNullOrEmpty(Site.Domain) ? Guid.NewGuid().ToString() : Site.Domain;
			}
			else
			{
				if (!IdentifyRegex.IsMatch(identify))
				{
					throw new SpiderExceptoin("Task Identify only can contains A-Z a-z 0-9 _ -");
				}
				Identify = identify;
			}

			DataRootDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\data\\" + Identify;
		}

		/// <summary>
		/// Start with more than one threads
		/// </summary>
		/// <param name="threadNum"></param>
		/// <returns></returns>
		public virtual Spider SetThreadNum(int threadNum)
		{
			CheckIfRunning();
			ThreadNum = threadNum;
			if (threadNum <= 0)
			{
				throw new ArgumentException("threadNum should be more than one!");
			}
			return this;
		}

		/// <summary>
		/// Set wait time when no url is polled.
		/// </summary>
		/// <param name="emptySleepTime"></param>
		public void SetEmptySleepTime(int emptySleepTime)
		{
			if (emptySleepTime > 10000)
			{
				_waitCountLimit = emptySleepTime / WaitInterval;
			}
			else
			{
				throw new SpiderExceptoin("Sleep time should be large than 10000.");
			}
		}

		/// <summary>
		/// Set startUrls of Spider. 
		/// Prior to startUrls of Site.
		/// </summary>
		/// <param name="startUrls"></param>
		/// <returns></returns>
		public Spider AddStartUrls(IList<string> startUrls)
		{
			CheckIfRunning();
			StartRequests = new List<Request>(UrlUtils.ConvertToRequests(startUrls, 1));
			return this;
		}

		/// <summary>
		/// Set startUrls of Spider. 
		/// Prior to startUrls of Site.
		/// </summary>
		/// <param name="startRequests"></param>
		/// <returns></returns>
		public Spider AddStartRequests(IList<Request> startRequests)
		{
			CheckIfRunning();
			StartRequests = new List<Request>(startRequests);
			return this;
		}

		/// <summary>
		/// Add urls to crawl.
		/// </summary>
		/// <param name="urls"></param>
		/// <returns></returns>
		public Spider AddStartUrl(params string[] urls)
		{
			foreach (string url in urls)
			{
				AddStartRequest(new Request(url, 1, null));
			}
			return this;
		}

		public Spider AddStartUrl(ICollection<string> urls)
		{
			foreach (string url in urls)
			{
				AddStartRequest(new Request(url, 1, null));
			}
			return this;
		}

		/// <summary>
		/// Add urls with information to crawl.
		/// </summary>
		/// <param name="requests"></param>
		/// <returns></returns>
		public Spider AddRequest(params Request[] requests)
		{
			foreach (Request request in requests)
			{
				AddStartRequest(request);
			}
			return this;
		}

		/// <summary>
		/// Add a pipeline for Spider
		/// </summary>
		/// <param name="pipeline"></param>
		/// <returns></returns>
		public Spider AddPipeline(IPipeline pipeline)
		{
			CheckIfRunning();
			Pipelines.Add(pipeline);
			return this;
		}

		/// <summary>
		/// Set pipelines for Spider
		/// </summary>
		/// <param name="pipelines"></param>
		/// <returns></returns>
		public Spider AddPipelines(IList<IPipeline> pipelines)
		{
			CheckIfRunning();
			foreach (var pipeline in pipelines)
			{
				AddPipeline(pipeline);
			}
			return this;
		}

		/// <summary>
		/// Clear the pipelines set
		/// </summary>
		/// <returns></returns>
		public Spider ClearPipeline()
		{
			Pipelines = new List<IPipeline>();
			return this;
		}

		/// <summary>
		/// Set the downloader of spider
		/// </summary>
		/// <param name="downloader"></param>
		/// <returns></returns>
		public Spider SetDownloader(IDownloader downloader)
		{
			CheckIfRunning();
			Downloader = downloader;
			return this;
		}

		public void InitComponent()
		{
			if (_init)
			{
				Logger.InfoFormat("Component already init.");
				return;
			}

			// workaround
			if (Application.OpenForms.Count == 0)
			{
				Console.CancelKeyPress += ConsoleCancelKeyPress;
			}

			Scheduler.Init(this);

			if (Downloader == null)
			{
				Downloader = new HttpClientDownloader();
			}

			Downloader.ThreadNum = ThreadNum;

			if (Pipelines.Count == 0)
			{
				Pipelines.Add(new FilePipeline());
			}
			if (ThreadPool == null)
			{
				ThreadPool = new CountableThreadPool(ThreadNum);
			}

			if (StartRequests != null)
			{
				if (StartRequests.Count > 0)
				{
					Parallel.ForEach(StartRequests, new ParallelOptions() { MaxDegreeOfParallelism = 100 }, request =>
					{
						Scheduler.Push((Request)request.Clone(), this);
					});

					ClearStartRequests();
					Logger.InfoFormat("Push Request to Scheduler success.");
				}
				else
				{
					Logger.InfoFormat("Push Zero Request to Scheduler.");
				}
			}

			Task.Factory.StartNew(() =>
			{
				if (ShowConsoleStatus)
				{
					IMonitorableScheduler monitor = (IMonitorableScheduler)Scheduler;
					while (true)
					{
						try
						{
							if (Stat == Status.Running)
							{
								Console.WriteLine($"Left: {monitor.GetLeftRequestsCount(this)} Total: {monitor.GetTotalRequestsCount(this)} AliveThread: {ThreadPool.ThreadAlive} ThreadNum: {ThreadPool.ThreadNum }");
							}
						}
						catch
						{
							// ignored
						}
						Thread.Sleep(2000);
					}
				}
			});

			_init = true;
		}

		public void Run()
		{
			CheckIfRunning();

			Stat = Status.Running;
			_runningExit = false;

			// ���뿪�����߳�����
			System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;

			Logger.Info("Spider " + Identify + " InitComponent...");
			InitComponent();

			Logger.Info("Spider " + Identify + " Started!");

			bool firstTask = false;

			while (Stat == Status.Running)
			{
				Request request = Scheduler.Poll(this);

				if (request == null)
				{
					if (ThreadPool.ThreadAlive == 0 && IsExitWhenComplete)
					{
						Stat = Status.Finished;
						break;
					}

					if (_waitCount > _waitCountLimit)
					{
						break;
					}

					// wait until new url added
					WaitNewUrl();
				}
				else
				{
					if (StartTime == DateTime.MinValue)
					{
						StartTime = DateTime.Now;
					}

					_waitCount = 0;

					ThreadPool.Push(obj =>
					{
						var request1 = obj as Request;
						if (request1 != null)
						{
							try
							{
								ProcessRequest(request1);

								Thread.Sleep(Site.SleepTime);

								OnSuccess(request1);

								Console.WriteLine($"Request: {request.Url} Sucess.");
							}
							catch (Exception e)
							{
								OnError(request1);
								Logger.Error("Request " + request1.Url + " failed.", e);
							}
							finally
							{
								if (Site.HttpProxyPoolEnable)
								{
									Site.ReturnHttpProxyToPool((HttpHost)request1.GetExtra(Request.Proxy), (int)request1.GetExtra(Request.StatusCode));
								}
								FinishedPageCount.Inc();
							}
							return true;
						}

						return false;
					}, request);

					if (!firstTask)
					{
						Thread.Sleep(3000);
						firstTask = true;
					}
				}
			}


			ThreadPool.WaitToExit();
			FinishedTime = DateTime.Now;

			// Pipeline���п����л�������, ��Ҫ����/�������ܰ�ȫ�˳�/��ͣ
			foreach (IPipeline pipeline in Pipelines)
			{
				SafeDestroy(pipeline);
			}

			if (Stat == Status.Finished)
			{
				OnClose();
			}

			if (Stat == Status.Stopped)
			{
				Logger.Info("Spider " + Identify + " stop success!");
			}

			_runningExit = true;
		}

		public void RunAsync()
		{
			Task.Factory.StartNew(Run).ContinueWith(t =>
			{
				if (t.Exception != null)
				{
					Logger.Error(t.Exception.Message);
				}
			});
		}

		public void Start()
		{
			RunAsync();
		}

		public void Stop()
		{
			Stat = Status.Stopped;
			Console.WriteLine("Trying to stop Spider " + Identify + "...");
		}

		protected void OnClose()
		{
			SpiderClosingEvent?.Invoke();
			SafeDestroy(Downloader);
			SafeDestroy(PageProcessor);
		}

		protected void OnError(Request request)
		{
			lock (this)
			{
				//д���ļ���, �û������յĽ������֪���ж��ٸ�Requestû����. �ṩReRun, Spider����������������Request�����ܹ�
				FileInfo file = FilePersistentBase.PrepareFile(Path.Combine(DataRootDirectory, "ErrorRequests.txt"));
				File.AppendAllText(file.FullName, JsonConvert.SerializeObject(request) + Environment.NewLine, Encoding.UTF8);
			}

			RequestFailedEvent?.Invoke(request);
		}

		protected void OnSuccess(Request request)
		{
			RequestSuccessedEvent?.Invoke(request);
		}

		protected Page AddToCycleRetry(Request request, Site site)
		{
			Page page = new Page(request);
			dynamic cycleTriedTimesObject = request.GetExtra(Request.CycleTriedTimes);
			if (cycleTriedTimesObject == null)
			{
				// ���Լ��ӵ�Ŀ��Request��(�޷��������߳��ټ��ش�Request), �������̺߳���TargetRequest�ӵ�Pool��
				request.Priority = 0;
				page.AddTargetRequest(request.PutExtra(Request.CycleTriedTimes, 1));
			}
			else
			{
				int cycleTriedTimes = (int)cycleTriedTimesObject;
				cycleTriedTimes++;
				if (cycleTriedTimes >= site.CycleRetryTimes)
				{
					// ��������Դ���, ���ؿ�.
					return null;
				}
				request.Priority = 0;
				page.AddTargetRequest(request.PutExtra(Request.CycleTriedTimes, cycleTriedTimes));
			}
			page.IsNeedCycleRetry = true;
			return page;
		}

		protected void ProcessRequest(Request request)
		{
			Page page = null;

			while (true)
			{
				try
				{

					// ����ҳ��
					page = Downloader.Download(request, this);

					if (page.IsSkip)
					{
						return;
					}

					// ����HTML��ȡ
					if (SubDownloadedHtml != null)
					{
						page.RawText = SubDownloadedHtml(page.RawText);
					}

					// ����ҳ������
					// PageProcess��2�ִ���1 ���ص�HTML���� 2��ʵ�ֵ�IPageProcessor����
					PageProcessor.Process(page);

					break;
				}
				catch (Exception e)
				{
					if (Site.CycleRetryTimes > 0)
					{
						page = AddToCycleRetry(request, Site);
					}

					Logger.Warn("Download page " + request.Url + " failed:" + e.Message);
					break;
				}
			}

			//watch.Stop();
			//Logger.Info("dowloader cost time:" + watch.ElapsedMilliseconds);

			if (page == null)
			{
				OnError(request);
				return;
			}

			// for cycle retry, ������س���ʱ, �������Request�ӻ�TargetUrls�����ظ��������Դ�ʱ��targetRequestsֻ�б���
			// ������Ҫ���� MissTargetUrls�����
			if (page.IsNeedCycleRetry)
			{
				ExtractAndAddRequests(page, true);
				return;
			}

			//watch.Stop();
			//Logger.Info("process cost time:" + watch.ElapsedMilliseconds);

			if (page.MissTargetUrls)
			{
				Logger.Info("Stoper trigger worked on this page.");
			}
			else
			{
				ExtractAndAddRequests(page, SpawnUrl);
			}

			// Pipeline�����������ݱ���ȹ���, �ǲ�������κβ���, �������,���ݴ�һ��϶�Ҳ��������, ���ֱ�ӹҵ�Spider�ȽϺá�
			if (!page.ResultItems.IsSkip)
			{
				foreach (IPipeline pipeline in Pipelines)
				{
					pipeline.Process(page.ResultItems, this);
				}
				//cts?.Cancel();
			}
			else
			{
				Logger.Warn($"Request {request.Url} 's result count is zero.");
			}

			//watch.Stop();
			//Logger.Info("pipeline cost time:" + watch.ElapsedMilliseconds);
		}

		protected void ExtractAndAddRequests(Page page, bool spawnUrl)
		{
			if (spawnUrl && page.Request.NextDepth < Deep && page.TargetRequests != null && page.TargetRequests.Count > 0)
			{
				foreach (Request request in page.TargetRequests)
				{
					AddStartRequest(request);
				}
			}
		}

		protected void CheckIfRunning()
		{
			if (Stat == Status.Running)
			{
				throw new SpiderExceptoin("Spider is already running!");
			}
		}

		//protected virtual List<ICollectorPipeline> GetCollectorPipeline(params Type[] types)
		//{
		//	return new List<ICollectorPipeline>() { new ResultItemsCollectorPipeline() };
		//}

		///// <summary>
		///// Download urls synchronizing.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="urls"></param>
		///// <returns></returns>
		//public IList<T> GetAll<T>(params string[] urls)
		//{
		//	DestroyWhenExit = false;
		//	SpawnUrl = false;

		//	foreach (Request request in UrlUtils.ConvertToRequests(urls, 1))
		//	{
		//		AddRequest(request);
		//	}
		//	ICollectorPipeline collectorPipeline = GetCollectorPipeline<T>();
		//	Pipelines.Clear();
		//	Pipelines.Add(collectorPipeline);
		//	Run();
		//	SpawnUrl = true;
		//	DestroyWhenExit = true;

		//	ICollection collection = collectorPipeline.GetCollected();

		//	try
		//	{
		//		return (from object current in collection select (T)current).ToList();
		//	}
		//	catch (Exception)
		//	{
		//		throw new SpiderExceptoin($"Your pipeline didn't extract data to model: {typeof(T).FullName}");
		//	}
		//}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		//public Dictionary<Type, List<dynamic>> GetAll(Type[] types, params string[] urls)
		//{
		//	//DestroyWhenExit = false;
		//	SpawnUrl = false;

		//	foreach (Request request in UrlUtils.ConvertToRequests(urls, 1))
		//	{
		//		AddRequest(request);
		//	}
		//	List<ICollectorPipeline> collectorPipelineList = GetCollectorPipeline(types);
		//	Pipelines.Clear();
		//	Pipelines.AddRange(collectorPipelineList);
		//	Run();
		//	SpawnUrl = true;
		//	//DestroyWhenExit = true;

		//	Dictionary<Type, List<dynamic>> result = new Dictionary<Type, List<dynamic>>();
		//	foreach (var collectorPipeline in collectorPipelineList)
		//	{
		//		ICollection collection = collectorPipeline.GetCollected();

		//		foreach (var entry in collection)
		//		{
		//			var de = (KeyValuePair<Type, List<dynamic>>)entry;

		//			if (result.ContainsKey(de.Key))
		//			{
		//				result[de.Key].AddRange(de.Value);
		//			}
		//			else
		//			{
		//				result.Add(de.Key, new List<dynamic>(de.Value));
		//			}
		//		}
		//	}

		//	return result;
		//}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void ClearStartRequests()
		{
			//Request tmpTequest;
			//while (StartRequests.TryTake(out tmpTequest))
			//{
			//	tmpTequest.Dispose();
			//}
			StartRequests.Clear();
			GC.Collect();
		}

		private void AddStartRequest(Request request)
		{
			Scheduler.Push(request, this);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void WaitNewUrl()
		{
			//double check
			//if (ThreadPool.GetThreadAlive() == 0 && ExitWhenComplete)
			//{
			//	return;
			//}

			Thread.Sleep(WaitInterval);
			++_waitCount;
		}

		private void SafeDestroy(object obj)
		{
			var disposable = obj as IDisposable;
			if (disposable != null)
			{
				try
				{
					disposable.Dispose();
				}
				catch (Exception e)
				{
					Logger.Warn(e);
				}
			}
		}

		private void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Stop();
			while (!_runningExit)
			{
				Thread.Sleep(1500);
			}
		}

		//public T Get<T>(string url)
		//{
		//	IList<T> resultItemses = GetAll<T>(url);
		//	if (resultItemses != null && resultItemses.Count > 0)
		//	{
		//		return resultItemses[0];
		//	}
		//	return default(T);
		//}
	}
}