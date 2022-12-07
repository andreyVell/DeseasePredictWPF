using System.Collections.Generic;
using System.Drawing;

namespace DeseasePredictWPF
{
    public interface IState
    {
        IState NextState(Dictionary<string, double> chances);
        void WriteCaseInStatistic(DailyStatistics cDailyStatistics);
        Color Color { get; }
        int TimeToNextStep { get; }
        double ChanceToInfect { get; }
    }
}
