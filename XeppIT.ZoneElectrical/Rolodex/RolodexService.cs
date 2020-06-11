using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using XeppIT.ZoneElectrical.Rolodex.Models;

namespace XeppIT.ZoneElectrical.Rolodex
{
    public class RolodexService
    {
        private readonly IMongoCollection<Address> _addressCollection;

        private readonly IMongoCollection<Company> _companyCollection;
        // Create
        // Read
        // Update
        // Delete

        public RolodexService(IMongoCollection<Address> addressCollection, IMongoCollection<Company> companyCollection)
        {
            _addressCollection = addressCollection;
            _companyCollection = companyCollection;
        }

        public async Task CreateAddressAsync(Address address)
        {
            await _addressCollection.InsertOneAsync(address);
        }
        public async Task<List<Address>> FindAllAddressesAsync()
        {
            var filter = Builders<Address>.Filter.Empty;
            var result = await _addressCollection.Find(filter).ToListAsync();
            return result;
        }
        public async Task<Address> FindAddressByIdAsync(string id)
        {
            var filter = Builders<Address>.Filter.Eq(a => a.Id, id);
            var result = await _addressCollection.Find(filter).FirstOrDefaultAsync();
            return result;
        }
        public async Task UpdateAddressAsync(Address address)
        {
            var filter = Builders<Address>.Filter.Eq(a => a.Id, address.Id);
            var result = await _addressCollection.ReplaceOneAsync(filter, address);
        }
        public async Task DeleteAddressAsync(Address address)
        {
            var filter = Builders<Address>.Filter.Eq(a => a.Id, address.Id);
            var result = await _addressCollection.DeleteOneAsync(filter);
        }

        public async Task CreateCompanyAsync(Company company)
        {
            await _companyCollection.InsertOneAsync(company);
        }
        public async Task<List<Company>> FindAllCompaniesAsync()
        {
            var filter = Builders<Company>.Filter.Empty;
            var result = await _companyCollection.Find(filter).ToListAsync();
            return result;
        }
        public async Task<Company> FindCompanyByIdAsync(string id)
        {
            var filter = Builders<Company>.Filter.Eq(a => a.Id, id);
            var result = await _companyCollection.Find(filter).FirstOrDefaultAsync();
            return result;
        }
        public async Task<Company> FindCompanyByNameAsync(string name)
        {
            var filter = Builders<Company>.Filter.Eq(a => a.Name, name);
            var result = await _companyCollection.Find(filter).FirstOrDefaultAsync();
            return result;
        }
        public async Task UpdateCompanyAsync(Company company)
        {
            var filter = Builders<Company>.Filter.Eq(a => a.Id, company.Id);
            var result = await _companyCollection.ReplaceOneAsync(filter, company);
        }
        public async Task DeleteCompanyAsync(Company company)
        {
            var filter = Builders<Company>.Filter.Eq(a => a.Id, company.Id);
            var result = await _companyCollection.DeleteOneAsync(filter);
        }
        public async Task AddEmployeeToCompanyAsync(Company company, Contact contact)
        {
            var check = company.Employees.FirstOrDefault(e => e.Email == contact.Email);
            if (check == null)
            {
                company.Employees.Add(contact);
                await UpdateCompanyAsync(company);
            }
        }
        public async Task RemoveEmployeeFromCompanyAsync(Company company, Contact contact)
        {
            var check = company.Employees.FirstOrDefault(e => e.Email == contact.Email);
            if (check != null)
            {
                company.Employees.Remove(check);
                await UpdateCompanyAsync(company);
            }
        }
        public async Task FindEmployeeByEmailAsync(string email)
        {

        }
    }
}
