using System;

namespace NuGetUtilities
{
	public sealed class PackageInfo : IEquatable<PackageInfo>
	{
		public string Id { get; set; }
		public string Version { get; set; }
		public string TargetFramework { get; set; }

		#region Overrides of Object

		public override string ToString()
		{
			return $"{Id} {Version}";
		}

		#endregion

		#region Equality members

		public bool Equals(PackageInfo other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(Id, other.Id, StringComparison.InvariantCulture)
			       && string.Equals(Version, other.Version, StringComparison.InvariantCulture)
			       && string.Equals(TargetFramework, other.TargetFramework, StringComparison.InvariantCulture);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as PackageInfo);
		}

		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			var id = Id;
			return id != null ? StringComparer.InvariantCulture.GetHashCode(id) : 0;
		}

		#endregion
	}
}