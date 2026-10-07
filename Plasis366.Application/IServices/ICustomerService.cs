using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;

namespace Plasis366.Application
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();

        Task<Customer?> GetByIdAsync(long customerId);

        Task<Customer> CreateAsync(Customer customer);

        Task<Customer> UpdateAsync(Customer customer);

        Task<bool> DeleteAsync(long customerId);
    }
}
