﻿using _8LETTE_TextRPG.QuestFolder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8LETTE_TextRPG.ItemFolder
{
    class EquipableItem : Item, IEquipable
    {
        public EquipmentType EquipmentType { get; set; }
        public bool IsEquipped { get; set; }

        public EquipableItem(string id, string name, string desc, float price, EquipmentType equipmentType, Dictionary<ItemEffect, float> effects)
        {
            Id = id;
            Name = name;
            Description = desc;

            Price = price;
            ItemType = ItemType.Equipment;

            Effects = effects;
            EquipmentType = equipmentType;
        }

        public override string GetEffectName()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<ItemEffect, float> effectPair in Effects)
            {
                if (effectPair.Value != 0f)
                {
                    switch (effectPair.Key)
                    {
                        case ItemEffect.Atk:
                            sb.Append("공격력 ");
                            sb.Append(effectPair.Value > 0f ? "+" : "");
                            sb.Append(effectPair.Value);
                            sb.Append(" ");
                            break;
                        case ItemEffect.Def:
                            sb.Append("방어력 ");
                            sb.Append(effectPair.Value > 0f ? "+" : "");
                            sb.Append(effectPair.Value);
                            sb.Append(" ");
                            break;
                        case ItemEffect.Hp:
                            sb.Append("최대HP ");
                            sb.Append(effectPair.Value > 0f ? "+" : "");
                            sb.Append(effectPair.Value);
                            sb.Append(" ");
                            break;
                        case ItemEffect.Critical:
                            sb.Append("치명 ");
                            sb.Append(effectPair.Value > 0f ? "+" : "");
                            sb.Append(effectPair.Value);
                            sb.Append(" ");
                            break;
                        case ItemEffect.Evasion:
                            sb.Append("회피 ");
                            sb.Append(effectPair.Value > 0f ? "+" : "");
                            sb.Append(effectPair.Value);
                            sb.Append(" ");
                            break;
                        case ItemEffect.MP:
                            sb.Append("최대MP ");
                            sb.Append(effectPair.Value > 0f ? "+" : "");
                            sb.Append(effectPair.Value);
                            sb.Append(" ");
                            break;
                        default:
                            sb.Append(" 알 수 없음 ");
                            sb.Append(effectPair.Value > 0f ? "+" : "");
                            sb.Append(effectPair.Value);
                            sb.Append(" ");
                            break;
                    }
                }
            }

            return sb.ToString();
        }

        public void Equip()
        {
            IsEquipped = true;
            Player.Instance.EquippedItems[EquipmentType] = Id;

            // 현재 체력이 최대체력보다 많아지는 거 방지
            if (Player.Instance.Health > Player.Instance.MaxHealth)
            {
                Player.Instance.Health = Player.Instance.MaxHealth;
            }
        }

        public void Unequip()
        {
            IsEquipped = false;
            Player.Instance.EquippedItems[EquipmentType] = string.Empty;
        }
    }
}
