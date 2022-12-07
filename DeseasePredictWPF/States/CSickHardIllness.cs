using System;
using System.Collections.Generic;
using System.Drawing;

namespace DeseasePredictWPF.States
{
    public class CSickHardIllness : IState
    {
        static private Random _randomChance;
        static private int _timeToNextStep;
        static private Color _color;
        static private double _chanceToInfect;

        public CSickHardIllness(Dictionary<string, int> timeInSteps, Dictionary<string, Color> colors, double chanceToInfect)
        {
            _randomChance = new Random();
            _timeToNextStep = timeInSteps["TP2"];
            _color = colors["P2Color"];
            _chanceToInfect = chanceToInfect;
        }
        public CSickHardIllness() { }
        public Color Color => _color;

        public int TimeToNextStep => _timeToNextStep;

        public double ChanceToInfect => _chanceToInfect;

        public IState NextState(Dictionary<string, double> chances)
        {
            double _chanceToDead = chances["epsilon"];
            double _chanceToHealthyWithAcquiredImmunity = chances["delta"];
            double x = _randomChance.NextDouble();
            if (0 < x && x <= _chanceToDead)
                return new CDead();
            if (_chanceToDead < x && x <= _chanceToDead + _chanceToHealthyWithAcquiredImmunity)
                return new CHealthyWithAcquiredImmunity();
            return this;
        }

        public void WriteCaseInStatistic(DailyStatistics cDailyStatistics)
        {
            cDailyStatistics.SickHardIllnessCount++;
        }
    }
}