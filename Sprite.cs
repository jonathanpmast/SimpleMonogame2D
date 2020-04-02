using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monogame1
{
    public class Sprite
    {
        public Sprite(GraphicsDevice graphicsDevice, Texture2D texture, float scale)
        {
            this.scale = scale;
            this.texture = texture;
        }

        public void Update(float elapsedTime)
        {
            this.x += this.dX * elapsedTime;
            this.y += this.dY * elapsedTime;
            this.angle += this.dA * elapsedTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(this.x, this.y);
            spriteBatch.Draw(texture, spritePosition, null, Color.White, this.angle, new Vector2(texture.Width / 2, texture.Height / 2), new Vector2(scale, scale), SpriteEffects.None, 0f);
        }

        public Texture2D texture { get; }
        public float x { get; set; }
        public float y { get; set; }
        public float angle { get; set; }
        public float dX { get; set; }
        public float dY { get; set; }

        public float dA { get; set; }

        public float scale { get; set; }
    }
}