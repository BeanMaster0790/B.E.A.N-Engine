using Bean.Debug;
using Microsoft.Xna.Framework;

namespace Bean
{
    public static class PropIds
    {
        private static string[] _food = new string[]
        {
            "Apple", "Banana", "Mango", "Pear", "Peach", "Plum", "Grape", "Kiwi",
            "Cherry", "Melon", "Lemon", "Lime", "Orange", "Carrot", "Potato", "Tomato",
            "Onion", "Pea", "Lettuce", "Spinach", "Radish", "Cabbage", "Chicken", "Beef",
            "Pork", "Bacon", "Sausage", "Ham", "Turkey", "Lamb", "Duck", "Salmon",
            "Tuna", "Shrimp", "Crab", "Tofu", "Bread", "Pasta", "Rice", "Oats",
            "Bagel", "Muffin", "Biscuit", "Cracker", "Cereal", "Milk", "Cheese", "Butter",
            "Yogurt", "Cream", "Custard", "Mozzarella", "Cheddar", "Cake", "Brownie", "Cookie",
            "Donut", "Pudding", "Waffle", "Pancake", "Candy", "Toffee", "Fudge", "Jelly",
            "Chocolate", "Popcorn", "Pretzel", "Pickle", "Hotdog", "Pizza", "Burger", "Wrap",
            "Sandwich", "Falafel", "Chip", "Fry", "Basil", "Mint", "Sage", "Thyme",
            "Salt", "Pepper", "Ketchup", "Mustard", "Mayo", "Soy", "Garlic", "Tea",
            "Coffee", "Juice", "Water", "Soda", "Cola", "Lemonade", "Smoothie", "Milkshake",
            "Wine", "Beer", "Cider", "Soup", "Gravy", "Broth", "Almond", "Peanut",
            "Walnut", "Pistachio", "Pecan", "Jam", "Gummy", "Jellybean", "Taffy", "Caramel",
            "Syrup", "Curry", "Skewer", "Granola", "Quiche", "Okra", "Yam", "Zucchini",
            "Eggplant", "Chickpea", "Lentil", "Bean", "Corn", "Mushroom", "Chive", "Dill"
        };

        private static string[] _colour = new string[]
        {
            "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Brown",
            "Black", "White", "Grey", "Cyan", "Magenta", "Teal", "Maroon", "Beige",
            "Gold", "Silver", "Navy", "Lime"
        };

        private static string[] _modifier = new string[]
        {
            "Ripe", "Rotten", "Burnt", "Hot", "Cold", "Dry", "Wet", "Sweet", "Sour",
            "Bland", "Fresh", "Old", "Soft", "Hard", "Spicy", "Zesty", "Sticky", "Juicy",
            "Smoky", "Flaky", "Toasted", "Mushy", "Raw", "Crispy", "Chewy", "Salty", "Greasy",
            "Plain", "Crumbly", "Fluffy", "Icy"
        };

        private static int _currentFood = -1;
        private static int _currentColour = 0;
        private static int _currentModifier = 0;

        private static int _repeats = 0;

        public static string GeneratePropId()
        {

            _currentFood++;

            if (_currentFood >= _food.Length)
            {
                _currentFood = 0;

                _currentColour++;
            }

            if (_currentColour >= _colour.Length)
            {
                _currentColour = 0;

                _currentModifier++;
            }

            if (_currentModifier >= _modifier.Length)
            {
                if (_repeats == 0)
                {
                    DebugServer.Log("⚠ WARNING: Prop IDs are repeating. Numbers will now be added to the end of IDs", Color.Orange);
                }

                _repeats++;

                _currentModifier = 0;
            }

            string id = $"{_modifier[_currentModifier]}{_colour[_currentColour]}{_food[_currentFood]}";

            if (_repeats != 0)
            {
                id += _repeats;
            }

            return id;
        }

        public static void Reset()
        {
            _currentFood = 0;
            _currentColour = 0;
            _currentModifier = 0;
            _repeats = 0;
        }
        
    }
    

}