namespace FairDice;
public static class DiceParser
{
    public static System.Collections.Generic.List<Dice> Parse(string[] args)
    {
        var diceList = new System.Collections.Generic.List<Dice>();
        foreach (var arg in args)
        {
            int[] faces = arg.Split(',')
                .Select(s => int.Parse(s.Trim()))
                .ToArray();
            if (faces.Length < 1)
                throw new Exception("Each dice must have at least one face.");
            diceList.Add(new Dice(faces));
        }
        return diceList;
    }
}