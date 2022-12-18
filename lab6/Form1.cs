using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
using System.Windows.Forms;

namespace lab6
{
    using static TransformationMethods;
    using static Projection;
    using static TriangleRasterization;
    using static BackfaceCulling;

    public partial class Form1 : Form
    {


        private PolyhedronType[] polyhedronTypes;
        private string[] polyhedronNames;
        private Projection[] projectionTypes;
        private string[] projectionNames;
        private CoordinatePlaneType[] rotationCoordinatePlaneTypes;
        private string[] rotationCcoordinatePlaneNames;
        private CoordinatePlaneType[] reflectionCoordinatePlaneTypes;
        private string[] reflectionCoordinatePlaneNames;

        private Camera camera = new Camera(0, 0, 0);

        private FacetRemovingType[] facetsRemovingTypes;
        private string[] facetsRemovingNames;
        private FacetRemovingType currentFacetsRemovingType;

        private Axis[] axisTypes;
        private string[] axisNames;
        private Axis currentAxisType;

        private Point previousMousePosition;
        private bool rotating = false;
        private Polyhedron currentPolyhedron;
        private List<Polyhedron> ListPolyhedron;

        private Projection currentProjectionType;
        private CoordinatePlaneType currentRotationCoordinatePlaneType;
        private CoordinatePlaneType currentReflectionCoordinatePlaneType;

        private Edge3d rotateAroundEdge;

        private double MashtabP = 1.1;
        private double MashtabM = 0.9;

        private bool IsTexture = false;
        private Bitmap TextureImage;

        private Point3d LightViewPoint = new Point3d(400, 0, 400);

        public Form1()
        {
            InitializeComponent();
            InitializePolyhedronStuff();
            InitializeProjectionStuff();
            InitializeRotationCoordinatePlaneStuff();
            InitializeReflectionCoordinatePlaneStuff();
            InitializeRotationBodyStuff();
            InitializeFacetsStuff();
            Size = new Size(1400, 800);
        }

        private void InitializePolyhedronStuff()
        {
            polyhedronTypes = Enum.GetValues(typeof(PolyhedronType)).Cast<PolyhedronType>().ToArray();
            polyhedronNames = polyhedronTypes.Select(pt => pt.GetPolyhedronName()).ToArray();
            polyhedronSelectionComboBox.Items.AddRange(polyhedronNames);
            polyhedronSelectionComboBox.SelectedIndex = 0;
            currentPolyhedron = polyhedronTypes[polyhedronSelectionComboBox.SelectedIndex].CreatePolyhedron();
            ListPolyhedron = new List<Polyhedron> {};
            ListPolyhedron.Add(currentPolyhedron);
            ChoiceComboBox.Items.Add(ListPolyhedron.Count);
            ChoiceComboBox.SelectedIndex = ListPolyhedron.Count - 1;
        }

        private void InitializeProjectionStuff()
        {
            projectionTypes = Enum.GetValues(typeof(Projection)).Cast<Projection>().ToArray();
            projectionNames = projectionTypes.Select(pt => pt.ProjectionName()).ToArray();
            projectionSelectionComboBox.Items.AddRange(projectionNames);
            projectionSelectionComboBox.SelectedIndex = 0;
            currentProjectionType = projectionTypes[projectionSelectionComboBox.SelectedIndex];
        }

        private void InitializeRotationCoordinatePlaneStuff()
        {
            rotationCoordinatePlaneTypes = Enum.GetValues(typeof(CoordinatePlaneType)).Cast<CoordinatePlaneType>().ToArray();
            rotationCcoordinatePlaneNames = rotationCoordinatePlaneTypes.Select(cpt => cpt.GetCoordinatePlaneName()).ToArray();
            rotationCoordinatePlaneComboBox.Items.AddRange(rotationCcoordinatePlaneNames);
            rotationCoordinatePlaneComboBox.SelectedIndex = 0;
            currentRotationCoordinatePlaneType = rotationCoordinatePlaneTypes[rotationCoordinatePlaneComboBox.SelectedIndex];
        }

        private void InitializeReflectionCoordinatePlaneStuff()
        {
            reflectionCoordinatePlaneTypes = Enum.GetValues(typeof(CoordinatePlaneType)).Cast<CoordinatePlaneType>().ToArray();
            reflectionCoordinatePlaneNames = reflectionCoordinatePlaneTypes.Select(cpt => cpt.GetCoordinatePlaneName()).ToArray();
            reflectionCoordinatePlaneComboBox.Items.AddRange(reflectionCoordinatePlaneNames);
            reflectionCoordinatePlaneComboBox.SelectedIndex = 0;
            currentReflectionCoordinatePlaneType = rotationCoordinatePlaneTypes[reflectionCoordinatePlaneComboBox.SelectedIndex];
        }

        private void InitializeRotationBodyStuff()
        {
            axisTypes = Enum.GetValues(typeof(Axis)).Cast<Axis>().ToArray();
            axisNames = axisTypes.Select(at => at.GetAxisName()).ToArray();
            chooseRotationBodyAxisComboBox.Items.AddRange(axisNames);
            chooseRotationBodyAxisComboBox.SelectedIndex = 0;
            currentAxisType = axisTypes[chooseRotationBodyAxisComboBox.SelectedIndex];
        }

        private void InitializeFacetsStuff()
        {
            facetsRemovingTypes = Enum.GetValues(typeof(FacetRemovingType)).Cast<FacetRemovingType>().ToArray();
            facetsRemovingNames = facetsRemovingTypes.Select(frt => frt.GetFacetRemovingName()).ToArray();
            facetsRemovingComboBox.Items.AddRange(facetsRemovingNames);
            facetsRemovingComboBox.SelectedIndex = 0;
            currentFacetsRemovingType = facetsRemovingTypes[facetsRemovingComboBox.SelectedIndex];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Project();
        }

        
        private void Project() {
            // Перед проецированием обязательно создается копия т.к.
            // проекция влияет на отображение фигуры а не на перемещение в пространстве
            var size = polyhedronPictureBox.Size;
            var bitmap = new Bitmap(size.Width, size.Height);           
            switch (currentFacetsRemovingType)
            {
                case FacetRemovingType.None:
                    DrawEdges(bitmap);
                    break;
                case FacetRemovingType.ZBuffer:
                    DrawZBuffer(bitmap);
                    break;            
                case FacetRemovingType.BackfaceCulling:
                    DrawBackfaceCulling(bitmap);
                    break;
                default:
                    break;
            }
            polyhedronPictureBox.Image = bitmap;
        }
        
        private void DrawBackfaceCulling(Bitmap drawingSurface)
        {
            var viewPoint = camera.Position;
            foreach (var item in ListPolyhedron)
            {
                if (item != currentPolyhedron)
                {
                    var removedFacets = RemoveBackFacets(item, viewPoint);
                    drawingSurface.DrawPolyhedron(camera.Project(removedFacets, currentProjectionType), Color.Blue);
                }
                else
                {
                    var removedFacets = RemoveBackFacets(currentPolyhedron, viewPoint);
                    drawingSurface.DrawPolyhedron(camera.Project(removedFacets, currentProjectionType), Color.Red);
                }
            }
        }

        private void DrawEdges(Bitmap drawingSurface)
        {
            foreach (var item in ListPolyhedron)
            {
                if (item != currentPolyhedron)
                {
                    drawingSurface.DrawPolyhedron(camera.Project(item, currentProjectionType), Color.Blue);
                }
                else
                {
                    drawingSurface.DrawPolyhedron(camera.Project(currentPolyhedron, currentProjectionType), Color.Red);
                }
            }
        }

        private void MashtabMinus(object sender, System.EventArgs e)
        {
            currentPolyhedron.ScaleCentered(MashtabM);
            Project();
        }

        private void MashtabPlus(object sender, System.EventArgs e)
        {
            currentPolyhedron.ScaleCentered(MashtabP);
            Project();
        }

        private void ReflectXY(object sender, System.EventArgs e)
        {
            currentPolyhedron.ReflectXY();
            Project();
        }

        private void ReflectYZButton(object sender, System.EventArgs e)
        {
            currentPolyhedron.ReflectYZ();
            Project();
        }

        private void ReflectZXButton(object sender, System.EventArgs e)
        {
            currentPolyhedron.ReflectZX();
            Project();
        }

        private void RotateAroundEdgeCentered(object sender, System.EventArgs e)
        {
            var edge3D = new Edge3d(new Point3d(0, 0, 0), new Point3d(1, 1, 1));
            currentPolyhedron.RotateAroundEdge(edge3D, 60);
            Project();
        }

        private void polyhedronPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (!rotating)
            {
                return;
            }

            if (previousMousePosition == null)
            {
                previousMousePosition = e.Location;
            }

            double degreesX, degreesY, degreesZ;

            switch(currentRotationCoordinatePlaneType)
            {
                case CoordinatePlaneType.XY:
                    degreesX = previousMousePosition.X - e.Location.X;
                    degreesY = previousMousePosition.Y - e.Location.Y;
                    degreesZ = 0;
                    break;
                case CoordinatePlaneType.YZ:
                    degreesX = 0;
                    degreesY = previousMousePosition.Y - e.Location.Y;
                    degreesZ = previousMousePosition.X - e.Location.X;
                    break;
                case CoordinatePlaneType.ZX:
                    degreesX = previousMousePosition.X - e.Location.X;
                    degreesY = 0;
                    degreesZ = previousMousePosition.Y - e.Location.Y;
                    break;
                default:
                    throw new ArgumentException("Unknown coordinate plane type");
            }

            currentPolyhedron.RotateAroundCenter(degreesX, degreesY, degreesZ);
            Project();

            previousMousePosition = e.Location;
        }

        private void polyhedronPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            rotating = !rotating;
        }

        private void polyhedronComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            currentPolyhedron = polyhedronTypes[polyhedronSelectionComboBox.SelectedIndex].CreatePolyhedron();
            currentPolyhedron.Translate(200, 200, 0);
            ListPolyhedron.Add(currentPolyhedron);
            ChoiceComboBox.Items.Add(ListPolyhedron.Count);
            ChoiceComboBox.SelectedIndex = ListPolyhedron.Count - 1;
            Project();
        }

        private void projectionSelectionComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            currentProjectionType = projectionTypes[projectionSelectionComboBox.SelectedIndex];
            Project();
        }

        private void rotationAroundEdgeEndPointTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void rotationAroundEdgeAngleButton_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(rotationAroundEdgeBeginPointXTextBox.Text, out var x1))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(rotationAroundEdgeBeginPointYTextBox.Text, out var y1))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(rotationAroundEdgeBeginPointZTextBox.Text, out var z1))
            {
                WarnInvalidInput();
                return;
            }

            if (!double.TryParse(rotationAroundEdgeEndPointXTextBox.Text, out var x2))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(rotationAroundEdgeEndPointYTextBox.Text, out var y2))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(rotationAroundEdgeEndPointZTextBox.Text, out var z2))
            {
                WarnInvalidInput();
                return;
            }

            if (!double.TryParse(rotationAroundEdgeAngleTextBox.Text, out var degrees))
            {
                WarnInvalidInput();
                return;
            }

            rotateAroundEdge = new Edge3d(new Point3d(x1, y1, z1), new Point3d(x2, y2, z2));

            currentPolyhedron.RotateAroundEdge(rotateAroundEdge, degrees);
            Project();
        }

        private void translateButton_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(translationXTextBox.Text, out var dx))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(translationYTextBox.Text, out var dy))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(translationZTextBox.Text, out var dz))
            {
                WarnInvalidInput();
                return;
            }

            currentPolyhedron.Translate(dx, dy, dz);
            Project();
        }

        private void WarnInvalidInput()
        {
            MessageBox.Show("Некорректный ввод");
        }

        private void rotateButton_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(rotationXTextBox.Text, out var xDegrees))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(rotationYTextBox.Text, out var yDegrees))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(rotationZTextBox.Text, out var zDegrees))
            {
                WarnInvalidInput();
                return;
            }

            currentPolyhedron.RotateAxis(xDegrees, yDegrees, zDegrees);
            Project();
        }

        private void scalingButton_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(scalingXTextBox.Text, out var mx))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(scalingYTextBox.Text, out var my))
            {
                WarnInvalidInput();
                return;
            }
            if (!double.TryParse(scalingZTextBox.Text, out var mz))
            {
                WarnInvalidInput();
                return;
            }

            currentPolyhedron.Scale(mx, my, mz);
            Project();
        }

        private void rotationCoordinatePlaneComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            currentRotationCoordinatePlaneType = rotationCoordinatePlaneTypes[rotationCoordinatePlaneComboBox.SelectedIndex];
        }

        private void reflectionCoordinatePlaneComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            currentReflectionCoordinatePlaneType = reflectionCoordinatePlaneTypes[reflectionCoordinatePlaneComboBox.SelectedIndex];
        }

        private void reflectButton_Click(object sender, EventArgs e)
        {
            switch (currentReflectionCoordinatePlaneType)
            {
                case CoordinatePlaneType.XY:
                    currentPolyhedron.ReflectXY();
                    break;
                case CoordinatePlaneType.YZ:
                    currentPolyhedron.ReflectYZ();
                    break;
                case CoordinatePlaneType.ZX:
                    currentPolyhedron.ReflectZX();
                    break;
                default:
                    throw new ArgumentException("Unknown coordinate plane type");
            }
            Project();
        }

        

        delegate double func(double x, double y);
        private void building_function(object sender, EventArgs e)
        {
            func f;
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    f = (arg1, arg2) => (double)(Cos(arg1 * arg1 + arg2 * arg2) / (arg1 * arg1 + arg2 * arg2 + 1));
                    break;
                case 1:
                    f = (arg1, arg2) => (double)(Sin(arg1 + arg2));
                    break;
                case 2:
                    f = (arg1, arg2) => (double)(1 / (1 + arg1 * arg1) + 1 / (1 + arg2 * arg2));
                    break;
                case 3:
                    f = (arg1, arg2) => (double)(Sin(arg1 * arg1 + arg2 * arg2));
                    break;
                case 4:
                    f = (arg1, arg2) => (double)(Sqrt(50 - arg1 * arg1 - arg2 * arg2));
                    break;
                default:
                    f = (arg1, arg2) => 0;
                    break;
            }

            bool b;
            double x0=0;
            double x1=0;
            double y0=0;
            double y1=0;
            int splitting=0;
            b = double.TryParse(x0TextBox.Text,out x0);
            b = b && double.TryParse(x1TextBox.Text, out x1);
            b = b && double.TryParse(y0TextBox.Text, out y0);
            b = b && double.TryParse(y1TextBox.Text, out y1);
            b = b && int.TryParse(splittingTextBox.Text, out splitting);
            if (!b)
            {
                MessageBox.Show("Введите числа корректно!");
                return;
            }
            double x = (x1 - x0) / splitting;
            double y = (y1 - y0) / splitting;
            List < List<Point3d> > Arr = new List<List<Point3d>>();
            List < Point3d > vertices =  new List<Point3d>();
            var edges = new List<Edge3d>();
            var facets = new List<Facet3d>();
            for (int i = 0; i <= splitting; i++)
            {
                List<Point3d> L = new List<Point3d>();
                for (int j = 0; j <= splitting; j++)
                {
                    double x_0 = x0 + i * x;
                    double y_0 = y0 + j * y;
                    double z = f(x_0, y_0);
                    if (z == double.MaxValue)
                        return;
                    Point3d P = new Point3d(x_0, y_0, z);
                    vertices.Add(P);
                    L.Add(P);
                }
                Arr.Add(L);
            }
            for (int i = 0; i < splitting; i++)
                for (int j = 0; j < splitting; j++)
                {
                    var edge0 = new Edge3d(Arr[i][j], Arr[i+1][j]);
                    var edge1 = new Edge3d(Arr[i][j], Arr[i][j+1]);
                    edges.Add(edge0);
                    edges.Add(edge1);
                }
            for (int i = 0; i < splitting; i++)
            {
                var edge0 = new Edge3d(Arr[i][splitting], Arr[i + 1][splitting]);
                edges.Add(edge0);
            }
            for (int j = 0; j < splitting; j++)
            {
                var edge0 = new Edge3d(Arr[splitting][j], Arr[splitting][j+1]);
                edges.Add(edge0);
            }

            for (int i = 0; i < splitting-1; i++)
                for (int j = 0; j < splitting-1; j++)
                    facets.Add(new Facet3d(new List<Point3d> { Arr[i][j], Arr[i+1][j], Arr[i][j+1], Arr[i+1][j+1] }, new List<Edge3d> { edges[2* (splitting*i+j)], edges[2 * (splitting * i + j) + 1], edges[2 * (splitting * i + j) + 2], edges[2 * (splitting * (i+1) + j) + 1] }));
            int t = 2*splitting * splitting;
            for (int i = 0; i < splitting - 1; i++)
                facets.Add(new Facet3d(new List<Point3d> { Arr[i][splitting-1], Arr[i + 1][splitting-1], Arr[i][splitting], Arr[i + 1][splitting] }, new List<Edge3d> { edges[2 * (splitting * i + (splitting - 1))], edges[2 * (splitting * i + (splitting - 1)) + 1], edges[t+i], edges[2 * (splitting * (i + 1) + (splitting - 1)) + 1] }));
            t += splitting;
            for (int j = 0; j < splitting - 1; j++)
                facets.Add(new Facet3d(new List<Point3d> { Arr[splitting - 1][j], Arr[splitting ][j], Arr[splitting - 1][j + 1], Arr[splitting ][j + 1] }, new List<Edge3d> { edges[2 * (splitting * (splitting - 1) + j)], edges[2 * (splitting * (splitting - 1) + j) + 1], edges[2 * (splitting * (splitting - 1) + j) + 2], edges[t+j] }));
            t--;
            facets.Add(new Facet3d(new List<Point3d> { Arr[splitting - 1][splitting - 1], Arr[splitting][splitting - 1], Arr[splitting - 1][splitting], Arr[splitting][splitting] }, new List<Edge3d> { edges[2 * (splitting * (splitting - 1) + (splitting - 1))], edges[2 * (splitting * (splitting - 1) + (splitting - 1)) + 1], edges[t], edges[t+ splitting] }));
            
            currentPolyhedron =  new Polyhedron(vertices, edges, facets);
            ListPolyhedron.Add(currentPolyhedron);
            ChoiceComboBox.Items.Add(ListPolyhedron.Count);
            ChoiceComboBox.SelectedIndex = ListPolyhedron.Count - 1;
            Project();

        }

        private void saveModelIntoFileButton_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                currentPolyhedron.SaveToFile(sfd.FileName);
            }
        }

        private void loadModelronFromFileButton_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                currentPolyhedron = Polyhedron.ReadFromFile(ofd.FileName);
                ListPolyhedron.Add(currentPolyhedron);
                ChoiceComboBox.Items.Add(ListPolyhedron.Count);
                ChoiceComboBox.SelectedIndex = ListPolyhedron.Count - 1;
                Project();
            }
        }

        private void buttonDoTask2_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(partitionsCountTextBox.Text, out int partitionsCount))
            {
                WarnInvalidInput();
                return;
            }

            var splitted = setGeneratrixTextBox.Text.Split();
            var points = new List<Point3d>();
            foreach (var p in splitted)
            {
                var coordinates = p.Split(';');

                if (!double.TryParse(coordinates[0], out var x)) {
                    WarnInvalidInput();
                    return;
                }

                if (!double.TryParse(coordinates[1], out var y))
                {
                    WarnInvalidInput();
                    return;
                }

                if (!double.TryParse(coordinates[2], out var z))
                {
                    WarnInvalidInput();
                    return;
                }

                points.Add(new Point3d(x, y, z));
            }

            var generatrix = new Generatrix(points, currentAxisType);

            currentPolyhedron = generatrix.CreateRotationBody(partitionsCount);
            ListPolyhedron.Add(currentPolyhedron);
            ChoiceComboBox.Items.Add(ListPolyhedron.Count);
            ChoiceComboBox.SelectedIndex = ListPolyhedron.Count - 1;
            Project();
        }
        

        private void chooseRotationBodyAxisComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            currentAxisType = axisTypes[chooseRotationBodyAxisComboBox.SelectedIndex];
        }

        private void ChoiceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int N = ChoiceComboBox.SelectedIndex;
            currentPolyhedron = ListPolyhedron[N];
            Project();
        }

        List<Facet3d> TriangulateFacet(Facet3d facet)
        {
            var triangles = new List<Facet3d>();
            var firstPoint = facet.Points[0];
            for (int i = 2; i < facet.Points.Count; i++)
            {
                var vertices = new List<Point3d> 
                { 
                    firstPoint, facet.Points[i - 1], facet.Points[i] 
                };
                var edges = new List<Edge3d>
                {
                    new Edge3d(firstPoint, facet.Points[i - 1]),
                    new Edge3d(facet.Points[i - 1], facet.Points[i]),
                    new Edge3d(facet.Points[i], firstPoint)
                };
                var triangle = new Facet3d(vertices, edges);
                triangles.Add(triangle);
            }
            return triangles;
        }

        private struct ZBuferStruct
        {
            public bool IsNotEmpty;
            public double Depth;
            public Color Color;
        }

        IEnumerable<DeptherizedPoint> TriangleToListPoint(Facet3d triangle)
        {
            var v1 = DeptherizedPoint.FromPoint3D(triangle.Points[0], LightViewPoint);
            var v2 = DeptherizedPoint.FromPoint3D(triangle.Points[1], LightViewPoint);
            var v3 = DeptherizedPoint.FromPoint3D(triangle.Points[2], LightViewPoint);
            var rasterizedPoints = RasteriseTriangle(v1, v2, v3);
            return rasterizedPoints;
        }

        private void ZBufer(ZBuferStruct[,] ZBuferArr, Facet3d Triangle, Color Clr)
        {
            var size = polyhedronPictureBox.Size;

            var LstPnt = TriangleToListPoint(Triangle);
            foreach (var item in LstPnt)
            {
                double depth = item.Depth;

                if ((item.X >= 0) && (item.X < size.Width) && (item.Y >= 0) && (item.Y < size.Height))
                {
                    if ((!ZBuferArr[item.X, item.Y].IsNotEmpty) || (depth > ZBuferArr[item.X, item.Y].Depth))
                    {
                        var intensivity = item.Intensivity;
                        var intensivityCOlor = Color.FromArgb((int)(Clr.R * intensivity), (int)(Clr.G * intensivity), (int)(Clr.B * intensivity));
                        ZBuferArr[item.X, item.Y].Depth = depth;
                        ZBuferArr[item.X, item.Y].Color = intensivityCOlor;//Clr;
                        ZBuferArr[item.X, item.Y].IsNotEmpty = true;
                    }
                }
            }
        }

        private void PaintZBufer(ZBuferStruct[,] ZBuferArr, Bitmap drawingSurface)
        {
            using (var fastDrawingSurface = new FastBitmap(drawingSurface))
            {
                for (int i = 0; i < fastDrawingSurface.Width; i++)
                {
                    for (int j = 0; j < fastDrawingSurface.Height; j++)
                    {
                        var zBufferItem = ZBuferArr[i, j];
                        if (zBufferItem.IsNotEmpty)
                        {
                            fastDrawingSurface[i, j] = zBufferItem.Color;
                        }
                    }
                }
            }
            //DrawEdges(drawingSurface);
        }

        private void DrawZBuffer(Bitmap drawingSurface)
        {
            var size = drawingSurface.Size;
            var zBuffer = new ZBuferStruct[size.Width, size.Height];

            foreach (var item in ListPolyhedron)
            {
                var itemCopy = camera.Project(item, currentProjectionType);

                var random = new Random();
                foreach (var facets in itemCopy.Facets)
                {
                    var triangulatedFacet = TriangulateFacet(facets);
                    var color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                    foreach (var triangle in triangulatedFacet)
                    {
                        //ZBufer(zBuffer, triangle, itemCopy.Color);
                        ZBufer(zBuffer, triangle, itemCopy.Color);
                    }
                }
            }
            PaintZBufer(zBuffer, drawingSurface);

            polyhedronPictureBox.Image = drawingSurface;
        }

        private void PaintArr(bool[,] Arr, Bitmap drawingSurface)
        {
            int h = drawingSurface.Height;
            for (int i = 0; i < drawingSurface.Width; i++)
                for (int j = 0; j < h; j++)
                    if(Arr[i,j])
                        drawingSurface.SetPixel(i, h-j-1, Color.Red);
        }
        
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //currentFacetsRemovingType = facetsRemovingTypes[facetsRemovingComboBox.SelectedIndex];
        }

        private void facetsRemovingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentFacetsRemovingType = facetsRemovingTypes[facetsRemovingComboBox.SelectedIndex];
            Project();
        }

        private void rotateCameraButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(cameraAngleXTextBox.Text, out int dx))
            {
                WarnInvalidInput();
                return;
            }

            if (!int.TryParse(cameraAngleYTextBox.Text, out int dy))
            {
                WarnInvalidInput();
                return;
            }

            if (!int.TryParse(cameraAngleZTextBox.Text, out int dz))
            {
                WarnInvalidInput();
                return;
            }

            //Point3D CNTR = ListPolyhedron.CommonCenter();
            Point3d CNTR = new Point3d(250, 250, 250);
            foreach (var item in ListPolyhedron)
            {
                item.RotateAroundPoint(dx, dy, dz, CNTR);
            }
            Project();
        }

        private void translateCameraButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(cameraXtranslationTextBox.Text, out int dx))
            {
                WarnInvalidInput();
                return;
            }

            if (!int.TryParse(cameraYtranslationTextBox.Text, out int dy))
            {
                WarnInvalidInput();
                return;
            }

            if (!int.TryParse(cameraZtranslationTextBox.Text, out int dz))
            {
                WarnInvalidInput();
                return;
            }
            foreach (var item in ListPolyhedron)
            {
                item.Translate(-dx, -dy, -dz);
            }
            Project();
        }

      

      

       

        private void zBufferUnite(ZBuferStruct[,] ZBuferArr, ZBuferStruct[,] ZB)
        {
            var size = polyhedronPictureBox.Size;

            for (int i = 0; i < size.Width; i++)
                for (int j = 0; j < size.Height; j++)
                    if (ZB[i, j].IsNotEmpty && ((ZBuferArr[i, j].IsNotEmpty && (ZB[i, j].Depth > ZBuferArr[i, j].Depth)) || !ZBuferArr[i, j].IsNotEmpty))
                        ZBuferArr[i, j] = ZB[i, j];
        }

       

        private Bitmap MakeNaklon(Bitmap TImage, Facet3d facet)
        {
            //TO DO:
            //наклонить изображение так, чтобы оно было в плоскости facet'a
            return TImage;
        }

        private void polyhedronSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void projectionSelectionLabel_Click(object sender, EventArgs e)
        {

        }

        private void polyhedronPictureBox_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}
