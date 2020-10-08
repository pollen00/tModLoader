using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Terraria.Avalon
{
	class WorldGenTex
	{
		public static Dictionary<Color, int> colorToLiquid;

		public static Dictionary<Color, int> colorToSlope;

		public static TexGen GetTexGenerator(Texture2D tileTex, Dictionary<Color, int> colorToTile, Texture2D wallTex = null, Dictionary<Color, int> colorToWall = null, Texture2D liquidTex = null, Texture2D slopeTex = null) {
			if (colorToLiquid == null) {
				colorToLiquid = new Dictionary<Color, int>();
				colorToLiquid[new Color(0, 0, 255)] = 0;
				colorToLiquid[new Color(255, 0, 0)] = 1;
				colorToLiquid[new Color(255, 255, 0)] = 2;
				colorToSlope = new Dictionary<Color, int>();
				colorToSlope[new Color(255, 0, 0)] = 1;
				colorToSlope[new Color(0, 255, 0)] = 2;
				colorToSlope[new Color(0, 0, 255)] = 3;
				colorToSlope[new Color(255, 255, 0)] = 4;
				colorToSlope[new Color(255, 255, 255)] = -1;
				colorToSlope[new Color(0, 0, 0)] = -2;
			}

			Color[] array = new Color[tileTex.Width * tileTex.Height];

			tileTex.GetData(0, tileTex.Bounds, array, 0, tileTex.Width * tileTex.Height);

			Color[] array2 = (wallTex != null) ? new Color[wallTex.Width * wallTex.Height] : null;

			if (array2 != null) {
				wallTex.GetData(0, wallTex.Bounds, array2, 0, wallTex.Width * wallTex.Height);
			}

			Color[] array3 = (liquidTex != null) ? new Color[liquidTex.Width * liquidTex.Height] : null;

			if (array3 != null) {
				liquidTex.GetData(0, liquidTex.Bounds, array3, 0, liquidTex.Width * liquidTex.Height);
			}

			Color[] array4 = (slopeTex != null) ? new Color[slopeTex.Width * slopeTex.Height] : null;

			if (array4 != null) {
				slopeTex.GetData(0, slopeTex.Bounds, array4, 0, slopeTex.Width * slopeTex.Height);
			}

			int num = 0;
			int num2 = 0;
			TexGen texGen = new TexGen(tileTex.Width, tileTex.Height);

			for (int i = 0; i < array.Length; i++) {
				Color key = array[i];
				Color key2 = (wallTex == null) ? Color.Black : array2[i];
				Color key3 = (liquidTex == null) ? Color.Black : array3[i];
				Color key4 = (slopeTex == null) ? Color.Black : array4[i];
				int id = colorToTile.ContainsKey(key) ? colorToTile[key] : (-1);
				int wid = (colorToWall != null && colorToWall.ContainsKey(key2)) ? colorToWall[key2] : (-1);
				int num3 = (colorToLiquid != null && colorToLiquid.ContainsKey(key3)) ? colorToLiquid[key3] : (-1);
				int sl = (colorToSlope != null && colorToSlope.ContainsKey(key4)) ? colorToSlope[key4] : (-1);

				texGen.tileGen[num, num2] = new TileInfo(id, 0, wid, num3, (num3 != -1) ? 255 : 0, sl);
				num++;

				if (num >= tileTex.Width) {
					num = 0;
					num2++;
				}

				if (num2 >= tileTex.Height) {
					break;
				}
			}

			return texGen;
		}
	}
}
