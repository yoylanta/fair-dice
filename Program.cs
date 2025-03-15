namespace FairDice;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            List<Dice> diceList = DiceParser.Parse(args);
            if (diceList.Count < 3)
            {
                Console.WriteLine("Error: At least 3 dice configurations are required.");
                Console.WriteLine("Example: game.exe 2,2,4,4,9,9 6,8,1,1,8,6 7,5,3,7,5,3");
                return;
            }
            int faceCount = diceList[0].GetFaceCount();
            foreach (var dice in diceList)
            {
                if (dice.GetFaceCount() != faceCount)
                {
                    Console.WriteLine("Error: All dice must have the same number of faces.");
                    return;
                }

                if (dice.GetFaceCount() < 4 || dice.GetFaceCount() > 20)
                {
                    Console.WriteLine("Error: Each dice must have between 4 and 20 faces.");
                    return;
                }
            }
            Game game = new Game(diceList);
            game.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Example usage: game.exe 2,2,4,4,9,9 6,8,1,1,8,6 7,5,3,7,5,3");
        }
    }
}