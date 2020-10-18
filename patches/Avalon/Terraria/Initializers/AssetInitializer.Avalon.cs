using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace Terraria.Initializers
{
	partial class AssetInitializer
	{
        private static void LoadAvalonTextures(AssetRequestMode mode) {
            TextureAssets.Lightning1 = LoadAsset<Texture2D>("Images\\Lightning1", mode);
            TextureAssets.Lightning2 = LoadAsset<Texture2D>("Images\\Lightning2", mode);
		}
	}
}
