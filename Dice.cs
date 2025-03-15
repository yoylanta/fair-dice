namespace FairDice;

// Represents a dice with an array of face values.
public class Dice
{
    public int[] Faces { get; }
    
    public Dice(int[] faces)
    {
        Faces = faces;
    }
    
    public int GetFaceCount()
    {
        return Faces.Length;
    }
}