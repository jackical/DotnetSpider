namespace Java2Dotnet.Spider.Extension
{
	public interface ISpiderTask
	{
		void Run();
		string Name { get; }
	}
}
