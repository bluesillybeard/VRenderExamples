using VRenderLib;
using VRenderLib.Interface;
using vmodel;
using OpenTK.Mathematics;
public sealed class Game
{
    //For larger projects, one should make a dictionary that maps from an ID to the object itself.
    // That way, objects can be loaded in a loop and a list of entities can safely reference them.
    // The next example will demonstrate.
    IRenderShader shader;
    IRenderMesh mesh;
    IRenderTexture texture;
    public Game()
    {
        //The Render was initialized before this constructor was called.
        var render = VRender.Render;
        VMesh m = new VMesh(
            new float[]{
                -1, -1, 0, 0, 0,
                -1,  1, 0, 0, 1,
                1,  -1, 0, 1, 0,
                1,   1, 0, 1, 1,
            },
            new uint[]{
                0, 1, 2,
                1, 2, 3,
            }, new Attributes(new EAttribute[]{EAttribute.position, EAttribute.textureCoords}),
            null
        );
        mesh = VRender.Render.LoadMesh(m);
        shader = VRender.Render.GetShader(new ShaderFeatures(m.attributes, true, false));
        var textureOrNone = VRender.Render.LoadTexture("dice.png", out var textureException);
        if(textureOrNone is null)
        {
            throw new Exception("Failed to load texture", textureException);
        }
        texture = textureOrNone;
    }

    public void Draw(TimeSpan delta)
    {
        for(int i=0; i<10000; i++)
        {
            //Draw the texture at a random location
            Vector3 pos = new Vector3(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f);
            Matrix4 transform = Matrix4.CreateScale(0.5f) * Matrix4.CreateTranslation(pos);
            var uniforms = new KeyValuePair<string, object>[]{
                new KeyValuePair<string, object>("model", transform)
            };
            VRender.Render.Draw(texture, mesh, shader, uniforms, true);
        }
        VRender.Render.EndRenderQueue();
    }
}