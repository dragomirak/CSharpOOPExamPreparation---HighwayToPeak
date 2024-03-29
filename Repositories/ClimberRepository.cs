using HighwayToPeak.Models.Contracts;
using HighwayToPeak.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Repositories
{
    public class ClimberRepository : IRepository<IClimber>
    {
        private HashSet<IClimber> allClimbers;

        public ClimberRepository()
        {
            allClimbers = new HashSet<IClimber>();
        }
        public IReadOnlyCollection<IClimber> All => allClimbers;

        public void Add(IClimber model)
        {
            allClimbers.Add(model);
        }

        public IClimber Get(string name)
        {
            return allClimbers.FirstOrDefault(x => x.Name == name);
        }
    }
}
