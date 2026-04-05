using Bean;
using Bean.Graphics.Animations;
using Bean.JsonVariables;
using Bean.PhysicsSystem;
using Bean.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;

namespace DemoGame;

[RequiresAddon(typeof(AnimationManager))]
public class PlayerController : Addon
{
    [Tinned("PlayerSpeed")]
    private float _playerSpeed = 45;
    
    public PlayerController(string name) : base(name)
    {
        
    }

    public override void Update()
    {
        base.Update();

        Vector2 moveVector = new Vector2();

        if (InputManager.Instance.IsKeyHeld(Keys.A))
            moveVector.X -= 1;
        if (InputManager.Instance.IsKeyHeld(Keys.D))
            moveVector.X += 1;
        if (InputManager.Instance.IsKeyHeld(Keys.W))
            moveVector.Y -= 1;
        if (InputManager.Instance.IsKeyHeld(Keys.S))
            moveVector.Y += 1;

        if (moveVector != Vector2.Zero)
        {
            moveVector.Normalize();
        }
        
        if(Parent.GetAddon<PhysicsObject>() != null)
            this.Parent.GetAddon<PhysicsObject>().Velocity = moveVector *  this._playerSpeed;

        if (moveVector.X != 0)
        {
            this.Parent.GetAddon<AnimationManager>().Play("WalkSide");
            
            this.Parent.GetAddon<Sprite>().ChangeSpriteEffect((moveVector.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            
        }
        else if (moveVector.Y != 0)
        {
            this.Parent.GetAddon<AnimationManager>().Play((moveVector.Y < 0) ? "WalkUp" : "WalkDown");
        }
        else
        {
            this.Parent.GetAddon<AnimationManager>().Play("Idle");
        }
        
        this.Parent.Scene.Camera.Position = Vector2.Lerp(this.Parent.Scene.Camera.Position, this.Parent.PropTransform.Position, 0.08f);


    }
}