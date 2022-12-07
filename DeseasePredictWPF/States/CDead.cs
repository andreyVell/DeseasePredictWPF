using System;
using System.Collections.Generic;
using System.Drawing;

namespace DeseasePredictWPF.States
{
    public class CDead : IState
    {
        static private Random _randomChance;
        static private int _timeToNextStep;
        static private Color _color;
        static private double _chanceToInfect;

        public CDead(Dictionary<string,int> timeInSteps, Dictionary<string, Color> colors, double chanceToInfect)
        {
            _randomChance = new Random();
            _timeToNextStep = timeInSteps["TD"];
            _color = colors["DColor"];
            _chanceToInfect = chanceToInfect;
        }
        public CDead() { }

        public Color Color => _color;

        public int TimeToNextStep => _timeToNextStep;

        public double ChanceToInfect => _chanceToInfect;

        public IState NextState(Dictionary<string, double> chances)
        {
            return this;
        }

        public void WriteCaseInStatistic(DailyStatistics cDailyStatistics)
        {
            cDailyStatistics.DeadCount++;
        }
    }
}