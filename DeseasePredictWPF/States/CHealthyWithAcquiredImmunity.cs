using System;
using System.Collections.Generic;
using System.Drawing;

namespace DeseasePredictWPF.States
{
    public class CHealthyWithAcquiredImmunity : IState
    {
        static private Random _randomChance;
        static private int _timeToNextStep;
        static private Color _color;
        static private double _chanceToInfect;

        public CHealthyWithAcquiredImmunity(Dictionary<string, int> timeInSteps, Dictionary<string, Color> colors, double chanceToInfect)
        {
            _randomChance = new Random();
            _timeToNextStep = timeInSteps["TH3"];
            _color = colors["H3Color"];
            _chanceToInfect = chanceToInfect;
        }
        public CHealthyWithAcquiredImmunity() { }

        public Color Color => _color;

        public int TimeToNextStep => _timeToNextStep;

        public double ChanceToInfect => _chanceToInfect;

        public IState NextState(Dictionary<string, double> chances)
        {
            return new CHealthyWithoutImmunity();
        }

        public void WriteCaseInStatistic(DailyStatistics cDailyStatistics)
        {
            cDailyStatistics.HealthyWithAcquiredImmunityCount++;
        }
    }
}
