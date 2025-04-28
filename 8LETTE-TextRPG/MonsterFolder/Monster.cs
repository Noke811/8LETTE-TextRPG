namespace _8LETTE_TextRPG.MonsterFolder
{
    public abstract class Monster
    {
        public enum State
        {
            Invalid = -1,
            Idle,
            Attack,
            Dead
        }

        public MonsterType Type { get; protected set; }
        public string? Name { get; protected set; }
        public int Level { get; protected set; }
        public float MaxHp { get; protected set; }
        private float _hp;
        public float Hp
        {
            get
            {
                return _hp;
            }
            set
            {
                if (value > MaxHp)
                {
                    _hp = MaxHp;
                }
                else if (value <= 0f)
                {
                    _hp = 0f;
                    Death();
                }
                else
                {
                    _hp = value;
                }
            }
        }
        public float Defense { get; protected set; }
        public float Attack { get; protected set; }
        public bool IsDead => CurState == State.Dead;

        public int GoldReward { get; protected set; }

        public State CurState
        {
            get
            {
                return _curState;
            }
            set
            {
                TransitionTo(value);
            }
        }

        public State InvalidState => State.Invalid;

        private State _curState;
        private State _prevState;

        private readonly Dictionary<State, StateElem> _states = [];

        protected class StateElem
        {
            public Action? Entered;
            public Action? Doing;
            public Action? Exited;
        }

        public Monster()
        {
            DefineStates();
        }

        protected abstract void DefineStates();

        protected void TransitionTo(State nextState)
        {
            _prevState = _curState;
            _curState = nextState;
            if (_prevState != InvalidState)
            {
                if (_states.TryGetValue(_prevState, out StateElem? stateElem))
                {
                    if (stateElem != null)
                    {
                        stateElem.Exited?.Invoke();
                    }
                }
            }

            if (_curState != InvalidState)
            {
                if (_states.TryGetValue(_curState, out StateElem? stateElem))
                {
                    if (stateElem != null)
                    {
                        stateElem.Entered?.Invoke();
                        stateElem.Doing?.Invoke();
                    }
                }
            }
        }

        protected void AddState(State state, StateElem stateElem)
        {
            _states.Add(state, stateElem);
        }

        public virtual void OnDamaged(float dmg)
        {
            Hp -= dmg;
        }

        public virtual void AttackTo(Player target)
        {
            if (target.IsDead)
            {
                return;
            }

            Random r = new Random();
            float varirance = (float)Math.Ceiling(Attack * 0.1f);

            
            float damage = Attack + r.Next(-(int)varirance, (int)varirance);
            damage = Math.Max(1, damage);
            float PlayerDefense = Player.Instance.GetBuffedDefense();

            // 방어력에 따른 데미지 감소 로직
            damage = Player.Instance.ApplyDefenseReduction(damage, PlayerDefense);

            //플레이어 회피율 계산
            bool isEvasion = Player.Instance.TryEvade();

            if (isEvasion)
            {
                // damage = 0;

                //몬스터 정보 출력
                Console.WriteLine($"Lv. {Level} {Name} 의 공격!");
                Console.WriteLine($"{target.Name} 을(를) 공격했지만 아무일도 일어나지 않았습니다.\n");

                // 카운터 패시브스킬이 있다면 모두 실행
                foreach (var refl in target.PassiveReflectSkill)
                {
                    refl.Execute(target, this);
                }
            }
            else
            {
                //몬스터 정보 출력
                Console.WriteLine($"Lv. {Level} {Name} 의 공격!");
                Console.WriteLine($"{target.Name} 을(를) 맞췄습니다. [데미지 : {damage}]\n");

                //체력 음수 방지
                float FinalHp = target.Health - damage;

                if (FinalHp < 0) { FinalHp = 0; }
                //캐릭터 정보 출력
                Console.WriteLine($"Lv.{target.Level.CurrentLevel} {target.Name}");
                Console.WriteLine($"HP {target.Health} -> {FinalHp}\n");

                target.OnDamaged(damage);
            }
        }

        protected virtual void Death()
        {
            CurState = State.Dead;
            Player.Instance.Gold += GoldReward;
            QuestManager.Instance?.SendProgress(QuestType.KillMonster, Type.ToString(), 1);
        }
    }
}
