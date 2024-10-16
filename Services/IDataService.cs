public interface IDataService
{
    Maze.Cell[,] Start(string name, (int x, int y) size);
    Result Move(string name, Direction direction);
}