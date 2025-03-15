namespace FairDice;

// Implements the protocol for receiving a user’s modular choice.
public static class FairRandomProtocol
{
    public static int GetUserModularChoice(int mod)
    {
        while (true)
        {
            Console.WriteLine("Add your number modulo " + mod + ":");
            for (int i = 0; i < mod; i++)
                Console.WriteLine($"{i} - {i}");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help (show probability table)");
            Console.Write("Your selection: ");
            string input = Console.ReadLine()?.Trim();
            if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
                return -1;
            if (input.Equals("?", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Help: Enter a number between 0 and " + (mod - 1) + ".");
                continue;
            }
            if (int.TryParse(input, out int value) && value >= 0 && value < mod)
                return value;
            Console.WriteLine("Invalid selection. Try again.");
        }
    }
}