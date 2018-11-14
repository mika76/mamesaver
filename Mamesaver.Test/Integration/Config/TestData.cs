using System.Collections.Generic;
using Mamesaver.Models;

namespace Mamesaver.Test.Integration.Config
{
    public static class TestData
    {
        public static class Manufacturers
        {
            public const string Atari = "Atari";
            public const string Sega = "Sega";
            public const string Cave = "Cave";
        }

        public static class Category
        {
            public const string Shooter = "Shooter";
            public const string Fighter = "Fighter";
        }

        public static List<SelectableGame> GameList()
        {
            return new List<SelectableGame>
            {
               new SelectableGame
                {
                    Manufacturer = Manufacturers.Atari,
                    Category = Category.Fighter,
                    Selected = true
                },
                new SelectableGame
                {
                    Manufacturer = Manufacturers.Atari,
                    Category = Category.Shooter,
                    Selected = true
                },
                new SelectableGame
                {
                    Manufacturer = Manufacturers.Atari,
                    Category = Category.Shooter,
                    Selected = true
                },
                new SelectableGame
                {
                    Manufacturer = Manufacturers.Sega,
                    Category = Category.Shooter,
                    Selected = true
                },
                new SelectableGame
                {
                    Manufacturer = Manufacturers.Cave,
                    Category = Category.Fighter,
                    Selected = true
                }
            };
        }

    }
}
