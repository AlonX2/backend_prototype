using System;
using System.IO;

namespace Backend_prototype_1
{
    class Program
    {
        static void Main(string[] args)
        {
            string pancake_factory_v2 = "0xcA143Ce32Fe78f1f7019d7d551a6402fC5350c73";//pretty sure this is the pancake factory address.
            string wrapped_bnb_address = "0xbb4CdB9CBd36B01bD1cBaEBF2De08d9173bc095c";
            string contract_pair_abi = File.ReadAllText("abi.json");
            SwapRouter router = new SwapRouter(pancake_factory_v2, wrapped_bnb_address);
        }
    }
}
