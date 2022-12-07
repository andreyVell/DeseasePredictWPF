using DeseasePredictWPF.Interfaces;
using DeseasePredictWPF.States;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeseasePredictWPF.Models
{
    public class CellularAutomatTriangle : CellularAutomat
    {
        private int _height;
        private int _width;
        private Cell[,] _grid;
        private List<DailyStatistics> _allTimeStatistics;
        private Random random;
        private NeighborhoodsGenerator _neighborhoodsGenerator;
        private enum SmallTriangleType
        {
            Up,
            Down,
        }


        public override int Height { get => _height; set => _height = value; }

        public override int Width { get => _width; set => _width = value; }

        public CellularAutomatTriangle(int height, int width, Dictionary<string, Color> colors, Dictionary<string, int> timeInSteps, Dictionary<string, double> chancesToInfect, Dictionary<string, double> stateSwitchChances, int startIllEasyCount, int startInnateImmunityCount, NeighborhoodsGenerator neighborhoodsGenerator)
        {
            _neighborhoodsGenerator = neighborhoodsGenerator;
            Height = height;
            Width = width;
            InitializeStates(timeInSteps, colors, chancesToInfect);
            InitializeGrid(stateSwitchChances);
            FillNeighbors();
            _allTimeStatistics = new List<DailyStatistics>();
            random = new Random();
            GenerateRandomIllnessAndInnateImmunity(startIllEasyCount, startInnateImmunityCount);
            //Изначальная статистика (нулевого дня)
            DailyStatistics cDailyStatistics = new DailyStatistics();
            cDailyStatistics.SickEasyIllnessCount = startIllEasyCount;
            cDailyStatistics.HealthyWithInnateImmunityCount = startInnateImmunityCount;
            cDailyStatistics.HealthyWithoutImmunityCount = height * width - startIllEasyCount - startInnateImmunityCount;
            _allTimeStatistics.Add(cDailyStatistics);
        }
        public override Cell this[int i] => throw new Exception("Topology error!");
        public override Cell this[int i, int j] => _grid[i, j];
        public override Cell this[int i, int j, int k] => throw new Exception("Topology error!");

        private void InitializeStates(Dictionary<string, int> timeInSteps, Dictionary<string, Color> colors, Dictionary<string, double> chancesToInfect)
        {
            CDead cDead = new CDead(timeInSteps, colors, chancesToInfect["D"]);
            CHealthyWithAcquiredImmunity cHealthyWithAcquiredImmunity = new CHealthyWithAcquiredImmunity(timeInSteps, colors, chancesToInfect["H3"]);
            CHealthyWithInnateImmunity cHealthyWithInnateImmunity = new CHealthyWithInnateImmunity(timeInSteps, colors, chancesToInfect["H2"]);
            CHealthyWithoutImmunity cHealthyWithoutImmunity = new CHealthyWithoutImmunity(timeInSteps, colors, chancesToInfect["H1"]);
            CSickEasyIllness cSickEasyIllness = new CSickEasyIllness(timeInSteps, colors, chancesToInfect["P1"]);
            CSickHardIllness cSickHardIllness = new CSickHardIllness(timeInSteps, colors, chancesToInfect["P2"]);
        }

        private void InitializeGrid(Dictionary<string, double> stateSwitchChances)
        {
            _grid = new Cell[Height, Width];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < i * 2 + 1; j++) 
                    _grid[i, j] = new Cell(i, j, new CHealthyWithoutImmunity(), stateSwitchChances);
        }

        private void FillNeighbors()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < i * 2 + 1; j++)
                    _grid[i, j].FormerNeighborСellsList(this, _neighborhoodsGenerator);
        }

        public override void NextStep()
        {
            IState[,] nextStates = new IState[Height, Width];
            DailyStatistics cDailyStatistics = new DailyStatistics();
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < i * 2 + 1; j++)
                    nextStates[i, j] = _grid[i, j].NextStep();
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < i * 2 + 1; j++)
                {
                    _grid[i, j].State = nextStates[i, j];
                    _grid[i, j].WriteCaseInStatistic(cDailyStatistics);
                }
            _allTimeStatistics.Add(cDailyStatistics);
        }

        public override Bitmap MakePicture(int _PictureHeight, int _PictureWidth)
        {
            //       П
            //      ППП
            //     ППППП
            //
            //   П ППППП П
            //  ППП ППП ППП
            // ППППП П ППППП
            //
            int bmpHeight = Height * 3;
            int bmpWidth = (Width / 2 + 1) * 5 + Width / 2;
            Bitmap bmp = new Bitmap(bmpWidth,bmpHeight);
            //TODO
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                    bmp.SetPixel(i, j, Color.White);
            for (int i = 0; i < Height; i++)
            {
                int smallTriangleStartPos = (Height - i) * 3 - 1;
                for (int j = 0; j < i * 2 + 1; j++)
                {
                    int smallTrianglePosHeight = i * 3 + 1;
                    int smallTrianglePosWidth = smallTriangleStartPos + j * 3;
                    DrawTriangle(bmp, smallTrianglePosHeight, smallTrianglePosWidth, j % 2 == 0 ? SmallTriangleType.Down: SmallTriangleType.Up, _grid[i,j].Color);
                }
            }
            if (bmp.Height < _PictureHeight || bmp.Width < _PictureWidth)
                bmp = TransformPictureToSize(bmp, _PictureHeight, _PictureWidth);
            return bmp;
        }

        private Bitmap TransformPictureToSize(Bitmap oldBmp, int height, int width)
        {
            Bitmap newBmp = new Bitmap(width, height);
            double heightScale = (double)height / oldBmp.Height;
            double widthScale = (double)width / oldBmp.Width;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    newBmp.SetPixel(i, j, oldBmp.GetPixel((int)(i / widthScale), (int)(j / heightScale)));
            return newBmp;
        }

        private void DrawTriangle(Bitmap bmp, int positionHeight, int positionWidth, SmallTriangleType triangleType, Color color)
        {
            switch (triangleType)
            {
                case SmallTriangleType.Up:
                    bmp.SetPixel(positionWidth, positionHeight + 1, color);

                    bmp.SetPixel(positionWidth-1, positionHeight, color);
                    bmp.SetPixel(positionWidth, positionHeight, color);
                    bmp.SetPixel(positionWidth+1, positionHeight, color);

                    bmp.SetPixel(positionWidth - 2, positionHeight - 1, color);
                    bmp.SetPixel(positionWidth - 1, positionHeight - 1, color);
                    bmp.SetPixel(positionWidth, positionHeight - 1, color);
                    bmp.SetPixel(positionWidth + 1, positionHeight - 1, color);
                    bmp.SetPixel(positionWidth + 2, positionHeight - 1, color);
                    break;
                case SmallTriangleType.Down:
                    bmp.SetPixel(positionWidth - 2, positionHeight + 1, color);
                    bmp.SetPixel(positionWidth - 1, positionHeight + 1, color);
                    bmp.SetPixel(positionWidth, positionHeight + 1, color);
                    bmp.SetPixel(positionWidth + 1, positionHeight + 1, color);
                    bmp.SetPixel(positionWidth + 2, positionHeight + 1, color);

                    bmp.SetPixel(positionWidth - 1, positionHeight, color);
                    bmp.SetPixel(positionWidth, positionHeight, color);
                    bmp.SetPixel(positionWidth + 1, positionHeight, color);

                    bmp.SetPixel(positionWidth, positionHeight - 1, color);
                    break;
                default:
                    throw new Exception("Ошибка визуализации!");
            }
        }

        private void GenerateRandomIllnessAndInnateImmunity(int illEasyCount, int immInnateCount)
        {
            //генерируем первоначальных больных и с врождённым иммунитетом 
            //генерируем illEasyCount и immInnateCount случайных уникальных значений в диапазоне (0..Height; 0..Width)
            List<int[]> XY = new List<int[]>();
            int[] _xy;
            for (int i = 0; i < illEasyCount; i++)
            {
                _xy = new int[2];
                while (true)
                {
                    _xy[0] = random.Next(0, Height);
                    _xy[1] = random.Next(0, _xy[0] * 2 + 1);
                    if (!IsListContainsThisPoint(XY, _xy))
                    {
                        _grid[_xy[0], _xy[1]].State = new CSickEasyIllness();
                        XY.Add(_xy);
                        break;
                    }
                }
            }
            for (int i = 0; i < immInnateCount; i++)
            {
                _xy = new int[2];
                while (true)
                {
                    _xy[0] = random.Next(0, Height);
                    _xy[1] = random.Next(0, _xy[0] * 2 + 1);
                    if (!IsListContainsThisPoint(XY, _xy))
                    {
                        _grid[_xy[0], _xy[1]].State = new CHealthyWithInnateImmunity();
                        XY.Add(_xy);
                        break;
                    }
                }
            }
            _xy = new int[2];
        }

        private bool IsListContainsThisPoint(List<int[]> list, int[] point)
        {
            foreach (var p in list)
                if (p[0] == point[0] && p[1] == point[1])
                    return true;
            return false;
        }

        public override List<DailyStatistics> GetStatistics() => _allTimeStatistics;
    }
    
}
