﻿using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Mono.Cecil;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;
using Task = System.Threading.Tasks.Task;

namespace Xpand.VSIX.Commands {
    public class LoadProjectFromReferenceCommand:OleMenuCommand{
        public LoadProjectFromReferenceCommand(int commandId= PackageIds.cmdidLoadProjectFromreference) :base((sender, args) => LoadProjects(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet, commandId)) {
            this.EnableForAssemblyReferenceSelection();
        }
        private static readonly DTE2 _dte = DteExtensions.DTE;


        private static void SkipBuild(Project project) {
            var solutionConfigurations = _dte.Solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration>();
            var solutionContexts = solutionConfigurations.SelectMany(
                solutionConfiguration => solutionConfiguration.SolutionContexts.Cast<SolutionContext>())
                .Where(context => Path.GetFileNameWithoutExtension(context.ProjectName) == project.Name).ToArray();
            foreach (var solutionContext in solutionContexts) {
                solutionContext.ShouldBuild = false;
            }

        }

        private static void ChangeActiveConfiguration(Project project) {
            var solutionConfigurationNames = _dte.Solution.SolutionBuild.SolutionConfigurations.Cast<SolutionConfiguration>()
                            .OrderByDescending(solutionConfiguration => solutionConfiguration == _dte.Solution.SolutionBuild.ActiveConfiguration).
                            ThenByDescending(solutionConfiguration => string.Equals(solutionConfiguration.Name, "debug", StringComparison.CurrentCultureIgnoreCase)).
                            Select(configuration => configuration.Name).ToArray();
            var configurationName = solutionConfigurationNames.First(solutionConfigurationName => project.ConfigurationManager.Cast<Configuration>()
                        .Any(configuration => configuration.ConfigurationName == solutionConfigurationName));
            var solutionContext = _dte.Solution.SolutionBuild.ActiveConfiguration.SolutionContexts.Cast<SolutionContext>().First(context => Path.GetFileNameWithoutExtension(context.ProjectName) == project.Name);
            solutionContext.ConfigurationName = !string.IsNullOrEmpty(OptionClass.Instance.DefaultConfiguration) ? OptionClass.Instance.DefaultConfiguration : configurationName;
            _dte.WriteToOutput(solutionContext.ConfigurationName + " configuration activated");
        }

        static void LoadProjects() {
            _dte.InitOutputCalls("LoadProjects");
            Task.Factory.StartNew(LoadProjectsCore, CancellationToken.None,TaskCreationOptions.None, TaskScheduler.Default);
        }

        private static void LoadProjectsCore(){
            try{
                var uihSolutionExplorer = _dte.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
                if (uihSolutionExplorer == null || uihSolutionExplorer.UIHierarchyItems.Count == 0)
                    throw new Exception("Nothing selected");
                var references = uihSolutionExplorer.GetReferences( reference => true);
                foreach (var reference in references){
                    var projectInfo = OptionClass.Instance.SourceCodeInfos.SelectMany(info => info.ProjectPaths)
                        .FirstOrDefault(
                            info =>
                                string.Equals(info.OutputPath, reference.Path, StringComparison.CurrentCultureIgnoreCase) &&
                                AssemblyDefinition.ReadAssembly(info.OutputPath).VersionMatch());
                    if (projectInfo != null){
                        _dte.WriteToOutput(reference.Name + " found at " + projectInfo.OutputPath);
                        if (
                            _dte.Solution.Projects()
                                .All(project => project.FullName != projectInfo.Path)){
                            var project = _dte.Solution.AddFromFile(projectInfo.Path);
                            SkipBuild(project);
                            ChangeActiveConfiguration(project);
                        }
                        else{
                            _dte.WriteToOutput(projectInfo.Path + " already loaded");
                        }
                    }
                    else{
                        _dte.WriteToOutput(reference.Name + " not found ");
                    }
                }
            }
            catch (Exception e){
                _dte.WriteToOutput(e.ToString());
            }
        }
    }
}
