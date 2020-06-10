using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using XeppIT.ZoneElectrical.Project;
using XeppIT.ZoneElectrical.Project.Configs;
using XeppIT.ZoneElectrical.Project.Models;

namespace XeppIT.ZoneElectrical.UnitTests
{
    [TestFixture()]
    public class ProjectTests
    {
        private readonly string _testingConnectionString = $"mongodb://root:admin@192.168.0.13:27017";
        private IMongoCollection<ProjectModel> _projectCollection;
        protected IServiceProvider ServiceProvider;

        protected ProjectManager _projectManager;

        [SetUp]
        public async Task Setup()
        {
            var services = new ServiceCollection();
            
            services.RegisterProjectServices(_testingConnectionString, "ZoneElectricalTest");

            ServiceProvider = services.BuildServiceProvider();

            _projectManager = ServiceProvider.GetService<ProjectManager>();

            _projectCollection = ServiceProvider.GetService<IMongoCollection<ProjectModel>>();

            await _projectCollection.Database.DropCollectionAsync("Projects");

            // Add uniqe index as dont know how to fire IHostedSerices in UnitTest
            // In Real application this is set by line "services.RegisterProjectServices(_testingConnectionString, "ZoneElectricalTest");"
            // from the start up cs file which calls the static method in SetIndexOnJobNumberAsync.cs
            var projectBuilder = Builders<ProjectModel>.IndexKeys;
            var keys = new CreateIndexModel<ProjectModel>(projectBuilder.Ascending(x => x.JobNo), new CreateIndexOptions() { Unique = true });
            await _projectCollection.Indexes.CreateOneAsync(keys).ConfigureAwait(false);
        }

        private ProjectModel GenerateTestProjectModel()
        {
            var project = _projectManager.GetNewProjectModel();

            Random random = new Random();
            project.JobNo = random.Next(1000, 9000);

            project.ProjectAddress = new ProjectAddress() { 
                Name = Guid.NewGuid().ToString(), 
                Street = Guid.NewGuid().ToString(), 
                Town = Guid.NewGuid().ToString(), 
                PostCode = Guid.NewGuid().ToString() };

            project.WorkDescription = Guid.NewGuid().ToString();

            project.ProjectManager = new ProjectContact()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                PhoneNumber = Guid.NewGuid().ToString(),
                Company = Guid.NewGuid().ToString()
            };

            project.Designer = new ProjectContact()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                PhoneNumber = Guid.NewGuid().ToString(),
                Company = Guid.NewGuid().ToString()
            };

            project.QuantitySurveyor = new ProjectContact()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                PhoneNumber = Guid.NewGuid().ToString(),
                Company = Guid.NewGuid().ToString()
            };

            project.SiteManager = new ProjectContact()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                PhoneNumber = Guid.NewGuid().ToString(),
                Company = Guid.NewGuid().ToString()
            };

            return project;
        }

        [Test]
        public async Task CreateProjectAsync_Test()
        {
            var newProject = GenerateTestProjectModel();

            await _projectManager.CreateProjectAsync(newProject);

            Assert.Pass();
        }

        [Test]
        public async Task CreateProjectAsync_WithJobNumber_Test()
        {
            var newProject = GenerateTestProjectModel();

            Random random = new Random();
            var jobNo = random.Next(1000, 9000);

            await _projectManager.CreateProjectAsync(newProject, jobNo);

            var result = await _projectManager.FindByProjectNumberAsync(jobNo);

            Assert.IsTrue(result.JobNo == jobNo);
        }

        [Test]
        public async Task CreateProjectAsync_WithSameJobNumber_Test()
        {
            var newProject1 = GenerateTestProjectModel();
            var newProject2 = GenerateTestProjectModel();

            var jobNo = 1000;
            try
            {
                await _projectManager.CreateProjectAsync(newProject1, jobNo);
                await _projectManager.CreateProjectAsync(newProject2, jobNo);
            }
            catch (MongoWriteException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task FindByProjectNumberAsync_Test()
        {
            var newProject = GenerateTestProjectModel();

            await _projectManager.CreateProjectAsync(newProject);

            var result = await _projectManager.FindByProjectNumberAsync(newProject.JobNo);

            Assert.IsTrue(result.JobNo == newProject.JobNo);
        }

        [Test]
        public async Task GetAllAsync_Test()
        {
            var newProject1 = GenerateTestProjectModel();
            var newProject2 = GenerateTestProjectModel();
            var newProject3 = GenerateTestProjectModel();
            var newProject4 = GenerateTestProjectModel();
            var newProject5 = GenerateTestProjectModel();

            await _projectManager.CreateProjectAsync(newProject1);
            await _projectManager.CreateProjectAsync(newProject2);
            await _projectManager.CreateProjectAsync(newProject3);
            await _projectManager.CreateProjectAsync(newProject4);
            await _projectManager.CreateProjectAsync(newProject5);

            var result = await _projectManager.GetAllProjectsAsync();

            Assert.IsTrue(result.Count == 5);
        }

        [Test]
        public async Task UpdateProjectAsync_Test()
        {
            // Arange
            var newProject1 = GenerateTestProjectModel();
            var newProject2 = GenerateTestProjectModel();
            var newProject3 = GenerateTestProjectModel();
            var newProject4 = GenerateTestProjectModel();
            var newProject5 = GenerateTestProjectModel();

            await _projectManager.CreateProjectAsync(newProject1);
            await _projectManager.CreateProjectAsync(newProject2);
            await _projectManager.CreateProjectAsync(newProject3);
            await _projectManager.CreateProjectAsync(newProject4);
            await _projectManager.CreateProjectAsync(newProject5);

            // Act
            // Save old work description to compare
            var oldWorkDescription = newProject3.WorkDescription;
            // Create new work description
            var newWorkDescription = Guid.NewGuid().ToString();
            // Update model with new work description
            newProject3.WorkDescription = newWorkDescription;
            // Call the update method
            var updateResult = await _projectManager.UpdateProjectAsync(newProject3);
            // Retrieve model from db
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject3.JobNo);

            // Assert
            // Check the Update method returned true
            Assert.IsTrue(updateResult, "Update Result Fail");
            // Check the new models work description with the old description
            Assert.IsTrue(findResult.WorkDescription != oldWorkDescription, "Find Result Fail");
        }

        [Test]
        public async Task DeleteProjectAsync_Test()
        {
            // Arrange
            var newProject1 = GenerateTestProjectModel();
            var newProject2 = GenerateTestProjectModel();
            var newProject3 = GenerateTestProjectModel();
            var newProject4 = GenerateTestProjectModel();
            var newProject5 = GenerateTestProjectModel();

            await _projectManager.CreateProjectAsync(newProject1);
            await _projectManager.CreateProjectAsync(newProject2);
            await _projectManager.CreateProjectAsync(newProject3);
            await _projectManager.CreateProjectAsync(newProject4);
            await _projectManager.CreateProjectAsync(newProject5);

            // Act
            var deleteResult = await _projectManager.DeleteProjectAsync(newProject4);
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject4.JobNo);


            // Assert
            // Check the Update method returned true
            Assert.IsTrue(deleteResult, "Delete Result Fail");
            // Check the new models work description with the old description
            Assert.IsTrue(findResult == null, "Find Result Fail");
        }
    }
}