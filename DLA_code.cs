using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Particle {
    public Vector2i Position;
    public Particle(int x, int y) {
        Position = new Vector2i(x, y);
    }
}

public class Simulation {
    private Random random = new Random();
    private const int WindowWidth = 1000;
    private const int WindowHeight = 600;
    private const int LogicalWidth = 50;
    private const int LogicalHeight = 30;
    private const int ParticleCount = 200;

    private RenderWindow window;
    private HashSet<Vector2i> occupiedPositions;
    private View view;

    public Simulation() {
        window = new RenderWindow(new VideoMode((uint)WindowWidth, (uint)WindowHeight), "DLA Simulation");
        window.Closed += (sender, e) => window.Close();

        view = new View(new FloatRect(0, 0, LogicalWidth, LogicalHeight));

        window.SetView(view);

        occupiedPositions = new HashSet<Vector2i>();

        var origin = new Particle(LogicalWidth/2, LogicalHeight/2);
        occupiedPositions.Add(origin.Position);
    }

    private void Diffuse(Particle particle) {

        while (! IsAdjacent(particle.Position)) {
            int dx = RandomWithEdgeCase(particle.Position, true); // true => x edge case
            int dy = RandomWithEdgeCase(particle.Position, false); // false +> y case

            particle.Position = new Vector2i(particle.Position.X + dx, particle.Position.Y + dy);
            }
        // once particle is adjacent to another particle, it remains there.
        occupiedPositions.Add(particle.Position);
        }

    private bool IsAdjacent(Vector2i position) {
        // 8 positions to check
        foreach (var offset in new[] { new Vector2i(-1, -1), new Vector2i(0, -1), new Vector2i(1, -1),
                                       new Vector2i(-1, 0), new Vector2i(1, 0),
                                       new Vector2i(-1, 1), new Vector2i(0, 1), new Vector2i(1, 1) })
        {
            var neighbor = position + offset;
            if (occupiedPositions.Contains(neighbor))
                return true;
        }
        return false;
    }

    private int RandomWithEdgeCase(Vector2i position, bool x) {
        if (x) {
            return position.X <= 0 ? 1 : (position.X >= LogicalWidth ? -1 : random.Next(-1, 2));
        } else {
            return position.Y <= 0 ? 1 : (position.Y >= LogicalHeight ? -1 : random.Next(-1, 2));
        }
    }


    public void Run() {
        while (window.IsOpen) {
            window.DispatchEvents();
            window.Clear();


            if (occupiedPositions.Count <= ParticleCount) {

                System.Console.WriteLine($"{occupiedPositions.Count}/{ParticleCount}");

                // particle at edge of window
                var x = random.Next(0, LogicalWidth);
                var y = random.Next(0, LogicalHeight);

                var newParticle = new Particle(x, y);
                Diffuse(newParticle);
            }

            // draw all the particles as 1x1 pixels
            foreach (var position in occupiedPositions) {
                var shape = new RectangleShape(new Vector2f(1, 1))
                {
                    Position = new Vector2f(position.X, position.Y),
                    FillColor = Color.Cyan
                };
                window.Draw(shape);
            }

            window.Display();
        }
    }

    public static void Main() {
        var simulation = new Simulation();
        simulation.Run();
    }
}
