namespace Java2Dotnet.Spider.Extension
{
	public interface ISpiderTask
	{
		void Run(params string[] args);
		string Name { get; }
	}
}
