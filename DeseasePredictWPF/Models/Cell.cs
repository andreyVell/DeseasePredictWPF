using DeseasePredictWPF.Interfaces;
using DeseasePredictWPF.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace DeseasePredictWPF
{
    public class Cell
    {
        private int _curHeight;
        private int _curWidth;
        private int _curLength;
        private IState _state;
        private List<Cell> _neighborСells;
        private Dictionary<string, double> _stateSwitchChances;
        private int _timeToNextStep;
        
        public Cell(int x, int y, IState state, Dictionary<string, double> stateSwitchChances, int z = 0)
        {
            _curHeight = x;
            _curWidth = y;
            _curLength = z;
            State = state;
            _stateSwitchChances = stateSwitchChances;
        }        

        public void WriteCaseInStatistic(DailyStatistics cDailyStatistics) => State.WriteCaseInStatistic(cDailyStatistics);

        public Color Color => State.Color;

        public double ChanceToInfect => State.ChanceToInfect;

        public IState State 
        {
            get { return _state; } 
            set 
            {
                //обновлять время только если меняется состояние
                if (_state?.GetType() != value.GetType())
                    _timeToNextStep = value.TimeToNextStep; 
                _state = value; 
            }
        }

        public IState NextStep()
        {
            if (--_timeToNextStep <= 0)
                return State.NextState(FindCurrrentChances()); 
            return State;
        }
        public void FormerNeighborСellsList(CellularAutomat automat, NeighborhoodsGenerator neighborhoodsGenerator)
        {
            //Формируем список соседей клетки 
            var neighborhoods = neighborhoodsGenerator.GetNeighborhoods(automat, _curHeight, _curWidth, _curLength);
            _neighborСells = new List<Cell>();
            foreach (var cell in neighborhoods)
                _neighborСells.Add(cell);
        }

        private Dictionary<string, double> FindCurrrentChances()
        {
            //Формируем вероятности перехода исходя из состояний соседей            
            Dictionary<string, double> curChances = new Dictionary<string, double>(_stateSwitchChances);
            //вычислять только альфа
            double alpha = 0;
            foreach (var cell in _neighborСells)
                alpha += cell.ChanceToInfect;
            if (alpha + curChances["dzeta"] > 1)
                alpha = 1 - curChances["dzeta"];

            curChances["alpha"] = alpha;
            return curChances;
        }
        
    }
}