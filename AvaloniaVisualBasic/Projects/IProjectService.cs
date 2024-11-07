using System.Threading.Tasks;
using AvaloniaVisualBasic.Runtime.ProjectElements;

namespace AvaloniaVisualBasic.Projects;

public interface IProjectService
{
    Task CreateNewProject();
    Task CreateNewProject(IProjectTemplate template);
    Task UnloadAllProjects();
    Task UnloadProject(ProjectDefinition project);
    Task OpenProject();
    Task SaveProject(ProjectDefinition project, bool saveAs);
    Task SaveAllProjects(bool saveAs);
    Task SaveForm(FormDefinition form, bool saveAs);
    Task MakeProject();
    Task MakeProject(ProjectDefinition project);
    Task EditProjectProperties(ProjectDefinition project);
    Task EditProjectReferences(ProjectDefinition project);
    Task EditProjectComponents(ProjectDefinition project);
}