using System;
using Microsoft.VisualStudio.Shell;

namespace NuGetReferencesValidator
{
    internal sealed class ErrorListHelper : IServiceProvider
    {
        private static readonly Guid PackageGuid = Guid.Parse("0b12d070-36a8-4b70-89d9-7d9dfef735a3");
        private static readonly string PackageName = "NuGetReferencesValidator";

        public object GetService(Type serviceType)
        {
            return Package.GetGlobalService(serviceType);
        }

        public ErrorListProvider GetErrorListProvider()
        {
            var provider = new ErrorListProvider(this)
            {
                ProviderName = PackageName,
                ProviderGuid = PackageGuid
            };
            return provider;
        }

        public void Write(
            TaskCategory category,
            TaskErrorCategory errorCategory,
            string context, //used as an indicator when removing
            string text,
            string document,
            int line,
            int column)
        {
            ErrorTask task = new ErrorTask();
            task.Text = text;
            task.ErrorCategory = errorCategory;
            //The task list does +1 before showing this numbers
            task.Line = line - 1;
            task.Column = column - 1;
            task.Document = document;
            task.Category = category;

            if (!string.IsNullOrEmpty(document))
            {
                //attach to the navigate event
                //task.Navigate += NavigateDocument;
            }
            GetErrorListProvider().Tasks.Add(task);//add it to the errorlistprovider
        }

	    public void Write(ErrorTask error)
	    {
		    GetErrorListProvider().Tasks.Add(error);
		}
	}
}