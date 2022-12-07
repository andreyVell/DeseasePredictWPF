using DeseasePredictWPF.Interfaces;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeseasePredictWPF.ViewModel
{
    public class NeighborhoodTypeVM:ViewModelBase
    {
        private string _displayName;
        private ENeighborhoodType _neighborhoodType;

        public NeighborhoodTypeVM(ENeighborhoodType neighborhoodType)
        {
            _neighborhoodType = neighborhoodType;
            switch (neighborhoodType)
            {
                case ENeighborhoodType.Cell2:
                    _displayName = "2 клетки";
                    break;
                case ENeighborhoodType.Cell3:
                    _displayName = "3 клетки";
                    break;
                case ENeighborhoodType.Cell4:
                    _displayName = "4 клетки";
                    break;
                case ENeighborhoodType.Cell6:
                    _displayName = "6 клеткок";
                    break;
                case ENeighborhoodType.Cell8:
                    _displayName = "8 клеток";
                    break;
                case ENeighborhoodType.Cell26:
                    _displayName = "26 клеток";
                    break;
                default:
                    _displayName = string.Empty;
                    break;
            }
        }

        public string DisplayName { get => _displayName; set => _displayName = value; }
        public ENeighborhoodType NeighborhoodType { get => _neighborhoodType; set => _neighborhoodType = value; }
    }
}
