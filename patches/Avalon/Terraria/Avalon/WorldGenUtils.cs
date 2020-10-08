using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria.ID;
using Terraria.ObjectData;

namespace Terraria.Avalon
{
	public class WorldGenUtils
	{
		public static void GenOre(int tileType, int amountInWorld = -1, float oreStength = 5f, int oreSteps = 5, int heightLimit = -1, bool mapDebuf = false) {
			if (!WorldGen.noTileActions) {
				if (heightLimit == -1) {
					heightLimit = (int)Main.worldSurface;
				}

				if (amountInWorld == -1) {
					amountInWorld = (int)((float)(Main.maxTilesX / 4200) * 50f);
				}

				for (int i = 0; i < amountInWorld; i++) {
					int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
					int y = WorldGen.genRand.Next(heightLimit, Main.maxTilesY - 150);

					WorldGen.OreRunner(x, y, oreStength, oreSteps, (ushort)tileType);
				}
			}
		}

		public static int GetWorldSize() {
			switch (Main.maxTilesX) {

				case 4200:
				default:
					return 1;

				case 6400:
					return 2;

				case 8400:
					return 3;
			}
		}

		public static int GetBelowFloatingIslandY() {
			switch (GetWorldSize()) {
				default:
				case 1:
					return 1200 + 1;

				case 2:
					return 1600 + 1;

				case 3:
					return 2000 + 1;
			}
		}

		public static bool KillChestAndItems(int x, int y) {
			for (int i = 0; i < 1000; i++) {
				Chest chest = Main.chest[i];
				if (chest != null && chest.x == x && chest.y == y) {
					chest = null;

					return true;
				}
			}

			return false;
		}

		public static void GenerateLiquid(int x, int y, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true) {
			if (WorldGen.InWorld(x, y)) {
				Tile tile = Main.tile[x, y];

				if (tile == null) {
					tile = new Tile();
				}

				liquidHeight = (int)MathHelper.Clamp(liquidHeight, 0f, 255f);
				tile.liquid = (byte)liquidHeight;

				switch (liquidType) {
					case 0:
						tile.lava(false);
						tile.honey(false);
						break;

					case 1:
						tile.lava(true);
						tile.honey(false);
						break;

					case 2:
						tile.lava(false);
						tile.honey(true);
						break;
				}

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
				NetMessage.SendTileSquare(-1, x + (int)((float)width * 0.5f) - 1, y + (int)((float)height * 0.5) - 1, widthOrHeight + 4);
			}
		}

		public static Vector2 FindTopLeft(int x, int y) {
			Tile tile = Main.tile[x, y];

			if (tile == null) {
				return new Vector2(x, y);
			}

			int num = 1;
			int num2 = 1;

			TileObjectData tileData = TileObjectData.GetTileData(tile.type, 0); // All FrameImportant tiles will have the same multi-tile measures

			x -= tile.frameX / 18 % tileData.Width;
			y -= tile.frameY / 18 % tileData.Height;

			return new Vector2(x, y);
		}

		public static void GenerateTile(int x, int y, int tile, int wall, int placeStyle = 0, bool active = true, bool removeLiquid = true, int slope = -2, bool silent = false, bool sync = true, bool reset = false) {
			try {
				if (!WorldGen.InWorld(x, y, 10)) {
					return;
				}

				if (Main.tile[x, y] == null) {
					Main.tile[x, y] = new Tile();
				}

				int num = 1;
				int num2 = 1;

				Main.tile[x, y].slope();
				Main.tile[x, y].halfBrick();

				if (tile != -1) {
					WorldGen.destroyObject = true;

					if (!silent && tile != -3) {
						WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: true);
					}

					WorldGen.destroyObject = false;

					if (active) {
						if (num <= 1 && num2 <= 1 && !Main.tileFrameImportant[tile]) {
							switch (tile) {
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
									Main.tile[x, y].type = (ushort)tile;
									Main.tile[x, y].active(active: true);
									Main.tile[x, y].slope(0);
									Main.tile[x, y].halfBrick(halfBrick: false);
									Main.tile[x, y].liquid = 0;
									Main.tile[x, y].lava(lava: false);
									break;
							}

							WorldGen.SquareTileFrame(x, y, resetFrame: true);
						}
						else {
							if (tile == -3) {
								Main.tile[x, y].liquid = 0;
								Main.tile[x, y].lava(lava: false);
							}

							WorldGen.destroyObject = true;

							if (!silent) {
								for (int i = 0; i < num; i++) {
									for (int j = 0; j < num2; j++) {
										if (tile != -3) {
											WorldGen.KillTile(x + i, y + j, fail: false, effectOnly: false, noItem: true);
										}
									}
								}
							}

							WorldGen.destroyObject = false;

							if (tile != -3) {
								WorldGen.PlaceTile(x, y, tile, mute: true, forced: true, -1, placeStyle);
							}

							for (int k = 0; k < num; k++) {
								for (int l = 0; l < num2; l++) {
									WorldGen.SquareTileFrame(x + k, y + l);
								}
							}
						}
					}
					else if (tile == -3) {
						Main.tile[x, y].liquid = 0;
						Main.tile[x, y].lava(lava: false);
						WorldGen.SquareTileFrame(x, y);
					}
					else {
						Main.tile[x, y].active(active: false);
					}
				}

				if (wall != -1) {

					if (wall == -2) {
						wall = 0;
					}

					Main.tile[x, y].wall = 0;
					Main.tile[x, y].liquid = 0;

					Main.tile[x, y].lava(lava: false);
					WorldGen.PlaceWall(x, y, wall, mute: true);
				}
				if (sync && Main.netMode != 0) {
					int num3 = num + Math.Max(0, num - 1);
					int num4 = num2 + Math.Max(0, num2 - 1);
					int num5 = (num3 > num4) ? num3 : num4;

					NetMessage.SendTileSquare(-1, x + (int)((float)num5 * 0.5f), y + (int)((float)num5 * 0.5f), num5 + 1);
				}
			}
			catch (Exception value) {
				using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", append: true)) {
					streamWriter.WriteLine(DateTime.Now);
					streamWriter.WriteLine(value);
					streamWriter.WriteLine("");
				}
			}
		}
	}
}
