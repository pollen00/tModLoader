using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Terraria.Avalon
{
	public class LightningBolt
	{
		public List<Line> segments = new List<Line>();
		public float alpha { get; set; }
		public float alphaMultiplier { get; set; }
		public float fadeOutRate { get; set; }
		public Color tint { get; set; }
		public Vector2 start => segments.First().a;
		public Vector2 end => segments.Last().b;
		public bool isComplete => alpha <= 0f;

		public LightningBolt(Vector2 source, Vector2 dest, Color color) {
			segments = CreateBolt(source, dest, 2f);
			tint = color;
			alpha = 1f;
			alphaMultiplier = 0.6f;
			fadeOutRate = 0.03f;
		}

		public LightningBolt(Vector2 source, Vector2 dest) : this(source, dest, new Color(0.9f, 0.8f, 1f)) { }

		public void Draw(SpriteBatch spriteBatch) {
			if (alpha > 0f) {
				foreach (Line segment in segments) {
					segment.Draw(spriteBatch, tint);
				}
			}
		}

		public virtual void Update() => alpha -= fadeOutRate;

		protected static List<Line> CreateBolt(Vector2 source, Vector2 dest, float thickness) {
			List<Line> list = new List<Line>();
			Vector2 value = dest - source;
			Vector2 value2 = Vector2.Normalize(new Vector2(value.Y, 0f - value.X));
			float num = value.Length();
			List<float> list2 = new List<float> {
				0f
			};

			for (int i = 0; i < num / 4f; i++) {
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
			float num = Vector2.Distance(start, end);
			Vector2 dir = (end - start) / num;
			position *= num;
			Line line = segments.Find((Line x) => Vector2.Dot(x.b - start, dir) >= position);
			float num2 = Vector2.Dot(line.a - start, dir);
			float amount = (position - num2) / (Vector2.Dot(line.b - start, dir) - num2);

			return Vector2.Lerp(line.a, line.b, amount);
		}

		private static float Rand(float min, float max) => (float)Main.rand.NextDouble() * (max - min) + min;

		private static float Square(float x) => x * x;
	}
}
