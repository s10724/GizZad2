using GizCore;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace GizZad2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        static string[] ColourValues = new string[] {
        "FF0000", "00FF00", "0000FF" , "00FFFF", "000000",
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
    };

        GraphCore graph = new GraphCore();
        GraphViewer graphViewer;
        Graph graphVisualizer;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            graphViewer = new GraphViewer();
            graphViewer.BindToPanel(GraphDockPanel);


            graphViewer.ObjectUnderMouseCursorChanged += GraphViewer_ObjectUnderMouseCursorChanged;
            graphViewer.MouseUp += GraphViewer_MouseUp;


            graph = GraphCore.LoadGraphFromFile(@"C:\Users\rnamedyn\Downloads\Telegram Desktop\bez cyklu.txt");

            DrawGraph();

        }


        private void CheckObject()
        {
            var newObjectId = graphViewer.ObjectUnderMouseCursor?.ToString();
            if (newObjectId != null && lastHoverId != newObjectId)
            {
                lastHoverId = newObjectId;
            }

            var isMarked = graphViewer.ObjectUnderMouseCursor?.MarkedForDragging;
            if (isMarked.HasValue)
            {
                if (isMarked.Value && lastSelectedId != lastHoverId)
                {
                    if (lastSelectedId != null)
                    {
                        Debug.WriteLine("Deselect: " + lastSelectedId);
                    }
                    lastSelectedId = lastHoverId;
                    Debug.WriteLine("Select: " + lastHoverId);
                }
                else if (!isMarked.Value && lastSelectedId == lastHoverId)
                {
                    lastSelectedId = string.Empty;
                    Debug.WriteLine("Deselect: " + lastHoverId);
                }

            }
        }

        private void GraphViewer_MouseUp(object sender, MsaglMouseEventArgs e)
        {
            string selectedNode = lastSelectedId;
            CheckObject();
            if (!string.IsNullOrWhiteSpace(lastSelectedId))
            {
                int lastSelectedGraphId;
                if (int.TryParse(lastSelectedId, out lastSelectedGraphId) && !string.IsNullOrWhiteSpace(selectedNode))
                {
                    var first = graph.FindVertex(graphVisualizer.Nodes.Where(x=>x.Id== selectedNode).FirstOrDefault().LabelText);
                    var second = graph.FindVertex(graphVisualizer.Nodes.Where(x => x.Id == lastSelectedId).FirstOrDefault().LabelText);
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        graph.ConnectVertex(first, second);
                    }
                    else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        graph.RemoveEdge(first, second);
                    }
                    else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                    {
                        graph.RemoveVertex(first);
                    }
                    DrawGraph();
                }
    

            }
        }

        private void GenerateGraphFromPruferCode_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            graph.AddVertex();
            DrawGraph();
        }

        private string lastSelectedId;
        private string lastHoverId;
        private void GraphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {

            CheckObject();
        }

        private void DrawGraph()
        {
            bool IsAnyCycleExist =  graph.IsAnyCycleExist();

            MessageTextBox.Text = IsAnyCycleExist ? "NIE" : "TAK";

            List<string> items;
            graphVisualizer = new Graph();
            graphVisualizer.Attr.LayerDirection = LayerDirection.LR;

            if (IsAnyCycleExist)
            {
                items = graph.Vertices.Select(x => x.Id.ToString()).ToList();
            }
            else
            {
                items = TopologicalSorting.Sort(graph.Vertices.Select(x => x.Id.ToString()).ToList(),
                graph.Edges.Select(x => new Tuple<string, string>(x.First.Id.ToString(), x.Second.Id.ToString())).ToList());
            }


            int j = 0;
            foreach (var item in items)
            {
                var node = new Node((j++).ToString());
                node.Label = new Label(item);
                graphVisualizer.AddNode(node);
            }



            if (!IsAnyCycleExist)
            {
                graphVisualizer.LayerConstraints.AddSequenceOfUpDownVerticalConstraint(graphVisualizer.Nodes.ToArray());
                graphVisualizer.LayoutAlgorithmSettings = new SugiyamaLayoutSettings();
                var sugiyamaSettings = (SugiyamaLayoutSettings)graphVisualizer.LayoutAlgorithmSettings;

                sugiyamaSettings.NodeSeparation = 50;
                sugiyamaSettings.LayerSeparation = 50;
            }


            foreach (var item in graph.Edges)
            {
                var idFirst = graphVisualizer.Nodes.Where(x => x.LabelText == item.First.Id.ToString()).FirstOrDefault().Id;
                var idSecond = graphVisualizer.Nodes.Where(x => x.LabelText == item.Second.Id.ToString()).FirstOrDefault().Id;
                var edge = graphVisualizer.AddEdge(idFirst, idSecond);
            }

            int i = 0;
            foreach (var item in graphVisualizer.Edges)
            {
                var color = System.Drawing.ColorTranslator.FromHtml("#" + ColourValues[i++]);
                item.Attr.Color = new Color(color.A, color.R, color.G, color.B);
                item.Attr.AddStyle(Microsoft.Msagl.Drawing.Style.Dotted);
            }

     

            graphViewer.Graph = graphVisualizer;
        }




        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                graph = GraphCore.LoadGraphFromFile(openFileDialog.FileName);

                DrawGraph();
            }
        }
    }
}
