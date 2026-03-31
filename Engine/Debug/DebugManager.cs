using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Text;
using Bean.Player;
using Microsoft.Xna.Framework.Input;
using Bean.PhysicsSystem;
using Bean.Graphics;
using Bean.Scenes;
using Bean.UI;
using FontStashSharp;

namespace Bean.Debug
{
	public class DebugManager
	{
		public static DebugManager Instance = new DebugManager();
		
		private List<IDebuggable> _debuggables = new List<IDebuggable>();

		private Dictionary<string, Texture2D> _renderedTextures = new Dictionary<string, Texture2D>();

		private SpriteFont _font;

		private bool ShowDebugInfo = false;
		private bool ShowColliders = false;
		private bool ShowLines = false;
		private bool SelectProp = false;
		private bool _showFPS = false;

		private UIText _fps;

		private UIText _propId;

		private Texture2D _pixel;

		private List<Line> _linesToDraw = new List<Line>();
		private List<Circle> _circlesToDraw = new List<Circle>();

		private string _lastActiveScene = "";


		public void Start()
		{
#if DEBUG
			_showFPS = true;
#endif
			
			this._fps = new UIText("FPSText")
			{
				LocalPosition = Vector2.Zero,
				Colour = Color.Red,
				Width = 100,
				Height = 50,
				FontSize = 24,
			};

			this._propId = new UIText("IdText")
			{
				LocalPosition = new Vector2(110, 0),
				Colour = Color.Red,
				Width = 100,
				Height = 50,
				FontSize = 24,
			};
		}

		public void AddDebugabble(IDebuggable debuggable)
		{
			this._debuggables.Add(debuggable);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if(this._pixel == null)
			{
				this._pixel = new Texture2D(GraphicsManager.Instance.GraphicsDevice, 1, 1);
				this._pixel.SetData(new Color[] {Color.White});
			}

			if (this.ShowDebugInfo)
				DrawDebugInfo(spriteBatch);

			if(this.ShowColliders)
				Physics.Instance.DrawColliders(spriteBatch);

			if(this.ShowLines)
				DrawLines(spriteBatch);

		}

		public void Update()
		{
			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.D))
				this.ShowDebugInfo = !this.ShowDebugInfo;

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.C))
				this.ShowColliders = !this.ShowColliders;

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.R))
				this.ShowLines = !this.ShowLines;

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.P))
				this.SelectProp = !this.SelectProp;

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.H))
			{
				#if DEBUG
				DebugServer.Log("Preforming HotReload", null);
				FileManager.HotReloadRequested?.Invoke(this, EventArgs.Empty);
				#endif
			}

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.G))
				SceneManager.Instance.ActiveScene.LightingManager.SetMapSize(this, null);

			if (InputManager.Instance.WasKeyPressed(Keys.F11))
			{
				GraphicsManager.Instance.SetFullScreen(!GraphicsManager.Instance.IsFullscreen);
				GraphicsManager.Instance.ApplyChanges();
			}

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.F))
			{
				SceneManager.Instance.ActiveScene.Camera.IsFreeCam = !SceneManager.Instance.ActiveScene.Camera.IsFreeCam;
			}

			if (InputManager.Instance.WasKeyPressed(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.P))
			{
				Engine.Instance.GameLoopPause = !Engine.Instance.GameLoopPause;
			}

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.E))
			{
#if DEBUG
				if (!SceneManager.Instance.DoesSceneExist("Editor"))
				{
					PropEditor editor = new PropEditor("Editor");
					
					SceneManager.Instance.AddNewScene(editor);
					SceneManager.Instance.LoadScene("Editor");
				}

				if (SceneManager.Instance.ActiveScene.Name == "Editor")
				{
					SceneManager.Instance.SetActiveScene(this._lastActiveScene);
				}
				else
				{
					this._lastActiveScene = SceneManager.Instance.ActiveScene.Name;
					
					SceneManager.Instance.SetActiveScene("Editor");
				}
#endif
			}

			if (InputManager.Instance.IsKeyHeld(Keys.F12) && InputManager.Instance.WasKeyPressed(Keys.L))
				SceneManager.Instance.ActiveScene.Camera.EnableLighting = !SceneManager.Instance.ActiveScene.Camera.EnableLighting;

			if (SceneManager.Instance.ActiveScene != null && SceneManager.Instance.ActiveScene.UIScene.GetPropWithName<UIText>("FPSText") == null)
			{
				SceneManager.Instance.ActiveScene.UIScene.AddUIProp(this._fps);
				SceneManager.Instance.ActiveScene.UIScene.AddUIProp(this._propId);
			}

			if (_showFPS)
			{
				this._fps.Text = $"{Time.Instance.DrawFps}";
			}

			if (this.SelectProp)
				HandlePropSelect();

			this._propId.IsVisable = this.SelectProp;

			
		}

		private void HandlePropSelect()
		{
			Vector2 mousePos = SceneManager.Instance.ActiveScene.Camera.ScreenToWorld(InputManager.Instance.MousePosition());

			Collider prop = Raycasts.Instance.OverlapBox(mousePos, 1);

			this._propId.Text = (prop != null) ? prop.Parent.PropID : "null";

			if (prop != null && InputManager.Instance.WasMiddleButtonPressed())
				DebugServer.FocucedProp = prop.Parent;
		}

		private void DrawLines(SpriteBatch spriteBatch)
		{
			int VectorToIndex(Vector2 point, Texture2D texture)
			{
				Vector2 center = new Vector2(texture.Width / 2, texture.Height / 2);

				int adjustedX = (int)((int)point.X + center.X);
				int adjustedY = (int)((int)point.Y + center.Y);

				if (adjustedX < 0) adjustedX += texture.Width;
				else if (adjustedX >= texture.Width) adjustedX -= texture.Width;

				if (adjustedY < 0) adjustedY += texture.Height;
				else if (adjustedY >= texture.Height) adjustedY -= texture.Height;

				return adjustedY * texture.Width + adjustedX;
			}


			foreach (Line line in this._linesToDraw.ToArray())
			{
				Vector2 lineScale = new Vector2(line.Length, line.Thickness);

				spriteBatch.Draw(this._pixel, line.StartPoint - SceneManager.Instance.ActiveScene.Camera.Position, null, line.Colour, MathHelper.ToRadians(line.Angle), new Vector2(0, line.Thickness / 2), lineScale, SpriteEffects.None, 0.91f);

				this._linesToDraw.Remove(line);
			}

			foreach (Circle circle in this._circlesToDraw.ToArray())
			{
				spriteBatch.Draw(Shapes.Circle(circle), circle.Position - SceneManager.Instance.ActiveScene.Camera.Position, null, Color.White, 0, new Vector2(circle.Radius, circle.Radius), 1f, SpriteEffects.None, 0.91f);

				this._circlesToDraw.Remove(circle);
			}
		}

		public void DrawCircle(Circle circle, bool filled = false)
		{
			circle.Filled = filled;

			if (this.ShowLines)
				this._circlesToDraw.Add(circle);	
		}

		public void DrawCircle(Circle circle, Color color, bool filled = false)
		{
			circle.Colour = color;

			DrawCircle(circle, filled);
		}

		public void DrawCircle(Vector2 position, int radius, Color colour, bool filled = false)
		{
			Circle circle = new Circle(position, radius, colour);

			DrawCircle(circle, filled);
		}

		public void DrawLine(Line line)
		{
			if (this.ShowLines)
				this._linesToDraw.Add(line);
		}

		public void DrawLine(Line line, Color colour, int thickness)
		{
			line.Colour = colour;
			line.Thickness = thickness;

			this.DrawLine(line);
		}

		public void DrawLine(Vector2 origin, Vector2 direction ,float length, Color colour, int thickness)
		{
			Vector2 endPoint = origin + direction * length;

			Line line = new Line(origin, endPoint, colour, thickness);

			this.DrawLine(line);
		}

		public void DrawLine(Vector2 origin, float direction, float length, Color colour, int thickness)
		{
			Vector2 vectorDirection = new Vector2(MathF.Cos(direction), MathF.Sin(direction));

			this.DrawLine(origin, vectorDirection, length, colour, thickness);	
		}

		private void DrawDebugInfo(SpriteBatch spriteBatch)
		{
			foreach (IDebuggable debugItem in this._debuggables)
			{
				if (!debugItem.ShowDebugInfo)
					continue;

				Vector2 lastPosition = debugItem.GetDebugDrawPosition();

				foreach (string debugValue in debugItem.DebugValueNames)
				{
					string displayedText = "Error";


					displayedText = debugValue + ": " + debugItem.GetType().GetField(debugValue).GetValue(debugItem);

					Vector2 drawPosition = new Vector2(lastPosition.X, lastPosition.Y);
					lastPosition = new Vector2(drawPosition.X, drawPosition.Y + this._font.MeasureString(displayedText).Y);

					spriteBatch.DrawString(this._font, displayedText, drawPosition - SceneManager.Instance.ActiveScene.Camera.Position, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
				}
			}
		}

		public void ToggleFps(bool enabled)
		{
			if (enabled)
			{
				this._showFPS =  true;
				this._fps.IsVisable =  true;
			}
			else
			{
				this._showFPS = false;
				this._fps.IsVisable =  false;
			}
		}
	}
}
