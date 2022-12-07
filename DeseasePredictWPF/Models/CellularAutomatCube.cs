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
    public class CellularAutomatCube:CellularAutomat
    {
        private int _height;
        private int _width;
        private int _length;
        private Cell[,,] _grid;
        private List<DailyStatistics> _allTimeStatistics;
        private Random random;
        private NeighborhoodsGenerator _neighborhoodsGenerator;

        public override int Height { get => _height; set => _height = value; }

        public override int Width { get => _width; set => _width = value; }

        public override int Length { get => _length; set => _length = value; }

        public CellularAutomatCube(int height, int width, int length, Dictionary<string, Color> colors, Dictionary<string, int> timeInSteps, Dictionary<string, double> chancesToInfect, Dictionary<string, double> stateSwitchChances, int startIllEasyCount, int startInnateImmunityCount, NeighborhoodsGenerator neighborhoodsGenerator)
        {
            _neighborhoodsGenerator = neighborhoodsGenerator;
            Height = height;
            Width = width;
            Length = length;
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
        public override Cell this[int i, int j] => throw new Exception("Topology error!");
        public override Cell this[int i, int j, int k] => _grid[i, j,k];

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
            _grid = new Cell[Height, Width, Length];
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    for (int k = 0; k < Length; k++)
                        _grid[i, j, k] = new Cell(i, j, new CHealthyWithoutImmunity(), stateSwitchChances, k);
        }

        private void FillNeighbors()
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    for (int k = 0; k < Length; k++)
                        _grid[i, j, k].FormerNeighborСellsList(this, _neighborhoodsGenerator);
        }

        public override void NextStep()
        {
            IState[,,] nextStates = new IState[Height, Width, Length];
            DailyStatistics cDailyStatistics = new DailyStatistics();
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    for (int k = 0; k < Length; k++)
                        nextStates[i, j, k] = _grid[i, j, k].NextStep();
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    for (int k = 0; k < Length; k++)
                    {
                        _grid[i, j, k].State = nextStates[i, j, k];
                        _grid[i, j, k].WriteCaseInStatistic(cDailyStatistics);
                    }
            _allTimeStatistics.Add(cDailyStatistics);
        }

        public override Bitmap MakePicture(int _PictureHeight, int _PictureWidth)
        {
            Bitmap bmp = new Bitmap(_PictureWidth, _PictureHeight);
            //for (int i = 0; i < bmp.Width; i++)
            //    for (int j = 0; j < bmp.Height; j++)
            //        bmp.SetPixel(i, j, _grid[i, j].Color);
            //if (bmp.Height < _PictureHeight || bmp.Width < _PictureWidth)
            //    bmp = TransformPictureToSize(bmp, _PictureHeight, _PictureWidth);
            return bmp;
        }

        //private Bitmap TransformPictureToSize(Bitmap oldBmp, int height, int width)
        //{
        //    Bitmap newBmp = new Bitmap(width, height);
        //    double heightScale = (double)height / oldBmp.Height;
        //    double widthScale = (double)width / oldBmp.Width;
        //    for (int i = 0; i < width; i++)
        //        for (int j = 0; j < height; j++)
        //            newBmp.SetPixel(i, j, oldBmp.GetPixel((int)(i / widthScale), (int)(j / heightScale)));
        //    return newBmp;
        //}

        private void GenerateRandomIllnessAndInnateImmunity(int illEasyCount, int immInnateCount)
        {
            //генерируем первоначальных больных и с врождённым иммунитетом 
            //генерируем illEasyCount и immInnateCount случайных уникальных значений в диапазоне (0..Height; 0..Width)
            List<int[]> XYZ = new List<int[]>();
            int[] _xyz;
            for (int i = 0; i < illEasyCount; i++)
            {
                _xyz = new int[3];
                while (true)
                {
                    _xyz[0] = random.Next(0, Height);
                    _xyz[1] = random.Next(0, Width);
                    _xyz[2] = random.Next(0, Length);
                    if (!IsListContainsThisPoint(XYZ, _xyz))
                    {
                        _grid[_xyz[0], _xyz[1], _xyz[2]].State = new CSickEasyIllness();
                        XYZ.Add(_xyz);
                        break;
                    }
                }
            }
            for (int i = 0; i < immInnateCount; i++)
            {
                _xyz = new int[3];
                while (true)
                {
                    _xyz[0] = random.Next(0, Height);
                    _xyz[1] = random.Next(0, Width);
                    _xyz[2] = random.Next(0, Length);
                    if (!IsListContainsThisPoint(XYZ, _xyz))
                    {
                        _grid[_xyz[0], _xyz[1], _xyz[2]].State = new CHealthyWithInnateImmunity();
                        XYZ.Add(_xyz);
                        break;
                    }
                }
            }
        }

        private bool IsListContainsThisPoint(List<int[]> list, int[] point)
        {
            foreach (var p in list)
                if (p[0] == point[0] && p[1] == point[1] && p[2] == point[2])
                    return true;
            return false;
        }

        public override List<DailyStatistics> GetStatistics() => _allTimeStatistics;
    }
}
