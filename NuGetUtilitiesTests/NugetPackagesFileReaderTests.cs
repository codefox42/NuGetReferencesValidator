using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using NuGetUtilities;
using NUnit.Framework;

namespace NuGetUtilitiesTests
{
	internal sealed class NuGetPackagesFileReaderTests
	{
		[Test]
		public void GetPackagesWithEmptyContent()
		{
			// Arrange
			var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				[@"C:\packages.config"] = new MockFileData("<packages/>")
			});
			var reader = new NuGetPackagesFileReader(fileSystem);

			// Act
			var packages = reader.GetPackages(@"C:\packages.config");

			// Assert
			Assert.That(packages, Is.Empty);
		}

		[Test]
		public void GetPackages()
		{
			// Arrange
			var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				[@"C:\packages.config"] = new MockFileData("<packages><package id=\"id1\" version=\"version1\" targetFramework=\"framework1\" /></packages>")
			});
			var reader = new NuGetPackagesFileReader(fileSystem);

			// Act
			var packages = reader.GetPackages(@"C:\packages.config");

			// Assert
			var package = packages.SingleOrDefault();
			Assert.That(package, Is.Not.Null);
			Assert.That(package.Id, Is.EqualTo("id1"));
			Assert.That(package.Version, Is.EqualTo("version1"));
			Assert.That(package.TargetFramework, Is.EqualTo("framework1"));
		}
	}
}