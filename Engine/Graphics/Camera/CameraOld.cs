//using BeanMachine.Graphics;
//using BeanMachine.PhysicsSystem;
//using BeanMachine.Scenes;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BeanMachine.Graphics
//{
//    public class CameraOld
//    {
//        private float _scale;

//        private GraphicsDeviceManager _graphics;

//        public GraphicsDevice GraphicsDevice;

//        private Scene _scene;

//        private Matrix _transformMatrix;

//        public Vector2 Position;

//        private RenderTarget2D _target;

//        public CameraOld(GraphicsDeviceManager graphicsDeviceManager, GraphicsDevice graphicsDevice, Scene scene)
//        {
//            this._graphics = graphicsDeviceManager;

//            this.GraphicsDevice = graphicsDevice;

//            this._scene = scene;

//            this.Position = Vector2.Zero;

//            this._target = new RenderTarget2D(graphicsDevice, Globals.VirtualWidth, Globals.VirtualHeight);
//        }

//        public void Draw(SpriteBatch spriteBatch, List<Prop> components)
//        {

//            this._transformMatrix = Matrix.CreateTranslation(new Vector3(Position, 0));

//            this._transformMatrix = Matrix.Invert(this._transformMatrix);

//            this._scale = 1f / (Globals.VirtualHeight / this._graphics.GraphicsDevice.Viewport.Height);

//            this.GraphicsDevice.SetRenderTarget(this._target);

//            this.GraphicsDevice.Clear(Color.CornflowerBlue);

//            spriteBatch.Begin(transformMatrix: this._transformMatrix, samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);

//            foreach (Prop component in components)
//            {
//                component.Draw(spriteBatch);
//            }

//            spriteBatch.End();

//            this.GraphicsDevice.SetRenderTarget(null);

//            spriteBatch.Begin();

//            spriteBatch.Draw(_target, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);

//            spriteBatch.End();
//        }

//        public Vector2 ScreenToGame(Vector2 position)
//        {
//            return Vector2.Transform(position / _scale, Matrix.Invert(this._transformMatrix));
//        }
//    }
//}
