namespace Java2Dotnet.Spider.Core.Redial
{
	public interface IRedialer
	{
		void WaitforRedialFinish();
		void Redial();
	}
}
