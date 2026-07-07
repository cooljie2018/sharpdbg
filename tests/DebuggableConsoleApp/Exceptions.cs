using System.Net.Sockets;

namespace DebuggableConsoleApp;

public static class Exceptions
{
	public static void Test(ExceptionToThrow exceptionToThrow)
	{
		if (exceptionToThrow is ExceptionToThrow.None) return;
		try
		{
			if (exceptionToThrow is ExceptionToThrow.Normal)
			{
				throw new InvalidOperationException("Test exception");
			}
			else if (exceptionToThrow is ExceptionToThrow.ExternalCode)
			{
				var myInt = int.Parse("x");
			}
		}
		catch (Exception e)
		{
			;
		}
	}
}

public enum ExceptionToThrow
{
	None = 0,
	Normal,
	ExternalCode,
}
