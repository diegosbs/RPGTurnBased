using System.Collections.Generic;
using System.Linq;

namespace RPGTurnBased.Combat
{
    public enum StatusType
    {
        DefenseBoost,
        AttackBoost,
        Poison,
        Regeneration,
        Stun
    }

    public class StatusEffect
    {
        public StatusType Type { get; set; }
        public int TurnsRemaining { get; set; }
        public float Value { get; set; } // Multiplicador ou valor fixo
        public string Name { get; set; }

        public StatusEffect(StatusType type, int turns, float value, string name)
        {
            Type = type;
            TurnsRemaining = turns;
            Value = value;
            Name = name;
        }

        public bool IsExpired => TurnsRemaining <= 0;

        public void DecrementTurn()
        {
            TurnsRemaining--;
        }
    }

    public class StatusManager
    {
        private List<StatusEffect> _activeEffects;

        public StatusManager()
        {
            _activeEffects = new List<StatusEffect>();
        }

        public void AddEffect(StatusEffect effect)
        {
            // Remove efeito existente do mesmo tipo
            RemoveEffect(effect.Type);
            _activeEffects.Add(effect);
        }

        public void RemoveEffect(StatusType type)
        {
            _activeEffects.RemoveAll(e => e.Type == type);
        }

        public StatusEffect GetEffect(StatusType type)
        {
            return _activeEffects.FirstOrDefault(e => e.Type == type);
        }

        public bool HasEffect(StatusType type)
        {
            return _activeEffects.Any(e => e.Type == type);
        }

        public List<StatusEffect> GetAllEffects()
        {
            return _activeEffects.ToList();
        }

        public void UpdateEffects()
        {
            // Decrementar turnos e remover efeitos expirados
            foreach (var effect in _activeEffects.ToList())
            {
                effect.DecrementTurn();
                if (effect.IsExpired)
                {
                    _activeEffects.Remove(effect);
                }
            }
        }

        public float GetDefenseMultiplier()
        {
            var defenseBoost = GetEffect(StatusType.DefenseBoost);
            return defenseBoost?.Value ?? 1.0f;
        }

        public float GetAttackMultiplier()
        {
            var attackBoost = GetEffect(StatusType.AttackBoost);
            return attackBoost?.Value ?? 1.0f;
        }

        public bool IsStunned()
        {
            return HasEffect(StatusType.Stun);
        }

        public void Clear()
        {
            _activeEffects.Clear();
        }
    }
}