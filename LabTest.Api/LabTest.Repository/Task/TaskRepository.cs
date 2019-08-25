using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabTest.Models;
using LabTest.Repository.Core;
using Microsoft.EntityFrameworkCore;

namespace LabTest.Repository.Task
{
    public class TaskRepository : ITaskRepository
    {
        private LabTestDbContext _dbContext;

        public TaskRepository(LabTestDbContext context)
        {
            _dbContext = context;
        }
        public async Task<bool> Delete(int Id)
        {
            var task = await _dbContext.Tasks.FindAsync(Id);
            _dbContext.Tasks.Remove(task);
               await  _dbContext.SaveChangesAsync();
            return true;
        }
     
    public async Task<List<Models.Task>> Get(int? assainedId)
        {
            if(assainedId.HasValue)
                return await _dbContext.Tasks.Include(t => t.Registration).Where(t => t.AssainedTo ==  assainedId).ToListAsync();
            else
               return  await _dbContext.Tasks.Include(t=>t.Registration).ToListAsync();
            
        }

        public async Task<Models.Task> GetByIdAysnc(int Id)
        {
            return await _dbContext.Tasks.FindAsync(Id);
        }

        public async Task<int> Insert(Models.Task task)
        {
            _dbContext.Add(task);
            await _dbContext.SaveChangesAsync();
            return task.TaskId;
        }

        public async Task<int> Update(Models.Task task)
        {
            _dbContext.Entry(task).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return task.TaskId;
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
