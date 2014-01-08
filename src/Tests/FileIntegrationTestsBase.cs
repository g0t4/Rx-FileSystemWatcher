namespace Tests
{
	using System;
	using System.IO;
	using NUnit.Framework;

	public class FileIntegrationTestsBase : AssertionHelper
	{
		protected string TempPath;

		[SetUp]
		public void BeforeEachTest()
		{
			TempPath = Guid.NewGuid().ToString();
			Directory.CreateDirectory(TempPath);
		}

		[TearDown]
		public void AfterEachTest()
		{
			if (!Directory.Exists(TempPath))
			{
				return;
			}
			Directory.Delete(TempPath, true);
		}
	}
}