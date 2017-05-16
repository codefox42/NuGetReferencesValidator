using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;

namespace NuGetUtilities
{
	public sealed class NuGetPackagesFileReader : INuGetPackagesFileReader
	{
		private static readonly MapperConfiguration MapperConfiguration =
			new MapperConfiguration(_ => { _.CreateMap<XElement, PackageInfo>(); });

		private readonly IFileSystem _fileSystem;

		public NuGetPackagesFileReader(IFileSystem fileSystem)
		{
			_fileSystem = fileSystem;
		}

		public IEnumerable<PackageInfo> GetPackages(string path)
		{
			var mapper = MapperConfiguration.CreateMapper();
			using (var reader = _fileSystem.File.OpenText(path))
			{
				var doc = XDocument.Load(reader);
				var packages = doc.Descendants("package");
				return packages.Select(package => new PackageInfo
				{
					Id = package.Attribute("id")?.Value,
					Version = package.Attribute("version")?.Value,
					TargetFramework = package.Attribute("targetFramework")?.Value
				});
			}
		}
	}
}