using Microsoft.Xna.Framework;
using System;

namespace RPGTurnBased.Utils
{
    public static class ColorPalette
    {
        // ============= BACKGROUND COLORS =============
        public static readonly Color BackgroundDark = new Color(20, 24, 28);
        public static readonly Color BackgroundMedium = new Color(40, 44, 48);
        public static readonly Color BackgroundLight = new Color(60, 64, 68);

        // ============= PLAYER COLORS =============
        public static readonly Color PlayerColor = new Color(64, 128, 255);        // Blue
        public static readonly Color PlayerColorDark = new Color(32, 64, 128);     // Dark blue
        public static readonly Color PlayerColorMedium = new Color(48, 96, 192);   // Medium blue

        // ============= ENEMY COLORS =============
        public static readonly Color EnemyDefault = new Color(180, 60, 60);        // Red
        public static readonly Color EnemyGoblin = new Color(120, 160, 80);        // Green
        public static readonly Color EnemyOrc = new Color(100, 80, 60);            // Brown
        public static readonly Color EnemySkeleton = new Color(220, 220, 200);     // Bone white
        public static readonly Color EnemyDragon = new Color(160, 40, 200);        // Purple
        public static readonly Color EnemySlime = new Color(80, 200, 120);         // Lime green
        public static readonly Color EnemyWolf = new Color(100, 100, 100);         // Gray

        // ============= UI COLORS =============
        public static readonly Color UIBackground = new Color(0, 0, 0, 200);       // Semi-transparent black
        public static readonly Color UIBorder = new Color(255, 255, 255, 180);     // Semi-transparent white
        public static readonly Color UIText = Color.White;
        public static readonly Color UITextHighlight = new Color(255, 255, 100);   // Yellow
        public static readonly Color UITextDisabled = new Color(120, 120, 120);    // Gray

        // ============= DIALOGUE COLORS =============
        public static readonly Color DialogueBackground = new Color(20, 20, 30, 220);
        public static readonly Color DialogueBorder = new Color(180, 180, 200);
        public static readonly Color DialogueText = Color.White;
        public static readonly Color DialogueOptionSelected = new Color(255, 255, 100);
        public static readonly Color DialogueOptionNormal = new Color(200, 200, 200);

        // ============= HEALTH BAR COLORS =============
        public static readonly Color HealthHigh = new Color(80, 200, 80);          // Green
        public static readonly Color HealthMedium = new Color(200, 200, 80);       // Yellow
        public static readonly Color HealthLow = new Color(200, 80, 80);           // Red
        public static readonly Color HealthBackground = new Color(40, 40, 40);     // Dark gray
        public static readonly Color HealthBorder = Color.White;

        // ============= COMBAT COLORS =============
        public static readonly Color CombatBackground = new Color(40, 20, 20);     // Dark red tint
        public static readonly Color AttackColor = new Color(255, 100, 100);       // Light red
        public static readonly Color DefenseColor = new Color(100, 150, 255);      // Light blue
        public static readonly Color ItemColor = new Color(100, 255, 100);         // Light green
        public static readonly Color EscapeColor = new Color(255, 255, 100);       // Yellow

        // ============= STATUS EFFECT COLORS =============
        public static readonly Color StatusPoison = new Color(120, 255, 120);      // Bright green
        public static readonly Color StatusBurn = new Color(255, 120, 60);         // Orange
        public static readonly Color StatusFreeze = new Color(120, 200, 255);      // Light blue
        public static readonly Color StatusStun = new Color(255, 255, 120);        // Bright yellow

        // ============= ENVIRONMENT COLORS =============
        public static readonly Color EnvironmentForest = new Color(40, 80, 40);    // Dark green
        public static readonly Color EnvironmentCave = new Color(60, 50, 40);      // Brown
        public static readonly Color EnvironmentDungeon = new Color(40, 40, 60);   // Dark blue-gray
        public static readonly Color EnvironmentDesert = new Color(120, 100, 60);  // Sandy

        // ============= UTILITY METHODS =============

        /// <summary>
        /// Get health bar color based on health percentage
        /// </summary>
        public static Color GetHealthColor(float healthPercentage)
        {
            if (healthPercentage > 0.6f)
                return HealthHigh;
            else if (healthPercentage > 0.3f)
                return HealthMedium;
            else
                return HealthLow;
        }

        /// <summary>
        /// Lerp between two colors
        /// </summary>
        public static Color LerpColor(Color color1, Color color2, float amount)
        {
            return Color.Lerp(color1, color2, amount);
        }

        /// <summary>
        /// Create a color with modified alpha
        /// </summary>
        public static Color WithAlpha(Color color, float alpha)
        {
            return color * alpha;
        }

        /// <summary>
        /// Create a darker version of a color
        /// </summary>
        public static Color Darken(Color color, float factor = 0.7f)
        {
            return new Color(
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor),
                color.A
            );
        }

        /// <summary>
        /// Create a lighter version of a color
        /// </summary>
        public static Color Lighten(Color color, float factor = 1.3f)
        {
            return new Color(
                Math.Min(255, (int)(color.R * factor)),
                Math.Min(255, (int)(color.G * factor)),
                Math.Min(255, (int)(color.B * factor)),
                color.A
            );
        }

        /// <summary>
        /// Get a pulsing color effect for animations
        /// </summary>
        public static Color GetPulsingColor(Color baseColor, float time, float speed = 2f, float intensity = 0.3f)
        {
            var pulse = (float)(Math.Sin(time * speed) * intensity + 1f);
            return baseColor * pulse;
        }

        /// <summary>
        /// Get random color variation for enemies of the same type
        /// </summary>
        public static Color GetColorVariation(Color baseColor, System.Random random, float variation = 0.2f)
        {
            var r = Math.Max(0, Math.Min(255, baseColor.R + random.Next((int)(-255 * variation), (int)(255 * variation))));
            var g = Math.Max(0, Math.Min(255, baseColor.G + random.Next((int)(-255 * variation), (int)(255 * variation))));
            var b = Math.Max(0, Math.Min(255, baseColor.B + random.Next((int)(-255 * variation), (int)(255 * variation))));

            return new Color(r, g, b, baseColor.A);
        }
    }
}