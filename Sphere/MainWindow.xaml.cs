using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sphere
{
    public partial class MainWindow : Window
    {
        //public static Point3D[,] point;
        //public static MeshGeometry3D mesh;
        //public static DiffuseMaterial diffMat;
        //public static GeometryModel3D geometry;
        //public static ModelUIElement3D model;
        //public static AxisAngleRotation3D axisAngRot3d;
        //public static RotateTransform3D rotation;
        public static int N, n;
        public static double R = 1.5;
        //public static double startTime, angle;
        //public static double[] r;

        public MainWindow()
        {
            InitializeComponent();
            //slider.Visibility = Visibility.Hidden;
            //slider.Value++;
            slider.Value--;
            //startTime = DateTime.Now.Millisecond + DateTime.Now.Second * 1000
            //        + DateTime.Now.Minute * 60000 + DateTime.Now.Hour * 3600000;
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked.Value)
            {
                
            }
            else
            {
                
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            N = (int)slider.Value;
            n = (int)slider.Value + 2;
            vp3d.Children.Clear();
            //angle = (DateTime.Now.Millisecond + DateTime.Now.Second * 1000
            //    + DateTime.Now.Minute * 60000 + DateTime.Now.Hour * 3600000 - startTime) * 0.072;

            vp3d.Camera = new PerspectiveCamera()
            {
                Position = new Point3D(0, 2, 5),
                LookDirection = -(Vector3D)new Point3D(0, 2, 5),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 60
            };

            DirectionalLight dLight = new DirectionalLight()
            {
                Color = Colors.White,
                Direction = new Vector3D(0, -1, -1)
            };

            AxisAngleRotation3D axisAngRot3d = new AxisAngleRotation3D(new Vector3D(1, 1, 1), 0);
            RotateTransform3D rotation = new RotateTransform3D(axisAngRot3d);
            //BindingOperations.SetBinding(axisAngRot3d, AxisAngleRotation3D.AngleProperty, new Binding("Value") { Source = slider });
            //rotation.Rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty,
            //    new DoubleAnimation()
            //    {
            //        From = 0,
            //        To = 360,
            //        Duration = new Duration(TimeSpan.FromSeconds(5)),
            //        RepeatBehavior = RepeatBehavior.Forever
            //    });

            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 0,
                To = 360,
                Duration = new Duration(TimeSpan.FromSeconds(5))
                //RepeatBehavior = RepeatBehavior.Forever
            };
            
            Storyboard storyboard = new Storyboard()
            {
                Duration = new Duration(TimeSpan.FromSeconds(5)),
                RepeatBehavior = RepeatBehavior.Forever
            };
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, axisAngRot3d);
            Storyboard.SetTargetProperty(animation, new PropertyPath(AxisAngleRotation3D.AngleProperty));

            storyboard.Begin();
            
            ModelVisual3D mv3d = new ModelVisual3D();
            mv3d.Content = dLight;
            vp3d.Children.Add(mv3d);

            double[] r = new double[N + 1];
            for (int i = 1; i < N + 1; i++) r[i] = R * Math.Cos(Math.PI / 2 - i * Math.PI / (N + 1));

            Point3D[,] point = new Point3D[N + 2, n];
            point[N + 1, 0].Offset(0, -R, 0);
            point[0, 0].Offset(0, R, 0);
            for (int i = 1; i < N + 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    point[i, j].Offset(-r[i] * Math.Sin(2 * Math.PI / n * j),
                        R * Math.Sin(Math.PI / 2 - i * Math.PI / (N + 1)), r[i] * Math.Cos(2 * Math.PI / n * j));
                }
            }

            MeshGeometry3D mesh = new MeshGeometry3D();
            for (int j = 0; j < n; j++)
            {
                if (j == n - 1)
                {
                    mesh.Positions.Add(point[0, 0]);
                    mesh.Positions.Add(point[1, 0]);
                    mesh.Positions.Add(point[1, j]);
                }
                else
                {
                    mesh.Positions.Add(point[0, 0]);
                    mesh.Positions.Add(point[1, j + 1]);
                    mesh.Positions.Add(point[1, j]);
                }
            }
            for (int i = 1; i < N; i++)
            {
                for (int k = 1; k < 3; k++)
                {
                    if (k % 2 == 1)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (j == n - 1)
                            {
                                mesh.Positions.Add(point[i, j]);
                                mesh.Positions.Add(point[i, 0]);
                                mesh.Positions.Add(point[i + 1, 0]);
                            }
                            else
                            {
                                mesh.Positions.Add(point[i, j]);
                                mesh.Positions.Add(point[i, j + 1]);
                                mesh.Positions.Add(point[i + 1, j + 1]);
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (j == n - 1)
                            {
                                mesh.Positions.Add(point[i, j]);
                                mesh.Positions.Add(point[i + 1, 0]);
                                mesh.Positions.Add(point[i + 1, j]);
                            }
                            else
                            {
                                mesh.Positions.Add(point[i, j]);
                                mesh.Positions.Add(point[i + 1, j + 1]);
                                mesh.Positions.Add(point[i + 1, j]);
                            }
                        }
                    }
                }
            }
            for (int j = 0; j < n; j++)
            {
                if (j == n - 1)
                {
                    mesh.Positions.Add(point[N, j]);
                    mesh.Positions.Add(point[N, 0]);
                    mesh.Positions.Add(point[N + 1, 0]);
                }
                else
                {
                    mesh.Positions.Add(point[N, j]);
                    mesh.Positions.Add(point[N, j + 1]);
                    mesh.Positions.Add(point[N + 1, 0]);
                }
            }
            for (int i = 0; i < 6 * N * n; i++) mesh.TriangleIndices.Add(i);

            DiffuseMaterial diffMat = new DiffuseMaterial(new SolidColorBrush(Colors.Tomato));
            GeometryModel3D geometry = new GeometryModel3D(mesh, diffMat);
            geometry.Transform = rotation;
            ModelUIElement3D model = new ModelUIElement3D();
            model.Model = geometry;

            vp3d.Children.Add(model);
            //startTime = DateTime.Now.Millisecond + DateTime.Now.Second * 1000
            //        + DateTime.Now.Minute * 60000 + DateTime.Now.Hour * 3600000;
        }
    }
}
