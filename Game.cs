namespace FairDice;

// Main game logic.
public class Game
{
    private readonly List<Dice> DiceList;
    private Dice userDice, computerDice;

    public Game(List<Dice> diceList)
    {
        DiceList = diceList;
    }
    public void Start()
    {
        Console.WriteLine("Let's determine who makes the first move.");
        FairRandomResult fairResult = FairRandomGenerator.Generate(2);
        int computerBit = fairResult.Number;
        Console.WriteLine($"I selected a random value in the range 0..1 (HMAC={fairResult.Hmac}).");
        Console.WriteLine("Try to guess my selection.");
        while (true)
        {
            Console.WriteLine("0 - 0");
            Console.WriteLine("1 - 1");
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help (show probability table)");
            Console.Write("Your selection: ");
            string input = Console.ReadLine()?.Trim();
            if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
                return;
            if (input.Equals("?", StringComparison.OrdinalIgnoreCase))
            {
                ProbabilityTable.DisplayTable(DiceList);
                continue;
            }
            if (int.TryParse(input, out int userGuess) && (userGuess == 0 || userGuess == 1))
            {
                Console.WriteLine($"My selection: {computerBit} (KEY={BitConverter.ToString(fairResult.Key).Replace("-", "")}).");
                bool userFirst = (userGuess == computerBit);
                Console.WriteLine(userFirst ? "You will select your dice first." : "I will select my dice first.");
                SelectDice(userFirst);
                PlayThrows();
                return;
            }
            Console.WriteLine("Invalid selection. Try again.");
        }
    }
    private void SelectDice(bool userFirst)
    {
        List<int> availableIndices = Enumerable.Range(0, DiceList.Count).ToList();
        
        if (userFirst)
        {
            userDice = PromptDiceSelection("Choose your dice:", availableIndices);
            computerDice = GetBestDiceForComputer(availableIndices, userDice);
            availableIndices.Remove(DiceList.IndexOf(userDice));
        }
        else
        {
            computerDice = GetBestDiceForComputer(availableIndices, null);
            availableIndices.Remove(DiceList.IndexOf(computerDice));
            userDice = PromptDiceSelection("Choose your dice:", availableIndices);
        }

        Console.WriteLine($"You selected the dice: [{string.Join(",", userDice.Faces)}]");
        Console.WriteLine($"I selected the dice: [{string.Join(",", computerDice.Faces)}]");
    }
    private Dice GetBestDiceForComputer(List<int> availableIndices, Dice? userDice)
    {
        Dice? bestChoice = null;

        if (userDice is not null)
        {
            bestChoice = availableIndices
                .Select(index => DiceList[index])
                .OrderByDescending(dice => ProbabilityCalculator.CalculateWinningProbability(dice, userDice))
                .FirstOrDefault();
        }
        else
        {
            bestChoice = availableIndices
                .Select(index => DiceList[index])
                .OrderByDescending(candidateDice =>
                    availableIndices
                        .Where(index => DiceList[index] != candidateDice)
                        .Select(comparisonDice => ProbabilityCalculator.CalculateWinningProbability(candidateDice, DiceList[comparisonDice]))
                        .DefaultIfEmpty(1.0)
                        .Min()
                )
                .FirstOrDefault();
        }

        // If no better choice was found, pick a random dice
        return bestChoice ?? DiceList[availableIndices[new Random().Next(availableIndices.Count)]];
    }
    private Dice PromptDiceSelection(string prompt, List<int> availableIndices)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            foreach (int idx in availableIndices)
            {
                Console.WriteLine($"{idx} - [{string.Join(",", DiceList[idx].Faces)}]");
            }
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help (show probability table)");
            Console.Write("Your selection: ");
            string input = Console.ReadLine()?.Trim();
            if (input.Equals("X", StringComparison.OrdinalIgnoreCase))
                Environment.Exit(0);
            if (input.Equals("?", StringComparison.OrdinalIgnoreCase))
            {
                ProbabilityTable.DisplayTable(DiceList);
                continue;
            }
            if (int.TryParse(input, out int selection) && availableIndices.Contains(selection))
            {
                return DiceList[selection];
            }
            Console.WriteLine("Invalid selection. Try again.");
        }
    }

    // Each party performs a throw using their dice.
    private void PlayThrows()
    {
        Console.WriteLine("---- Computer's Throw ----");
        int compIndex = GetFairDiceIndex(computerDice);
        int computerThrow = computerDice.Faces[compIndex];
        Console.WriteLine($"My throw is {computerThrow}.");

        Console.WriteLine("---- Your Throw ----");
        int userIndex = GetFairDiceIndex(userDice);
        int userThrow = userDice.Faces[userIndex];
        Console.WriteLine($"Your throw is {userThrow}.");

        if (userThrow > computerThrow)
            Console.WriteLine($"You win! ({userThrow}>{computerThrow})");
        else if (userThrow < computerThrow)
            Console.WriteLine($"You loose! ({userThrow}<{computerThrow})");
        else
            Console.WriteLine($"It's a tie!({userThrow}={computerThrow})");
    }

    // Implements the fair random protocol for a dice throw.
    private int GetFairDiceIndex(Dice dice)
    {
        int sides = dice.Faces.Length;
        FairRandomResult fairResult = FairRandomGenerator.Generate(sides);
        Console.WriteLine($"I selected a random value in the range 0..{sides - 1} (HMAC={fairResult.Hmac}).");
        int userContribution = FairRandomProtocol.GetUserModularChoice(sides);
        if (userContribution == -1) Environment.Exit(0);
        int finalIndex = (fairResult.Number + userContribution) % sides;
        Console.WriteLine($"My number is {fairResult.Number} (KEY={BitConverter.ToString(fairResult.Key).Replace("-", "")}).");
        Console.WriteLine($"The result is ({fairResult.Number} + {userContribution}) mod {sides} = {finalIndex}.");
        return finalIndex;
    }
}