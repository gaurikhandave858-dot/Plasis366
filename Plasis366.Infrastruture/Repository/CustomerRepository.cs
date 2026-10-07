using System;
using System.Collections.Generic;
using System.Text;
using Plasis366.Domain;
using Microsoft.EntityFrameworkCore;

namespace Plasis366.Infrastructure
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(long customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customerId &&
                    x.IsActive);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            var existingCustomer =
                await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customer.CustomerId);

            if (existingCustomer == null)
                return customer;

            existingCustomer.TenantId = customer.TenantId;
            existingCustomer.UserId = customer.UserId;
            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.Address = customer.Address;
            existingCustomer.IsActive = customer.IsActive;

            await _context.SaveChangesAsync();

            return existingCustomer;
        }

        public async Task<bool> DeleteAsync(long customerId)
        {
            var customer =
                await _context.Customers
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customerId);

            if (customer == null)
                return false;

            customer.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
