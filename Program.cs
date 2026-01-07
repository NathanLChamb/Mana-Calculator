using System.ComponentModel.Design;
using System.Data;

namespace ManaCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Mana Calculator";
            Calculations calcs = new Calculations();
            Player player = new Player();
            ConsoleKey key;
            bool manaRecoveryGood = false;
            do
            {
                Console.Clear();
                Console.WriteLine($"Current specs are:");
                Console.WriteLine($"\t\tDaily Mana Recovery: {calcs.DailyManaRecovery}");
                Console.WriteLine($"\t\tCalc Interval: {calcs.CalcInterval}");
                Console.WriteLine($"\t\tBase Interval: {calcs.BaseInterval}");
                Console.WriteLine("\t   " + new string('-', 35));
                Console.WriteLine($"\t\tDaily Intervals: {calcs.DailyIntervals}");
                Console.WriteLine($"\n\n");
                Console.WriteLine($"Change specs? y/n\n");
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Y:
                        calcs.ChangeSpecs();
                        break;
                    case ConsoleKey.N:
                        manaRecoveryGood = true;
                        break;
                }
            } while (!manaRecoveryGood);


            int cursorLocation = 0;
            SpellData previousSpell = null;
            int damage = 0;
            int turn = 1;
            Random random = new Random();
            List<string> log = new();
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Round: {turn}   {player.Name}       Mana: {player.CurrentMana}/{player.MaxMana}");
                if (turn > 1 && key == ConsoleKey.Enter || key == ConsoleKey.X)
                    if (key == ConsoleKey.X) log.Add($"You rested and restored {calcs.CalcInterval} mana.");
                    else log.Add($"You restored {calcs.CalcInterval} mana and cast {previousSpell.name} dealing {damage} damage.");
                Console.WriteLine($"\n\n");
                Console.WriteLine("You open your spell book and decide what spell to cast: \n");

                for (int row = 0; row < player.spellBook.Count; row++)
                {
                    if (cursorLocation == row)
                        Console.WriteLine($"  > {player.spellBook[row].name,-10}  Mana Cost: {player.spellBook[row].manaCost,-6}  Damage: {player.spellBook[row].minDamage}-{player.spellBook[row].maxDamage}");
                    else
                        Console.WriteLine($"    {player.spellBook[row].name,-10}  Mana Cost: {player.spellBook[row].manaCost,-6}  Damage: {player.spellBook[row].minDamage}-{player.spellBook[row].maxDamage}");
                }
                (int left, int right) = Console.GetCursorPosition();
                Console.SetCursorPosition(10, Console.WindowHeight - 15);
                int start = Math.Max(0, log.Count - 5);
                for (int i = start; i < log.Count; i++)
                {
                    Console.SetCursorPosition(10, Console.CursorTop);
                    Console.WriteLine("     " + log[i]);
                }

                key = Console.ReadKey(true).Key;
                Console.SetCursorPosition(left, right);
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        cursorLocation = (cursorLocation + player.spellBook.Count - 1) % player.spellBook.Count;
                        break;
                    case ConsoleKey.DownArrow:
                        cursorLocation = (cursorLocation + 1) % player.spellBook.Count;
                        break;
                    case ConsoleKey.Enter:
                        player.CastSpell(player.spellBook[cursorLocation]);
                        previousSpell = player.spellBook[cursorLocation];
                        damage = random.Next(previousSpell.minDamage, previousSpell.maxDamage);
                        break;
                    case ConsoleKey.X:
                        break;
                }

                if (key == ConsoleKey.Enter)
                {
                    player.CurrentMana += calcs.CalcInterval;
                    turn++;
                }
                if (key == ConsoleKey.X)
                {
                    player.CurrentMana += calcs.CalcInterval;
                    turn++;
                }
            }
        }
    }
    class Calculations
    {
        public int DailyManaRecovery { get; set; } = 100;
        public int CalcInterval { get; set; } = 2;
        public int BaseInterval { get; set; } = 24;
        public int DailyIntervals => BaseInterval / CalcInterval;
        public Calculations() { }
        public void ChangeSpecs()
        {
            int input = -1;
            while (input < 1 || input > 3)
            {
                Console.WriteLine("Available specs to change:");
                Console.WriteLine("  1. Daily Mana Recovery");
                Console.WriteLine("  2. Calculate Interval");
                Console.WriteLine("  3. Base Interval");

                int.TryParse(Console.ReadLine(), out input);
            }
            int newValue = 0;
            while (newValue < 1)
            {
                Console.Write("Enter new value: ");
                int.TryParse(Console.ReadLine(), out newValue);
            }
            switch (input)
            {
                case 1:
                    this.DailyManaRecovery = newValue;
                    break;
                case 2:
                    this.CalcInterval = newValue;
                    break;
                case 3:
                    this.BaseInterval = newValue;
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    class Player
    {
        public string Name = "Player";
        public int CurrentMana { get; set; } = 50;
        public int MaxMana { get; set; } = 50;
        public int CurrentHP { get; set; } = 20;
        public int MaxHP { get; set; } = 20;
       
        public Player() 
        {
            spellBook.Add(SpellCatalogue.Icebolt);
            spellBook.Add(SpellCatalogue.Blizzard);
            spellBook.Add(SpellCatalogue.Firebolt);
            spellBook.Add(SpellCatalogue.Fireball);
        }
        public List<SpellData> spellBook = new();
        public void CastSpell(SpellData spellData)
        {
            this.CurrentMana -= spellData.manaCost;
        }

    }
    public record SpellData(int id, string name, int manaCost, int minDamage, int maxDamage);
    public static class SpellCatalogue
    {
        public static readonly SpellData Icebolt = new SpellData(1, "Icebolt", 2, 1, 2);
        public static readonly SpellData Blizzard = new SpellData(1, "Blizzard", 15, 10, 15);
        public static readonly SpellData Firebolt = new SpellData(1, "Firebolt", 2, 1, 3);
        public static readonly SpellData Fireball = new SpellData(1, "Fireball", 4, 3, 6);
    }
}
