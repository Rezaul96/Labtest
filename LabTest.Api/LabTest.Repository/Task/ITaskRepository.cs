using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LabTest.Repository.Task
{
   public interface ITaskRepository :IDisposable
    {
        Task<List<Models.Task>> Get(int? assainedId);
        Task<Models.Task> GetByIdAysnc(int Id);
        Task<int> Insert(Models.Task task);
        Task<int> Update(Models.Task task);
        Task<bool> Delete(int Id);
    }
}
