using _8LETTE_TextRPG.ItemFolder;
using _8LETTE_TextRPG.JobFolder;
using _8LETTE_TextRPG.MonsterFolder;
using _8LETTE_TextRPG.SkillFolder;
using System.Diagnostics.CodeAnalysis;
using TextRPG;

namespace _8LETTE_TextRPG
{

    /// <summary>
    /// 직업 클래스 관련해서
    /// 게임 스크린 또는 게임 시작 루프에서
    /// 선택한 직업을 적용하기 위한 로직은 다음과 같습니다.
    ///  List<Job> jobs = Job.GetAllJobs(); -> 직업 리스트를 가져와서서
    /// Job selectedJob = [선택한 직업] (ex: jobs[입력받은 수 -1])
    /// Player player = new Player(name, selectedJob);플레이어 객체 생성
    /// </summary>
    public class Player
    {
        /// <summary>
        /// 초기화용. 외부 클래스에서 인스턴스 사용 시 CS8602 경고 뜨는 것 방지
        /// </summary>
        private static Player? _instance;

        /// <summary>
        /// 플레이어 인스턴스
        /// </summary>
        [NotNull]
        public static Player Instance
        {
            get => _instance ?? throw new NullReferenceException();
            private set => _instance = value;
        }

        private PlayerContext _context;

        public string Name => _context.Name ?? throw new NullReferenceException();
        public JobBase Job => _context.Job ?? throw new NullReferenceException();
        public Level Level => _context.Level ?? throw new NullReferenceException();
        public PlayerStats Stats => _context.Stats ?? throw new NullReferenceException();

        public float Attack
        {
            get
            {
                if (Stats.BaseAttack + Stats.LevelBonusAtk + Inventory.EquippedAttackBonus() < 0f)
                {
                    return 0f;
                }
                else
                {
                    return Stats.BaseAttack + Stats.LevelBonusAtk + Inventory.EquippedAttackBonus();
                }
            }
        }
        public float Defense
        {
            get
            {
                if (Stats.BaseDefense + Stats.LevelBonusDfs + Inventory.EquippedDefenseBonus() < 0f)
                {
                    return 0f;
                }
                else
                {
                    return Stats.BaseDefense + Stats.LevelBonusDfs + Inventory.EquippedDefenseBonus();
                }
            }
        }
        public float MaxHealth
        {
            get
            {
                if (Stats.BaseHealth + Inventory.EquippedHpBonus() < 1f)
                {
                    return 1f;
                }
                else
                {
                    return Stats.BaseHealth + Inventory.EquippedHpBonus();
                }
            }
        }
        public float Health
        {
            get
            {
                return Stats.CurHealth;
            }
            set
            {
                if (value > MaxHealth)
                {
                    Stats.CurHealth = MaxHealth;
                }
                else if (value <= 0)
                {
                    Stats.CurHealth = 0f;
                    Death();
                }
                else
                {
                    Stats.CurHealth = value;
                }
            }
        }
        public float MaxMana
        {
            get
            {
                if (Stats.BaseMP + Inventory.EquippedMpBonus() < 1f)
                {
                    return 1f;
                }
                else
                {
                    return Stats.BaseMP + Inventory.EquippedMpBonus();
                }
            }
        }
        public float Mana
        {
            get
            {
                return Stats.CurMP;
            }
            set
            {
                if (value > MaxMana)
                {
                    Stats.CurMP = MaxMana;
                }
                else if (value <= 0)
                {
                    Stats.CurMP = 0f;
                }
                else
                {
                    Stats.CurMP = value;
                }
            }
        }

        public float Gold
        {
            get => _context.Gold ?? throw new NullReferenceException();
            set => _context.Gold = value;
        }
        public bool IsDead
        {
            get => _context.IsDead ?? throw new NullReferenceException();
            set => _context.IsDead = value;
        }

        public float CriticalChance
        {
            get
            {
                if (Stats.BaseCriticalChance + Inventory.EquippedCriticalBonus() < 0f)
                {
                    return 0f;
                }
                else if (Stats.BaseCriticalChance + Inventory.EquippedCriticalBonus() > 100f)
                {
                    return 100f;
                }
                else
                {
                    return Stats.BaseCriticalChance + Inventory.EquippedCriticalBonus();
                }
            }
        }
        public float EvasionRate
        {
            get
            {
                if (Stats.BaseEvasionRate + Inventory.EquippedEvasionBonus() < 0f)
                {
                    return 0f;
                }
                else if (Stats.BaseEvasionRate + Inventory.EquippedEvasionBonus() > 100f)
                {
                    return 100f;
                }
                else
                {
                    return Stats.BaseEvasionRate + Inventory.EquippedEvasionBonus();
                }
            }
        }

        //인벤토리
        public Inventory Inventory => _context.Inventory ?? throw new ArgumentNullException("Inventory is not defined.");
        public Dictionary<EquipmentType, string?> EquippedItems => _context.EquippedItems ?? throw new ArgumentNullException("EquippedItems is not defined.");

        //스킬
        private List<Buff> _buffs = new List<Buff>();
        public IEnumerable<Skill> PassiveReflectSkill => Job.Skills.Where(s => s.Type == SkillType.Passive);

        public Player()
        {
            Instance = this;

            PlayerContext? context = PlayerContext.Load();
            if (context == null)
            {
                //이름 입력
                Console.WriteLine("이름을 입력해주세요.");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("\n>> ");
                Console.ResetColor();
                string? userName = Console.ReadLine();
                userName = string.IsNullOrEmpty(userName) ? "8LETTE" : userName;

                _context = new PlayerContext();
                _context.Initialize(userName, new Junior());
            }
            else
            {
                _context = context;
            }
        }

        public void OnContextChanged()
        {
            _context.Save();
        }

        public void GainExp(int exp)
        {
            bool leveledUp = Level.AddExp(exp);
            if (leveledUp)
            {
                Job.IncreaseStats();
            }

            OnContextChanged();
        }

        /// <summary>
        /// 승진 메소드
        /// </summary>
        /// <param name="job"></param>
        public void Promote(JobBase? job)
        {
            if (job == null)
            {
                return;
            }

            _context.Job = job;

            Stats.BaseAttack = job.BaseAttack;
            Stats.BaseDefense = job.BaseDefense;
            Stats.BaseHealth = job.BaseHealth;
            Stats.BaseMP = job.BaseMP;
            Stats.BaseCriticalChance = job.CriticalChance;
            Stats.BaseEvasionRate = job.EvasionRate;

            // 디렉터에서 스킬 계수 강화
            float enforce = (job.PromotionType == PromotionType.Director) ? 1.5f : 1f;
            foreach (Skill skill in job.Skills)
            {
                skill.PromotionMultiplier = enforce;
            }

            OnContextChanged();
        }

        public void AddBuff(Buff buff)
        {
            _buffs.Add(buff);
        }

        /// <summary>
        /// 버프로 인한 공격력증가 반환
        /// </summary>
        /// <returns></returns>
        public float GetBuffAttack()
        {
            float atk = Attack;

            foreach (var buff in _buffs)
            {
                atk *= buff.AttackMultiplier;
            }

            return atk;
        }

        /// <summary>
        /// 버프된 방어력, 몬스터의 데미지 부분에 이 함수 호출
        /// </summary>
        /// <returns></returns>
        public float GetBuffedDefense()
        {
            float def = Defense;
            foreach (var buff in _buffs)
            {
                def *= buff.DefenseMultiplier;
            }
            return def;
        }

        public float GetBuffEvasion() 
        {
            float evs = EvasionRate;
            foreach (Buff buff in _buffs)
            {
                evs += buff.EvasionMultiplier;
            }

            return MathF.Min(evs * 0.01f, 1f); 
        }

        public float GetBuffCritical() 
        {  
            float critical = CriticalChance;
            foreach (Buff buff in _buffs)
            {
                critical += buff.CriticalMultiplier;  
            }

            return MathF.Min(critical * 0.01f, 1f);
        }

        //턴 종료 시 버프 없애기 : 스크린 클래스에서 플레이어가 공격 시 사용
        public void EndTurn()
        {
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                var buff = _buffs[i];
                if (buff.Duration == DurationType.OneTurn)
                {
                    buff.TurnsRemaining--;
                    if (buff.TurnsRemaining <= 0)
                    {
                        //버프 종료
                        _buffs.RemoveAt(i);
                    }
                }
            }
        }

        //전투가 끝나는 타이밍에 버프 제거 메소드
        public void ClearBattleBuffs()
        {
            _buffs.RemoveAll(b => b.Duration == DurationType.UntilBattleEnd);
        }
            
        //방어력에 따른 데미지 감면 로직 구현
        public float ApplyDefenseReduction(float Damage, float Defense)
        {
            float k = 50f / (50f + Defense);
            float mitigate = Damage * k;
            mitigate = (float)Math.Ceiling(mitigate);
            return Math.Max(1, mitigate);
        }

        /// <summary>
        /// 몬스터 공격 메소드. 공격한 몬스터 객체를 파라미터로 받아와서 해당 몬스터의 체력 감소 로직 작성
        /// </summary>
        /// <param name="target"></param>
        public void AttackTo(Monster target)
        {
            Random r = new Random();

            float varirance = (float)Math.Ceiling(Attack * 0.1f);

            //공격력 버프 적용, 버프가 없으면 기본 공격력 적용
            float atk = GetBuffAttack();

            float damage = atk + r.Next(-(int)varirance, (int)varirance);
          
            damage = Math.Max(1, damage);//최소 데미지 보장

            //크리티컬 계산
            bool isCritical = TryCritical();
            if (isCritical)
            {
                //방어력에 따른 데미지 감소
                
                damage = (float)Math.Ceiling(damage * 1.6);
                damage = ApplyDefenseReduction(damage, target.Defense);
                Console.WriteLine($"{Name}의 공격!");
                Console.WriteLine($"Lv.{target.Level} {target.Name}을 공격.  {damage}의 데미지 - 치명타 공격!!");

                //데미지 계산 처리는 몬스터 클래스에서
                target.OnDamaged(damage);

               
            }
            else
            {
                //데미지 계산 처리는 몬스터 클래스에서
                Console.WriteLine($"{Name}의 공격!");
                Console.WriteLine($"Lv.{target.Level} {target.Name}에게 {damage}의 데미지를 입혔습니다.");
                damage = ApplyDefenseReduction(damage, target.Defense);
                target.OnDamaged(damage);

               
            }

            if (target.IsDead)
            {   
                //Console.WriteLine($"\n{target.Name}을(를) 처치했습니다!");
                GainExp(target.Level);
                //만일 몬스터 별로 경험치가 다르게 구현해서
                //속성을 추가해서 파라미터로 받아오게 하면
                //Gain(target.Exp);
            }
        }

        /// <summary>
        /// 회피 확률 계산
        /// </summary>
        /// <returns></returns>
        public bool TryEvade()
        {
            Random r = new Random();
            return r.NextSingle() <= GetBuffEvasion();
        }

        /// <summary>
        /// 치명타 확률 계산
        /// </summary>
        /// <returns></returns>
        public bool TryCritical()
        {
            Random r = new Random();
            return r.NextSingle() <= GetBuffCritical();
        }

        public void OnDamaged(float dmg)
        {
            if (IsDead)
            {
                return;
            }

            Health -= dmg;

            OnContextChanged();
        }

        public void Death()
        {
            _context.IsDead = true;

            OnContextChanged();
        }
    }
}