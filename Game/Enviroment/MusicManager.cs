using Bean;
using Bean.Sounds;

namespace DemoGame
{
    public class MusicManager : WorldProp
    {
        public SoundHolder soundHolder;

        public override void Start()
        {
            base.Start();

            this.soundHolder = new SoundHolder();

            this.AddAddon(this.soundHolder);

            this.soundHolder.AddSound("DayTime", "Music/DayTimeTune");
            this.soundHolder.AddSound("NightTime", "Music/NightTimeTune", isLooped: true);
        }
    }
}