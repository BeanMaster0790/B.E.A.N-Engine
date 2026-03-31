using Bean.Player;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bean.Graphics
{
    internal class CameraController
    {
		private Vector2? _mouseOrigin = null;

        internal void MoveCam()
		{

			if (InputManager.Instance.IsKeyHeld(Keys.Add))
			{
				SceneManager.Instance.ActiveScene.Camera.MoveZ(3);
			}
			if (InputManager.Instance.IsKeyHeld(Keys.Subtract))
			{
				SceneManager.Instance.ActiveScene.Camera.MoveZ(-3);
			}

			if(InputManager.Instance.WasMiddleButtonPressed())
				this._mouseOrigin = SceneManager.Instance.ActiveScene.Camera.ScreenToWorld(InputManager.Instance.MousePosition());

			if(InputManager.Instance.IsMiddleButtonHeld())
			{
				if(this._mouseOrigin != null)
				{
					Vector2? direction = this._mouseOrigin - SceneManager.Instance.ActiveScene.Camera.ScreenToWorld(InputManager.Instance.MousePosition());

					SceneManager.Instance.ActiveScene.Camera.FreeCamPosition += (Vector2)direction;
				}
			}

			if(!InputManager.Instance.IsMiddleButtonHeld() && !InputManager.Instance.WasMiddleButtonPressed())
				this._mouseOrigin = null;

			if(InputManager.Instance.IsKeyHeld(Keys.LeftControl) && InputManager.Instance.HasScrolledUp()){
				SceneManager.Instance.ActiveScene.Camera.MoveZ(25);

				Vector2 direction = SceneManager.Instance.ActiveScene.Camera.FreeCamPosition - SceneManager.Instance.ActiveScene.Camera.ScreenToWorld(InputManager.Instance.MousePosition());

				SceneManager.Instance.ActiveScene.Camera.FreeCamPosition += -direction * 0.05f;
			}

			if(InputManager.Instance.IsKeyHeld(Keys.LeftControl) && InputManager.Instance.HasScrolledDown())
				SceneManager.Instance.ActiveScene.Camera.MoveZ(-25);
			
		}
    }
}