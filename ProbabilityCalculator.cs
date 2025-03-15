namespace FairDice;

public static class ProbabilityCalculator
{
    /// <summary>
    /// Calculates the probability that diceA wins over diceB.
    /// A win is counted when a face from diceA is strictly greater than a face from diceB.
    /// </summary>
    /// <param name="diceA">The first dice.</param>
    /// <param name="diceB">The second dice.</param>
    /// <returns>A value between 0.0 and 1.0 representing the winning probability.</returns>
    public static double CalculateWinningProbability(Dice diceA, Dice diceB)
    {
        int wins = 0;
        int totalComparisons = diceA.Faces.Length * diceB.Faces.Length;
        foreach (var faceA in diceA.Faces)
        {
            foreach (var faceB in diceB.Faces)
            {
                if (faceA > faceB)
                    wins++;
            }
        }
        return (double)wins / totalComparisons;
    }
}