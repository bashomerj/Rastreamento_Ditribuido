using System.Threading.Tasks;

namespace SEG.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();

        void BeginTran();

        void CommitTran();

        void RollbackTran();

        //Task<bool> DetachAllEntities();
    }

   
}