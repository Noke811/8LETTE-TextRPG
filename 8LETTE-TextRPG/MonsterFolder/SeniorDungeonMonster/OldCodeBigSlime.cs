﻿using _8LETTE_TextRPG.ItemFolder;
using _8LETTE_TextRPG.ScreenFolder;

namespace _8LETTE_TextRPG.MonsterFolder.SeniorDungeonMonster
{
    class OldCodeBigSlime : Monster
    {
        public OldCodeBigSlime()
        {
            Type = MonsterType.Slime;
            Name = "올드코드 빅슬라임";
            Level = 11;
            MaxHp = 40f;
            Hp = MaxHp;
            Attack = 30f;
            Defense = 30f;
            GoldReward = 600;
        }

        protected override void DefineStates()
        {
            AddState(State.Idle, new StateElem
            {
                Doing = new Action(OnIdle)
            });

            AddState(State.Attack, new StateElem
            {
                Doing = new Action(AttackDoing)
            });

            AddState(State.Dead, new StateElem
            {
                Doing = new Action(OnDeath)
            });
        }

        private void OnIdle()
        {

        }

        private void AttackDoing()
        {
            AttackTo(Player.Instance);
        }

        private void OnDeath()
        {
            Random r = new Random();
            Console.WriteLine($"{Name}을(를) 처지했습니다!");
            if (r.NextSingle() <= 0.05f)
            {
                Item droppedItem = new UsableItem(Guid.NewGuid().ToString(), "체력 포션 (20)", "사용 시 HP를 20 회복합니다.", 70f, UseType.Potion, new Dictionary<ItemEffect, float>
                {
                    { ItemEffect.Hp, 20 }
                });
                Player.Instance.Inventory.AddItem(droppedItem);

                Console.WriteLine($"{Name}(이)가 {droppedItem.Name}을(를) 드랍했습니다.");
                BattleResultScreen.Instance.PrintDropItem += () => Console.WriteLine($"{Name}(이)가 {droppedItem.Name}을(를) 드랍했습니다.");
            }
        }

        public override void OnDamaged(float dmg)
        {
            base.OnDamaged(dmg);
        }

        public override void AttackTo(Player victim)
        {
            base.AttackTo(victim);
        }
    }
}