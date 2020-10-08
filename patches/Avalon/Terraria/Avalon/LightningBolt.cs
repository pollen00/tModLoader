using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.Avalon
{
	public class LightningBolt
	{
		public class Line
		{
			public Vector2 A;
			public Vector2 B;
			public float Thickness;

			public Line() { }

			public Line(Vector2 a, Vector2 b, float thickness = 1f) {
				A = a;
				B = b;
				Thickness = thickness;
			}

			public void Draw(SpriteBatch spriteBatch, Color tint) {
				// TODO: Asset loading
				/*Vector2 vector = B - A;
				float num = (float)Math.Atan2(vector.Y, vector.X);
				float num2 = Thickness / 8f;
				Vector2 origin = new Vector2(Main.lightning1Tex.Width, (float)Main.lightning1Tex.Height / 2f);
				spriteBatch.Draw(origin: new Vector2(0f, (float)Main.lightning2Tex.Height / 2f), scale: new Vector2(vector.Length(), num2), texture: Main.lightning2Tex, position: A, sourceRectangle: null, color: tint, rotation: num, effects: SpriteEffects.None, layerDepth: 0f);
				spriteBatch.Draw(Main.lightning1Tex, A, null, tint, num, origin, num2, SpriteEffects.None, 0f);
				spriteBatch.Draw(Main.lightning1Tex, B, null, tint, num + (float)Math.PI, origin, num2, SpriteEffects.None, 0f);*/
			}
		}

		public List<Line> Segments = new List<Line>();

		private static Random rand = new Random();

		public Vector2 Start => Segments[0].A;

		public Vector2 End => Segments.Last().B;

		public bool IsComplete => Alpha <= 0f;

		public float Alpha {
			get;
			set;
		}

		public float AlphaMultiplier {
			get;
			set;
		}

		public float FadeOutRate {
			get;
			set;
		}

		public Color Tint {
			get;
			set;
		}

		public LightningBolt(Vector2 source, Vector2 dest)
			: this(source, dest, new Color(0.9f, 0.8f, 1f)) {
		}

		public LightningBolt(Vector2 source, Vector2 dest, Color color) {
			Segments = CreateBolt(source, dest, 2f);
			Tint = color;
			Alpha = 1f;
			AlphaMultiplier = 0.6f;
			FadeOutRate = 0.03f;
		}

		public void Draw(SpriteBatch spriteBatch) {
			if (Alpha <= 0f) {
				return;
			}
			foreach (Line segment in Segments) {
				segment.Draw(spriteBatch, Tint);
			}
		}

		public virtual void Update() {
			Alpha -= FadeOutRate;
		}

		protected static List<Line> CreateBolt(Vector2 source, Vector2 dest, float thickness) {
			List<Line> list = new List<Line>();
			Vector2 value = dest - source;
			Vector2 value2 = Vector2.Normalize(new Vector2(value.Y, 0f - value.X));
			float num = value.Length();
			List<float> list2 = new List<float>();
			list2.Add(0f);
			for (int i = 0; (float)i < num / 4f; i++) {
				list2.Add(Rand(0f, 1f));
			}
			list2.Sort();
			Vector2 a = source;
			float num2 = 0f;
			for (int j = 1; j < list2.Count; j++) {
				float num3 = list2[j];
				float num4 = num * 0.0125f * (num3 - list2[j - 1]);
				float num5 = (num3 > 0.95f) ? (20f * (1f - num3)) : 1f;
				float num6 = Rand(-80f, 80f);
				num6 -= (num6 - num2) * (1f - num4);
				num6 *= num5;
				Vector2 vector = source + num3 * value + num6 * value2;
				list.Add(new Line(a, vector, thickness));
				a = vector;
				num2 = num6;
			}
			list.Add(new Line(a, dest, thickness));
			return list;
		}

		public Vector2 GetPoint(float position) {
			Vector2 start = Start;
			float num = Vector2.Distance(start, End);
			Vector2 dir = (End - start) / num;
			position *= num;
			Line line = Segments.Find((Line x) => Vector2.Dot(x.B - start, dir) >= position);
			float num2 = Vector2.Dot(line.A - start, dir);
			float num3 = Vector2.Dot(line.B - start, dir);
			float amount = (position - num2) / (num3 - num2);
			return Vector2.Lerp(line.A, line.B, amount);
		}

		private static float Rand(float min, float max) => (float)rand.NextDouble() * (max - min) + min;

		private static float Square(float x) => x * x;
	}
}
