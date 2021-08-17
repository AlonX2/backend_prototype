using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Web3;
using System.Threading.Tasks;

namespace Backend_prototype_1
{
    class SwapRouter
    {
        public string factory;
        public string WBNB;//wbnb and not WETH??
        public string abi;//for web3 call 
        public SwapRouter(string _factory, string _WBNB, string _abi)
        {
            this.factory = _factory;
            this.WBNB = _WBNB;
            this.abi = _abi;
        }
        public async Task<uint[]> swapExactETHForTokens(uint amountOutMin, string[] path, string to, uint deadline, uint value )//high level function, call to create order, maybe return bool?
        {
            //the value param is insted of the msg object in the blockchain.

            //make initial transaction.
            if (path[0] != WBNB)
                throw new Exception("invalid path");
            uint[] amounts = await PancakeLibrary.getAmountsOut(factory, value, path);
            //do amounts check later here
            //not sure that the first deposit is needed... seems as if it deposits to router.
            //insert lotsa random transactions here.
            _swap(amounts, path, to);
            return amounts; // why tf do you need to do that
        }
        internal void _swap(uint[] amounts, string[] path, string _to)
        {
            for(int i=0; i < path.Length -1; i++)
            {
                string input = path[i];
                string output = path[i + 1];
                string token0 = PancakeLibrary.sortTokens(input, output)[0];//the first token of the pair
                uint amountOut = amounts[i + 1];
                uint amount0Out = 1;//should init these inside the "WTF if block". for now I put random values in these.
                uint amount1Out = 1;
                if(input == token0)//??
                {
                    //wtf should i put here

                }
                string to = i < path.Length - 2 ? PancakeLibrary.pairFor(factory, output, path[i + 2]) : _to; // copy-pasted with some changes.
                Web3 web3 = new Web3("http://localhost:8545/");
                Nethereum.Contracts.Contract pairContract = web3.Eth.GetContract(abi,PancakeLibrary.pairFor(factory, input, output));
                pairContract.GetFunction("swap").SendTransactionAsync(_to, new object[] { amount0Out, amount1Out, to, new Byte() }); // sign???
                //should put the actual swap function using web3.
            }
        }
    }
}
