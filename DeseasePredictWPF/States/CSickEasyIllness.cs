using System;
using System.Collections.Generic;
using System.Drawing;

namespace DeseasePredictWPF.States
{
    public class CSickEasyIllness : IState
    {
        static private Random _randomChance;
        static private int _timeToNextStep;
        static private Color _color;
        static private double _chanceToInfect;

        public CSickEasyIllness(Dictionary<string, int> timeInSteps, Dictionary<string, Color> colors, double chanceToInfect)
        {
            _randomChance = new Random();
            _timeToNextStep = timeInSteps["TP1"];
            _color = colors["P1Color"];
            _chanceToInfect = chanceToInfect;
        }
        public CSickEasyIllness() { }
        public Color Color => _color;

        public int TimeToNextStep => _timeToNextStep;

        public double ChanceToInfect => _chanceToInfect;

        public IState NextState(Dictionary<string, double> chances)
        {
            double _chanceToHealthyWithAcquiredImmunity = chances["gamma"];
            double _chanceToSickHardIllness = chances["beta"];
            double x = _randomChance.NextDouble();

            if (0 < x && x <= _chanceToSickHardIllness)
                return new CSickHardIllness();
            if (_chanceToSickHardIllness < x && x <= _chanceToSickHardIllness + _chanceToHealthyWithAcquiredImmunity)
                return new CHealthyWithAcquiredImmunity();
            return this;
        } 

        public void WriteCaseInStatistic(DailyStatistics cDailyStatistics)
        {
            cDailyStatistics.SickEasyIllnessCount++;
        }
    }
}