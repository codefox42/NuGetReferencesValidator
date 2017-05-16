using System;
using Clide;
using Clide.Solution;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuGetReferencesValidator
{
    internal sealed class SolutionEventDispatcher : IVsSolutionEvents
    {
        public SolutionEventDispatcher(IDevEnv devEnv)
        {
            DevEnv = devEnv;
        }

        private IDevEnv DevEnv { get; }

        public event EventHandler<ProjectEventArgs> ProjectOpened;

        public event EventHandler<ProjectEventArgs> ProjectLoaded;

        public void AttachToSolutionEvents(IVsSolution solution)
        {
            solution?.AdviseSolutionEvents(this, out uint _);
        }

        private void OnProjectOpened(ProjectEventArgs e)
        {
            ProjectOpened?.Invoke(this, e);
        }

        private void OnProjectLoaded(ProjectEventArgs e)
        {
            ProjectLoaded?.Invoke(this, e);
        }

        #region Implementation of IVsSolutionEvents

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            pHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int) __VSHPROPID.VSHPROPID_ExtObject,
                out object objProj);
            var project = objProj as Project;

            OnProjectOpened(new ProjectEventArgs {Project = GetProjectNode(project)});
            return VSConstants.S_OK;
        }

        private IProjectNode GetProjectNode(Project project)
        {
            return DevEnv
                .SolutionExplorer()
                .Solution.FindProject(_ => _.DisplayName == project.Name);
        }

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            pRealHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int) __VSHPROPID.VSHPROPID_ExtObject,
                out object objProj);
            var project = objProj as Project;

            OnProjectLoaded(new ProjectEventArgs {Project = GetProjectNode(project)});
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}