﻿namespace _8LETTE_TextRPG
{
    class Rest
    {
        public float Price { get; private set; }

        public Rest(float price)
        {
            Price = price;
        }

        public void StartRest()
        {
            if (Player.Instance.Gold < Price || Player.Instance.Health == Player.Instance.MaxHealth && Player.Instance.Mana == Player.Instance.MaxMana && !Player.Instance.IsDead)
            {
                return;
            }

            Player.Instance.Gold -= Price;
            Player.Instance.IsDead = false;
            Player.Instance.Health = Player.Instance.MaxHealth;
            Player.Instance.Mana = Player.Instance.MaxMana;

            Player.Instance.OnContextChanged();
        }
    }
}
