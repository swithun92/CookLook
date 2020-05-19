using CookLook.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CookLook
{
    class RecipeLogic
    {
        private int RecipeChoice { get; set; }
        private ICollection<string> Health { get; set; }
        private ICollection<string> Removable { get; set; }
        public RecipeList RecipeList { get; set; }

        private static readonly RecipeLogic instance = new RecipeLogic();
        static RecipeLogic()
        {
        }
        private RecipeLogic()
        {
            this.Health = new string[] { " low-carb ", " vegan ", " vegetarian ", " alcohol-free ", " high protein ", "low-fat", "peanut-free", " balanced " };
            this.Removable = new string[] { " with ", " and ", " or ", "find ", " me ", " a ", " recipe ", " for " };
        }
        public static RecipeLogic Instance
        {
            get
            {
                return instance;
            }
        }
        public string RecipeResponder(string input)
        {
            if (input.Contains("find") && input.Contains("recipe"))
            {
                var RecipesOutput = this.SearchRecipes(input);
                if (RecipesOutput!= null && RecipesOutput.Count > 0)
                {
                    this.RecipeList = RecipesOutput;
                    var output = RecipesOutput.Count + " recipes found." + " The first 5 are: ";
                    int i = 0;
                    foreach (Hit recipeHit in RecipesOutput.Hits)
                    {
                        i++;
                        output += i + ". " + recipeHit.Recipe.Label + " from " + recipeHit.Recipe.Source + ". ";
                    }
                    output.Trim(',');
                    return output;
                }
                else { return "No recipes for that search criteria were found"; }
            }
            if (input.Contains("choose") && this.RecipeList != null)
            {
                int digit = input.Where(i => char.IsDigit(i)).FirstOrDefault()-48;
                if (digit < 6 && digit > 0)
                {
                    this.RecipeChoice = digit;
                    return "Recipe " + digit + " selected: " + this.GetRecipeTitle(digit);
                }
            }
            if (input.Contains("ingredients"))
            {
                int digit = input.Where(i => char.IsDigit(i)).FirstOrDefault() - 48;
                if (digit > 0 && digit < 6)
                {
                    return "Ingredients for recipe " + digit + ": " + this.GetRecipeTitle(digit) + " are " + this.GetRecipeIngredients(digit);
                }
                else if (this.RecipeChoice > 0)
                {
                    return "Ingredients for " + this.GetRecipeTitle(this.RecipeChoice) + " are: " + this.GetRecipeIngredients(this.RecipeChoice);
                }
            }
            if (input.Contains("load"))
            {
                Process process = new Process();
                process.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                process.StartInfo.Arguments = this.RecipeList.Hits[this.RecipeChoice-1].Recipe.Url + " --new-window";
                process.Start();
                return "Loading the recipe in a new chrome window.";
            }
            return null;

        }

        private string GetRecipeIngredients(int digit)
        {
            var output = "";
            var ingredients = this.RecipeList.Hits[digit - 1].Recipe.IngredientLines;
            for (int i = 0; i<ingredients.Count;i++)
            {
                output += i + 1 + ". " + ingredients[i] + ". ";
            }
            return output;
        }

        private string GetRecipeTitle(int digit)
        {
            var recipe = this.RecipeList.Hits[digit - 1].Recipe;
            return recipe.Label + " from " + recipe.Source;
        }

        private RecipeList SearchRecipes(string input)
        {
            var DownloadLink = "";
            foreach (string health in this.Health)
            {
                if (input.Contains(health))
                {
                    DownloadLink += "&health=" + health.Trim();
                    input = input.Replace(health, " ");
                }
            }
            foreach (string rmword in this.Removable)
            {
                input = input.Replace(rmword, " ");
            }
            input = input.Replace("  ", " ").Trim().Replace(" ", "+");
            
            DownloadLink += "&q=" + input;
            using (var client = new WebClient())
            {

                var recipeJson = client.DownloadString("https://api.edamam.com/search?app_id=084c75ba&app_key=dc0c53baf15766be071aa494ecd8fae6&to=5" + DownloadLink);
                RecipeList recipeList = JsonSerializer.Deserialize<RecipeList>(recipeJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return recipeList;
            }
            
        }
    }
}
