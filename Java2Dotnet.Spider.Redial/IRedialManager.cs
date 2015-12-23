namespace Java2Dotnet.Spider.Redial
{
	public interface IRedialManager
	{
		void WaitforRedialFinish();
		RedialResult Redial();
		INetworkValidater NetworkValidater { get; set; }
		IRedialer Redialer { get; set; }
	}
}
