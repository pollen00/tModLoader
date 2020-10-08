namespace Terraria.Avalon
{
	public class TileInfo
	{
		public int tileID = -1;
		public int tileStyle;
		public int wallID = -1;
		public int liquidType = -1;
		public int liquidAmount;
		public int slope = -1;
		public int wire = -1;

		public TileInfo(int id, int style, int wid = -1, int lType = -1, int lAmt = 0, int sl = -2, int w = -1) {
			tileID = id;
			tileStyle = style;
			wallID = wid;
			liquidType = lType;
			liquidAmount = lAmt;
			slope = sl;
			wire = w;
		}
	}
}
