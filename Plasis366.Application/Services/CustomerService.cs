using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Plasis366.Infrastructure;

namespace Plasis366.Application
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(long customerId)
        {
            return await _repository.GetByIdAsync(customerId);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            return await _repository.CreateAsync(customer);
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            return await _repository.UpdateAsync(customer);
        }

        public async Task<bool> DeleteAsync(long customerId)
        {
            return await _repository.DeleteAsync(customerId);
        }
    }
}
