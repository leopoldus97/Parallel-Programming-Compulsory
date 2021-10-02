using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUI
{
    public class PrimeGenerator : IPrimeGenerator
    {
        // 1. Sequential implementation for the GetPrime function.
        public Task<List<long>> GetPrimesSequential(long first, long last)
        {
            return Task.Factory.StartNew(() =>
            {
                List<long> primeList = new List<long>();
                for (long i = first; i <= last; i++)
                {
                    if (IsPrime(i))
                    {
                        primeList.Add(i);
                    }
                }
                return primeList;
            });
        }

        // 2. Parallel implementation for the GetPrime function.
        public Task<List<long>> GetPrimesParallel(long first, long last)
        {
            return Task.Factory.StartNew(() =>
            {
                List<long> primeList = new List<long>();
                object lockObject = new List<long>();
                Parallel.ForEach(Partitioner.Create(first, last + 1), range =>
                {
                    var localPrimeList = new List<long>();
                    for (long i = range.Item1; i < range.Item2; i++)
                    {
                        if (IsPrime(i))
                            localPrimeList.Add(i);
                    }
                    // 2. Locking for solving the race-condition.
                    lock (lockObject)
                    {
                        primeList.AddRange(localPrimeList);
                    }
                });
                return primeList.AsParallel().OrderBy(num => num).ToList();
            });
        }

        public Task<List<long>> GetPrimesSequentialFromArray(long first, long last) =>
            Task.Factory.StartNew(() => FillNumberArray(first, last)
            .Where(num => IsPrime(num))
            .ToList());

        public Task<List<long>> GetPrimesParallelFromArray(long first, long last)
        {
            return Task.Factory.StartNew(() =>
            {
                IEnumerable<long> numList = FillNumberArray(first, last);
                ConcurrentBag<long> primeList = new ConcurrentBag<long>();
                Parallel.ForEach(numList, number =>
                {
                    if (IsPrime(number))
                        primeList.Add(number);
                });
                return primeList.AsParallel().OrderBy(num => num).ToList();
            });
        }

        private bool IsPrime(long number)
        {
            if (number == 1)
            {
                return false;
            }

            if (number % 2 == 0 && number != 2)
            {
                return false;
            }

            for (double divisor = 2; divisor <= Math.Sqrt(number); divisor++)
            {
                if (number % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<long> FillNumberArray(long first, long last)
        {
            if (first < 1)
                throw new ArgumentOutOfRangeException("Starting number has to be bigger than 1!");
            if (last < 1 || last < first)
                throw new ArgumentOutOfRangeException("Ending number has to be bigger than 1 and the starting number!");
            List<long> numberArray = new List<long>();
            object lockObject = new object();
            Parallel.ForEach(Partitioner.Create(first, last + 1), range =>
            {
                List<long> localList = new List<long>();
                for (long i = range.Item1; i < range.Item2; i++)
                {
                    localList.Add(i);
                }
                lock (lockObject)
                {
                    numberArray.AddRange(localList);
                }

            });
            return numberArray;
        }
    }
}
