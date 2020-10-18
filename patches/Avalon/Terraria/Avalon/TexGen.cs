namespace Terraria.Avalon
{
	public class TexGen
	{
		public int width;
		public int height;
		public TileInfo[,] tileGen;
		public int torchStyle;
		public int platformStyle;

		public TexGen(int width, int height) {
			this.width = width;
			this.height = height;
			tileGen = new TileInfo[width, height];
		}

		public void Generate(int x, int y, bool silent, bool sync, int tStyle = 0, int pStyle = 0, int l1Style = 0, int l2Style = 0, int cStyle = 0, int bStyle = 0, int dStyle = 0) {
			if (Main.tile[x, y] == null) {
				Main.tile[x, y] = new Tile();
			}

			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					int num = x + i;
					int num2 = y + j;

					if (Main.tile[num, num2] == null) {
						Main.tile[num, num2] = new Tile();
					}

					TileInfo tileInfo = tileGen[i, j];
					int tileStyle = (tileInfo.tileID == 4) ? tStyle : ((tileInfo.tileID == 19) ? pStyle : ((tileInfo.tileID == 93) ? l1Style : ((tileInfo.tileID == 92) ? l2Style : ((tileInfo.tileID == 21) ? cStyle : ((tileInfo.tileID == 101) ? bStyle : ((tileInfo.tileID == 10) ? dStyle : 0))))));

					if (tileInfo.tileID != -1 || tileInfo.wallID != -1 || tileInfo.liquidType != -1 || tileInfo.wire != -1) {
						if (tileInfo.tileID != -1 || tileInfo.wallID > -1 || tileInfo.wire > -1) {
							WorldGen.GenerateTile(num, num2, tileInfo.tileID, tileInfo.wallID, tileStyle, tileInfo.tileID > -1, tileInfo.liquidAmount == 0, tileInfo.tileSlope, silent: false, sync);
						}
						if (tileInfo.liquidType > -1) {
							WorldGen.GenerateLiquid(num, num2, tileInfo.liquidType, updateFlow: false, tileInfo.liquidAmount, sync);
						}
					}
				}
			}
		}
	}
}
