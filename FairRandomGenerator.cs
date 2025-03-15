using System.Text;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace FairDice;

public class FairRandomResult
{
    public int Number { get; set; }
    public string Hmac { get; set; }
    public byte[] Key { get; set; }
}

public static class FairRandomGenerator
{
    public static FairRandomResult Generate(int range)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] key = new byte[32];
            rng.GetBytes(key);

            int number;
            do
            {
                byte[] randomBytes = new byte[4];
                rng.GetBytes(randomBytes);
                number = BitConverter.ToInt32(randomBytes, 0) & int.MaxValue;
            } while (number >= int.MaxValue - (int.MaxValue % range));

            number %= range;

            var hmac = new HMac(new Sha3Digest(256)); // HMAC-SHA3-256
            hmac.Init(new KeyParameter(key));
            byte[] hash = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(Encoding.UTF8.GetBytes(number.ToString()), 0, number.ToString().Length);
            hmac.DoFinal(hash, 0);

            return new FairRandomResult
            {
                Number = number,
                Hmac = BitConverter.ToString(hash).Replace("-", "").ToLower(),
                Key = key
            };
        }
    }
}