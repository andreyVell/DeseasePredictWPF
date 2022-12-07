using DeseasePredictWPF.Interfaces;
using DeseasePredictWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeseasePredictWPF.Models
{
    public class NeighborhoodsGenerator
    {
        private TopologyTypeVM _topologyType;
        private NeighborhoodTypeVM _neighborhoodType;
        private bool _leftRightIsolation;
        private bool _upDownIsolation;
        private bool _forwardBackIsolation;

        public NeighborhoodsGenerator(TopologyTypeVM topologyType, 
            NeighborhoodTypeVM neighborhoodType, 
            bool leftRightIsolation, 
            bool upDownIsolation, 
            bool forwardBackIsolation)
        {
            _topologyType = topologyType;
            _neighborhoodType = neighborhoodType;
            _leftRightIsolation = leftRightIsolation;
            _upDownIsolation = upDownIsolation;
            _forwardBackIsolation = forwardBackIsolation;
        }

        public List<Cell> GetNeighborhoods(CellularAutomat automat, int currCellHeight = 0, int currCellWidth = 0, int currCellLength = 0)
        {
            var neighborhoods = new List<Cell>();

            switch (_topologyType.Type)
            {
                case ETopologyType.Square:
                    switch (_neighborhoodType.NeighborhoodType)
                    {
                        case ENeighborhoodType.Cell4:
                            //Верхний
                            if (currCellHeight == automat.Height - 1)
                            { 
                                if (_upDownIsolation)
                                    neighborhoods.Add(automat[0, currCellWidth]); 
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight + 1, currCellWidth]);

                            //Нижний
                            if (currCellHeight == 0)
                            {
                                if (_upDownIsolation)
                                    neighborhoods.Add(automat[automat.Height - 1, currCellWidth]);
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight - 1, currCellWidth]);

                            //Правый
                            if (currCellWidth == automat.Width - 1)
                            {
                                if (_leftRightIsolation)
                                    neighborhoods.Add(automat[currCellHeight, 0]);
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight, currCellWidth + 1]);

                            //Левый
                            if (currCellWidth == 0)
                            {
                                if (_leftRightIsolation)
                                    neighborhoods.Add(automat[currCellHeight, automat.Width - 1]);
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight, currCellWidth - 1]);                            
                            
                            break;
                        case ENeighborhoodType.Cell8:
                            //Верхний
                            if (currCellHeight == automat.Height - 1)
                            {
                                if (_upDownIsolation)
                                    neighborhoods.Add(automat[0, currCellWidth]);
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight + 1, currCellWidth]);

                            //Нижний
                            if (currCellHeight == 0)
                            {
                                if (_upDownIsolation)
                                    neighborhoods.Add(automat[automat.Height - 1, currCellWidth]);
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight - 1, currCellWidth]);

                            //Правый
                            if (currCellWidth == automat.Width - 1)
                            {
                                if (_leftRightIsolation)
                                    neighborhoods.Add(automat[currCellHeight, 0]);
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight, currCellWidth + 1]);

                            //Левый
                            if (currCellWidth == 0)
                            {
                                if (_leftRightIsolation)
                                    neighborhoods.Add(automat[currCellHeight, automat.Width - 1]);
                            }
                            else
                                neighborhoods.Add(automat[currCellHeight, currCellWidth - 1]);

                            //Левый вверхний
                            if (currCellHeight==automat.Height-1)
                            {
                                if (currCellWidth==0)
                                {
                                    if (_leftRightIsolation && _upDownIsolation)
                                        neighborhoods.Add(automat[0,automat.Width-1]);
                                }
                                else
                                {
                                    if (_upDownIsolation)
                                        neighborhoods.Add(automat[0, currCellWidth - 1]);
                                }
                            }
                            else
                            {
                                if (currCellWidth == 0)
                                {
                                    if (_leftRightIsolation)
                                        neighborhoods.Add(automat[currCellHeight + 1, automat.Width - 1]);
                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellHeight + 1, currCellWidth - 1]);
                                }
                            }
                            //Правый вверхний
                            if (currCellHeight == automat.Height-1)
                            {
                                if (currCellWidth==automat.Width-1)
                                {
                                    if (_leftRightIsolation && _upDownIsolation)
                                        neighborhoods.Add(automat[0, 0]);
                                }
                                else
                                {
                                    if (_upDownIsolation)
                                        neighborhoods.Add(automat[0,currCellWidth + 1]);
                                }
                            }
                            else
                            {
                                if (currCellWidth == automat.Width - 1)
                                {
                                    if (_leftRightIsolation)
                                        neighborhoods.Add(automat[currCellHeight + 1, 0]);
                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellHeight + 1, currCellWidth + 1]);
                                }
                            }

                            //Левый нижний
                            if (currCellHeight == 0)
                            {
                                if (currCellWidth == 0)
                                {
                                    if (_leftRightIsolation && _upDownIsolation)
                                        neighborhoods.Add(automat[automat.Height - 1, automat.Width - 1]);
                                }
                                else
                                {
                                    if (_upDownIsolation)
                                        neighborhoods.Add(automat[automat.Height - 1, currCellWidth - 1]);
                                }
                            }
                            else
                            {
                                if (currCellWidth == 0)
                                {
                                    if (_leftRightIsolation)
                                        neighborhoods.Add(automat[currCellHeight - 1, automat.Width - 1]);
                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellHeight - 1, currCellWidth - 1]);
                                }
                            }

                            //Правый нижний
                            if (currCellHeight == 0)
                            {
                                if (currCellWidth == automat.Width-1)
                                {
                                    if (_upDownIsolation && _leftRightIsolation)
                                        neighborhoods.Add(automat[automat.Height - 1, 0]);
                                }
                                else
                                {
                                    if (_upDownIsolation)
                                        neighborhoods.Add(automat[automat.Height - 1, currCellWidth + 1]);
                                }
                            }
                            else
                            {
                                if (currCellWidth == automat.Width - 1)
                                {
                                    if (!_leftRightIsolation)
                                        neighborhoods.Add(automat[currCellHeight - 1, 0]);
                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellHeight - 1, currCellWidth + 1]);
                                }
                            }

                            break;
                        default:
                            throw new Exception("Ошибка во время формирования окрестности");
                    }
                    break;
                case ETopologyType.Line:
                    switch (_neighborhoodType.NeighborhoodType)
                    {
                        case ENeighborhoodType.Cell2:
                            //Правый
                            if (currCellWidth == automat.Width-1)
                            {
                                if (_leftRightIsolation)
                                    neighborhoods.Add(automat[0]);
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellWidth + 1]);
                            }

                            //Левый
                            if (currCellWidth == 0)
                            {
                                if (_leftRightIsolation)
                                    neighborhoods.Add(automat[automat.Width-1]);
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellWidth - 1]);
                            }
                            break;
                        case ENeighborhoodType.Cell4:
                            //Правый + //Правый правый
                            if (currCellWidth == automat.Width - 1)
                            {
                                if (_leftRightIsolation)
                                { 
                                    neighborhoods.Add(automat[0]);
                                    neighborhoods.Add(automat[1]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellWidth + 1]);
                                if (currCellWidth == automat.Width - 2)
                                {
                                    if (_leftRightIsolation)
                                        neighborhoods.Add(automat[0]);
                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellWidth + 2]);
                                }
                            }

                            //Левый + //Левый левый
                            if (currCellWidth == 0)
                            {
                                if (_leftRightIsolation)
                                { 
                                    neighborhoods.Add(automat[automat.Width - 1]);
                                    neighborhoods.Add(automat[automat.Width - 2]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellWidth - 1]);
                                if (currCellWidth == 1)
                                {
                                    if (_leftRightIsolation)
                                        neighborhoods.Add(automat[automat.Width - 1]);
                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellWidth - 2]);
                                }
                            }
                            
                            break;
                        default:
                            throw new Exception("Ошибка во время формирования окрестности");
                    }
                    break;
                case ETopologyType.Cube:
                    switch (_neighborhoodType.NeighborhoodType)
                    {
                        case ENeighborhoodType.Cell6:
                            //Нижний
                            if (currCellHeight == 0)
                            {
                                if (_upDownIsolation)
                                {
                                    neighborhoods.Add(automat[automat.Height-1, currCellWidth, currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight - 1, currCellWidth, currCellLength]);
                            }

                            //Верхний
                            if(currCellHeight == automat.Height - 1)
                            {
                                if (_upDownIsolation)
                                {
                                    neighborhoods.Add(automat[0, currCellWidth, currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight + 1, currCellWidth, currCellLength]);
                            }

                            //Слева
                            if (currCellWidth == 0)
                            {
                                if (_leftRightIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight, automat.Width - 1, currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth - 1, currCellLength]);
                            }

                            //Справа
                            if (currCellWidth == automat.Width - 1)
                            {
                                if (_leftRightIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight,0,currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth+1, currCellLength]);
                            }
                            //Спереди
                            if (currCellLength == automat.Length-1)
                            {
                                if (_forwardBackIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight, currCellWidth, 0]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth, currCellLength+1]);
                            }

                            //Сзади
                            if (currCellLength == 0)
                            {
                                if (_forwardBackIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight, currCellWidth, automat.Length-1]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth, currCellLength-1]);
                            }
                            break;
                        case ENeighborhoodType.Cell26:
                            //Нижний
                            if (currCellHeight == 0)
                            {
                                if (_upDownIsolation)
                                {
                                    neighborhoods.Add(automat[automat.Height - 1, currCellWidth, currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight - 1, currCellWidth, currCellLength]);
                            }

                            //Верхний
                            if (currCellHeight == automat.Height - 1)
                            {
                                if (_upDownIsolation)
                                {
                                    neighborhoods.Add(automat[0, currCellWidth, currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight + 1, currCellWidth, currCellLength]);
                            }

                            //Слева
                            if (currCellWidth == 0)
                            {
                                if (_leftRightIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight, automat.Width - 1, currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth - 1, currCellLength]);
                            }

                            //Справа
                            if (currCellWidth == automat.Width - 1)
                            {
                                if (_leftRightIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight, 0, currCellLength]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth + 1, currCellLength]);
                            }
                            //Спереди
                            if (currCellLength == automat.Length - 1)
                            {
                                if (_forwardBackIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight, currCellWidth, 0]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth, currCellLength + 1]);
                            }

                            //Сзади
                            if (currCellLength == 0)
                            {
                                if (_forwardBackIsolation)
                                {
                                    neighborhoods.Add(automat[currCellHeight, currCellWidth, automat.Length - 1]);
                                }
                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth, currCellLength - 1]);
                            }
                            break;
                        default:
                            throw new Exception("Ошибка во время формирования окрестности");
                    }
                    break;
                case ETopologyType.Triangle:
                    switch (_neighborhoodType.NeighborhoodType)
                    {
                        case ENeighborhoodType.Cell3:
                            //слева
                            if (currCellWidth == 0)
                            {

                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth-1]);
                            }
                            //справа
                            if (currCellWidth == currCellHeight*2)
                            {

                            }
                            else
                            {
                                neighborhoods.Add(automat[currCellHeight, currCellWidth + 1]);
                            }
                            //снизу или сверху (зависит от местоположения ячейки)
                            if (currCellWidth % 2 == 0)
                            {
                                //Треугольничек смотрих вверх, следовательно брать нижнего соседа
                                if (currCellHeight == automat.Height - 1)
                                {

                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellHeight+1,currCellWidth+1]);
                                }
                            }
                            else
                            {
                                //Треугольничек смотрит вниз, следовательно брать вверхнего соседа
                                if (currCellHeight == 0)
                                {

                                }
                                else
                                {
                                    neighborhoods.Add(automat[currCellHeight-1,currCellWidth-1]);
                                }
                            }
                            break;
                        default:
                            throw new Exception("Ошибка во время формирования окрестности");
                    }
                    break;
                default:
                    throw new Exception("Ошибка во время формирования окрестности");
            }            

            return neighborhoods;
        }
    }
}
