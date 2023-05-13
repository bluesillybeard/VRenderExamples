/*
EXAMPLE ONE: VRENDER Hello World

This is supposed to be as simple as possible, to get a gist of how VRender's API is organized.
*/

using VRenderLib;
using VRenderLib.Interface;
using OpenTK.Mathematics;
using vmodel;
public class Program
{
    public static void Main()
    {
        RenderSettings settings = new RenderSettings()
        {
            TargetFrameTime = 1/60f,
            WindowTitle = "VRender Hello World",
            BackgroundColor = 0x000000FF,
            size = new Vector2i(800, 600),

        };
        VRender.InitRender(settings);
        VRender.Render.OnStart = Start;
        VRender.Render.OnDraw = Draw;
        VRender.Render.Run();
    }
    //one should create a separate Game class that gets constructed in the Start method,
    // then objects are loaded in that constructor.
    // That way, C# doesn't complain about uninitialized objects. The next example will demonstrate that.
    #nullable disable
    static IRenderShader shader;
    static IRenderMesh mesh;
    static IRenderTexture texture;
    #nullable restore

    //The reason assets are initialized here instead of in Main Is because it allows asynchronously loading assets.
    // The example after the next will demonstrate how and why.
    public static void Start()
    {
        //Instead of loading a mesh from a file, just define it right here.
        // It's just a square, so it's not worth making an entire 3D model for it.
        VMesh m = new VMesh(
            new float[]{
                // First 3 are X Y Z position, next two are X Y texture coordinates
                // Not always the case, VRender can handle any theoretical combination of vertex attributes
                -1, -1, 0, 0, 0,
                -1,  1, 0, 0, 1,
                1,  -1, 0, 1, 0,
                1,   1, 0, 1, 1,
            },
            new uint[]{
                0, 1, 2,
                1, 2, 3,
            },
            //This lets the mesh know what the actual vertex attributes are. 
            new Attributes(new EAttribute[]{EAttribute.position, EAttribute.textureCoords}),
            null
        );
        //Normally, you want to use the async versions to speed it up.
        // But in this case there are only three assets to load, so don't worry.

        //Load the mesh into the GPU.
        mesh = VRender.Render.LoadMesh(m);
        //Get a shader program.
        // Shaders can also be in GLSL code. The function used here is for basic rendering functionality.
        // It assumes the attributes has at least a "EAttribute.position" and one or more color sources (texture, RGB or RGBA),
        // it will throw an exception if not.
        //If there are multiple color-proving attributes (For example both texture coords and an RGBA color),
        // It will try to blend them in the order that they appear in the shader.
        shader = VRender.Render.GetShader(new ShaderFeatures(m.attributes, true, false));
        //Fun fact, the dice.png file is from my first ever GMTK game jam submission.
        // It's not an asset of where it came from, since the entire game is drawn programatically.
        // I just took a screenshot of the rendered result and cropped it into a png.
        var textureOrNone = VRender.Render.LoadTexture("dice.png", out var textureException);
        if(textureOrNone is null)
        {
            throw new Exception("Failed to load texture", textureException);
        }
        texture = textureOrNone;
    }

    public static void Draw(TimeSpan delta)
    {
        //Draw a bunch of objects.
        // Note that drawing can be done from any thread after this function is called and before EndRenderQueue is called.
        for(int i=0; i<10000; i++)
        {
            //Draw the texture at a random location
            Vector3 pos = new Vector3(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f);
            Matrix4 transform = Matrix4.CreateScale(0.5f) * Matrix4.CreateTranslation(pos);
            //In the future VRender might have a better option for shader uniforms.
            // But for now, creating lists of uniforms is more or less required.
            var uniforms = new KeyValuePair<string, object>[]{
                new KeyValuePair<string, object>("model", transform)
            };
            VRender.Render.Draw(texture, mesh, shader, uniforms, true);
        }
        VRender.Render.EndRenderQueue();
        // One could put code here, but in practice that rarely happens.
    }
}