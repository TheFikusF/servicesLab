public enum Direction
{
    Up = 0, 
    Down = 1, 
    Left = 2, 
    Right = 3,
}

public enum ResultType
{
    Success, Failure, Finish
}

public struct Result
{
    public ResultType Type;
    public (int x, int y) PlayerPos;
    public (int x, int y) FinishPos;
    public Maze.Cell[,] CurrentMap;
}

public class DataService : IDataService
{
    private Dictionary<string, Maze.Cell[,]> _sessions = new(); 
    private Dictionary<string, (int x, int y)> _sizes = new(); 
    private Dictionary<string, (int x, int y)> _positions = new(); 

    private (int x, int y) this[Direction direction] => direction switch
    {
        Direction.Up => (0, -1),
        Direction.Down => (0, 1),
        Direction.Left => (-1, 0),
        Direction.Right => (1, 0),
        _ => (0, 0),
    };

    public Maze.Cell[,] Start(string name, (int x, int y) size)
    {
        _sessions[name] = Maze.GetNew(size.x, size.y);
        _positions[name] = (0, 0);
        _sizes[name] = size;

        return _sessions[name];
    }

    public Result Move(string name, Direction direction)
    {
        if(_sessions.ContainsKey(name) == false)
        {
            throw new Exception($"There is no session with name: {name}");
        }

        var position = _positions[name];
        var size = _sizes[name];
        var (dX, dY) = this[direction];
        
        ResultType MoveAction()
        {
            _positions[name] = (position.x + dX, position.y + dY);
            return _positions[name].x == size.x - 1 && _positions[name].y == size.y - 1 ? ResultType.Finish : ResultType.Success;
        }

        Result FinishMove(ResultType type) => new Result() { Type = type, PlayerPos = _positions[name], FinishPos = (size.x - 1, size.y - 1), CurrentMap = _sessions[name] };

        if(position.x == size.x - 1 && position.y == size.y - 1)
        {
            return FinishMove(ResultType.Finish);
        }

        if(_sessions[name][position.x, position.y].Walls[(int)direction] == false)
        {
            return FinishMove(MoveAction());
        }

        return FinishMove(ResultType.Failure);
    }
}