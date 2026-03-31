using Bean.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Xml.Serialization;

namespace Bean.Sounds
{
    public class SoundHolder : Addon
    {
        private Dictionary<string, Sound> _sounds = new Dictionary<string, Sound>();

        private SoundManager _soundManager;

        public SoundHolder(string name) : base(name)
        {
            this._soundManager = this.Parent.Scene.SoundManager;
        }

        public void AddSound(string name, string soundPath, bool is3D = false, bool isLooped = false)
        {

            Sound sound = new Sound(this._soundManager, FileManager.LoadFromFile<SoundEffect>(soundPath), is3D);

            sound.IsLooped = isLooped;

            this._sounds.Add(name, sound);

            this._soundManager.AddSound(sound);
        }

        public void RemoveSound(string name)
        {
            this._soundManager.RemoveSound(this._sounds[name]);
            
            this._sounds.Remove(name);
        }

        public void PlaySound(string name)
        {
            this._soundManager.PlaySound(this._sounds[name].SoundKey);
        }

        public void PlaySound(string name, Vector2 position)
        {
            this._sounds[name].Position = position;
            this._soundManager.PlaySound(this._sounds[name].SoundKey);
        }

        public void PauseSound(string name)
        {
            this._soundManager.PauseSound(this._sounds[name].SoundKey);
        }

        public void StopSound(string name, bool instantly)
        {
            this._soundManager.StopSound(this._sounds[name].SoundKey, instantly);
        }

        public void ResumeSound(string name)
        {
            this._soundManager.ResumeSound(this._sounds[name].SoundKey);
        }

        public override void Update()
        {
            base.Update();

            // foreach (KeyValuePair<string, Sound> sound in this._sounds)
            // {
            //     if (sound.Value.Is3D)
            //     {
            //         Parent.Scene.SoundManager.UpdateSoundPan(sound.Value.SoundKey, base.Parent.Position);
            //         Parent.Scene.SoundManager.UpdateSoundVoloume(sound.Value.SoundKey, base.Parent.Position);
            //         Parent.Scene.SoundManager.UpdateSoundModifier(sound.Value.SoundKey);
            //     }
            // }
        }

        public override void Destroy()
        {
            foreach (KeyValuePair<string, Sound> sound in this._sounds)
            {
                Parent.Scene.SoundManager.DestroySound(sound.Value.SoundKey);
            }

            base.Destroy();
        }
    }
}
