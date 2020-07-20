using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleMod.Content.Items
{
	public class ExampleInstancedItem : ModItem
	{
		public override string Texture => "ExampleMod/Content/Items/ExampleItem";

		public override bool CloneNewInstances => false;

		public Color color;

		public ExampleInstancedItem() {
			color = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.5f);
		}

		public override void SetStaticDefaults() {
			Tooltip.SetDefault("This is a modded item."); //The (English) text shown below your weapon's name
		}

		public override void SetDefaults() {
			item.width = 20; //The item texture's width
			item.height = 20; //The item texture's height

			item.maxStack = 1; //The item's max stack value
			item.value = Item.buyPrice(silver: 1); //The value of the item in copper coins.
			item.rare = ItemRarityID.Blue; //The rarity of the weapon.
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			TooltipLine line = tooltips.FirstOrDefault(x => x.Name == "ItemName");
			if (line != null) {
				line.overrideColor = color;
			}
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<ExampleItem>(10)
				.Register();
		}
	}
}