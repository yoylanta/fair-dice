using ConsoleTables; 
namespace FairDice;

public static class ProbabilityTable
{
    /// <summary>
    /// Displays an ASCII table of winning probabilities for each dice pair.
    /// </summary>
    /// <param name="diceList">The list of dice.</param>
    public static void DisplayTable(List<Dice> diceList)
    {
        int n = diceList.Count;
        Console.WriteLine("Dice Definitions:");
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"D{i} = {{{ShowDiceFaces(diceList[i].Faces)}}}");
        }
        
        var header = new List<string> { "Dice" };
        for (int j = 0; j < n; j++)
        {
            header.Add($"D{j}");
        }
        
        var table = new ConsoleTable(header.ToArray());
        for (int i = 0; i < n; i++)
        {
            var row = new List<string> { $"D{i}" };
            for (int j = 0; j < n; j++)
            {
                if (i == j)
                {
                    row.Add("-");
                }
                else
                {
                    double prob = ProbabilityCalculator.CalculateWinningProbability(diceList[i], diceList[j]);
                    row.Add((prob * 100).ToString("F2") + "%");
                }
            }
            table.AddRow(row.ToArray());
        }
        
        Console.WriteLine("Winning Probabilities Table (Row dice wins over Column dice):");
        table.Write(Format.Alternative);
    }
    
    static string ShowDiceFaces(IEnumerable<int> numbers)
    {
        return string.Join(", ", numbers);
    }
}    
