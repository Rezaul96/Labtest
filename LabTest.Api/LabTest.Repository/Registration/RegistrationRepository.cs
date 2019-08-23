using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LabTest.Models;
using LabTest.Repository.Core;
using Microsoft.EntityFrameworkCore;

namespace LabTest.Repository.Registration
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private LabTestDbContext _dbContext;

        public RegistrationRepository(LabTestDbContext context)
        {
            _dbContext = context;
        }
        public async Task<List<Models.Registration>> Get()
        {
            return await _dbContext.Registrations.ToListAsync();
        }

        public async Task<Models.Registration> GetByIdAysnc(int Id)
        {
            return await _dbContext.Registrations.FindAsync(Id);
        }

        public async Task<int> Insert(Models.Registration registration)
        {
            _dbContext.Add(registration);
              await _dbContext.SaveChangesAsync();
            return registration.Id;
        }

        public async Task<int> Update(Models.Registration registration)
        {
            _dbContext.Entry(registration).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return registration.Id;
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext?.Dispose();
                }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

      
    }
}
