using NuGetUtilities;
using NUnit.Framework;

namespace NuGetUtilitiesTests
{
	internal sealed class PackageInfoTests
	{
		[TestCase("id1", "id1", null, null, null, null, true)]
		[TestCase("id1", "id1", "v1", "v1", "f1", "f1", true)]
		[TestCase("id1", "id2", "v1", "v1", "f1", "f1", false)]
		[TestCase("id1", "id1", "v1", "v2", "f1", "f1", false)]
		[TestCase("id1", "id1", "v1", "v1", "f1", "f2", false)]
		public void CompareEquality(string id1, string id2, string version1, string version2, string targetFramework1,
			string targetFramework2, bool expectedResult)
		{
			// Arrange
			var package1 = new PackageInfo {Id = id1, Version = version1, TargetFramework = targetFramework1};
			var package2 = new PackageInfo {Id = id2, Version = version2, TargetFramework = targetFramework2};

			// Act
			var result = package1.Equals(package2);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}