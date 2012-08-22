//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using System.Threading;
//using Tools.Core.Progress;
//using System.Reflection;
//using System.IO;

//namespace Tools.Benchmark.TSPLIB
//{
//    public partial class DemoForm : Form, IProgressReporter
//    {
//        private Image _city_image;

//        private Graphics _city_graphics;

//        private TspLibProblem _problem;

//        private Tools.Math.AI.Genetic.Individual<int, Math.TravellingSalesman.Genetic.Solver.GeneticProblem, Math.TravellingSalesman.Genetic.Solver.Fitness> _fittest;

//        private float _scale_x;
//        private float _scale_y;

//        public DemoForm()
//        {
//            InitializeComponent();

//            this.CreateGraphics();
//        }

//        private void CreateGraphics()
//        {
//            _city_image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
//            _city_graphics = Graphics.FromImage(_city_image);

//            this.Scale();
//        }

//        private void Scale()
//        {
//            if (_problem == null)
//            {
//                _scale_x = 1;
//                _scale_y = 1;
//            }
//            else
//            {
//                _scale_x = ((float)(_city_image.Width) / (float)_problem.Box.Width);
//                _scale_y = ((float)(_city_image.Height) / (float)_problem.Box.Height);
//            }
//        }

//        private void DrawCities(Graphics graphics)
//        {
//            graphics.FillRectangle(Brushes.White, 0, 0, _city_image.Width, _city_image.Height);

//            if (_problem != null)
//            {
//                foreach (Point p in _problem.Points)
//                {
//                    PointF p_scaled = this.TranslateAndScale(p);
//                    graphics.DrawEllipse(Pens.Black, p_scaled.X - 2, p_scaled.Y - 2, 5, 5);
//                }
//            }

//            this.pictureBox1.Image = _city_image;
//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            Tools.Math.TSP.Main.Facade.RegisterProgressReporter(this);

//            //Tools.Math.TSP.Genetic.Solver.Operations.Helpers.BestPlacementHelper.NewRoute += new Math.TravellingSalesman.Genetic.Solver.Operations.Helpers.BestPlacementHelper.NewRouteDelegate(BestPlacementHelper_NewRoute);
            
//            _city_image = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
//            _city_graphics = Graphics.FromImage(_city_image);

//            Thread thread = new Thread(new ThreadStart(StartTSP));
//            thread.Start();
//        }

//        private PointF TranslateAndScale(Point p)
//        {
//            // translate.
//            float x = p.X - _problem.Box.X;
//            float y = p.Y - _problem.Box.Y;

//            return new PointF(x * _scale_x, y * _scale_y);
//        }

//        private int _prev_count;

//        void BestPlacementHelper_NewRoute(List<int> route)
//        {
//            this.Scale();

//            //if ((_prev_count < route.Count - (_problem.Points.Count / 5)) || route.Count == _problem.Points.Count - 1)
//            //{
//            //    _prev_count = route.Count;

//            //    this.DrawCities(_city_graphics);

//            //    for (int city_idx = 0; city_idx < route.Count; city_idx++)
//            //    {
//            //        int city_idx_next = city_idx + 1;
//            //        if (city_idx_next == route.Count)
//            //        {
//            //            city_idx_next = 0;
//            //        }

//            //        int city_from = route[city_idx];
//            //        int city_to = route[city_idx_next];

//            //        _city_graphics.DrawLine(Pens.Black,
//            //            this.TranslateAndScale(new Point(_problem.Points[city_from].X, _problem.Points[city_from].Y)),
//            //            this.TranslateAndScale(new Point(_problem.Points[city_to].X, _problem.Points[city_to].Y)));
//            //    }

//            //    if (this.InvokeRequired)
//            //    {
//            //        this.Invoke(new RefreshDelegate(DoRefresh));
//            //    }
//            //}
             
//            //if (route.Count == _problem.Points.Count - 1)
//            //{
//            //    _prev_count = 0;
//            //}
//        }

//        private void StartTSP()
//        {
//            //string file = "a280.tsp";
//            //string file = "bier127.tsp";
//            string file = "berlin52.tsp";
//            //string file = "u1060.tsp";
//            //string file = "pr107.tsp";
//            //string file = "tsp225.tsp";
//            //string file = "ts225.tsp";
//            //string file = "dantzig42.tsp";

//            _problem = TspLibProblem.CreateTspLibProblem(new FileInfo(
//                new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName + string.Format(@"\Problems\{0}",file)));

//            this.Scale();

//            this.DrawCities(_city_graphics);

//            //Tools.Math.TSP.Main.Facade.NewFittest += new Math.TravellingSalesman.Main.Facade.NewFittestDelegate(Facade_NewFittest);
//            Tools.Math.TSP.Main.Facade.Solve(_problem);
//            //Tools.Math.TSP.Main.Facade.NewFittest -= new Math.TravellingSalesman.Main.Facade.NewFittestDelegate(Facade_NewFittest);
//        }

//        void Facade_NewFittest(Tools.Math.AI.Genetic.Individual<int, Math.TravellingSalesman.Genetic.Solver.GeneticProblem, Math.TravellingSalesman.Genetic.Solver.Fitness> individual)
//        {
//            _fittest = individual;

//            this.Scale();

//            //Tools.Math.TSP.Genetic.Solver.Operations.Helpers.BestPlacementHelper.NewRoute -= new Math.TravellingSalesman.Genetic.Solver.Operations.Helpers.BestPlacementHelper.NewRouteDelegate(BestPlacementHelper_NewRoute);

//            this.DrawCities(_city_graphics);

//            List<int> cities =  individual.Genomes;
//            cities.Add(_problem.First);

//            for (int city_idx = 0; city_idx < cities.Count; city_idx++)
//            {
//                int city_idx_next = city_idx + 1;
//                if (city_idx_next == individual.Genomes.Count)
//                {
//                    city_idx_next = 0;
//                }

//                int city_from = individual.Genomes[city_idx];
//                int city_to = individual.Genomes[city_idx_next];

//                _city_graphics.DrawLine(Pens.Black,
//                    this.TranslateAndScale(new Point(_problem.Points[city_from].X, _problem.Points[city_from].Y)),
//                    this.TranslateAndScale(new Point(_problem.Points[city_to].X, _problem.Points[city_to].Y)));
//            }

//            if (this.InvokeRequired)
//            {
//                this.Invoke(new RefreshDelegate(DoRefresh));
//            }

//            //Application.DoEvents();
//        }

//        private delegate void RefreshDelegate();

//        private void DoRefresh()
//        {
//            if (_fittest != null)
//            {
//                lblBest.Text = _fittest.Fitness.Weight.ToString();
//            }
//            this.pictureBox1.Image = _city_image;
//            this.pictureBox1.Refresh();
//        }

//        private delegate void ReportDelegate(ProgressStatus status);

//        public void Report(ProgressStatus status)
//        {
//            if (this.InvokeRequired)
//            {
//                this.Invoke(new ReportDelegate(Report), status);
//                return;
//            }

//            this.progressBar1.Value = (int)(System.Math.Min(((double)status.CurrentNumber / (double)status.TotalNumber) * 100.0,100.0));
//        }
//    }
//}
