using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CDGService.Data.Store;

namespace CDGService.Store.Store
{
    internal class DataFactory : IDataFactory
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

 
    }
}
