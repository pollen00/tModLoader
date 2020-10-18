namespace Terraria.Avalon
{
	public struct TileInfo
	{
		public int tileID;
		public int tileStyle;
		public int tileSlope;
		public int wallID;
		public int liquidType;
		public int liquidAmount;
		public int wire;

		public TileInfo(int tileID, int tileStyle, int tileSlope, int wallID, int liquidType, int liquidAmount, int wire) {
			this.tileID = tileID;
			this.tileStyle = tileStyle;
			this.tileSlope = tileSlope;
			this.wallID = wallID;
			this.liquidType = liquidType;
			this.liquidAmount = liquidAmount;
			this.wire = wire;
		}
	}
}
