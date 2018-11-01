using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using HelixToolkit.Wpf;
namespace Test
{
    public class MainViewModel : BaseViewModel
    {
        private IView _view;
        private IModel _model;
        CancellationTokenSource cts = new CancellationTokenSource();
        private Model3D model3D;
        private Model3D _boundingmodel;
        private string _selectedTask;
        private bool _task5vis;
        private int _maxValue;
        private int _minValue;
        ManualResetEvent manual;

        Task _moveTask;
        public ObservableCollection<string> Tasks { get; set; }
  
       
        public bool Task5Vis
        {
            get { return _task5vis; }
            set { _task5vis = value; OnPropertyChanged(new PropertyChangedEventArgs("Task5Vis")); }
        }
        public string SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedTask"));
                TasksView(value);
               
            }
        }

        

        public int MinValue
        {
            get { return _minValue; }
            set { _minValue = value; OnPropertyChanged(new PropertyChangedEventArgs("MinValue")); }
        }
        public int MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; OnPropertyChanged(new PropertyChangedEventArgs("MinValue")); }
        }
        public Model3D MyModel
        {
            get { return model3D; }
            set
            {
                model3D = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MyModel"));
              
            }
        }
        public Model3D BoundingModel
        {
            get
            {
                return _boundingmodel;
            }
            set
            {
                _boundingmodel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("BoundingModel"));
            }
        }
           

        public MainViewModel(IView view)
        {
            _model = new Model();
            _view = view;
            Task5Vis = false;
            manual = new ManualResetEvent(false);
            Tasks = new ObservableCollection<string>()
            {
                "Task 3",
                "Task 4",
                "Task 5"
            };
            SelectedTask = Tasks[0];
            MinValue = -25;
            MaxValue = 25;
        }
        DelegateCommand OpenObjDelegate;
        public ICommand OpenCommand
        {
            get
            {
                if (OpenObjDelegate == null)
                    OpenObjDelegate = new DelegateCommand(Open, null);

                return OpenObjDelegate;
            }
        }
        DelegateCommand ClearObjDelegate;
        public ICommand ClearCommand
        {
            get
            {
                if (ClearObjDelegate == null)
                    ClearObjDelegate = new DelegateCommand(Clear, CanClear);

                return ClearObjDelegate;
            }
        }
        DelegateCommand StartDelegate;
        public ICommand StartCommand
        {
            get
            {
                if (StartDelegate == null)
                {
                    StartDelegate = new DelegateCommand(Start, CanStart);
                }
                return StartDelegate;
            }
        }
        DelegateCommand StopDelegate;
        public ICommand StopCommand
        {
            get
            {
                if (StopDelegate == null)
                {
                    StopDelegate = new DelegateCommand(Stop, CanStop);
                }
                return StopDelegate;
            }
        }

        private bool CanStop(object obj)
        {
          

            return manual.WaitOne(0);
        }

        private void Stop(object obj)
        {
            manual?.Reset();
        }

        private bool CanStart(object obj)
        {
            if (model3D == null)
                return false;
            
      


            return true;
        }

        private  void Start(object obj)
        {
            manual.Set();
            if (_moveTask == null)
            {
                cts = new CancellationTokenSource();
                _moveTask = new Task(()=>Move(),cts.Token);
                _moveTask.Start();
            }
        }

        private bool CanClear(object obj)
        {
            if (MyModel==null && BoundingModel==null)
            {
                return false;
            }
            return true;
        }

        private void Clear(object obj)
        {
            MyModel = null;
            BoundingModel = null;
            manual?.Set();
            cts.Cancel();
            _moveTask = null;
            manual= new ManualResetEvent(false);
        }

        private void Open(object obj)
        {
            var path = _view.GetFilePath();
            try
            {
                byte[] buff = _model.LoadFromFile(path);
                using (MemoryStream ms = new MemoryStream(buff))
                {
                    ObjReader CurrentHelixObjReader = new ObjReader();
                    MyModel = CurrentHelixObjReader.Read(ms);
                }
            }
            catch(Exception)
            {
                try
                {
                    ObjReader CurrentHelixObjReader = new ObjReader();
                    MyModel = CurrentHelixObjReader.Read(path);
                }
                catch(Exception ex)
                {
                    _view.ShowError(ex.Message);
                }
              
            }
            finally
            {
                if (Tasks.IndexOf(SelectedTask) == 1)
                {
                    BoundingModel = MyModel;
                }
            }
        }

        private void TasksView(string selectedTask)
        {
            switch (Tasks.IndexOf(SelectedTask))
            {
                case 1:
                    Stop(null);
                    BoundingModel = MyModel;
                    Task5Vis = false;    
                    break;
                case 2:
                    BoundingModel = null;
                    Task5Vis = true;
                    break;
                default:
                    Stop(null);
                    BoundingModel = null;
                    Task5Vis = false;
                    break;
            }
        }
        private void Move()
        {
            double val = 1;

            while (true)
            {
                if (manual.WaitOne())
                {

                    Thread.Sleep(20);
                    MyModel.Dispatcher?.Invoke(() =>
                    {
                        var matrix = MyModel.Transform.Value;
                        if (matrix.OffsetZ > MaxValue)
                        {
                            matrix.OffsetZ = MaxValue - 5;
                        }
                        if (matrix.OffsetZ < MinValue)
                        {
                            matrix.OffsetZ = MinValue + 5;
                        }
                        if (matrix.OffsetZ == MaxValue || MinValue == matrix.OffsetZ)
                        {
                            val = val * -1;
                        }

                        if (manual.WaitOne(0) == true)
                        {
                            matrix.OffsetZ += val;
                            MyModel.Transform = new MatrixTransform3D(matrix);
                        }

                    });
                }
                if (cts.Token.IsCancellationRequested)
                {
                    return;
                }
                    
            }
        }
    }
}
