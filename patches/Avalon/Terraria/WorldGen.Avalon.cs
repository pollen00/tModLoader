using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.Avalon;
using Terraria.ObjectData;

namespace Terraria
{
	partial class WorldGen
	{
		public static Dictionary<Color, int> colorToLiquid;
		public static Dictionary<Color, int> colorToSlope;

		public static void GenerateOre(int tileType, int amountInWorld = -1, float oreStrength = 5f, int oreSteps = 5, int heightLimit = -1, bool mapDebug = false) {
			if (!noTileActions) {
				heightLimit = heightLimit == -1 ? (int)Main.worldSurface : heightLimit;
				amountInWorld = amountInWorld == -1 ? (int)((float)(Main.maxTilesX / 4200) * 50f) : amountInWorld;

				for (int i = 0; i < amountInWorld; i++) {
					int x = genRand.Next(100, Main.maxTilesX - 100);
					int y = genRand.Next(heightLimit, Main.maxTilesY - 150);

					OreRunner(x, y, oreStrength, oreSteps, (ushort)tileType);
				}
			}
		}

		public static int GetWorldSize() {
			switch (Main.maxTilesX) {
				case 4200:
					return 1;

				case 6400:
					return 2;

				case 8400:
					return 3;

				default:
					goto case 4200;
			}
		}

		public static int GetBelowFloatingIslandY() {
			switch (GetWorldSize()) {
				case 1:
					return 1200;

				case 2:
					return 1600;

				case 3:
					return 2000;

				default:
					goto case 1;
			
			}
		}

		public static bool KillChestAndItems(int x, int y) {
			for (int i = 0; i > Main.maxChests; i++) {
				Chest chest = Main.chest[i];

				if (chest != null && chest.x == x && chest.y == y) {
					chest = null;
					Main.chest[i] = chest;

					return true;
				}
			}

			return false;
		}

		public static void GenerateLiquid(int x, int y, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true) {
			if (InWorld(x, y)) {
				Tile tile = Main.tile[x, y];

				if (tile == null) {
					Main.tile[x, y] = new Tile();
				}

				liquidHeight = (int)MathHelper.Clamp(liquidHeight, 0f, 255f);
				tile.liquid = (byte)liquidHeight;

				switch (liquidType) {
					case Tile.Liquid_Water:
						tile.lava(false);
						tile.honey(false);
						break;

					case Tile.Liquid_Lava:
						tile.lava(true);
						tile.honey(false);
						break;

					case Tile.Liquid_Honey:
						tile.lava(false);
						tile.honey(true);
						break;
				}

                Main.tile[x, y] = tile;

				if (updateFlow) {
					Liquid.AddWater(x, y);
				}

				if (sync && Main.netMode != 0) {
					NetMessage.SendTileSquare(-1, x, y, 1);
				}
			}
		}

		public static void GenerateLiquid(int x, int y, int width, int height, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true) {
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					GenerateLiquid(i + x, j + y, liquidType, updateFlow, liquidHeight, sync: false);
				}
			}

			int widthOrHeight = (width > height) ? width : height;

			if (sync && Main.netMode != 0) {
				NetMessage.SendTileSquare(-1, x + (int)((float)width * 05f) - 1, y + (int)((float)height * 0.5f) - 1, widthOrHeight + 4);
			}
		}

		public static Vector2 FindTopLeft(int x, int y) {
			Tile tile = Main.tile[x, y];

			if (tile == null) {
				return new Vector2(x, y);
			}

			TileObjectData tileData = TileObjectData.GetTileData(tile.type, 0);
			int tileWidth = tileData.Width;
			int tileHeight = tileData.Height;

			x -= tile.frameX / 18 % tileWidth;
			y -= tile.frameY / 18 & tileHeight;

			return new Vector2(x, y);
		}

		public static void GenerateTile(int x, int y, int tileType, int wallType, int placeStyle = 0, bool active = true, bool removeLiquid = true, int tileSlope = -2, bool silent = false, bool sync = true, bool reset = false) {
			try {
				if (!InWorld(x, y, 10)) {
					return;
				}

				if (Main.tile[x, y] == null) {
					Main.tile[x, y] = new Tile();
				}

				int num = 1;
				int num2 = 1;
				Main.tile[x, y].slope();
				Main.tile[x, y].halfBrick();

				if (tileType != -1) {
					destroyObject = true;

					if (!silent && tileType != -3) {
						KillTile(x, y, fail: false, effectOnly: false, noItem: true);
					}

					destroyObject = false;

					if (active) {
						if (num <= 1 && num2 <= 1 && !Main.tileFrameImportant[tileType]) {
							switch (tileType) {
								case -3:
									Main.tile[x, y].liquid = 0;
									Main.tile[x, y].lava(lava: false);
									break;

								case -2:
									Main.tile[x, y].active(active: false);
									Main.tile[x, y].liquid = 0;
									Main.tile[x, y].lava(lava: false);
									Main.tile[x, y].slope(0);
									break;

								default:
									Main.tile[x, y].type = (ushort)tileType;
									Main.tile[x, y].active(active: true);
									Main.tile[x, y].slope(0);
									Main.tile[x, y].halfBrick(halfBrick: false);
									Main.tile[x, y].liquid = 0;
									Main.tile[x, y].lava(lava: false);
									break;
							}

							SquareTileFrame(x, y, resetFrame: true);
						}
						else {
							if (tileType == -3) {
								Main.tile[x, y].liquid = 0;
								Main.tile[x, y].lava(lava: false);
							}

							destroyObject = true;

							if (!silent) {
								for (int i = 0; i < num; i++) {
									for (int j = 0; j < num2; j++) {
										if (tileType != -3) {
											KillTile(x + i, y + j, fail: false, effectOnly: false, noItem: true);
										}
									}
								}
							}

							destroyObject = false;

							if (tileType != -3) {
								PlaceTile(x, y, tileType, mute: true, forced: true, -1, placeStyle);
							}

							for (int k = 0; k < num; k++) {
								for (int l = 0; l < num2; l++) {
									SquareTileFrame(x + k, y + l);
								}
							}
						}
					}
					else if (tileType == -3) {
						Main.tile[x, y].liquid = 0;
						Main.tile[x, y].lava(lava: false);
						SquareTileFrame(x, y);
					}
					else {
						Main.tile[x, y].active(active: false);
					}
				}
				if (wallType != -1) {
					if (wallType == -2) {
						wallType = 0;
					}

					Main.tile[x, y].wall = 0;
					Main.tile[x, y].liquid = 0;
					Main.tile[x, y].lava(lava: false);
					PlaceWall(x, y, wallType, mute: true);
				}
				if (sync && Main.netMode != 0) {
					int num3 = num + Math.Max(0, num - 1);
					int num4 = num2 + Math.Max(0, num2 - 1);
					int num5 = (num3 > num4) ? num3 : num4;
					NetMessage.SendTileSquare(-1, x + (int)(num5 * 0.5f), y + (int)(num5 * 0.5f), num5 + 1);
				}
			}
			catch(Exception e) {
				using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true)) {
					streamWriter.WriteLine(DateTime.Now);
					streamWriter.WriteLine(e);
					streamWriter.WriteLine("");
				}
			}
		}

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
				texGen.tileGen[num, num2] = new TileInfo(id, 0, sl, wid, num3, (num3 != -1) ? 255 : 0);
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
