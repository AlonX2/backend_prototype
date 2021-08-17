using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;


namespace Backend_prototype_1
{
    class tempTools
    {
        // temp class
        public static string Keccak256(string rawTransaction) //should be in web3. use this for now
        {
            var offset = rawTransaction.StartsWith("0x") ? 2 : 0;

            var txByte = Enumerable.Range(offset, rawTransaction.Length - offset)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(rawTransaction.Substring(x, 2), 16))
                             .ToArray();

            //Note: Not intended for intensive use so we create a new Digest.
            //if digest reuse, prevent concurrent access + call Reset before BlockUpdate
            var digest = new KeccakDigest(256);

            digest.BlockUpdate(txByte, 0, txByte.Length);
            var calculatedHash = new byte[digest.GetByteLength()];
            digest.DoFinal(calculatedHash, 0);

            var transactionHash = BitConverter.ToString(calculatedHash, 0, 32).Replace("-", "").ToLower();

            return transactionHash;
        }
    }
}
