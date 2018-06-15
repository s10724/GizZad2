using GizCore;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GizZad2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GraphCore graph = new GraphCore();
        GraphViewer graphViewer;
        Graph graphVisualizer;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            graphViewer = new GraphViewer();
            graphViewer.BindToPanel(GraphDockPanel);


            graphViewer.ObjectUnderMouseCursorChanged += GraphViewer_ObjectUnderMouseCursorChanged;
            graphViewer.MouseUp += GraphViewer_MouseUp;


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
                int lastSelectedGraphId = int.Parse(lastSelectedId);

                if (!string.IsNullOrWhiteSpace(selectedNode))
                {
                    var first = graph.FindVertex(selectedNode);
                    var second = graph.FindVertex(lastSelectedId);
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

        private void GenerateGraphFromPruferCode_Click(object sender, RoutedEventArgs e)
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
            graphVisualizer = new Graph();
            graphVisualizer.Attr.LayerDirection = LayerDirection.None;
            graphVisualizer.Attr.OptimizeLabelPositions = true;
            foreach (var item in graph.Vertices)
            {
                graphVisualizer.AddNode(item.Id.ToString());
            }

            foreach (var item in graph.Edges)
            {
                var edge = graphVisualizer.AddEdge(item.First.Id.ToString(), item.Second.Id.ToString());
            }


            graphViewer.Graph = graphVisualizer;
            MessageTextBox.Text = graph.IsAnyCycleExist() ? "NIE" : "TAK";

        }
        private void Button_Click(object sender, RoutedEventArgs e)
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
