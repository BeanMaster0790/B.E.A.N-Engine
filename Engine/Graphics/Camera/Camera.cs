using Bean.Debug;
using Bean.UI;
using Bean.PhysicsSystem;
using Bean.Player;
using Bean.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bean.Graphics
{
    public class Camera
    {
        public Vector2 Position 
        {
            get
            {
                return (this.IsFreeCam) ? this.FreeCamPosition : this._position;
            }

            set
            {
                this._position = value;
            }
        }

        public Color BackgroundColor = new Color(255, 255, 255, 255);

        internal Vector2 FreeCamPosition;

        private Vector2 _position;

        public Scene Scene;

        private float _z;
        private float _baseZ;

        private float _aspectRatio;
        private float _fov;

        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        private BasicEffect _effect;

        private GraphicsDevice _graphicsDevice;

        public EventHandler SizeChange;

        public bool EnableLighting;

        private CameraController _cameraController;

        public bool IsFreeCam;

        public RenderTarget2D CameraTarget {get; private set;}

        public Camera(GraphicsDevice graphicsDevice)
        {
            this._cameraController = new CameraController();

            this.Position = new Vector2(0, 0);
            this.FreeCamPosition = new Vector2(0, 0);

            this.CameraTarget = new RenderTarget2D(GraphicsManager.Instance.GraphicsDevice, (int)GraphicsManager.Instance.GetGameResolution().X, (int)GraphicsManager.Instance.GetGameResolution().Y);

            this.UpdateAspectRatio(this, EventArgs.Empty);

            this._fov = MathHelper.PiOver2;

            this._baseZ = GetZFromHeight(1080);
            this._z = this._baseZ;

            this._graphicsDevice = graphicsDevice;

            this._effect = new BasicEffect(graphicsDevice);
            this._effect.FogEnabled = false;
            this._effect.TextureEnabled = true;
            this._effect.LightingEnabled = false;
            this._effect.PreferPerPixelLighting = false;
            this._effect.VertexColorEnabled = true;
            this._effect.Texture = null;
            this._effect.Projection = Matrix.Identity;
            this._effect.View = Matrix.Identity;
            this._effect.World = Matrix.Identity;

            this.UpdateMatix(this, EventArgs.Empty);

            GraphicsManager.Instance.GraphicsChanged += this.UpdateAspectRatio;
            GraphicsManager.Instance.GraphicsChanged += this.UpdateMatix;
            GraphicsManager.Instance.GraphicsChanged += (object sender, EventArgs e) => 
            {
                this.CameraTarget.Dispose(); 
                this.CameraTarget = new RenderTarget2D(GraphicsManager.Instance.GraphicsDevice, (int)GraphicsManager.Instance.GetGameResolution().X, (int)GraphicsManager.Instance.GetGameResolution().Y);
            };

		}

        public float GetZFromHeight(float height)
        {
            return -((height / 2) / MathF.Tan(this._fov / 2));
        }

        public float GetHeight()
        {
            return (this._z * -2) * MathF.Tan(this._fov / 2);
        }

        public float GetWidth()
        {
            return GetHeight() * this._aspectRatio;
        }

        public float GetTop()
        {
            return this.Position.Y - GetHeight() / 2;
        }

        public float GetBottom() 
        {
			return this.Position.Y + GetHeight() / 2;
		}

        public float GetLeft()
        {
            return this.Position.X - GetWidth() / 2;
        }

        public float GetRight()
        {
            return this.Position.X + GetWidth() / 2;
        }

        public void Update()
        {
            if(this.IsFreeCam)
                this._cameraController.MoveCam();
        }

        public void UpdateMatix(object sender, EventArgs e)
        {
            this._viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, this._z), Vector3.Zero, Vector3.Down);
            this._projectionMatrix = Matrix.CreatePerspectiveFieldOfView(this._fov, this._aspectRatio, 1, 8192);

            this.SizeChange?.Invoke(sender, EventArgs.Empty);
        }

        public void UpdateAspectRatio(object sender, EventArgs e)
        {
            this._aspectRatio = GraphicsManager.Instance.GetGameAspectRatio();
        }

        public void MoveZ(float amount)
        {
            this._z += amount;

            this._z = Math.Clamp(this._z, -100000, -10);
			UpdateMatix(this, EventArgs.Empty);

		}

		public void SetZ(float z)
        {
            this._z = z;
            UpdateMatix(this, EventArgs.Empty);
        }

		BlendState multiplyBlend = new BlendState()
		{
			ColorBlendFunction = BlendFunction.Add,
			ColorSourceBlend = Blend.DestinationColor,
			ColorDestinationBlend = Blend.Zero,
		};

		public RenderTarget2D Draw(SpriteBatch spriteBatch, WorldProp[] components)
        {
            GraphicsManager.Instance.GraphicsDevice.SetRenderTarget(this.CameraTarget);

            this._effect.View = this._viewMatrix;
            this._effect.Projection = this._projectionMatrix;

            this._graphicsDevice.Clear(this.BackgroundColor);

            RenderTarget2D lightingRenderTarget = null;

            if(EnableLighting)
            {
                lightingRenderTarget = this.Scene.LightingManager.DrawLightingMap(spriteBatch, this._effect, this.CameraTarget);
            }

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone, effect: this._effect, sortMode: SpriteSortMode.FrontToBack);

            foreach (WorldProp component in components)
            {

                if (component.IsVisable)
                {
                    component.Draw(spriteBatch);   
                }
            }

            DebugManager.Instance.Draw(spriteBatch);

            spriteBatch.End();

            if(EnableLighting)
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone, effect: this._effect, sortMode: SpriteSortMode.Immediate, blendState: multiplyBlend);

                spriteBatch.Draw(lightingRenderTarget, Vector2.Zero, null, Color.White, 0, new Vector2((float)lightingRenderTarget.Width / 2, (float)lightingRenderTarget.Height / 2), 1, SpriteEffects.None, 0f);

                spriteBatch.End();
            }

			return this.CameraTarget;
        }

        public bool InCameraBounds(Vector2 worldPosition, float width, float height, Vector2 origin)
        {

            float left = this.GetLeft();
            float right = this.GetRight();

            float top = this.GetTop();
            float bottom = this.GetBottom();


            return (worldPosition.X - origin.X + width > left && worldPosition.X - origin.X < right) && (worldPosition.Y - origin.Y + height > top && worldPosition.Y - origin.Y < bottom); 
        }

        public Vector2 ScreenToWorld(Vector2 position)
        {
            Vector2 screenSize = GraphicsManager.Instance.GetCurentWindowSize();

            Viewport screenViewport = new Viewport(0, 0, (int)screenSize.X, (int)screenSize.Y);

            Ray ray = CreateMouseRay(position, screenViewport);

            Plane worldPlane = new Plane(new Vector3(0, 0, 1), 0f);

            float? dist = ray.Intersects(worldPlane);
            Vector3 interceptPoint = ray.Position + ray.Direction * dist.Value;

            Vector2 result = new Vector2(interceptPoint.X, interceptPoint.Y) + this.Position;

            return result;
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            Vector2 screenSize = (GraphicsManager.Instance.IsFullscreen) 
                ? GraphicsManager.Instance.UserFullscreenSize 
                : GraphicsManager.Instance.GetGameResolution();

            Viewport screenViewport = new Viewport(0, 0, (int)screenSize.X, (int)screenSize.Y);

            // Use Z = 0 for 2D projection
            Vector3 screenPosition = screenViewport.Project(
                new Vector3(worldPosition - this.Position, 0), 
                this._projectionMatrix,  // Ensure this is correct
                this._viewMatrix,        // Ensure this matches the camera's view matrix
                Matrix.Identity          // Use Identity for world transform
            );

            return new Vector2(screenPosition.X, screenPosition.Y);
        }



		private Ray CreateMouseRay(Vector2 position, Viewport viewport)
        {
            Vector3 nearPoint = new Vector3(position, 0);
            Vector3 farPoint = new Vector3(position, 1);

            nearPoint = viewport.Unproject(nearPoint, this._projectionMatrix, this._viewMatrix, Matrix.Identity);
            farPoint = viewport.Unproject(farPoint, this._projectionMatrix, this._viewMatrix, Matrix.Identity);

            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            Ray ray = new Ray(nearPoint, direction);

            return ray;
        }
    }
}
