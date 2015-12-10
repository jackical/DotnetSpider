namespace Java2Dotnet.Spider.Lib.Redial
{
	public interface IRedialManager
	{
		void WaitforRedialFinish();
		RedialResult Redial();
	}
}
