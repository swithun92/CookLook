using System;

namespace CookLook
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var input = Console.ReadLine();
                var recipeLogic = RecipeLogic.Instance;
                Console.WriteLine(recipeLogic.RecipeResponder(input));
            }
        }
    }
}
