using System.Collections.Generic;
using System.Threading.Tasks;

namespace GUI
{
    public interface IPrimeGenerator
    {
        Task<List<long>> GetPrimesSequential(long first, long last);
        Task<List<long>> GetPrimesParallel(long first, long last);
        Task<List<long>> GetPrimesSequentialFromArray(long first, long last);
        Task<List<long>> GetPrimesParallelFromArray(long first, long last);
    }
}
