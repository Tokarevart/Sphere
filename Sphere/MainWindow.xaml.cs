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
using System.Windows.Threading;

namespace Sphere
{
    public partial class MainWindow : Window
    {
        public static AxisAngleRotation3D 
            axisAngRot3dHoriz, axisAngRot3dVert, axisAngRot3dDiag, 
            camAxisAngRot3dHoriz, camAxisAngRot3dVert;
        public static RotateTransform3D rotationHoriz, rotationVert, rotationDiag;
        public static Transform3DGroup trans3dGroup = new Transform3DGroup();
        public static Point P0, P1;
        public static DispatcherTimer
            autoRotationTimer =
            new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(10) },
            autoChangeVertNumTimer =
            new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(10) };
        public static bool isInitialized = false;
        public static int N, n;
        public static double 
            R = 1.5,
            dVertNumSliderValue = 1;

        public MainWindow()
        {
            InitializeComponent();
            isInitialized = true;


            DirectionalLight dLight = new DirectionalLight()
            {
                Color = Colors.White,
                Direction = new Vector3D(0, -1, -1)
            };

            ModelVisual3D mv3d = new ModelVisual3D()
            {
                Content = dLight
            };
            vp3d.Children.Add(mv3d);


            axisAngRot3dHoriz = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            rotationHoriz = new RotateTransform3D(axisAngRot3dHoriz);

            axisAngRot3dVert = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            rotationVert = new RotateTransform3D(axisAngRot3dVert);

            axisAngRot3dDiag = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            rotationDiag = new RotateTransform3D(axisAngRot3dDiag);
            
            trans3dGroup.Children = new Transform3DCollection() { rotationHoriz, rotationVert, rotationDiag };


            vp3d.Camera = new PerspectiveCamera()
            {
                Position = new Point3D(0, 2, 5),
                LookDirection = -(Vector3D)new Point3D(0, 2, 5),
                UpDirection = new Vector3D(0, 1, 0),
                FieldOfView = 60
            };

            camAxisAngRot3dHoriz = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            RotateTransform3D camRotationHoriz = new RotateTransform3D(camAxisAngRot3dHoriz);

            camAxisAngRot3dVert = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            RotateTransform3D camRotationVert = new RotateTransform3D(camAxisAngRot3dVert);

            Transform3DGroup camTrans3dGroup = new Transform3DGroup();
            camTrans3dGroup.Children = new Transform3DCollection() { camRotationHoriz, camRotationVert };

            vp3d.Camera.Transform = camTrans3dGroup;


            autoRotationTimer.Tick += (o, e) =>
            {
                if (!autoRotationCheckBox.IsChecked.Value)
                    autoRotationTimer.Stop();

                axisAngRot3dHoriz.Angle += 1;
                axisAngRot3dVert.Angle += 1;
            };


            autoChangeVertNumTimer.Tick += (o, e) =>
            {
                if (!autoChangeVertNumCheckBox.IsChecked.Value)
                    autoChangeVertNumTimer.Stop();

                if (vertNumSlider.Value >= vertNumSlider.Maximum - 0.005 ||
                    vertNumSlider.Value <= vertNumSlider.Minimum + 0.005)
                    dVertNumSliderValue *= -1;

                vertNumSlider.Value += dVertNumSliderValue / 10;
            };

            
            vertNumSlider.Value--;
            dVertNumSliderValue = changeVertNumVelSlider.Value;
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Перетаскивайте курсор мыши с зажатой лековой кнопкой мыши, чтобы вращать сферу.\n" +
                "С зажатой правой кнопкой мыши - чтобы вращать камеру вокруг сферы.");
        }

        private void autoRotationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            autoRotationTimer.Start();
        }
        
        private void autoChangeVertNumCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            autoChangeVertNumTimer.Start();
        }

        private void mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.GetPosition(mainWindow).Y >= 100)
            {
                P0 = Mouse.GetPosition(mainWindow);
                mainWindow.Cursor = Cursors.SizeAll;
            }
        }

        private void mainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            P1 = Mouse.GetPosition(mainWindow);
            
            if (Mouse.LeftButton == MouseButtonState.Pressed && 
                P1.Y >= 100)
            {
                axisAngRot3dHoriz.Angle += P1.X - P0.X;
                axisAngRot3dVert.Angle += P1.Y - P0.Y;
            }
            else if (Mouse.RightButton == MouseButtonState.Pressed &&
                P1.Y >= 100)
            {
                camAxisAngRot3dHoriz.Angle -= P1.X - P0.X;
                camAxisAngRot3dVert.Angle -= P1.Y - P0.Y;
            }

            P0 = P1;
        }

        private void mainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mainWindow.Cursor = Cursors.Arrow;
        }

        private void changeVertNumVelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dVertNumSliderValue = Math.Sign(dVertNumSliderValue) * changeVertNumVelSlider.Value;
        }

        private void vertNumSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!isInitialized)
                return;

            N = (int)vertNumSlider.Value;
            n = (int)vertNumSlider.Value + 2;
            if (vp3d.Children.Count == 2)
                vp3d.Children.RemoveAt(1);

            AddSphere();
        }

        private void AddSphere()
        {
            double[] r = new double[N + 1];
            for (int i = 1; i < N + 1; i++)
                r[i] = R * Math.Cos(Math.PI / 2 - i * Math.PI / (N + 1));

            Point3D[,] point = new Point3D[N + 2, n];
            point[N + 1, 0].Offset(0, -R, 0);
            point[0, 0].Offset(0, R, 0);
            for (int i = 1; i < N + 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    point[i, j].Offset(
                        -r[i] * Math.Sin(2 * Math.PI / n * j),
                        R * Math.Sin(Math.PI / 2 - i * Math.PI / (N + 1)),
                        r[i] * Math.Cos(2 * Math.PI / n * j));
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
            for (int i = 0, max = 6 * N * n; i < max; i++)
                mesh.TriangleIndices.Add(i);

            DiffuseMaterial diffMat = new DiffuseMaterial(new SolidColorBrush(Colors.Tomato));
            GeometryModel3D geometry = new GeometryModel3D(mesh, diffMat);
            geometry.Transform = trans3dGroup;
            ModelUIElement3D model = new ModelUIElement3D();
            model.Model = geometry;

            vp3d.Children.Add(model);
        }
    }
}
