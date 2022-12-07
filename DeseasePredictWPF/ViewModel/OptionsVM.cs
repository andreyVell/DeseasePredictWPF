using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using DeseasePredictWPF.Interfaces;

namespace DeseasePredictWPF.ViewModel
{
    public class OptionsVM:ViewModelBase
    {
        private int _n;
        private int _height;
        private int _width;
        private int _length;
        private int _p1;
        private int _h2;
        private int _th3;
        private int _tp1;
        private int _tp2;
        private double _dzeta;
        private double _phi;
        private double _omega;
        private double _beta;
        private double _gamma;
        private double _delta;
        private double _epsilon;
        private ObservableCollection<TopologyTypeVM> _topologies;
        private ObservableCollection<NeighborhoodTypeVM> _neighborhoods;
        private TopologyTypeVM _selectedTopology;
        private NeighborhoodTypeVM _selectedNeighborhood;
        private bool _needToIsolation;
        private bool _enableNeedToIsolation;
        private bool _leftRightIsolation;
        private bool _upDownIsolation;
        private bool _forwardBackIsolation;
        private string _stringSizes;

        public OptionsVM()
        {
            _stringSizes = string.Empty;
            _topologies = new ObservableCollection<TopologyTypeVM>();
            _neighborhoods = new ObservableCollection<NeighborhoodTypeVM>();
            _needToIsolation = false;
            _enableNeedToIsolation = true;
            FillTopologies();
        }

        

        public int N
        {
            get => _n;
            set
            {
                _n = value;
                switch (SelectedTopology?.Type)
                {
                    case ETopologyType.Square:
                        Height = (int)Math.Sqrt(_n);
                        Width = (int)Math.Sqrt(_n);
                        _stringSizes = $"{Height} X {Width}";
                        break;
                    case ETopologyType.Line:
                        Height = 1;
                        Width = _n;
                        _stringSizes = $"{Height} X {Width}";
                        break;
                    case ETopologyType.Cube:
                        Height = (int)Math.Pow(_n, 1 / 3f);
                        Width = (int)Math.Pow(_n, 1 / 3f);
                        Length = (int)Math.Pow(_n, 1 / 3f);
                        _stringSizes = $"{Height} X {Width} X {Length}";
                        break;
                    case ETopologyType.Triangle:
                        Height = (int)Math.Sqrt(_n);
                        Width = (int)Math.Sqrt(_n) * 2 - 1;
                        _stringSizes = $"Длина стороны треугольника = {Height}";
                        break;
                    default:
                        break;
                }
                RaisePropertyChanged(nameof(StringSizes));
            }
        }
        public int Height { get => _height; set { _height = value; } }
        public int Width { get => _width; set { _width = value; } }
        public int Length { get => _length; set => _length = value; }
        public string StringSizes { get => _stringSizes; set => _stringSizes = value; }
        public int P1
        {
            get => _p1;
            set
            {
                _p1 = value;
                if (_height * _width - _p1 - _h2 < 0)
                {
                    System.Windows.MessageBox.Show("Сумма числа изначально больных и числа обладающих врождённым иммунитетом не должна быть больше чем кол-во клеток в автомате");
                    _p1 /= 10;
                    RaisePropertyChanged(nameof(P1));
                }
            }
        }
        public int H2
        {
            get => _h2;
            set
            {
                _h2 = value;
                if (_height * _width - _p1 - _h2 < 0)
                {
                    System.Windows.MessageBox.Show("Сумма числа изначально больных и числа обладающих врождённым иммунитетом не должна быть больше чем кол-во клеток в автомате");
                    _h2 /= 10;
                    RaisePropertyChanged(nameof(H2));
                }
            }
        }
        public int Th3 { get => _th3; set => _th3 = value; }
        public int Tp1 { get => _tp1; set => _tp1 = value; }
        public int Tp2 { get => _tp2; set => _tp2 = value; }
        public double Dzeta { get => _dzeta; set => _dzeta = value; }
        public double Phi { get => _phi; set => _phi = value; }
        public double Omega { get => _omega; set => _omega = value; }
        public double Beta { get => _beta; set { _beta = value; if (_beta + _gamma > 1) { System.Windows.MessageBox.Show("Сумма вероятности переболеть без острой фазы и вероятности переболеть с острой фазой не должна превышать 1."); _beta = 0; RaisePropertyChanged(); } } }
        public double Gamma { get => _gamma; set { _gamma = value; if (_beta + _gamma > 1) { System.Windows.MessageBox.Show("Сумма вероятности переболеть без острой фазы и вероятности переболеть с острой фазой не должна превышать 1."); _gamma = 0; RaisePropertyChanged(); } } }
        public double Delta { get => _delta; set { _delta = value; if (_delta + _epsilon > 1) { System.Windows.MessageBox.Show("Сумма вероятности выздовления после острой фазы и вероятности летального исхода после острой фазой не должна превышать 1."); _delta = 0; RaisePropertyChanged(); } } }
        public double Epsilon { get => _epsilon; set { _epsilon = value; if (_delta + _epsilon > 1) { System.Windows.MessageBox.Show("Сумма вероятности выздовления после острой фазы и вероятности летального исхода после острой фазой не должна превышать 1."); _epsilon = 0; RaisePropertyChanged(); } } }
        public ObservableCollection<TopologyTypeVM> Topologies => _topologies;
        public ObservableCollection<NeighborhoodTypeVM> Neighborhoods => _neighborhoods;

        public TopologyTypeVM SelectedTopology 
        { 
            get
            {
                return _selectedTopology;
            }
            set
            {
                _selectedTopology = value;
                if (_selectedTopology != null)
                {
                    FillNeighborhoodTypes();
                    FillIsolationTypes();
                    _n = 0;
                    RaisePropertyChanged(nameof(N));
                }
            }
        }

        public NeighborhoodTypeVM SelectedNeighborhood
        {
            get
            {
                return _selectedNeighborhood;
            }
            set
            {
                _selectedNeighborhood = value;
            }
        }

        public bool NeedToIsolation
        {
            get
            {
                return _needToIsolation;
            }
            set
            {
                _needToIsolation = value;
                if (value)
                {                                 
                    switch (SelectedTopology?.Type)
                    {
                        case ETopologyType.Square:
                            EnableLeftRightIsolation = true;
                            EnableUpDownIsolation = true;
                            break;
                        case ETopologyType.Line:
                            EnableLeftRightIsolation = true;
                            break;
                        case ETopologyType.Cube:
                            EnableForwardBackIsolation = true;
                            EnableLeftRightIsolation = true;
                            EnableUpDownIsolation = true;
                            break;
                        case ETopologyType.Triangle:
                            EnableForwardBackIsolation = false;
                            EnableLeftRightIsolation = false;
                            EnableUpDownIsolation = false;
                            break;
                        default:
                            break;
                    }
                }
                else
                {                    
                    EnableForwardBackIsolation = false;
                    EnableLeftRightIsolation = false;
                    EnableUpDownIsolation = false;
                }
                LeftRightIsolation = false;
                UpDownIsolation = false;
                ForwardBackIsolation = false;
                RaisePropertyChanged(nameof(LeftRightIsolation));
                RaisePropertyChanged(nameof(UpDownIsolation));
                RaisePropertyChanged(nameof(ForwardBackIsolation));

                RaisePropertyChanged(nameof(EnableLeftRightIsolation));
                RaisePropertyChanged(nameof(EnableUpDownIsolation));
                RaisePropertyChanged(nameof(EnableForwardBackIsolation));
            }
        }

        public bool EnableNeedToIsolation
        {
            get
            {
                return _enableNeedToIsolation;
            }
            set
            {
                _enableNeedToIsolation = value;
            }
        }

        public bool LeftRightIsolation
        {
            get
            {
                return _leftRightIsolation;
            }
            set
            {
                _leftRightIsolation = value;
            }
        }

        public bool UpDownIsolation
        {
            get
            {
                return _upDownIsolation;
            }
            set
            {
                _upDownIsolation = value;
            }
        }

        public bool ForwardBackIsolation
        {
            get
            {
                return _forwardBackIsolation;
            }
            set
            {
                _forwardBackIsolation = value;
            }
        }

        public bool EnableLeftRightIsolation { get; set; }

        public bool EnableUpDownIsolation { get; set; }

        public bool EnableForwardBackIsolation { get; set; }







        private void FillIsolationTypes()
        {
            switch(SelectedTopology?.Type)
            {
                case ETopologyType.Square:
                    EnableNeedToIsolation = true;
                    NeedToIsolation = false;
                    break;
                case ETopologyType.Line:
                    EnableNeedToIsolation = true;
                    NeedToIsolation = false;
                    break;
                case ETopologyType.Cube:
                    EnableNeedToIsolation = true;
                    NeedToIsolation = false;
                    break;
                case ETopologyType.Triangle:
                    EnableNeedToIsolation = false;
                    NeedToIsolation = false;
                    break;
                default:
                    break;
            }
            RaisePropertyChanged(nameof(NeedToIsolation));
            RaisePropertyChanged(nameof(EnableNeedToIsolation));
        }

        private void FillNeighborhoodTypes()
        {

            _selectedNeighborhood = null;
            _neighborhoods.Clear();
            switch (SelectedTopology?.Type)
            {
                case ETopologyType.Square:
                    _neighborhoods.Add(new NeighborhoodTypeVM(ENeighborhoodType.Cell4));
                    _neighborhoods.Add(new NeighborhoodTypeVM(ENeighborhoodType.Cell8));
                    break;
                case ETopologyType.Cube:
                    _neighborhoods.Add(new NeighborhoodTypeVM(ENeighborhoodType.Cell6));
                    _neighborhoods.Add(new NeighborhoodTypeVM(ENeighborhoodType.Cell26));
                    break;
                case ETopologyType.Line:
                    _neighborhoods.Add(new NeighborhoodTypeVM(ENeighborhoodType.Cell2));
                    _neighborhoods.Add(new NeighborhoodTypeVM(ENeighborhoodType.Cell4));
                    break;
                case ETopologyType.Triangle:
                    _neighborhoods.Add(new NeighborhoodTypeVM(ENeighborhoodType.Cell3));
                    break;
                default:
                    break;
            }
            RaisePropertyChanged(nameof(SelectedNeighborhood));
            RaisePropertyChanged(nameof(Neighborhoods));
        }

        private void FillTopologies()
        {
            _topologies.Clear();
            _topologies.Add(new TopologyTypeVM(ETopologyType.Square));
            _topologies.Add(new TopologyTypeVM(ETopologyType.Cube));
            _topologies.Add(new TopologyTypeVM(ETopologyType.Line));
            _topologies.Add(new TopologyTypeVM(ETopologyType.Triangle));
            _selectedTopology = null;
            RaisePropertyChanged(nameof(SelectedTopology));
            RaisePropertyChanged(nameof(Topologies));
        }



        private RelayCommand _saveParamsCommand;
        public RelayCommand SaveParamsCommand =>
            _saveParamsCommand ?? (_saveParamsCommand = new RelayCommand(SaveParams));
        private void SaveParams()
        {
            try
            {
                var dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Filter = "SOAP|*.soap|Binary|*.bin";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                using (var fs =
                    new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
                {
                    switch (Path.GetExtension(dlg.FileName))
                    {
                        case ".bin":
                            var bf = new BinaryFormatter();
                            bf.Serialize(fs, _selectedTopology.DisplayName);
                            bf.Serialize(fs, _selectedTopology.Type);
                            bf.Serialize(fs, _selectedNeighborhood.DisplayName);
                            bf.Serialize(fs, _selectedNeighborhood.NeighborhoodType);
                            bf.Serialize(fs, _needToIsolation);
                            bf.Serialize(fs, _leftRightIsolation);
                            bf.Serialize(fs, _upDownIsolation);
                            bf.Serialize(fs, _forwardBackIsolation);
                            bf.Serialize(fs, _n);
                            bf.Serialize(fs, _height);
                            bf.Serialize(fs, _width);
                            bf.Serialize(fs, _length);
                            bf.Serialize(fs, _p1);
                            bf.Serialize(fs, _h2);
                            bf.Serialize(fs, _th3);
                            bf.Serialize(fs, _tp1);
                            bf.Serialize(fs, _tp2);
                            bf.Serialize(fs, _dzeta);
                            bf.Serialize(fs, _phi);
                            bf.Serialize(fs, _omega);
                            bf.Serialize(fs, _beta);
                            bf.Serialize(fs, _gamma);
                            bf.Serialize(fs, _delta);
                            bf.Serialize(fs, _epsilon);
                            break;
                        case ".soap":
                            var sf = new SoapFormatter();
                            sf.Serialize(fs, _selectedTopology.DisplayName);
                            sf.Serialize(fs, _selectedTopology.Type);
                            sf.Serialize(fs, _selectedNeighborhood.DisplayName);
                            sf.Serialize(fs, _selectedNeighborhood.NeighborhoodType);
                            sf.Serialize(fs, _needToIsolation);
                            sf.Serialize(fs, _leftRightIsolation);
                            sf.Serialize(fs, _upDownIsolation);
                            sf.Serialize(fs, _forwardBackIsolation);
                            sf.Serialize(fs, _n);
                            sf.Serialize(fs, _height);
                            sf.Serialize(fs, _width);
                            sf.Serialize(fs, _length);
                            sf.Serialize(fs, _p1);
                            sf.Serialize(fs, _h2);
                            sf.Serialize(fs, _th3);
                            sf.Serialize(fs, _tp1);
                            sf.Serialize(fs, _tp2);
                            sf.Serialize(fs, _dzeta);
                            sf.Serialize(fs, _phi);
                            sf.Serialize(fs, _omega);
                            sf.Serialize(fs, _beta);
                            sf.Serialize(fs, _gamma);
                            sf.Serialize(fs, _delta);
                            sf.Serialize(fs, _epsilon);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка сохранения файла!" + ex.Message);
            }
        }

        private RelayCommand _openParamsCommand;
        public RelayCommand OpenParamsCommand =>
            _openParamsCommand ?? (_openParamsCommand = new RelayCommand(OpenParams));


        private void OpenParams()
        {
            try
            {
                var dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.Filter = "SOAP|*.soap|Binary|*.bin";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                using (var fs =
                    new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
                {
                    string desDisplayName;
                    ETopologyType desTopologyType;
                    ENeighborhoodType desNType;
                    switch (Path.GetExtension(dlg.FileName))
                    {
                        case ".bin":
                            var bf = new BinaryFormatter();
                            desDisplayName = (string)bf.Deserialize(fs);
                            desTopologyType = (ETopologyType)bf.Deserialize(fs);
                            SelectedTopology = _topologies.Where(e => e.DisplayName == desDisplayName && e.Type == desTopologyType).FirstOrDefault();

                            desDisplayName = (string)bf.Deserialize(fs);
                            desNType = (ENeighborhoodType)bf.Deserialize(fs);
                            SelectedNeighborhood = _neighborhoods.Where(e => e.DisplayName == desDisplayName && e.NeighborhoodType == desNType).FirstOrDefault();

                            NeedToIsolation = (bool)bf.Deserialize(fs);
                            _leftRightIsolation = (bool)bf.Deserialize(fs);
                            _upDownIsolation = (bool)bf.Deserialize(fs);
                            _forwardBackIsolation = (bool)bf.Deserialize(fs);
                            N = (int)bf.Deserialize(fs);
                            _height = (int)bf.Deserialize(fs);
                            _width = (int)bf.Deserialize(fs);
                            _length = (int)bf.Deserialize(fs);
                            _p1 = (int)bf.Deserialize(fs);
                            _h2 = (int)bf.Deserialize(fs);
                            _th3 = (int)bf.Deserialize(fs);
                            _tp1 = (int)bf.Deserialize(fs);
                            _tp2 = (int)bf.Deserialize(fs);
                            _dzeta = (double)bf.Deserialize(fs);
                            _phi = (double)bf.Deserialize(fs);
                            _omega = (double)bf.Deserialize(fs);
                            _beta = (double)bf.Deserialize(fs);
                            _gamma = (double)bf.Deserialize(fs);
                            _delta = (double)bf.Deserialize(fs);
                            _epsilon = (double)bf.Deserialize(fs);
                            break;
                        case ".soap":
                            var sf = new SoapFormatter();
                            desDisplayName = (string)sf.Deserialize(fs);
                            desTopologyType = (ETopologyType)sf.Deserialize(fs);
                            SelectedTopology = _topologies.Where(e => e.DisplayName == desDisplayName && e.Type == desTopologyType).FirstOrDefault();

                            desDisplayName = (string)sf.Deserialize(fs);
                            desNType = (ENeighborhoodType)sf.Deserialize(fs);
                            SelectedNeighborhood = _neighborhoods.Where(e => e.DisplayName == desDisplayName && e.NeighborhoodType == desNType).FirstOrDefault();

                            NeedToIsolation = (bool)sf.Deserialize(fs);
                            _leftRightIsolation = (bool)sf.Deserialize(fs);
                            _upDownIsolation = (bool)sf.Deserialize(fs);
                            _forwardBackIsolation = (bool)sf.Deserialize(fs);
                            N = (int)sf.Deserialize(fs);
                            _height = (int)sf.Deserialize(fs);
                            _width = (int)sf.Deserialize(fs);
                            _length = (int)sf.Deserialize(fs);
                            _p1 = (int)sf.Deserialize(fs);
                            _h2 = (int)sf.Deserialize(fs);
                            _th3 = (int)sf.Deserialize(fs);
                            _tp1 = (int)sf.Deserialize(fs);
                            _tp2 = (int)sf.Deserialize(fs);
                            _dzeta = (double)sf.Deserialize(fs);
                            _phi = (double)sf.Deserialize(fs);
                            _omega = (double)sf.Deserialize(fs);
                            _beta = (double)sf.Deserialize(fs);
                            _gamma = (double)sf.Deserialize(fs);
                            _delta = (double)sf.Deserialize(fs);
                            _epsilon = (double)sf.Deserialize(fs);   
                            break;
                    }
                    RaisePropertyChanged(nameof(SelectedTopology));
                    RaisePropertyChanged(nameof(SelectedNeighborhood));
                    RaisePropertyChanged(nameof(EnableForwardBackIsolation));
                    RaisePropertyChanged(nameof(EnableLeftRightIsolation));
                    RaisePropertyChanged(nameof(EnableUpDownIsolation));
                    RaisePropertyChanged(nameof(EnableNeedToIsolation));
                    RaisePropertyChanged(nameof(NeedToIsolation));
                    RaisePropertyChanged(nameof(LeftRightIsolation));
                    RaisePropertyChanged(nameof(UpDownIsolation));
                    RaisePropertyChanged(nameof(ForwardBackIsolation));
                    RaisePropertyChanged(nameof(N));
                    RaisePropertyChanged(nameof(Height));
                    RaisePropertyChanged(nameof(Width));
                    RaisePropertyChanged(nameof(Length));
                    RaisePropertyChanged(nameof(P1));
                    RaisePropertyChanged(nameof(H2));
                    RaisePropertyChanged(nameof(Th3));
                    RaisePropertyChanged(nameof(Tp1));
                    RaisePropertyChanged(nameof(Tp2));
                    RaisePropertyChanged(nameof(Dzeta));
                    RaisePropertyChanged(nameof(Beta));
                    RaisePropertyChanged(nameof(Omega));
                    RaisePropertyChanged(nameof(Phi));
                    RaisePropertyChanged(nameof(Gamma));
                    RaisePropertyChanged(nameof(Epsilon));
                    RaisePropertyChanged(nameof(Delta));
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Ошибка загрузки файла!");
            }
        }
    }
}
