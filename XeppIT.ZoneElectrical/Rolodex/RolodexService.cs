using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using XeppIT.ZoneElectrical.Rolodex.Addresses.Model;
using XeppIT.ZoneElectrical.Rolodex.Companies.Model;
using XeppIT.ZoneElectrical.Rolodex.Contacts.Model;

namespace XeppIT.ZoneElectrical.Rolodex
{
    public class RolodexService
    {
        private readonly IMongoCollection<Address> _addressCollection;

        private readonly IMongoCollection<Company> _companyCollection;

        private readonly IMongoCollection<Contact> _contactCollection;
        // Create
        // Read
        // Update
        // Delete

        public RolodexService(IMongoCollection<Address> addressCollection, IMongoCollection<Company> companyCollection, IMongoCollection<Contact> contactCollection)
        {
            _addressCollection = addressCollection;
            _companyCollection = companyCollection;
            _contactCollection = contactCollection;
        }

        public IMongoQueryable<T> GetFiltered<T>(IMongoCollection<T> collection, Expression<Func<T, bool>> predicate)
        {
            return collection.AsQueryable()
                .Where(predicate);
        }

        #region Address
        public async Task<bool> CreateAddressAsync(Address address)
        {
            address.Name = address.Name.Trim();
            address.Street = address.Street.Trim();
            address.Town = address.Town.Trim();
            address.Postcode = address.Postcode.Trim();

            try
            {
                await _addressCollection.InsertOneAsync(address);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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

        // Todo Needs refactoring badly
        public async Task<List<Address>> FindAllAddressesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await FindAllAddressesAsync();

            var result = await GetFiltered(_addressCollection,
                _ => _.Name.ToLower().Contains(name.ToLower())).ToListAsync();
            return result;
        }
        // Todo make this into generic service or static extenstion


        public async Task<ReplaceOneResult> UpdateAddressAsync(Address address)
        {
            address.Name = address.Name.Trim();
            address.Street = address.Street.Trim();
            address.Town = address.Town.Trim();
            address.Postcode = address.Postcode.Trim();

            var filter = Builders<Address>.Filter.Eq(a => a.Id, address.Id);

            return await _addressCollection.ReplaceOneAsync(filter, address);
        }
        public async Task DeleteAddressAsync(Address address)
        {
            var filter = Builders<Address>.Filter.Eq(a => a.Id, address.Id);
            var result = await _addressCollection.DeleteOneAsync(filter);
        }
        #endregion

        #region Company
        public async Task<bool> CreateCompanyAsync(Company company)
        {
            company.Name = company.Name.Trim();

            try
            {
                await _companyCollection.InsertOneAsync(company);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public async Task<List<Company>> FindAllCompaniesAsync()
        {
            var filter = Builders<Company>.Filter.Empty;
            var result = await _companyCollection.Find(filter).ToListAsync();
            var orderBy = result.OrderBy(c => c.Name);
            return orderBy.ToList();
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

        // Todo Needs refactoring badly
        public async Task<List<Company>> FindAllCompaniesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await FindAllCompaniesAsync();

            var result = await GetFilteredCompany(
                _ => _.Name.ToLower().Contains(name.ToLower())).ToListAsync();
            return result;
        }
        // Todo make this into generic service or static extenstion
        public IMongoQueryable<Company> GetFilteredCompany(Expression<Func<Company, bool>> predicate)
        {
            return _companyCollection.AsQueryable()
                .Where(predicate);
        }
        public async Task<bool> CompanyNameExistsAsync(string name)
        {
            var filter = Builders<Company>.Filter.Eq(a => a.Name, name);
            var result = await _companyCollection.Find(filter).FirstOrDefaultAsync();
            
            return result != null;
        }
        public async Task<ReplaceOneResult> UpdateCompanyAsync(Company company)
        {
            company.Name = company.Name.Trim();

            var filter = Builders<Company>.Filter.Eq(a => a.Id, company.Id);
            return await _companyCollection.ReplaceOneAsync(filter, company);
        }
        public async Task DeleteCompanyAsync(Company company)
        {
            var filter = Builders<Company>.Filter.Eq(a => a.Id, company.Id);
            var result = await _companyCollection.DeleteOneAsync(filter);
        }
        #endregion

        #region Contact
        public async Task<bool> CreateContactAsync(Contact contact)
        {
            contact.FirstName = contact.FirstName.Trim();
            contact.LastName = contact.LastName.Trim();
            contact.Email = contact.Email.Trim();
            contact.PhoneNumber = contact.PhoneNumber.Trim();
            contact.Company = contact.Company.Trim();

            try
            {
                await _contactCollection.InsertOneAsync(contact);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public async Task<List<Contact>> FindAllContactsAsync()
        {
            var filter = Builders<Contact>.Filter.Empty;
            var result = await _contactCollection.Find(filter).ToListAsync();
            return result;
        }
        public async Task<Contact> FindContactByIdAsync(string id)
        {
            var filter = Builders<Contact>.Filter.Eq(a => a.Id, id);
            var result = await _contactCollection.Find(filter).FirstOrDefaultAsync();
            return result;
        }
        public async Task<Contact> FindContactByEmailAsync(string email)
        {
            var filter = Builders<Contact>.Filter.Eq(a => a.Email, email);
            var result = await _contactCollection.Find(filter).FirstOrDefaultAsync();
            return result;
        }
        // Todo Needs refactoring badly
        public async Task<List<Contact>> FindAllContactsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await FindAllContactsAsync();

            var result = await GetFilteredContact(
                _ => _.Email.ToLower().Contains(name.ToLower())).ToListAsync();
            return result;
        }
        // Todo make this into generic service or static extenstion
        public IMongoQueryable<Contact> GetFilteredContact(Expression<Func<Contact, bool>> predicate)
        {
            return _contactCollection.AsQueryable()
                .Where(predicate);
        }
        public async Task<ReplaceOneResult> UpdateContactAsync(Contact contact)
        {
            contact.FirstName = contact.FirstName.Trim();
            contact.LastName = contact.LastName.Trim();
            contact.Email = contact.Email.Trim();
            contact.PhoneNumber = contact.PhoneNumber.Trim();
            contact.Company = contact.Company.Trim();

            var filter = Builders<Contact>.Filter.Eq(a => a.Id, contact.Id);
            return await _contactCollection.ReplaceOneAsync(filter, contact);
        }
        public async Task DeleteContactAsync(Contact contact)
        {
            var filter = Builders<Contact>.Filter.Eq(a => a.Id, contact.Id);
            var result = await _contactCollection.DeleteOneAsync(filter);
        } 
        #endregion
    }
}
