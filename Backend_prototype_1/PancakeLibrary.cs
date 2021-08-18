using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Nethereum.ABI;
using Nethereum.Web3;
using System.Threading.Tasks;

namespace Backend_prototype_1
{
    class PancakeLibrary
    {
        internal static string[] sortTokens(string tokenA, string tokenB)
        {
            //make sure the address is a non-zero address?
            if (tokenA == tokenB)
                throw new Exception("identical addresses");//for now it's an error, but maybe I should just print.
            if(int.Parse(tokenA, System.Globalization.NumberStyles.HexNumber) < int.Parse(tokenB, System.Globalization.NumberStyles.HexNumber))
                return new string[] { tokenA, tokenB };
            return new string[] { tokenB, tokenA };
            
        }

        // calculates the CREATE2 address for a pair without making any external calls
        // their words, not mine.
        internal static string pairFor(string factory, string tokenA, string tokenB)  

        {
            string[] sortedTokens = sortTokens(tokenA, tokenB);
            string token0 = sortedTokens[0];
            string token1 = sortedTokens[1];
            ABIEncode abi = new ABIEncode();
            object[] tokens = { token0, token1 };
            object[] data = { "ff", factory, abi.GetSha3ABIEncoded(abi.GetABIEncodedPacked(tokens)), "d0d4c4cd0848c93cb4fd1f498d7013ee6bfb25783ea21593d5834f5d250ece66"};
            return Encoding.Default.GetString(abi.GetSha3ABIEncoded(abi.GetABIEncodedPacked(data)));
        }
        
        internal static uint getAmountOut(uint amountIn, uint reserveIn, uint reserveOut)
        {
            if (amountIn < 0)
                throw new Exception("insufficient input amount");
            if (reserveIn < 0 || reserveOut < 0)
                throw new Exception("insufficient liquidity");
            //they used custom arithmetic functions that are supposedly "safe", but I am not gay, so...
            uint amountInWithFee = amountIn * 998;
            uint numerator = amountInWithFee * reserveOut;
            uint denominator = reserveIn * 1000 + amountInWithFee;
            return numerator / denominator;

        }

        internal static async Task<uint[]> getAmountsOut(string factory, uint amountIn, string[] path)
        {
            if (path.Length < 2)
                throw new Exception("invalid path");
            uint[] amounts = new uint[path.Length];
            amounts[0] = amountIn;
            for (int i = 0; i < path.Length - 1; i++)
            {
                uint[] reserves = await getReserves(factory, path[i], path[i + 1]);
                uint reserveIn = reserves[0];
                uint reserveOut = reserves[1];
                amounts[i + 1] = getAmountOut(amounts[i], reserveIn, reserveOut);
            }
            return amounts;

        }
        
        internal static async Task<uint[]> getReserves(string factory, string tokenA, string tokenB)
        {
            string token0 = sortTokens(tokenA, tokenB)[0];
            pairFor(factory, tokenA, tokenB);//this seems retarded af
            Web3 web3 = new Web3("http://localhost:8545/");
            Nethereum.Contracts.Contract pairContract = web3.Eth.GetContract(File.ReadAllText("abi.json"), PancakeLibrary.pairFor(factory, tokenA, tokenB));
            uint[] retU = await pairContract.GetFunction("getReserves").CallAsync<uint[]>(new object[0]);//longshot, remember async proggramming
            return retU;
        }


    }
}
