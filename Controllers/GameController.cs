using System.Text;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IDataService _dataService;

    public GameController(IDataService dataService)
    {
        _dataService = dataService;
    }

    /// <summary>
    /// Starts a session with name and size of the maze.
    /// </summary>
    /// <param name="name">name of the player</param>
    /// <param name="x">width</param>
    /// <param name="y">height</param>
    /// <returns></returns>
    [HttpPost("start&name={name}&size={x},{y}", Name = "start")]
    public ActionResult<string> Start(string name, int x, int y)
    {
        var result = _dataService.Start(name, (x, y));
        var sb = new StringBuilder();
        Maze.PrintMaze(sb, result, new (){ (0, 0, '@'), (x - 1, y - 1, 'F'), });
        return Ok($"name: {name}\n\n{sb}");
    }

    /// <summary>
    /// Move player by direction.
    /// </summary>
    /// <param name="name">name of the player</param>
    /// <param name="direction">direction where to move</param>
    /// <returns></returns>
    [HttpPost("move&name={name}&direction={direction}", Name = "move")]
    public ActionResult<string> Move(string name, Direction direction)
    {
        try
        {
            var result = _dataService.Move(name, direction);
            var sb = new StringBuilder();
            Maze.PrintMaze(sb, result.CurrentMap, new (){ (result.PlayerPos.x, result.PlayerPos.y, '@'), (result.FinishPos.x, result.FinishPos.y, 'F'), });
            return Ok($"name: {name}\nresult: {result.Type}\n\n{sb}");
        } 
        catch(Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}