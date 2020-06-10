using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Project.Models;

namespace XeppIT.ZoneElectrical.Project
{
    public class ProjectManager
    {
        private readonly IMongoCollection<ProjectModel> _projectCollection;

        public ProjectManager(IMongoCollection<ProjectModel> projectCollection)
        {
            _projectCollection = projectCollection;
        }

        public ProjectModel GetNewProjectModel()
        {
            return new ProjectModel();
        }

        public async Task CreateProjectAsync(ProjectModel project)
        {
            await _projectCollection.InsertOneAsync(project);
        }

        public async Task CreateProjectAsync(ProjectModel project, int projectNumber)
        {
            project.JobNo = projectNumber;
            await _projectCollection.InsertOneAsync(project);
        }

        public async Task<ProjectModel> FindByProjectNumberAsync(int projectNumber)
        {
            return await _projectCollection.Find(x => x.JobNo == projectNumber).FirstOrDefaultAsync();
        }

        public async Task<List<ProjectModel>> GetAllProjectsAsync()
        {
            return await _projectCollection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<bool> UpdateProjectAsync(ProjectModel project)
        {
            var result =
                await _projectCollection.ReplaceOneAsync(x => x.Id.Equals(project.Id),
                    project,
                    new ReplaceOptions());

            if (!result.IsModifiedCountAvailable || !result.IsAcknowledged) return false;

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProjectAsync(ProjectModel project)
        {
            var result = await _projectCollection.DeleteOneAsync(u => u.Id.Equals(project.Id));

            if (!result.IsAcknowledged) return false;

            return result.DeletedCount > 0;
        }
    }
}
