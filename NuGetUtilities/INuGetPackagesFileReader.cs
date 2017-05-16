using System.Collections.Generic;

namespace NuGetUtilities
{
	public interface INuGetPackagesFileReader
	{
		IEnumerable<PackageInfo> GetPackages(string path);
	}
}