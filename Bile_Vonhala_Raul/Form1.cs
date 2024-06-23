namespace Bile_Vonhala_Raul
{
    public partial class Form1 : Form
    {
        private const int BallCount = 15;
        private List<Ball> balls = new List<Ball>();
        private Random random = new Random();
        bool finished;
        private Brush brush = new SolidBrush(Color.Black);
        public Form1()
        {

            InitializeComponent();
            InitializeBalls();
        }
        private void InitializeBalls()
        {
            for (int i = 0; i < BallCount; i++)
            {
                BallType type = (BallType)random.Next(0, 3);
                int radius = random.Next(10, 20);
                Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                int x = random.Next(radius * 2 + 30, canvas.Width - radius * 2 - 30);
                int y = random.Next(radius * 2 + 30, canvas.Height - radius * 2 - 30);
                int dx = random.Next(-3, 4); // Viteza de la -3 la 3
                int dy = random.Next(-3, 4); // Viteza de la -3 la 3


                balls.Add(new Ball(radius, color, new Point(x, y), dx, dy, type));
            }
        }
        private void DrawBalls(Graphics g)
        {

            foreach (Ball ball in balls)
            {
                Brush brush = new SolidBrush(ball.Color);
                g.FillEllipse(brush, ball.Position.X, ball.Position.Y, ball.Radius * 2, ball.Radius * 2);
                brush.Dispose();
            }
        }
        private void Turn()
        {

            foreach (Ball ball in balls)
            {
                ball.Move(canvas.ClientRectangle);

                foreach (Ball other in balls)
                {
                    if (ball != other && ball.Intersects(other))
                    {
                        ball.Collide(other);
                    }
                }
            }

            balls.RemoveAll(ball => ball.Type == BallType.Regular && ball.Radius <= 0);
            if (balls.FindAll(ball => ball.Type == BallType.Regular).Count == 0)
            {
                finished = true;
            }
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            timer.Start();
        }
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            DrawBalls(e.Graphics);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (!finished)
            {
                Turn();
                canvas.Refresh();
            }
            else
            {
                timer.Stop();
            }
        }

        public enum BallType
        {
            Regular,
            Monster,
            Repellent
        }
        public class Ball
        {
            public int Radius { get; set; }
            public Color Color { get; set; }
            public Point Position { get; set; }
            public int Dx { get; set; }
            public int Dy { get; set; }
            public BallType Type { get; set; }

            public Ball(int radius, Color color, Point position, int dx, int dy, BallType type)
            {
                Radius = radius;
                Color = color;
                Position = position;
                Dx = dx;
                Dy = dy;
                Type = type;
                if (Type == BallType.Monster)
                {
                    dx = 0;
                    dy = 0;
                }
            }
        }
        public void Move(Rectangle bounds)
        {
            Position = new Point(Position.X + Dx, Position.Y + Dy);

            if (Position.X <= bounds.Left || Position.X + Radius * 2 >= bounds.Right)
            {
                Dx = -Dx; // Reflexie pe axa X
            }

            if (Position.Y <= bounds.Top || Position.Y + Radius * 2 >= bounds.Bottom)
            {
                Dy = -Dy; // Reflexie pe axa Y
            }
        }
        public bool Intersects(Ball other)
        {
            int distance = (int)Math.Sqrt(Math.Pow(Position.X - other.Position.X, 2) + Math.Pow(Position.Y - other.Position.Y, 2));
            return distance <= Radius + other.Radius;
        }

        public void Collide(Ball other)
        {
            switch (Type)
            {
                case BallType.Regular:
                    switch (other.Type)
                    {
                        case BallType.Regular:
                            if (Radius > other.Radius)
                            {
                                Radius += other.Radius;
                                Color = CombineColors(Color, other.Color, other.Radius);
                                other.Radius = 0;
                            }
                            else
                            {
                                other.Radius += Radius;
                                other.Color = CombineColors(Color, other.Color, Radius);
                                Radius = 0;
                            }
                            break;
                        case BallType.Monster:
                            other.Radius += Radius;
                            Radius = 0;
                            break;
                        case BallType.Repellent:
                            Color = other.Color;
                            Dx = -Dx; // Schimbare directie
                            break;
                    }
                    break;
                case BallType.Repellent:
                    switch (other.Type)
                    {
                        case BallType.Repellent:
                            Color temp = Color;
                            Color = other.Color;
                            other.Color = temp;
                            break;
                        case BallType.Monster:
                            Radius /= 2;
                            break;
                    }
                    break;
                case BallType.Monster:
                    break;
            }
        }

        private Color CombineColors(Color color1, Color color2, int weight)
        {
            if (weight == 0)
                weight = 1;
            int r = (color1.R * weight + color2.R * (weight * 2)) / (weight * 3);
            int g = (color1.G * weight + color2.G * (weight * 2)) / (weight * 3);
            int b = (color1.B * weight + color2.B * (weight * 2)) / (weight * 3);
            return Color.FromArgb(r, g, b);
        }
    }
}