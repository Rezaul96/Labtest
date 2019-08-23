using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LabTest.Repository.Registration
{
   public interface IRegistrationRepository : IDisposable
    {
        Task<List<Models.Registration>> Get();
        Task<Models.Registration> GetByIdAysnc(int Id);
        Task<int> Insert(Models.Registration registration);
        Task<int> Update(Models.Registration registration);
        Task<Models.Registration> GetByEmailAysnc(string Email);
    }
}
