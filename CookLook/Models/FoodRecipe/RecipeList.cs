using System;
using System.Collections.Generic;
using System.Text;

namespace CookLook.Models
{
    class RecipeList
    {
        public int ChosenRecipe { get; set; }
        public string Q { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public bool More { get; set; }
        public int Count { get; set; }
        public List<Hit> Hits { get; set; }

    }
}
