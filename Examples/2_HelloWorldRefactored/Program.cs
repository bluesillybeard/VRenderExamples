/*
EXAMPLE TWO: VRENDER Hello World.. again?

Basically, it's a refactored version of the original Hello World example,
To better demonstrate how to utilze Vrender's functionality more effectively.

The changes from example one are:
- Refactor game functionality to a separate Game class.
- 
*/

using VRenderLib;
using VRenderLib.Interface;
using OpenTK.Mathematics;
public class Program
{
    public static Game? game;
    public static void Main()
    {
        RenderSettings settings = new RenderSettings()
        {
            TargetFrameTime = 1/60f,
            WindowTitle = "VRender Hello World... again?",
            BackgroundColor = 0x000000FF,
            size = new Vector2i(800, 600),

        };
        VRender.InitRender(settings);
        VRender.Render.OnStart = Start;
        VRender.Render.OnDraw = Draw;
        VRender.Render.Run();
    }


    //The reason assets are initialized here instead of in Main Is because it allows asynchronously loading assets.
    // The example after the next will demonstrate how and why.
    public static void Start()
    {
        game = new Game();

    }

    public static void Draw(TimeSpan delta)
    {
        if(game is not null)game.Draw(delta);
    }
}