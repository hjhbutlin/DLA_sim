// TODO
// random position on edge of window rather than anywhere maybe
// colour to represent particle time to aggregate
// properly parameterise scale
// show diffusion path

using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

public class Particle {
    public Vector2i Position;
    public Particle(int x, int y) {
        Position = new Vector2i(x, y);
    }
}

public class Simulation {
    private const int WindowWidth = 800;
    private const int WindowHeight = 800;
    private const int LogicalWidth = 200;
    private const int LogicalHeight = 200;
    private const int ParticleCount = 100;

    private RenderWindow window;
    private List<Particle> particles;
    private HashSet<Vector2i> occupiedPositions;
    private View view;

    public Simulation() {
        window = new RenderWindow(new VideoMode((uint)WindowWidth, (uint)WindowHeight), "DLA Simulation");
        window.Closed += (sender, e) => window.Close();

        view = new View(new FloatRect(0, 0, LogicalWidth, LogicalHeight));

        window.SetView(view);

        particles = new List<Particle>();
        occupiedPositions = new HashSet<Vector2i>();

        var origin = new Particle(LogicalWidth/2, LogicalHeight/2);
        particles.Add(origin);
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
        particles.Add(new Particle(particle.Position.X, particle.Position.Y));
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

        Random random = new Random();

        if (x) {
            if (position.X <= 0) {
                return 1;
                } else if (position.X >= LogicalWidth) {
                    return -1;
                }
            }

            else {
                if (position.Y <= 0) {
                return 1;
                } else if (position.Y >= LogicalHeight) {
                    return -1;
                }
            }

        return random.Next(-1,2);
        }


    public void Run() {
        int particleScale = 1;
        Random random = new Random();
        int i = 1;
        while (window.IsOpen) {
            window.DispatchEvents();
            window.Clear();


            if (particles.Count < ParticleCount+1) {

                System.Console.WriteLine($"{i}/{ParticleCount}");

                // particle at edge of window
                var x = random.Next(0, LogicalWidth);
                var y = random.Next(0, LogicalHeight);

                var newParticle = new Particle(x, y);
                Diffuse(newParticle);
            }

            // draw all the particles as 1x1 pixels
            foreach (var particle in particles)
            {
                var shape = new RectangleShape(new Vector2f(particleScale, particleScale))
                {
                    Position = new Vector2f(particle.Position.X * particleScale, particle.Position.Y * particleScale),
                    FillColor = Color.Red
                };
                window.Draw(shape);
            }

            window.Display();
            i++;
        }
    }

    public static void Main()
    {
        var simulation = new Simulation();
        simulation.Run();
    }
}
