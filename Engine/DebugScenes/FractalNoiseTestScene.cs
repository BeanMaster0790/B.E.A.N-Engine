using Bean;
using Bean.Graphics;
using Bean.Noise;
using Bean.Scenes;
using Bean.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Bean.DebugScenes
{
    class NoiseTestScene : Scene 
    {
        public Sprite Map;

        public PerlinNoise Temp;
        public PerlinNoise Humidity;

        private int colourCount = 0;


        public NoiseTestScene(string name) : base(name)
        {

        }

        public override void LoadScene(object caller = null)
        {
            base.LoadScene(caller);

            this.Name = "Fractal Noise Test";

            GraphicsManager.Instance.GraphicsChanged += (object sender, EventArgs e) => 
            {
                this.UIScene.RefernceWidth = (int)GraphicsManager.Instance.GetCurentWindowSize().X;
                this.UIScene.RefrenceHeight = (int)GraphicsManager.Instance.GetCurentWindowSize().Y;
            };

            this.Camera.SetZ(this.Camera.GetZFromHeight(200));

            this.Camera.IsFreeCam = true;

            WorldProp mapProp = new WorldProp("Map");
            
            Map = new Sprite("MapSprite", "");
            
            mapProp.AddAddon(Map);

            this.AddToScene(mapProp);

            UIAlignContainer optionsContainer = new UIAlignContainer("Options Container")
            {
                Width = 1280,
                Height = 50,
                AlignDirection = AlignDirection.Horizontal,
                Colour = Color.Transparent,
                Spacing = 5,
                isScrollable = true
            };

            this.UIScene.AddUIProp(optionsContainer);

            UIAlignContainer seedInput = CreateNoiseOption("Seed: ", "Seed", "rand", "rand");
            seedInput.Parent = optionsContainer;

            UIAlignContainer widthInput = CreateNoiseOption("Width: ", "Width", "1000", "1000");
            widthInput.Parent = optionsContainer;

            UIAlignContainer heightInput = CreateNoiseOption("Height: ", "Height", "1000", "1000");
            heightInput.Parent = optionsContainer;

            UIAlignContainer scaleInput = CreateNoiseOption("Scale: ", "Scale", "0.05", "0.05");
            scaleInput.Parent = optionsContainer;

            UIAlignContainer octavesInput = CreateNoiseOption("Octaves: ", "Oct", "2", "2");
            octavesInput.Parent = optionsContainer;

            UIAlignContainer persistenceInput = CreateNoiseOption("Persistence: ", "Per", "0.5", "0.5");
            persistenceInput.Parent = optionsContainer;

            UIAlignContainer frequencyInput = CreateNoiseOption("Frequency: ", "Freq", "0.5", "0.5");
            frequencyInput.Parent = optionsContainer;

            UIAlignContainer amplitudeInput = CreateNoiseOption("Amplitude: ", "Amp", "0.5", "0.5");
            amplitudeInput.Parent = optionsContainer;

            UIAlignContainer strechXInput = CreateNoiseOption("X Strech: ", "XS", "1", "1");
            strechXInput.Parent = optionsContainer;

            UIAlignContainer strechYInput = CreateNoiseOption("Y Strech: ", "YS", "1", "1");
            strechYInput.Parent = optionsContainer;

            UIAlignContainer genButton = new UIAlignContainer("Gen Button") 
            {
                Width = 100,
                Height = 50,
                Colour = Color.Green,
                Parent = optionsContainer,
                VerticalAlign = VerticalAlign.Center
            };

            this.UIScene.AddUIProp(genButton);

            UIText genText = new UIText("GenerateText")
            {
                Parent = genButton,
                Text = "Generate",
                Colour = Color.White
            }; 

            this.UIScene.AddUIProp(genText);

            genButton.OnLeftClick += (object sender, EventArgs e) =>
            {
                GenerateMap();
            };

            UIAlignContainer coloursContainer = new UIAlignContainer("Colour Container")
            {
                LocalPosition = new Vector2(0, 55),
                Width = 100,
                Height = 665,
                AlignDirection = AlignDirection.Vertical,
                Name = "ColoursContainer",
                Colour = Color.Transparent,
                isScrollable = true,
                Spacing = 5
            };

            this.UIScene.AddUIProp(coloursContainer);

            UIAlignContainer newRuleButton = new UIAlignContainer("New Rule") 
            {
                Width = 100,
                Height = 25,
                Colour = Color.Green,
                Parent = coloursContainer
            };


            this.UIScene.AddUIProp(newRuleButton);

            UIText newRuleText = new UIText("Add Rule Text")
            {
                Parent = newRuleButton,
                Text = "Add Rule",
                Colour = Color.White
            }; 

            this.UIScene.AddUIProp(newRuleText);

            newRuleButton.OnLeftClick += (object sender, EventArgs e) => 
            {
                this.UIScene.RemoveUIProp(newRuleButton);
                this.AddColourOption();

                this.UIScene.AddUIProp(newRuleButton);
                this.UIScene.AddUIProp(newRuleText);
            };

            GenerateMap();

        }

        private void GenerateMap()
        {
            string seedInput = this.UIScene.GetPropWithName<UIInputText>("Seed").Text;

            int seed = Bean.Random.RandomInt(-123456, 123456);

            if(seedInput.ToLower() != "rand")
                seed = int.Parse(seedInput);

            int width = int.Parse(this.UIScene.GetPropWithName<UIInputText>("Width").Text);
            int height = int.Parse(this.UIScene.GetPropWithName<UIInputText>("Height").Text);
            float scale = float.Parse(this.UIScene.GetPropWithName<UIInputText>("Scale").Text);
            int oct = int.Parse(this.UIScene.GetPropWithName<UIInputText>("Oct").Text);
            float per = float.Parse(this.UIScene.GetPropWithName<UIInputText>("Per").Text);
            float freq = float.Parse(this.UIScene.GetPropWithName<UIInputText>("Freq").Text);
            float amp = float.Parse(this.UIScene.GetPropWithName<UIInputText>("Amp").Text);
            float XS = float.Parse(this.UIScene.GetPropWithName<UIInputText>("XS").Text);
            float YS = float.Parse(this.UIScene.GetPropWithName<UIInputText>("YS").Text);

            this.Temp = new PerlinNoise(seed)
            {
                Scale = scale,
                Octaves = oct,
                Persistence = per,
                Frequency = freq,
                Amplitude = amp,
                StrechX = XS,
                StrechY = YS
            };

            this.Humidity = new PerlinNoise(seed)
            {
                Scale = scale,
                Octaves = oct,
                Persistence = per,
                Frequency = freq,
                Amplitude = amp,
                Offset = new Vector2(1000, 1000)
            };

            this.Map.DisposeTexture();

            this.Map.ChangeTexture(this.GenerateTexture(width, height));
            this.Map.ChangeOrigin(new Vector2(width / 2f, height / 2f));
        }

        private Texture2D GenerateTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(GraphicsManager.Instance.GraphicsDevice, width, height);
            Color[] colours = new Color[width * height];

            List<float> ruleTemps = new List<float>();
            List<float> ruleHumids = new List<float>();
            List<Color> ruleColours = new List<Color>();

            int ruleIndex = 0;


            while (this.UIScene.GetPropWithName<UIAlignContainer>($"ColourOption{ruleIndex}Container") != null)
            {
                ruleTemps.Add(float.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{ruleIndex}Temp").Text));
                ruleHumids.Add(float.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{ruleIndex}Humidity").Text));

                ruleColours.Add(
                    new Color
                    (
                        int.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{ruleIndex}Red").Text),
                        int.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{ruleIndex}Green").Text),
                        int.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{ruleIndex}Blue").Text)
                    )
                );

                ruleIndex++;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float temp = this.Temp.FractalNoise(x, y);
                    float humid = this.Humidity.FractalNoise(x, y);

                    temp = Math.Clamp(temp, 0f, 1f);
                    humid = Math.Clamp(humid, 0f, 1f);

                    Color colour = new Color(temp + humid, temp + humid, temp + humid);

                    bool tempMatched = false;

                    for(int i = 0; i < ruleTemps.Count; i++)
                    {
                        float ruleTemp = ruleTemps[i];
                        float ruleHumidity = ruleHumids[i];

                        // Find the first temp group that this pixel fits into
                        if (!tempMatched && temp < ruleTemp)
                        {
                            tempMatched = true;
                        }

                        // Only process humidity rules *after* temp has matched
                        if (tempMatched && humid < ruleHumidity)
                        {
                            colour = ruleColours[i];
                            break; // Done
                        }

                        ruleIndex++;
                    }

                    int index = x + y * width;
                    colours[index] = colour;
                }
            }

            texture.SetData(colours);

            return texture;
        }

        private UIAlignContainer CreateNoiseOption(string labelText, string name, string placeholder, string defaultValue)
        {
            var container = new UIAlignContainer(name + "Container")
            {
                AlignDirection = AlignDirection.Horizontal,
                VerticalAlign = VerticalAlign.Center,
                HorizontalAlign = HorizontalAlign.Left,
                Width = 100,
                Height = 50,
                Colour = Color.Gray,
            };

            var label = new UIText(name + "Label")
            {
                Text = labelText,
                Parent = container,
                Colour = Color.White
            };

            var input = new UIInputText(name)
            {
                Colour = Color.White,
                PlaceHolderText = placeholder,
                Text = defaultValue,
                Width = 100,
                Height = 50,
                Parent = container,
            };

            this.UIScene.AddUIProp(container);
            this.UIScene.AddUIProp(label);
            this.UIScene.AddUIProp(input);

            return container;
        }

        private void AddColourOption()
        {
            UIAlignContainer colourOptionContainer = new UIAlignContainer($"ColourOption{this.colourCount}Container")
            {
                Parent = this.UIScene.GetPropWithName<UIAlignContainer>("ColoursContainer"),
                AlignDirection = AlignDirection.Vertical,
                Width = 100,
                Height = 325,
                Colour = Color.Transparent,
            };

            this.UIScene.AddUIProp(colourOptionContainer);

            UIAlignContainer colourOptionTemp = CreateNoiseOption("If < temp: ", $"ColourOption{this.colourCount}Temp", "1", "1");
            colourOptionTemp.Parent = colourOptionContainer;

            UIAlignContainer colourOptionHumidity = CreateNoiseOption("& < humidity: ", $"ColourOption{this.colourCount}Humidity", "1", "1");
            colourOptionHumidity.Parent = colourOptionContainer;

            UIAlignContainer colourOptionColourContainer = new UIAlignContainer($"ColourOption{this.colourCount}Container")
            {
                Parent = colourOptionContainer,
                AlignDirection = AlignDirection.Vertical,
                Width = 100,
                Height = 150
            };

            this.UIScene.AddUIProp(colourOptionColourContainer);

            UIAlignContainer colourOptionRed = CreateNoiseOption("R: ", $"ColourOption{this.colourCount}Red", "256", "256");
            colourOptionRed.Parent = colourOptionColourContainer;

            UIAlignContainer colourOptionGreen = CreateNoiseOption("G: ", $"ColourOption{this.colourCount}Green", "255", "255");
            colourOptionGreen.Parent = colourOptionColourContainer;

            UIAlignContainer colourOptionBlue = CreateNoiseOption("B: ", $"ColourOption{this.colourCount}Blue", "0", "0");
            colourOptionBlue.Parent = colourOptionColourContainer;

            UIContainer colourPreview = new UIContainer("Colour Preview") 
            {
                Width = 100,
                Height = 50,
                Colour = GetColourFromOption(this.colourCount),
                Parent = colourOptionContainer
            };

            colourPreview.OnLeftClick += (object sender, EventArgs e) => 
            {
                colourPreview.Colour = GetColourFromOption(int.Parse(colourPreview.Parent.Name.Split("n")[1][0].ToString()));
            };

            this.UIScene.AddUIProp(colourPreview);


            this.colourCount++;

            UIAlignContainer destroyButton = new UIAlignContainer("Colour Preview") 
            {
                Width = 100,
                Height = 25,
                Colour = Color.Red,
                Parent = colourOptionContainer
            };

            destroyButton.OnRightClick += (object sender, EventArgs e) => 
            {
                this.UIScene.RemoveUIProp(colourOptionContainer);
                this.colourCount--;
            };

            this.UIScene.AddUIProp(destroyButton);

            UIText destroyText = new UIText("Remove Rule")
            {
                Parent = destroyButton,
                Text = "Remove Rule (RMB)",
                Colour = Color.White
            }; 

            this.UIScene.AddUIProp(destroyText);
        } 

        private Color GetColourFromOption(int index) 
        {
            int r = int.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{index}Red").Text);
            int g = int.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{index}Green").Text);
            int b = int.Parse(this.UIScene.GetPropWithName<UIInputText>($"ColourOption{index}Blue").Text);

            return new Color(r,g,b);
        }

        public override void Update()
        {
            base.Update();

            // Noise.Offset += new Vector2(0.1f,0.1f);

            // Map.Texture.Dispose();

            // Map.Texture = null;

            // Map.Texture = Noise.GetTexture(256, 256);
        }
    }
}