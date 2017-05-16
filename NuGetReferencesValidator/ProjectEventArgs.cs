using System;
using Clide.Solution;

namespace NuGetReferencesValidator
{
    internal sealed class ProjectEventArgs : EventArgs
    {
        public IProjectNode Project { get; set; }
    }
}