using System;
using System.Collections.Generic;
using System.Drawing;

namespace DeseasePredictWPF.States
{
    public class CHealthyWithoutImmunity : IState
    {
        static private Random _randomChance;
        static private int _timeToNextStep;
        static private Color _color;
        static private double _chanceToInfect;

        public CHealthyWithoutImmunity(Dictionary<string, int> timeInSteps, Dictionary<string, Color> colors, double chanceToInfect)
        {            
            _randomChance = new Random();
            _timeToNextStep = timeInSteps["TH1"];
            _color = colors["H1Color"];
            _chanceToInfect = chanceToInfect;
        }
        public CHealthyWithoutImmunity() { }

        public Color Color => _color;

        public int TimeToNextStep => _timeToNextStep;

        public double ChanceToInfect => _chanceToInfect;

        public IState NextState(Dictionary<string, double> chances)
        {
            double _chanceToSickEasyIllness = chances["alpha"];
            double _chanceToHealthyWithAcquiredImmunity = chances["dzeta"];
            double x = _randomChance.NextDouble();

            if (0 < x && x <= _chanceToSickEasyIllness)
                return new CSickEasyIllness();
            if (_chanceToSickEasyIllness < x && x <= _chanceToSickEasyIllness + _chanceToHealthyWithAcquiredImmunity) 
                return new CHealthyWithAcquiredImmunity();
            return this;
        }

        public void WriteCaseInStatistic(DailyStatistics cDailyStatistics)
        {
            cDailyStatistics.HealthyWithoutImmunityCount++;
        }
    }
}