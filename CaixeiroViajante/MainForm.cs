namespace CaixeiroViajante;

public partial class MainForm : Form
{
    private static readonly int[,] coordsDistances = new int[,]
        {
            {0, 20, 0, 0, 0, 0, 0, 29, 0, 0, 0, 29, 37, 0, 0, 0, 0, 0},
            {20, 0, 25, 0, 0, 0, 0, 28, 0, 0, 0, 39, 0, 0, 0, 0, 0, 0},
            {0, 25, 0, 25, 0, 0, 0, 30, 0, 0, 0, 0, 54, 0, 0, 0, 0, 0},
            {0, 0, 25, 0, 39, 32, 42, 0, 23, 33, 0, 0, 0, 56, 0, 0, 0, 0},
            {0, 0, 0, 39, 0, 12, 26, 0, 0, 19, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 32, 12, 0, 17, 0, 0, 35, 30, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 42, 26, 17, 0, 0, 0, 0, 38, 0, 0, 0, 0, 0, 0, 0},
            {29, 28, 30, 0, 0, 0, 0, 0, 0, 0, 0, 25, 22, 0, 0, 0, 0, 0},
            {0, 0, 0, 23, 0, 0, 0, 0, 0, 26, 0, 0, 34, 0, 0, 0, 0, 0},
            {0, 0, 0, 33, 19, 35, 0, 0, 26, 0, 24, 0, 0, 30, 19, 0, 0, 0},
            {0, 0, 0, 0, 0, 30, 38, 0, 0, 24, 0, 0, 0, 0, 26, 0, 0, 36},
            {29, 39, 0, 0, 0, 0, 0, 25, 0, 0, 0, 0, 27, 0, 0, 43, 0, 0},
            {37, 0, 54, 0, 0, 0, 0, 22, 34, 0, 0, 27, 0, 24, 0, 19, 0, 0},
            {0, 0, 0, 56, 0, 0, 0, 0, 0, 30, 0, 0, 24, 0, 20, 19, 17, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 19, 26, 0, 0, 20, 0, 0, 18, 21},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 43, 0, 43, 19, 19, 0, 0, 26, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 17, 18, 26, 0, 15},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 36, 0, 0, 0, 21, 0, 15, 0}
        };

    /* private static readonly int[,] coordsDistances = new int[,]
        {
            { 0, 2, 0, 3, 6 },
            { 2, 0, 4, 3, 0 },
            { 0, 4, 0, 7, 3 },
            { 3, 3, 7, 0, 3 },
            { 6, 0, 3, 3, 0 }
        }; */

    private Panel graphPanel = null!;
    private Dictionary<int, PointF> nodePositions = null!;
    private List<(int from, int to, int weight)> edges = null!;
    private const int NodeRadius = 20;
    
    private Panel controlPanel = null!;
    private NumericUpDown startNodeInput = null!;
    private Button solveButton = null!;
    private Label statusLabel = null!;
    private Label resultLabel = null!;
    private TravellingSalesmanSolver? solver;
    
    public MainForm()
    {
        InitializeComponent();
        this.Size = new Size(1000, 700);
        InitializeGraphPanel();
        InitializeControlPanel();

        BuildGraph();
        CalculateNodePositions();
        solver = new TravellingSalesmanSolver(coordsDistances);
    }

    private void InitializeControlPanel()
    {
        controlPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 100,
            BackColor = Color.FromArgb(240, 240, 240),
            Padding = new Padding(15)
        };

        Label startLabel = new Label
        {
            Text = "Cidade Inicial:",
            Location = new Point(15, 18),
            Width = 100,
        };

        startNodeInput = new NumericUpDown
        {
            Location = new Point(120, 15),
            Width = 60,
            Minimum = 1,
            Maximum = coordsDistances.GetLength(0),
            Value = 1,
        };

        Label infoLabel = new Label
        {
            Text = "(Visitará todas as cidades e retornará à origem)",
            Location = new Point(190, 18),
            Width = 350,
            ForeColor = Color.FromArgb(100, 100, 100),
            Font = new Font("Segoe UI", 8, FontStyle.Italic)
        };

        solveButton = new Button
        {
            Text = "Calcular Melhor Rota",
            Location = new Point(550, 13),
            Width = 200,
            Height = 30,
        };
        solveButton.Click += SolveButton_Click;

        statusLabel = new Label
        {
            Text = "Pronto para calcular a rota do Caixeiro Viajante",
            Location = new Point(15, 55),
            Width = 900,
            Height = 20,
            ForeColor = Color.FromArgb(0, 100, 180),
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };

        resultLabel = new Label
        {
            Text = "",
            Location = new Point(15, 75),
            Width = 900,
            Height = 20,
            ForeColor = Color.FromArgb(0, 130, 0),
            Font = new Font("Segoe UI", 9, FontStyle.Regular)
        };

        controlPanel.Controls.AddRange(
        [
            startLabel, startNodeInput, infoLabel,
            solveButton, statusLabel, resultLabel 
        ]);

        this.Controls.Add(controlPanel);
    }

    private async void SolveButton_Click(object? sender, EventArgs e)
    {
        if (solver == null) return;

        int startNode = (int)startNodeInput.Value - 1;

        solveButton.Enabled = false;
        statusLabel.Text = "Calculando...";
        statusLabel.ForeColor = Color.Orange;
        resultLabel.Text = "";

        var progress = new Progress<string>(message =>
        {
            statusLabel.Text = message;
            statusLabel.Refresh();
            Application.DoEvents();
        });

        try
        {
            var result = await Task.Run(() => solver.Solve(startNode, progress));

            if (result.Success)
            {
                string pathStr = string.Join(" → ", result.BestPath.Select(n => n + 1));
                statusLabel.Text = $"Concluído em {result.ElapsedTime.TotalSeconds:F2}s";
                statusLabel.ForeColor = Color.Green;
                resultLabel.Text = $"Melhor Rota: {pathStr} | Distância Total: {result.BestDistance} | Rotas exploradas: {result.PermutationsChecked:N0}";
            }
            else
            {
                statusLabel.Text = result.Message;
                statusLabel.ForeColor = Color.Red;
                resultLabel.Text = $"Tempo: {result.ElapsedTime.TotalSeconds:F2}s";
            }
        }
        catch (Exception ex)
        {
            statusLabel.Text = "Erro: " + ex.Message;
            statusLabel.ForeColor = Color.Red;
        }
        finally
        {
            solveButton.Enabled = true;
        }
    }
    
    private void InitializeGraphPanel()
    {
        graphPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Name = "graphPanel"
        };
        graphPanel.Paint += GraphPanel_Paint;
        this.Controls.Add(graphPanel);
    }

    private void BuildGraph()
    {
        edges = [];
        int numNodes = coordsDistances.GetLength(0);

        for (int i = 0; i < numNodes; i++)
        {
            for (int j = i + 1; j < numNodes; j++)
            {
                if (coordsDistances[i, j] > 0)
                {
                    edges.Add((i, j, coordsDistances[i, j]));
                }
            }
        }
    }

    private void CalculateNodePositions()
    {
        int numNodes = coordsDistances.GetLength(0);
        nodePositions = [];
        
        Random rand = new(123);
        float width = graphPanel.Width;
        float height = graphPanel.Height;
        
        for (int i = 0; i < numNodes; i++)
        {
            nodePositions[i] = new PointF(
                (float)(rand.NextDouble() * (width - 100) + 50),
                (float)(rand.NextDouble() * (height - 100) + 50)
            );
        }
        
        float k = (float)Math.Sqrt(width * height / numNodes); // Constante de mola ideal
        int iterations = 100;
        
        for (int iter = 0; iter < iterations; iter++)
        {
            Dictionary<int, PointF> displacement = [];
            
            for (int i = 0; i < numNodes; i++)
            {
                displacement[i] = new PointF(0, 0);
            }
            
            for (int i = 0; i < numNodes; i++)
            {
                for (int j = i + 1; j < numNodes; j++)
                {
                    PointF delta = new(
                        nodePositions[i].X - nodePositions[j].X,
                        nodePositions[i].Y - nodePositions[j].Y
                    );
                    float distance = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                    
                    if (distance < 0.01f) distance = 0.01f;
                    
                    float repulsiveForce = k * k / distance;
                    PointF force = new(
                        delta.X / distance * repulsiveForce,
                        delta.Y / distance * repulsiveForce
                    );
                    
                    displacement[i] = new PointF(
                        displacement[i].X + force.X,
                        displacement[i].Y + force.Y
                    );
                    displacement[j] = new PointF(
                        displacement[j].X - force.X,
                        displacement[j].Y - force.Y
                    );
                }
            }
            
            foreach (var (from, to, weight) in edges)
            {
                PointF delta = new(
                    nodePositions[from].X - nodePositions[to].X,
                    nodePositions[from].Y - nodePositions[to].Y
                );
                float distance = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                
                if (distance < 0.01f) distance = 0.01f;
                
                float attractiveForce = distance * distance / k;
                PointF force = new(
                    delta.X / distance * attractiveForce,
                    delta.Y / distance * attractiveForce
                );
                
                displacement[from] = new PointF(
                    displacement[from].X - force.X,
                    displacement[from].Y - force.Y
                );
                displacement[to] = new PointF(
                    displacement[to].X + force.X,
                    displacement[to].Y + force.Y
                );
            }
            
            float temperature = width / 10f * (1f - (float)iter / iterations);
            
            for (int i = 0; i < numNodes; i++)
            {
                float dispLength = (float)Math.Sqrt(
                    displacement[i].X * displacement[i].X + 
                    displacement[i].Y * displacement[i].Y
                );
                
                if (dispLength > 0.01f)
                {
                    float limitedDisp = Math.Min(dispLength, temperature);
                    nodePositions[i] = new PointF(
                        nodePositions[i].X + (displacement[i].X / dispLength) * limitedDisp,
                        nodePositions[i].Y + (displacement[i].Y / dispLength) * limitedDisp
                    );
                    
                    nodePositions[i] = new PointF(
                        Math.Max(50, Math.Min(width - 50, nodePositions[i].X)),
                        Math.Max(50, Math.Min(height - 50, nodePositions[i].Y))
                    );
                }
            }
        }
    }

    private void GraphPanel_Paint(object? sender, PaintEventArgs e)
    {
        if (nodePositions == null || edges == null) return;
        
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        if (nodePositions.Count == 0)
        {
            CalculateNodePositions();
        }

        using (Pen edgePen = new(Color.Gray, 1.5f))
        using (Font weightFont = new("Arial", 8, FontStyle.Bold))
        using (SolidBrush weightBrush = new(Color.Blue))
        {
            foreach (var edge in edges)
            {
                PointF from = nodePositions[edge.from];
                PointF to = nodePositions[edge.to];
                
                g.DrawLine(edgePen, from, to);

                float midX = (from.X + to.X) / 2;
                float midY = (from.Y + to.Y) / 2;
                
                string weightText = edge.weight.ToString();
                SizeF textSize = g.MeasureString(weightText, weightFont);
                RectangleF textRect = new(
                    midX - textSize.Width / 2 - 2,
                    midY - textSize.Height / 2 - 1,
                    textSize.Width + 4,
                    textSize.Height + 2
                );
                g.FillRectangle(Brushes.White, textRect);
                
                StringFormat sf = new()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(weightText, weightFont, weightBrush, midX, midY, sf);
            }
        }

        using SolidBrush nodeBrush = new(Color.Red);
        using Pen nodeBorder = new(Color.DarkRed, 2);
        using Font nodeFont = new("Arial", 10, FontStyle.Bold);
        using SolidBrush textBrush = new(Color.White);
        foreach (var node in nodePositions)
        {
            float x = node.Value.X - NodeRadius;
            float y = node.Value.Y - NodeRadius;

            g.FillEllipse(nodeBrush, x, y, NodeRadius * 2, NodeRadius * 2);
            g.DrawEllipse(nodeBorder, x, y, NodeRadius * 2, NodeRadius * 2);

            // Desenhar número do nó
            StringFormat sf = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            g.DrawString((node.Key + 1).ToString(), nodeFont, textBrush,
                node.Value.X, node.Value.Y, sf);
        }
    }
}
