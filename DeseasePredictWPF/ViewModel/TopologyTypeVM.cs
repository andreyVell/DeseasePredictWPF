using DeseasePredictWPF.Interfaces;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeseasePredictWPF.ViewModel
{    
    public class TopologyTypeVM:ViewModelBase
    {
        private string _displayName;
        private ETopologyType _type;

        public TopologyTypeVM(ETopologyType type)
        {            
            _type = type;
            switch (type)
            {
                case ETopologyType.Square:
                    _displayName = "Квадрат";
                    break;
                case ETopologyType.Cube:
                    _displayName = "Куб";
                    break;
                case ETopologyType.Line:
                    _displayName = "Линия";
                    break;
                case ETopologyType.Triangle:
                    _displayName = "Треугольник";
                    break;
                default:
                    _displayName = string.Empty;
                    break;
            }
        }

        public string DisplayName { get => _displayName; set => _displayName = value; }
        public ETopologyType Type { get => _type; set => _type = value; }
    }
}
