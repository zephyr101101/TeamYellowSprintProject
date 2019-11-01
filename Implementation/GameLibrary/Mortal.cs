using System;

namespace GameLibrary {      
    public class Mortal 
    {
        #region Constants
        private const float INIT_HEALTH = 100;
        private const float INIT_STR = 10;
        private const float INIT_DEF = 5;
        private const float INIT_LUCK = 2;
        private const float INIT_SPEED = 2;
        private const float INIT_MANA = 40;

        // New Stats INIT values
        private const float INIT_STRENGTH = 2;
        private const float INIT_DEXTERITY = 2;
        private const float INIT_CONSTITUTION = 2;
        private const float INIT_INTELLIGENCE = 2;
        private const float INIT_WISDOM = 2;
        private const float INIT_CHARISMA = 2;
        

        private const float LVLINC_HEALTH = 20;
        private const float LVLINC_STR = 3;
        private const float LVLINC_DEF = 2;
        private const float LVLINC_LUCK = 1;
        private const float LVLINC_SPEED = 2;
        private const float LVLINC_MANA = 10;

        // New Stats LVLINC values
        private const float LVLINC_STRENGTH = 1;
        private const float LVLINC_DEXTERITY = 1;
        private const float LVLINC_CONSTITUTION = 1;
        private const float LVLINC_INTELLIGENCE = 1;
        private const float LVLINC_WISDOM = 1;
        private const float LVLINC_CHARISMA = 1;

        private const float SIMPLEATTACK_RANDOM_AMT = 0.25f;
        #endregion
            
        public string Name { get; protected set; }
        public int Level { get; protected set; }
        public float MaxHealth { get; protected set; }
        public float Health { get; protected set; }
        public float MaxMana { get; protected set; }
        public float Mana { get; protected set; }

        // New Stats
        public float Strength { get; protected set; }
        public float Dexterity { get; protected set; }
        public float Constitution { get; protected set; }
        public float Intelligence { get; protected set; }
        public float Wisdom { get; protected set; }
        public float Charisma { get; protected set; }
        public float Luck { get; protected set; }


        public float Str { get; protected set; }
        public float Def { get; protected set; }
        
        public float Speed { get; protected set; }

        private Random rand;
    
        public Mortal(string name, int level) {
            Name = name;
            ResetStats();
            SetLevel(level);
            rand = new Random();
        }
        public virtual void ResetStats() {
            Level = 1;
            MaxHealth = INIT_HEALTH;
            Health = MaxHealth;
            MaxMana = INIT_MANA;
            Mana = MaxMana;
            Str = INIT_STR;
            Def = INIT_DEF;
            Luck = INIT_LUCK;
            Speed = INIT_SPEED;

            // New Stat Reset Function
            Strength = INIT_STRENGTH;
            Dexterity = INIT_DEXTERITY;
            Constitution = INIT_CONSTITUTION;
            Intelligence = INIT_INTELLIGENCE;
            Wisdom = INIT_WISDOM;
            Charisma = INIT_CHARISMA;
            Luck = INIT_LUCK;

        }
        public void SetLevel(int level) {
            for (int i = 1; i < level; i++) {
            LevelUp();
            }
        }
        public virtual void LevelUp() {
            // level increases
            Level++;

            // health and mana
            MaxHealth += LVLINC_HEALTH;
            MaxMana += LVLINC_MANA;
            Health = MaxHealth;
            Mana = MaxMana;

            // other stats
            Str += LVLINC_STR;
            Def += LVLINC_DEF;
            Luck += LVLINC_LUCK;
            Speed += LVLINC_SPEED;

            // New stats increment
            Strength += LVLINC_STRENGTH;
            Dexterity += LVLINC_DEXTERITY;
            Constitution += LVLINC_CONSTITUTION;
            Intelligence += LVLINC_INTELLIGENCE;
            Wisdom += LVLINC_WISDOM;
            Charisma += LVLINC_CHARISMA;
        }
        public void RefillHealthAndMana() {
            Health = MaxHealth;
            Mana = MaxMana;
        }
        public void SimpleAttack(Mortal receiver) 
        {
            if(Str <= 0)
            {
                return;
            }
            float baseDamage = Math.Abs(Str * 1.2f - receiver.Def);
            float randMax = 1 + SIMPLEATTACK_RANDOM_AMT;
            float randMin = 1 - SIMPLEATTACK_RANDOM_AMT;
            float randMult = (float)(rand.NextDouble() * (randMax - randMin)) + randMin;
            receiver.Health -= (baseDamage * randMult);
        }
        public void Attack2(Mortal receiver)
        {
            if(Mana != 0)
            {
                float baseDamage = (float) Math.Abs(Mana * .75 + Level);
                receiver.Health -= baseDamage;
                Mana = 0;
            }
            return;
           
        }
        /*
        //function to check if characters stats are below 0 ie mana,strength,etc
        public void statCheck(Mortal Character, Mortal receiver)
        {
            if(Character)
        }
        */
        public void WeakenAttack(Mortal receiver)
        {
            if (receiver.Str <= 0 )
            {
                return;
            }
            receiver.Str -= 1;
            Mana -= 10;
        }
    }
}
