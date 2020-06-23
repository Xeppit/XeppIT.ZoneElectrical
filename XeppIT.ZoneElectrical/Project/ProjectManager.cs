using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Project.Models;
using XeppIT.ZoneElectrical.Rolodex.Addresses.Model;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Model;

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

        public async Task<int> GetNextJobNumberAsync()
        {
            var sortOptions = Builders<ProjectModel>.Sort.Descending("JobNo");
            var findOptions = Builders<ProjectModel>.Filter.Empty;
            var projection = Builders<ProjectModel>.Projection.Include(x => x.JobNo);
            var result = await _projectCollection.Find(findOptions).Project(projection).Sort(sortOptions).Limit(1).FirstOrDefaultAsync();

            if (result == null)
            {
                return 1000;
            }
            return result[1].ToInt32()+1;
        }

        public async Task<int> CreateProjectAsync(ProjectModel project)
        {
            project.JobNo = await GetNextJobNumberAsync();
            await _projectCollection.InsertOneAsync(project);
            return project.JobNo;
        }

        public async Task<int> CreateProjectAsync(ProjectModel project, int projectNumber)
        {
            project.JobNo = projectNumber;
            await _projectCollection.InsertOneAsync(project);
            return projectNumber;
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

        public async Task<bool> AddProjectManagerAsync(ProjectModel project, Contact projectManager)
        {
            project.Client = projectManager;

            var result =
                await _projectCollection.ReplaceOneAsync(x => x.Id.Equals(project.Id),
                    project,
                    new ReplaceOptions());

            if (!result.IsModifiedCountAvailable || !result.IsAcknowledged) return false;

            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveProjectManagerAsync(ProjectModel project)
        {
            project.Client = null;

            var result =
                await _projectCollection.ReplaceOneAsync(x => x.Id.Equals(project.Id),
                    project,
                    new ReplaceOptions());

            if (!result.IsModifiedCountAvailable || !result.IsAcknowledged) return false;

            return result.ModifiedCount > 0;
        }

        public async Task<bool> AddProjectAddressAsync(ProjectModel project, Address projectAddress)
        {
            project.ProjectAddress = projectAddress;

            var result =
                await _projectCollection.ReplaceOneAsync(x => x.Id.Equals(project.Id),
                    project,
                    new ReplaceOptions());

            if (!result.IsModifiedCountAvailable || !result.IsAcknowledged) return false;

            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveProjectAddressAsync(ProjectModel project)
        {
            project.ProjectAddress = null;

            var result =
                await _projectCollection.ReplaceOneAsync(x => x.Id.Equals(project.Id),
                    project,
                    new ReplaceOptions());

            if (!result.IsModifiedCountAvailable || !result.IsAcknowledged) return false;

            return result.ModifiedCount > 0;
        }
    }
}
