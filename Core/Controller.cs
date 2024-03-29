using HighwayToPeak.Core.Contracts;
using HighwayToPeak.Models;
using HighwayToPeak.Models.Contracts;
using HighwayToPeak.Repositories;
using HighwayToPeak.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighwayToPeak.Core
{
    public class Controller : IController
    {
        private IRepository<IPeak> peaks;
        private IRepository<IClimber> climbers;
        private IBaseCamp baseCamp;

        public Controller()
        {
            peaks = new PeakRepository();
            climbers = new ClimberRepository();
            baseCamp = new BaseCamp();
        }
        public string AddPeak(string name, int elevation, string difficultyLevel)
        {
            if (peaks.Get(name) != null)
            {
                return $"{name} is already added as a valid mountain destination.";
            }

            if (difficultyLevel is not ("Extreme" or "Hard" or "Moderate"))
            {
                return $"{difficultyLevel} peaks are not allowed for international climbers.";
            }

            IPeak peak = new Peak(name, elevation, difficultyLevel);
            peaks.Add(peak);

            return $"{name} is allowed for international climbing. See details in {peaks.GetType().Name}.";
        }

        public string AttackPeak(string climberName, string peakName)
        {
            IClimber currentClimber;
            IPeak currentPeak;

            if ((currentClimber = climbers.Get(climberName)) == null)
            {
                return $"Climber - {climberName}, has not arrived at the BaseCamp yet.";
            }

            if ((currentPeak = peaks.Get(peakName)) == null)
            {
                return $"{peakName} is not allowed for international climbing.";
            }

            if (!baseCamp.Residents.Contains(climberName))
            {
                return $"{climberName} not found for gearing and instructions. The attack of {peakName} will be postponed.";
            }

            if (currentClimber.GetType().Name == "NaturalClimber" && currentPeak.DifficultyLevel == "Extreme")
            {
                return $"{climberName} does not cover the requirements for climbing {peakName}.";
            }

            baseCamp.LeaveCamp(climberName);
            currentClimber.Climb(currentPeak);

            if (currentClimber.Stamina == 0)
            {
                return $"{climberName} did not return to BaseCamp.";
            }

            baseCamp.ArriveAtCamp(currentClimber.Name);
            return $"{climberName} successfully conquered {peakName} and returned to BaseCamp.";
        }

        public string BaseCampReport()
        {
            if (baseCamp.Residents.Count == 0)
            {
                return "BaseCamp is currently empty.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("BaseCamp residents:");
            foreach (var resident in baseCamp.Residents)
            {
                var currentResident = climbers.All.FirstOrDefault(c => c.Name == resident);
                if (currentResident != null)
                {
                    sb.AppendLine($"Name: {currentResident.Name}, Stamina: {currentResident.Stamina}, Count of Conquered Peaks: {currentResident.ConqueredPeaks.Count}");
                }
            }


            return sb.ToString().TrimEnd();
        }

        public string CampRecovery(string climberName, int daysToRecover)
        {
            if (!baseCamp.Residents.Contains(climberName))
            {
                return $"{climberName} not found at the BaseCamp.";
            }

            IClimber climber = climbers.Get(climberName);
            if (climber.Stamina == 10)
            {
                return $"{climberName} has no need of recovery.";
            }

            climber.Rest(daysToRecover);
            return $"{climberName} has been recovering for {daysToRecover} days and is ready to attack the mountain.";
        }

        public string NewClimberAtCamp(string name, bool isOxygenUsed)
        {
            if (climbers.Get(name) is not null)
            {
                return $"{name} is a participant in {climbers.GetType().Name} and cannot be duplicated.";
            }

            IClimber climber;
            if (isOxygenUsed)
            {
                climber = new OxygenClimber(name);
            }
            else
            {
                climber = new NaturalClimber(name);
            }

            climbers.Add(climber);
            baseCamp.ArriveAtCamp(name);

            return $"{climber.Name} has arrived at the BaseCamp and will wait for the best conditions.";
        }

        public string OverallStatistics()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("***Highway-To-Peak***");
            var orderedClimbers = climbers.All
                .OrderByDescending(c => c.ConqueredPeaks.Count)
                .ThenBy(c => c.Name);

            foreach (var climber in orderedClimbers)
            {
                sb.AppendLine(climber.ToString());
                List<IPeak> listOfPeaks = new List<IPeak>();
                foreach (var peakName in climber.ConqueredPeaks)
                {
                    var peak = peaks.Get(peakName);
                    listOfPeaks.Add(peak);
                }
                var sortedListOfPeaks = listOfPeaks.OrderByDescending(p => p.Elevation);
                foreach (var peak in sortedListOfPeaks)
                {
                    sb.AppendLine(peak.ToString());
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}
