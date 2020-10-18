using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;

namespace Terraria.Avalon
{
	public class Line
	{
		public Vector2 a;
		public Vector2 b;
		public float thickness;

		public Line() { }

		public Line(Vector2 a, Vector2 b, float thickness = 1f) {
			this.a = a;
			this.b = b;
			this.thickness = thickness;
		}

		public void Draw(SpriteBatch spriteBatch, Color tint) {
			Vector2 vector = b - a;
			float num = (float)Math.Atan2(vector.Y, vector.X);
			float num2 = thickness / 8f;
			Vector2 origin = new Vector2(TextureAssets.Lightning1.Width(), TextureAssets.Lightning1.Height() / 2f);

			spriteBatch.Draw(origin: new Vector2(0f, TextureAssets.Lightning1.Height() / 2f), scale: new Vector2(vector.Length(), num2), texture: TextureAssets.Lightning1.Value, position: a, sourceRectangle: null, color: tint, rotation: num, effects: SpriteEffects.None, layerDepth: 0f);
			spriteBatch.Draw(TextureAssets.Lightning1.Value, a, null, tint, num, origin, num2, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureAssets.Lightning1.Value, b, null, tint, num + (float)Math.PI, origin, num2, SpriteEffects.None, 0f);
		}
	}
}
