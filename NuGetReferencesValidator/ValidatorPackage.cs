//------------------------------------------------------------------------------
// <copyright file="ValidatorPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using Clide;
using Clide.Sdk.Solution;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGetUtilities;

namespace NuGetReferencesValidator
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The minimum requirement for a class to be considered a valid package for Visual Studio
    ///         is to implement the IVsPackage interface and register itself with the shell.
    ///         This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///         to do it: it derives from the Package class that provides the implementation of the
    ///         IVsPackage interface and uses the registration attributes defined in the framework to
    ///         register itself and its components with the shell. These attributes tell the pkgdef creation
    ///         utility what data to put into .pkgdef file.
    ///     </para>
    ///     <para>
    ///         To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
    ///         &gt; in .vsixmanifest file.
    ///     </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids.SolutionHasMultipleProjects)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class ValidatorPackage : Package
    {
        /// <summary>
        ///     ValidatorPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0a605259-855c-4d8a-a27b-95c2bd403900";

        private IDictionary<string, PackageInfo> PackagesByName { get; } = new Dictionary<string, PackageInfo>();
        private INuGetPackagesFileReader PackagesFileReader { get; } = new NuGetPackagesFileReader(new FileSystem());

        private SolutionEventDispatcher SolutionEventDispatcher { get; set; }

        private IDevEnv DevEnv { get; set; }

        private IVsSolution VsSolution { get; set; }

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            DevEnv = Host.Initialize(this);

            VsSolution = GetService(typeof(SVsSolution)) as IVsSolution;
            SolutionEventDispatcher = new SolutionEventDispatcher(DevEnv);
            SolutionEventDispatcher.AttachToSolutionEvents(VsSolution);
            SolutionEventDispatcher.ProjectOpened += OnProjectOpenedOrLoaded;
            SolutionEventDispatcher.ProjectLoaded += OnProjectOpenedOrLoaded;
        }

        private void OnProjectOpenedOrLoaded(object sender, ProjectEventArgs e)
        {
            Trace.WriteLine($"Project opened or loaded: {e.Project.DisplayName}", "[ValidatorPackage]");
            var items = e.Project.Nodes.Traverse().OfType<ItemNode>();
            var packagesItem = items.FirstOrDefault(_ => _.DisplayName.ToLowerInvariant() == "packages.config");
            var path = packagesItem?.PhysicalPath;
            if (!File.Exists(path))
                return;

            ProcessPackagesFile(path);
        }

        private void ProcessPackagesFile(string path)
        {
            Trace.WriteLine($"Processing: {path}", "[ValidatorPackage]");

            var packages = PackagesFileReader.GetPackages(path);
            foreach (var package in packages)
                if (PackagesByName.TryGetValue(package.Id, out PackageInfo existingPackage))
                {
                    if (!package.Equals(existingPackage))
                    {
                        Trace.WriteLine($"Conflicting package {package} in {path}", "[ValidatorPackage]");

                        LogError(package, existingPackage);
                    }
                    else
                    {
                        Trace.WriteLine($"Matching package {package} in {path}", "[ValidatorPackage]");
                    }
                }
                else
                {
                    Trace.WriteLine($"New package {package} in {path}", "[ValidatorPackage]");

                    PackagesByName.Add(package.Id, package);
                }
        }

        private static void LogError(PackageInfo package, PackageInfo existingPackage)
        {
            var message =
                $"Inconsistent NuGet reference for package {package.Id}: {package.Version}/{package.TargetFramework} vs {existingPackage.Version}/{existingPackage.TargetFramework}";
            var helper = new ErrorListHelper();
            helper.Write(TaskCategory.BuildCompile, TaskErrorCategory.Error, "NuGetReferencesValidatorContext", message,
                null, 0, 0);
            //helper.Write(new ErrorTask
            //{
            //	Category = TaskCategory.BuildCompile,
            //	ErrorCategory = TaskErrorCategory.Error,
            //	Text = message,
            //	Document = null,
            //	Line = 0,
            //	Column = 0
            //});
        }
    }
}