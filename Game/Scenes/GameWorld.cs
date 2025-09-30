using System;
using Bean;
using Bean.Debug;
using Bean.Graphics;
using Bean.PhysicsSystem;
using Bean.Player;
using Bean.Scenes;
using Bean.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Random = Bean.Random;

namespace DemoGame {

    class GameWorld : Scene
    {
        public Player Player;

        public CameraController CameraController;

        public DemoWorldTestMap Map;

        public TimeShifter TimeShifter;

        public MushroomSpawner MushroomSpawner;

        private MusicManager _musicManager;

        public int CurrentNight = 0;

        public UISlider PlayerHealthTextSlider;

        public UIText PlayerMoneyText;

        private UIAlignContainer _nightCountContainer;
        private UIText _nightCountText;

        private bool _nightCountAnimatonStart;
        private float _timeSinceAnimationStart = 100;
        private float _animationTime = 1;
        private float _animationWaitTime = 1;

        public MenuHandler MenuHandler;

        private Sprite _pauseSprite;

        public WorldTime CurrentTime;

        private Timer _spawnTimer = new Timer();

        private int _spawnTime = 10;

        private int _maxSpawnTime = 10;

        private int _minSpawnTime = 3;

        private int _slowDownAmount = 10;

        private int _currentSlowDown = 0;

        private float _timeToSlow = 4;

        private float _gradualSlowTimer = 0;

        public bool IsGameSlow;


        public GameWorld(string name) : base(name)
        {
        }

        float count = 0;

        public override void LateUpdate()
        {
            this.MenuHandler.LateUpdate();

            if (this.MenuHandler.IsOpen && this.MenuHandler.CurrentMenu != "Upgrades")
            {
                return;
            }

            if (Player.SlowTime)
            {
                this._gradualSlowTimer += Time.Instance.DeltaTime;

                if (this._gradualSlowTimer > (float)(this._timeToSlow / (float)this._slowDownAmount) && this._currentSlowDown < this._slowDownAmount)
                {
                    this._currentSlowDown += 1;

                    this._gradualSlowTimer = 0;

                    DebugServer.Log(this._currentSlowDown, this);
                }
            }
            else
            {
                this._gradualSlowTimer += Time.Instance.DeltaTime;

                if (this._gradualSlowTimer > (float)(this._timeToSlow / (float)this._slowDownAmount) && this._currentSlowDown > 0)
                {
                    this._currentSlowDown -= 1;

                    this._gradualSlowTimer = 0;

                    DebugServer.Log(this._currentSlowDown, this);
                }
            }

            this._currentSlowDown = Math.Clamp(this._currentSlowDown, 0, this._slowDownAmount);

            this.IsGameSlow = (this._currentSlowDown <= 0) ? false : true;

            if (count > 1)
            {

                if (count >= this._currentSlowDown)
                {
                    count = 0;
                }
                else
                {
                    count += 1;
                }

                this.Player.LateUpdate();
                this.CameraController.LateUpdate();

                return;
            }

            count++;


            base.LateUpdate();
        }

        public override void LoadScene(object caller = null)
        {
            base.LoadScene(caller);

            this.MenuHandler = new MenuHandler(this, this.UIScene);

            this.MenuHandler.LoadScene();

            this.MenuHandler.MainMenuList.IsVisable = false;

            this._spawnTimer = new Timer();


            Texture2D pixel = new Texture2D(GraphicsManager.Instance.GraphicsDevice, 1, 1);

            Game game = (Game)Engine.Instance;

            this.UIScene.RenderScale = game.Settings.ResolutionScale;
            this.UIScene.RefernceWidth = (int)GraphicsManager.Instance.GetCurentWindowSize().X;
            this.UIScene.RefrenceHeight = (int)GraphicsManager.Instance.GetCurentWindowSize().Y;

            GraphicsManager.Instance.GraphicsChanged += (object sender, EventArgs e) =>
            {
                this.UIScene.RefernceWidth = (int)GraphicsManager.Instance.GetCurentWindowSize().X;
                this.UIScene.RefrenceHeight = (int)GraphicsManager.Instance.GetCurentWindowSize().Y;

                this.UIScene.SetTargetSize(this, EventArgs.Empty);
            };

            this.Camera.EnableLighting = true;

            pixel.SetData(new Color[1] { new Color(255, 255, 255, 0.5f) });


            this._pauseSprite = new Sprite()
            {
                IsActive = false,
                IsVisable = false,
                Texture = pixel,
                Scale = 100_000,
                Colour = Color.Black,
                Layer = 1
            };

            this.AddToScene(this._pauseSprite);

            this.Map = new DemoWorldTestMap(false)
            {
                TileScale = 2f
            };

            this.AddToScene(Map);

            this.Player = new Player()
            {
                Name = "Player",
                Layer = 0.02f
            };

            this.CameraController = new CameraController()
            {
                Target = this.Player,
            };

            this.AddToScene(this.CameraController);

            this.AddToScene(this.Player);

            for (int i = 0; i < 100; i++)
            {
                FireFly sprite = new FireFly()
                {
                    Texture = pixel,
                    Layer = 0.03f,

                    Position = new Vector2(Random.RandomInt(-519, 519), Random.RandomInt(-519, 519))
                };

                sprite.AddAddon(new FlickeringLight(0.25f, 10, 5, 3, 0.5f, Color.Orange));

                this.AddToScene(sprite);
            }

            this.TimeShifter = new TimeShifter();

            this.AddToScene(this.TimeShifter);

            this.TimeShifter.TimeChange += OnTimeChange;

            this.MushroomSpawner = new MushroomSpawner();

            this.AddToScene(MushroomSpawner);

            this._musicManager = new MusicManager();

            this.AddToScene(this._musicManager);

            LoadUI();


        }

        private void LoadUI()
        {
            


            UIAlignContainer PlayerMoney = new UIAlignContainer
            {
                AlignDirection = AlignDirection.Horizontal,
                VerticalAlign = VerticalAlign.Center,
                Width = 275,
                Height = 100,
                AnchorPoint = UIPropAnchorPoint.BottomRight
            };

            this.UIScene.AddUIProp(PlayerMoney);

            UIContainer playerMoneyIcon = new UIContainer()
            {
                Width = 64,
                Height = 64,
                TexturePath = "UI/CoinIcon",
                Parent = PlayerMoney
            };

            this.UIScene.AddUIProp(playerMoneyIcon);

            UIAlignContainer playerMoneyTextContainer = new UIAlignContainer()
            {
                Width = 250,
                Height = 100,
                TexturePath = "UI/SmallUIBox",
                Parent = PlayerMoney,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Center,
            };

            this.UIScene.AddUIProp(playerMoneyTextContainer);

            this.PlayerMoneyText = new UIText()
            {
                Colour = Color.White,
                Width = 100,
                Height = 50,
                FontSize = 32,
                FontPath = "GameFont1",
                Text = "172",
                Parent = playerMoneyTextContainer
            };

            this.UIScene.AddUIProp(PlayerMoneyText);

            UIAlignContainer playerHealth = new UIAlignContainer
            {
                AlignDirection = AlignDirection.Horizontal,
                VerticalAlign = VerticalAlign.Center,
                Width = 500,
                Height = 100,
                AnchorPoint = UIPropAnchorPoint.BottomLeft,
                ChildrenLayer = 1f,
                Layer = 0.9f
            };

            this.UIScene.AddUIProp(playerHealth);

            UIContainer playerHealthIcon = new UIContainer()
            {
                Width = 64,
                Height = 64,
                TexturePath = "UI/HealthIcon",
                Parent = playerHealth
            };

            this.UIScene.AddUIProp(playerHealthIcon);

            this.PlayerHealthTextSlider = new UISlider()
            {
                Width = 250,
                Height = 100,
                MaxFill = 500,
                TexturePath = "UI/Bar",
                Parent = playerHealth,
                IsInteractable = true
            };

            this.UIScene.AddUIProp(PlayerHealthTextSlider);

            this._nightCountContainer = new UIAlignContainer
            {
                LocalPosition = new Vector2(0, -100),
                Width = 400,
                Height = 200,
                TexturePath = "UI/MedUIBox",
                AnchorPoint = UIPropAnchorPoint.TopCenter,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Center,
            };

            this.UIScene.AddUIProp(this._nightCountContainer);

            this._nightCountText = new UIText
            {
                Colour = Color.White,
                Width = 100,
                Height = 50,
                FontSize = 32,
                FontPath = "GameFont1",
                Text = "Night 1",
                Parent = _nightCountContainer,
            };

            this.UIScene.AddUIProp(this._nightCountText);
        }

        private void OnTimeChange(object sender, TimeChangeArgs timeChange)
        {
            DebugServer.Log($"Time: {timeChange.CurrentTime} Length: {timeChange.CycleLength}", this, Color.OrangeRed);

            this.CurrentTime = timeChange.CurrentTime;

            if (timeChange.CurrentTime == WorldTime.Night)
            {
                this.CurrentNight++;

                this._nightCountText.Text = $"Night {this.CurrentNight}";

                this._nightCountAnimatonStart = true;

                this._timeSinceAnimationStart = 0;

                this.MushroomSpawner.SpawnMushrooms(5);

                this._spawnTime = this._maxSpawnTime;

                this._spawnTimer.StartTimer(this._spawnTime);
                
                this._musicManager.soundHolder.PlaySound("NightTime");    
            }
            else if (timeChange.CurrentTime == WorldTime.Sunrise)
            {
                this._musicManager.soundHolder.PlaySound("DayTime");
            }

        }

        public void OpenMenu()
        {
            this.MenuHandler.OpenMenu("Main");

            this._pauseSprite.IsVisable = true;

            this._pauseSprite.Position = this.Camera.ScreenToWorld(new Vector2(-100, -100));
        }

        public void CloseMenu()
        {
            this.MenuHandler.OpenMenu(null);

            this._pauseSprite.IsVisable = false;
        }

        public override void Update()
        {

            if (InputManager.Instance.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape) && this.MenuHandler.IsOpen && this.MenuHandler.CurrentMenu != "Upgrades")
            {
                this.CloseMenu();
            }
            else if (InputManager.Instance.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape) && this.MenuHandler.CurrentMenu != "Upgrades")
            {
                this.OpenMenu();
            }

            if (this.MenuHandler.IsOpen && this.MenuHandler.CurrentMenu != "Upgrades")
            {
                this.UIScene.Update();
                return;
            }

            base.Update();

            if (InputManager.Instance.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.E))
            {
                this.MenuHandler.OpenMenu("Upgrades");
            }

            if (this.CurrentTime == WorldTime.Night)
            {
                this._spawnTimer.Update();

                if (this._spawnTimer.IsFinished)
                {
                    this._spawnTime -= 1;

                    this._spawnTime = Math.Clamp(this._spawnTime, this._minSpawnTime, this._maxSpawnTime);

                    this._spawnTimer.StartTimer(this._spawnTime);

                    this.MushroomSpawner.SpawnMushrooms(5);
                }
            }

            if (this._nightCountAnimatonStart)
            {
                this._timeSinceAnimationStart += Time.Instance.DeltaTime;

                float amount = MathF.Min(this._timeSinceAnimationStart / this._animationTime, this._animationTime);

                this._nightCountContainer.LocalPosition = Vector2.Lerp(new Vector2(0, -120), new Vector2(0, 100), amount);

                if (this._timeSinceAnimationStart >= this._animationTime + this._animationWaitTime)
                {
                    this._nightCountAnimatonStart = false;
                    this._timeSinceAnimationStart = 0;
                }
            }
            else
            {
                this._timeSinceAnimationStart += Time.Instance.DeltaTime;

                float amount = MathF.Min(this._timeSinceAnimationStart / this._animationTime, this._animationTime);

                this._nightCountContainer.LocalPosition = Vector2.Lerp(new Vector2(0, 100), new Vector2(0, -120), amount);
            }
        }
    }

}