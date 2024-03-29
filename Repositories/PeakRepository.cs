using HighwayToPeak.Models.Contracts;
using HighwayToPeak.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Repositories
{
    public class PeakRepository : IRepository<IPeak>
    {
        private HashSet<IPeak> allPeaks;

        public PeakRepository()
        {
            allPeaks = new HashSet<IPeak>();
        }
        public IReadOnlyCollection<IPeak> All => allPeaks;

        public void Add(IPeak model)
        {
            allPeaks.Add(model);
        }

        public IPeak Get(string name)
        {
            return allPeaks.FirstOrDefault(p => p.Name == name);
        }
    }
}
