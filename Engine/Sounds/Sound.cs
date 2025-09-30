using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Bean.Sounds
{
    public class Sound
    {
        public float ClonedSoundModifier = 0.20f;

        public SoundEffect Effect { get; private set; }

        public float FadeOutStart = 100;

        public float FadeOutDistance = 500;

        public bool Is3D;

        public bool IsLooped;

        public float MaxVoloume = 1f;

        public int SoundKey { get; private set; }

        public float SoundRollOffAmout = 5;

        public Vector2 Position;

        public Sound(SoundManager manager, SoundEffect effect, bool is3D = false)
        {
            this.Effect = effect;
            this.Is3D = is3D;

            this.SoundKey = manager.GenerateSoundKey();
        }
    }
}
