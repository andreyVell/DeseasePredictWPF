using DeseasePredictWPF.Interfaces;
using DeseasePredictWPF.Models;
using DeseasePredictWPF.ViewModel;
using DeseasePredictWPF.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Excel = Microsoft.Office.Interop.Excel;

namespace DeseasePredictWPF
{
    public class CCellularAutomatVM:ViewModelBase, INotifyPropertyChanged
    {
        private MainWindow _parentWindow;
        private CellularAutomat _automat;
        private ResearchVisualization _researchVisualization;
        private OptionsVM _currOptions = new OptionsVM();
        

        private int _researchTime;
        private bool _visualize;
        private bool _isStop = false;
        private int _visualizeDelay=1;
        Dictionary<string, Color> _colors;
        Dictionary<string, int> _timeInSteps;
        Dictionary<string, double> _chancesToInfect;
        Dictionary<string, double> _stateSwitchChances;

        public CCellularAutomatVM(MainWindow parentWindow)
        {
            _parentWindow = parentWindow;            
        }        

        
        public int ResearchTime { get => _researchTime; set => _researchTime = value; }
        public bool Visualize 
        { 
            get => _visualize;
            set 
            { 
                _visualize = value; 
                if (_visualize)
                {
                    _parentWindow.delayTextBlock.Visibility = Visibility.Visible;
                    _parentWindow.delayBorder.Visibility = Visibility.Visible;
                }    
                else
                {
                    _parentWindow.delayTextBlock.Visibility = Visibility.Hidden;
                    _parentWindow.delayBorder.Visibility = Visibility.Hidden;
                    VisualizeDelay = 1;
                }
            } 
        }
        public int VisualizeDelay { get => _visualizeDelay; set => _visualizeDelay = value; }

        private RelayCommand _openOptionsCommand;
        public RelayCommand OpenOptionsCommand =>
            _openOptionsCommand ?? (_openOptionsCommand = new RelayCommand(OpenOptions));

        private void OpenOptions()
        {
            var optionsForm = new OptionsWindow(_currOptions);
            optionsForm.ShowDialog();
        }


        private RelayCommand _startResearchCommand;
        public RelayCommand StartResearchCommand =>
            _startResearchCommand ?? (_startResearchCommand = new RelayCommand(StartResearch));
        private void StartResearch()
        {
            _isStop = false;
            UpdateUIBeforeStart();
            try
            {
                //формируем начальные данные
                FormerEnterData();
                var generator = new NeighborhoodsGenerator(_currOptions.SelectedTopology, 
                    _currOptions.SelectedNeighborhood,
                    _currOptions.LeftRightIsolation,
                    _currOptions.UpDownIsolation,
                    _currOptions.ForwardBackIsolation);                
                switch (_currOptions.SelectedTopology.Type)
                {
                    case ETopologyType.Square:
                        _automat = new CellularAutomatSquare(
                            _currOptions.Height,
                            _currOptions.Width,
                            _colors,
                            _timeInSteps,
                            _chancesToInfect,
                            _stateSwitchChances,
                            _currOptions.P1,
                            _currOptions.H2,
                            generator);
                        break;
                    case ETopologyType.Cube:
                        _automat = new CellularAutomatCube(
                            _currOptions.Height,
                            _currOptions.Width,
                            _currOptions.Length,
                            _colors,
                            _timeInSteps,
                            _chancesToInfect,
                            _stateSwitchChances,
                            _currOptions.P1,
                            _currOptions.H2,
                            generator);
                        break;
                    case ETopologyType.Triangle:
                        _automat = new CellularAutomatTriangle(
                            _currOptions.Height,
                            _currOptions.Width,
                            _colors,
                            _timeInSteps,
                            _chancesToInfect,
                            _stateSwitchChances,
                            _currOptions.P1,
                            _currOptions.H2,
                            generator);
                        break;
                    case ETopologyType.Line:
                        _automat = new CellularAutomatLine(
                            _currOptions.Height,
                            _currOptions.Width,
                            _colors,
                            _timeInSteps,
                            _chancesToInfect,
                            _stateSwitchChances,
                            _currOptions.P1,
                            _currOptions.H2,
                            generator);
                        break;
                    default:
                        throw new Exception("Ошибка во входных данных!");
                }
                new Action(async () =>
                {
                    if (_visualize)
                    { 
                        _researchVisualization = new ResearchVisualization();
                        _researchVisualization.Show();
                    }
                    for (int i = 1; i <= _researchTime; i++)
                    {
                        if (_isStop)
                            break;                        
                        await Task.Delay(_visualizeDelay);
                        _parentWindow.researchProgress.Value++;
                        _parentWindow.progressTextBlock.Text = "День: " + (i) + "/" + _researchTime;
                        _automat.NextStep();
                        if (_visualize)
                        {
                            _researchVisualization.AutomatImage.Source =
                                  ConvertImageToRightFormat(_automat.MakePicture(
                                      (int)_researchVisualization.AutomatImage.Height,
                                      (int)_researchVisualization.AutomatImage.Width));
                            _researchVisualization.Title = "День: " + (i);
                            _researchVisualization.curDayTB.Text = "День: " + (i);
                        }                    
                    }
                    UpdateUIAfterEnd();
                    System.Windows.MessageBox.Show("Прогноз успешно завершён!");
                }).Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void FormerEnterData()
        {            
            _colors = new Dictionary<string, Color>();
            _colors.Add("H1Color", Color.Green);
            _colors.Add("H2Color", Color.PeachPuff);
            _colors.Add("H3Color", Color.Blue);
            _colors.Add("P1Color", Color.Yellow);
            _colors.Add("P2Color", Color.Red);
            _colors.Add("DColor", Color.Black);
            _timeInSteps = new Dictionary<string, int>();
            _timeInSteps.Add("TH1", 0);
            _timeInSteps.Add("TH2", 0);
            _timeInSteps.Add("TH3", _currOptions.Th3);
            _timeInSteps.Add("TP1", _currOptions.Tp1);
            _timeInSteps.Add("TP2", _currOptions.Tp2);
            _timeInSteps.Add("TD", 0);
            _chancesToInfect = new Dictionary<string, double>();
            _chancesToInfect.Add("H1", 0);
            _chancesToInfect.Add("H2", 0);
            _chancesToInfect.Add("H3", 0);
            _chancesToInfect.Add("P1", _currOptions.Phi);
            _chancesToInfect.Add("P2", _currOptions.Omega);
            _chancesToInfect.Add("D", 0);
            _stateSwitchChances = new Dictionary<string, double>();
            _stateSwitchChances.Add("alpha", 0);
            _stateSwitchChances.Add("dzeta", _currOptions.Dzeta);
            _stateSwitchChances.Add("beta", _currOptions.Beta);
            _stateSwitchChances.Add("gamma", _currOptions.Gamma);
            _stateSwitchChances.Add("delta", _currOptions.Delta);
            _stateSwitchChances.Add("epsilon", _currOptions.Epsilon);
        }
        private void UpdateUIBeforeStart()
        {
            _parentWindow.ButtonExportStatistics.IsEnabled = false;
            _parentWindow.ButtonStartResearch.IsEnabled = false;
            _parentWindow.ButtonStopResearch.IsEnabled = true;
            _parentWindow.researchProgress.Visibility = Visibility.Visible;
            _parentWindow.progressTextBlock.Visibility = Visibility.Visible;
            _parentWindow.researchProgress.Value = 0;
            _parentWindow.researchProgress.Minimum = 0;
            _parentWindow.researchProgress.Maximum = _researchTime - 1;
            _parentWindow.ButtonOpenOptions.IsEnabled = false;
            //_parentWindow.settingsGrid.IsEnabled = false;
            _parentWindow.tTB.IsEnabled = false;
            _parentWindow.delayTB.IsEnabled = false;    
            _parentWindow.VisualizeCheckBox.IsEnabled = false;
        }
        private void UpdateUIAfterEnd()
        {
            _parentWindow.ButtonStartResearch.IsEnabled = true;
            _parentWindow.ButtonStopResearch.IsEnabled = false;
            _parentWindow.ButtonExportStatistics.IsEnabled = true;
            _parentWindow.researchProgress.Visibility = Visibility.Hidden;
            _parentWindow.progressTextBlock.Visibility = Visibility.Hidden;
            _parentWindow.ButtonOpenOptions.IsEnabled = true;
            //_parentWindow.settingsGrid.IsEnabled = true;
            _parentWindow.tTB.IsEnabled = true;
            _parentWindow.delayTB.IsEnabled = true;
            _parentWindow.VisualizeCheckBox.IsEnabled = true;
        }

        private RelayCommand _stopResearchCommand;
        public RelayCommand StopResearchCommand =>
            _stopResearchCommand ?? (_stopResearchCommand = new RelayCommand(StopResearch));
        private void StopResearch()
        {            
            _isStop = true;
        }

        private RelayCommand _exportStatisticCommand;
        public RelayCommand ExportStatisticCommand =>
            _exportStatisticCommand ?? (_exportStatisticCommand = new RelayCommand(ExportStatistic));
        private void ExportStatistic()
        {
            try
            {
                new Action(async () =>
                {
                    UpdateUIBeforeStart();
                    Excel.Application app = new Excel.Application();
                    app.Visible = true;
                    Excel.Workbook wb = app.Workbooks.Add(1);
                    Excel.Worksheet ws = (Excel.Worksheet)wb.Sheets[1];
                    ws.Cells[1][1] = "День";
                    ws.Cells[2][1] = "Здоровые, без иммунитета";
                    ws.Cells[3][1] = "Здоровые, с врождённым иммунитетом";
                    ws.Cells[4][1] = "Здоровые, с приобритённым иммунитетом";
                    ws.Cells[5][1] = "Больные, без симптомов(лёгкое течение болезни)";
                    ws.Cells[6][1] = "Больные, активная фаза болезни";
                    ws.Cells[7][1] = "Умершие";
                    List<DailyStatistics> cDailyStatistics = _automat.GetStatistics();
                    for (int i = 0; i < cDailyStatistics.Count; i++)
                    {
                        await Task.Delay(1);
                        ws.Cells[1][i + 2] = i;
                        ws.Cells[2][i + 2] = cDailyStatistics[i].HealthyWithoutImmunityCount;
                        ws.Cells[3][i + 2] = cDailyStatistics[i].HealthyWithInnateImmunityCount;
                        ws.Cells[4][i + 2] = cDailyStatistics[i].HealthyWithAcquiredImmunityCount;
                        ws.Cells[5][i + 2] = cDailyStatistics[i].SickEasyIllnessCount;
                        ws.Cells[6][i + 2] = cDailyStatistics[i].SickHardIllnessCount;
                        ws.Cells[7][i + 2] = cDailyStatistics[i].DeadCount;

                        _parentWindow.researchProgress.Value++;
                        _parentWindow.progressTextBlock.Text = "День: " + (i) + "/" + _researchTime;
                    }
                    UpdateUIAfterEnd();
                    System.Windows.MessageBox.Show("Выгрузка успешно завершена!");
                }).Invoke();
            }
            catch
            {
                Console.Write("Ошибка сохранения");
            }

        }

        private BitmapImage ConvertImageToRightFormat(Bitmap bmp)
        {
            BitmapImage bitmapImage;
            using (MemoryStream memory = new MemoryStream())
            {
                bmp.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
        
    }
}