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
    public class ProjectManagerTests
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

        private ProjectContact GenerateTestProjectContact()
        {
            var contact = new ProjectContact()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                PhoneNumber = Guid.NewGuid().ToString(),
                Company = Guid.NewGuid().ToString()
            };

            return contact;
        }
        private ProjectAddress GenerateTestProjectAddress()
        {
            var projectAddress = new ProjectAddress()
            {
                Name = Guid.NewGuid().ToString(),
                Street = Guid.NewGuid().ToString(),
                Town = Guid.NewGuid().ToString(),
                PostCode = Guid.NewGuid().ToString()
            };

            return projectAddress;
        }

        [Test]
        public async Task GetNextJobNumberAsync_Test()
        {
            // Arrange
            var newProject1 = GenerateTestProjectModel();
            var newProject2 = GenerateTestProjectModel();
            var newProject3 = GenerateTestProjectModel();
            var newProject4 = GenerateTestProjectModel();
            var newProject5 = GenerateTestProjectModel();

            await _projectManager.CreateProjectAsync(newProject1, 1);
            await _projectManager.CreateProjectAsync(newProject2, 2);
            await _projectManager.CreateProjectAsync(newProject3, 3);
            await _projectManager.CreateProjectAsync(newProject4, 4065);
            await _projectManager.CreateProjectAsync(newProject5, 5);

            // Act
            var result = await _projectManager.GetNextJobNumberAsync();

            // Assert
            // Check the method returned = 6
            Assert.IsTrue(result == 4066, $"Wrong result returned, {result}");
        }

        [Test]
        public async Task CreateProjectAsync_Test()
        {
            var newProject = GenerateTestProjectModel();

            var nextJobNumber = await _projectManager.GetNextJobNumberAsync();

            newProject.JobNo = await _projectManager.CreateProjectAsync(newProject);

            var result = await _projectManager.FindByProjectNumberAsync(nextJobNumber);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.JobNo == nextJobNumber);
        }

        [Test]
        public async Task CreateProjectAsync_WithJobNumber_Test()
        {
            var newProject = GenerateTestProjectModel();

            Random random = new Random();
            var jobNo = random.Next(1000, 9000);

            newProject.JobNo = await _projectManager.CreateProjectAsync(newProject, jobNo);

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

            newProject.JobNo = await _projectManager.CreateProjectAsync(newProject);

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

            newProject1.JobNo = await _projectManager.CreateProjectAsync(newProject1);
            newProject2.JobNo = await _projectManager.CreateProjectAsync(newProject2);
            newProject3.JobNo = await _projectManager.CreateProjectAsync(newProject3);
            newProject4.JobNo = await _projectManager.CreateProjectAsync(newProject4);
            newProject5.JobNo = await _projectManager.CreateProjectAsync(newProject5);

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

            newProject1.JobNo = await _projectManager.CreateProjectAsync(newProject1);
            newProject2.JobNo = await _projectManager.CreateProjectAsync(newProject2);
            newProject3.JobNo = await _projectManager.CreateProjectAsync(newProject3);
            newProject4.JobNo = await _projectManager.CreateProjectAsync(newProject4);
            newProject5.JobNo = await _projectManager.CreateProjectAsync(newProject5);

            // Act
            var deleteResult = await _projectManager.DeleteProjectAsync(newProject4);
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject4.JobNo);


            // Assert
            // Check the Update method returned true
            Assert.IsTrue(deleteResult, "Delete Result Fail");
            // Check the new models work description with the old description
            Assert.IsTrue(findResult == null, "Find Result Fail");
        }

        [Test]
        public async Task Add_Remove_ProjectManagerAsync_Test()
        {
            // Arrange
            var newProject1 = GenerateTestProjectModel();
            newProject1.ProjectManager = new ProjectContact();
            await _projectManager.CreateProjectAsync(newProject1);

            var newTestContact = GenerateTestProjectContact();

            // Act
            var AddResult = await _projectManager.AddProjectManagerAsync(newProject1, newTestContact);
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);

            // Assert
            // Check the method returned true
            Assert.IsTrue(AddResult, "Add Result Fail");
            // Check the retrieved model with the new value
            Assert.IsTrue(findResult.ProjectManager.FirstName == newTestContact.FirstName, "Find Result Fail - Add");

            // Act
            var removeResult = await _projectManager.RemoveProjectManagerAsync(newProject1);
            var findResult2 = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);


            // Assert
            // Check the Update method returned true
            Assert.IsTrue(removeResult, "Remove Result Fail");
            // Check the retrieved model had null value
            Assert.IsTrue(findResult2.ProjectManager == null, "Find Result Fail - Remove");
        }

        [Test]
        public async Task Add_Remove_DesignerAsync_Test()
        {
            // Arrange
            var newProject1 = GenerateTestProjectModel();
            newProject1.ProjectManager = new ProjectContact();
            await _projectManager.CreateProjectAsync(newProject1);

            var newTestContact = GenerateTestProjectContact();

            // Act
            var AddResult = await _projectManager.AddDesignerAsync(newProject1, newTestContact);
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);

            // Assert
            // Check the method returned true
            Assert.IsTrue(AddResult, "Add Result Fail");
            // Check the retrieved model with the new value
            Assert.IsTrue(findResult.Designer.FirstName == newTestContact.FirstName, "Find Result Fail - Add");

            // Act
            var removeResult = await _projectManager.RemoveDesignerAsync(newProject1);
            var findResult2 = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);


            // Assert
            // Check the Update method returned true
            Assert.IsTrue(removeResult, "Remove Result Fail");
            // Check the retrieved model had null value
            Assert.IsTrue(findResult2.Designer == null, "Find Result Fail - Remove");
        }

        [Test]
        public async Task Add_Remove_QuantitySurveyorAsync_Test()
        {
            // Arrange
            var newProject1 = GenerateTestProjectModel();
            newProject1.QuantitySurveyor = new ProjectContact();
            await _projectManager.CreateProjectAsync(newProject1);

            var newTestContact = GenerateTestProjectContact();

            // Act
            var AddResult = await _projectManager.AddQuantitySurveyorAsync(newProject1, newTestContact);
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);

            // Assert
            // Check the method returned true
            Assert.IsTrue(AddResult, "Add Result Fail");
            // Check the retrieved model with the new value
            Assert.IsTrue(findResult.QuantitySurveyor.FirstName == newTestContact.FirstName, "Find Result Fail - Add");

            // Act
            var removeResult = await _projectManager.RemoveQuantitySurveyorAsync(newProject1);
            var findResult2 = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);


            // Assert
            // Check the Update method returned true
            Assert.IsTrue(removeResult, "Remove Result Fail");
            // Check the retrieved model had null value
            Assert.IsTrue(findResult2.QuantitySurveyor == null, "Find Result Fail - Remove");
        }

        [Test]
        public async Task Add_Remove_SiteManagerAsync_Test()
        {
            // Arrange
            var newProject1 = GenerateTestProjectModel();
            newProject1.SiteManager = new ProjectContact();
            await _projectManager.CreateProjectAsync(newProject1);

            var newTestContact = GenerateTestProjectContact();

            // Act
            var AddResult = await _projectManager.AddSiteManagerAsync(newProject1, newTestContact);
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);

            // Assert
            // Check the Update method returned true
            Assert.IsTrue(AddResult, "Add Result Fail");
            // Check the retrieved model with the new value
            Assert.IsTrue(findResult.SiteManager.FirstName == newTestContact.FirstName, "Find Result Fail - Add");

            // Act
            var removeResult = await _projectManager.RemoveSiteManagerAsync(newProject1);
            var findResult2 = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);


            // Assert
            // Check the Update method returned true
            Assert.IsTrue(removeResult, "Remove Result Fail");
            // Check the retrieved model had null value
            Assert.IsTrue(findResult2.SiteManager == null, "Find Result Fail - Remove");
        }

        [Test]
        public async Task Add_Remove_ProjectAddressAsync_Test()
        {
            // Arrange
            var newProject1 = GenerateTestProjectModel();
            newProject1.ProjectAddress = new ProjectAddress();
            await _projectManager.CreateProjectAsync(newProject1);

            var newTestProjectAddress = GenerateTestProjectAddress();

            // Act
            var AddResult = await _projectManager.AddProjectAddressAsync(newProject1, newTestProjectAddress);
            var findResult = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);

            // Assert
            // Check the method returned true
            Assert.IsTrue(AddResult, "Add Result Fail");
            // Check the retrieved model with the new value
            Assert.IsTrue(findResult.ProjectAddress.Name == newTestProjectAddress.Name, "Find Result Fail - Add");

            // Act
            var removeResult = await _projectManager.RemoveProjectAddressAsync(newProject1);
            var findResult2 = await _projectManager.FindByProjectNumberAsync(newProject1.JobNo);

            // Assert
            // Check the method returned true
            Assert.IsTrue(removeResult, "Remove Result Fail");
            // Check the retrieved model had null value
            Assert.IsTrue(findResult2.ProjectAddress == null, "Find Result Fail - Remove");
        }
    }
}