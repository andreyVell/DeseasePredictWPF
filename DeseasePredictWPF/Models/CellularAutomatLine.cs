using DeseasePredictWPF.Interfaces;
using DeseasePredictWPF.States;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DeseasePredictWPF.Models
{
    public class CellularAutomatLine:CellularAutomat
    {
        private int _height;
        private int _width;
        private Cell[] _grid;
        private List<DailyStatistics> _allTimeStatistics;
        private Random random;
        private NeighborhoodsGenerator _neighborhoodsGenerator;

        public override int Width { get => _width; set => _width = value; }
        public override int Height { get => _height; set => _height = value; }
        public override int Length { get => 1; set => base.Length = value; }

        public CellularAutomatLine(int height, int width, Dictionary<string, Color> colors, Dictionary<string, int> timeInSteps, Dictionary<string, double> chancesToInfect, Dictionary<string, double> stateSwitchChances, int startIllEasyCount, int startInnateImmunityCount, NeighborhoodsGenerator neighborhoodsGenerator)
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
        public override Cell this[int i] => _grid[i];
        public override Cell this[int i, int j] => throw new Exception("Topology error!");
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
            _grid = new Cell[Width];
            for (int i = 0; i < Width; i++)
                _grid[i] = new Cell(0, i, new CHealthyWithoutImmunity(), stateSwitchChances);
        }

        private void FillNeighbors()
        {
            for (int j = 0; j < Width; j++)
                _grid[j].FormerNeighborСellsList(this, _neighborhoodsGenerator);
        }

        public override void NextStep()
        {
            IState[] nextStates = new IState[Width];
            DailyStatistics cDailyStatistics = new DailyStatistics();
            for (int j = 0; j < Width; j++)
                nextStates[j] = _grid[j].NextStep();
            for (int j = 0; j < Width; j++)
            {
                _grid[j].State = nextStates[j];
                _grid[j].WriteCaseInStatistic(cDailyStatistics);
            }
            _allTimeStatistics.Add(cDailyStatistics);
        }

        public override Bitmap MakePicture(int _PictureHeight, int _PictureWidth)
        {
            Bitmap bmp = new Bitmap(Width, 1);
            for (int j = 0; j < bmp.Width; j++)
                bmp.SetPixel(j, 0, _grid[j].Color);
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
                    newBmp.SetPixel(i, j, Color.White);
            for (int i = 0; i < width; i++)
                for (int j = (int)((height/2) - (widthScale/2)-1); j < (int)((height / 2) + (widthScale / 2)); j++)
                    newBmp.SetPixel(i, j, oldBmp.GetPixel((int)(i / widthScale), (int)(j / heightScale)));
            return newBmp;
        }

        private void GenerateRandomIllnessAndInnateImmunity(int illEasyCount, int immInnateCount)
        {
            //генерируем первоначальных больных и с врождённым иммунитетом 
            //генерируем illEasyCount и immInnateCount случайных уникальных значений в диапазоне (0..Height; 0..Width)
            List<int> XY = new List<int>();
            int _xy;
            for (int i = 0; i < illEasyCount; i++)
            {
                while (true)
                {
                    _xy = random.Next(0, Width);
                    if (!XY.Contains(_xy))
                    {
                        _grid[_xy].State = new CSickEasyIllness();
                        XY.Add(_xy);
                        break;
                    }
                }
            }
            for (int i = 0; i < immInnateCount; i++)
            {
                while (true)
                {
                    _xy = random.Next(0, Width);
                    if (!XY.Contains(_xy))
                    {
                        _grid[_xy].State = new CHealthyWithInnateImmunity();
                        XY.Add(_xy);
                        break;
                    }
                }
            }
        }

        public override List<DailyStatistics> GetStatistics() => _allTimeStatistics;
    }
}
