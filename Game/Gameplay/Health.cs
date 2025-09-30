using System;
using Bean;

namespace DemoGame
{
    public class Health : Addon
    {
        public int MaxHealth;

        [DebugServerVariable]
        public int CurrentHealth { get; set; }

        private bool _finalStand;

        public EventHandler OnDeath;

        public Health(int maxHealth, bool finalStand = false)
        {
            this.MaxHealth = maxHealth;

            this._finalStand = finalStand;
        }

        public override void Start()
        {
            base.Start();

            this.CurrentHealth = MaxHealth;
        }

        public void AddHealth(int amount)
        {
            this.CurrentHealth += amount;

            this.CurrentHealth = Math.Clamp(this.CurrentHealth, 1, this.MaxHealth);
        }

        public void RemoveHealth(int amount)
        {
            if (this._finalStand && this.CurrentHealth == 1)
                this.Die();

            this.CurrentHealth -= amount;

            this.CurrentHealth = Math.Clamp(this.CurrentHealth, (this._finalStand) ? 1 : 0, this.MaxHealth);

            if (this.CurrentHealth <= 0 && !this._finalStand)
                this.Die();
        }

        public void Die()
        {
            this.OnDeath?.Invoke(this, EventArgs.Empty);
        }



    }
}