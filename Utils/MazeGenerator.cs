using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public class Maze
{
    const int width = 10;
    const int height = 10;
    
    public struct Cell
    {
        public bool Visited;
        public bool[] Walls;
    }

    public static (bool[,], bool[,]) GenerateLevel(int w, int h)
    {
        const double HOLE_PROBABILITY = 0.12;
        var hwalls = new bool[w + 1, h];
        var vwalls = new bool[w, h + 1];
        var st = new int[w, h];

        void dfs(int x, int y)
        {
            st[x, y] = 1;
            var dirs = new[]
            {
                (x - 1, y, hwalls, x, y, 90, Direction.Left),
                (x + 1, y, hwalls, x + 1, y, 90, Direction.Right),
                (x, y - 1, vwalls, x, y, 0, Direction.Down),
                (x, y + 1, vwalls, x, y + 1, 0, Direction.Up),
            };

            foreach (var (nx, ny, wall, wx, wy, ang, k) in dirs.OrderBy(d => Random.Shared.NextDouble()))
            {
                if (!(0 <= nx && nx < w && 0 <= ny && ny < h) || (st[nx, ny] == 2 && Random.Shared.NextDouble() > HOLE_PROBABILITY))
                {
                    wall[wx, wy] = true;
                }
                else if (st[nx, ny] == 0)
                {
                    dfs(nx, ny);
                }
            }

            st[x, y] = 2;
        }
        dfs(0, 0);

        return (hwalls, vwalls);
    }

    public static Cell[,] GetNew(int w, int h)
    {
        var walls = GenerateLevel(w + 1, h + 1);
        var cells = new Cell[w, h];
        for(int y = 0; y < h; y++)
        {
            for(int x = 0; x < w; x++)
            {
                cells[x, y] = new Cell()
                {
                    Walls = new bool[]{
                        y == 0 || walls.Item2[x, y],
                        y == h - 1 || walls.Item2[x, y + 1],
                        walls.Item1[x, y],
                        walls.Item1[x + 1, y],
                    }
                };
            }
        }
        return cells;
    }

    public static void PrintMaze(StringBuilder builder, Cell[,] maze, List<(int x, int y, char)> specialCoordinates)
    {
        // Top boundary
        for (int x = 0; x < width; x++)
        {
            builder.Append("+---");
        }
        builder.AppendLine("+");

        for (int y = 0; y < height; y++)
        {
            // Left boundary of each row
            for (int x = 0; x < width; x++)
            {
                // Check if there's a special character at this coordinate
                char specialChar = ' ';
                foreach (var (sx, sy, sc) in specialCoordinates)
                {
                    if (sx == x && sy == y)
                    {
                        specialChar = sc;
                        break;
                    }
                }

                // Print left wall and special character (or space)
                builder.Append(maze[x, y].Walls[(int)Direction.Left] ? "|" : " ");
                builder.Append($" {specialChar} ");
            }
            builder.AppendLine("|");

            // Bottom boundary of each cell
            for (int x = 0; x < width; x++)
            {
                builder.Append(maze[x, y].Walls[(int)Direction.Down] ? "+---" : "+   ");
            }
            builder.AppendLine("+");
        }
    }
}