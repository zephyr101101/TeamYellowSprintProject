using System.Windows.Forms;

namespace GameLibrary {
    public struct Position {
        public int row;
        public int col;

        /// <summary>
        /// Construct a new 2D position
        /// </summary>
        /// <param name="row">Given row or y value</param>
        /// <param name="col">Given col or x value</param>
        public Position(int row, int col) {
            this.row = row;
            this.col = col;
        }
    }

    /// <summary>
    /// This represents our player in our game
    /// </summary>
    public class Character : Mortal {
        public PictureBox Pic { get; set; }
        private Position pos;
        public float XP { get; private set; }
        public bool ShouldLevelUp { get; private set; }

        public Character(PictureBox pb, Position pos) : base("Player 1", 1) {
            Pic = pb;
            this.pos = pos;
            ShouldLevelUp = false;
        }
        /// <summary>
        /// Adds XP to the player. If character is higher level than enemy then the player gets less xp.
        /// Reverse is true for if the player is a lower level than the enemy
        /// </summary>
        /// <param name="amount">Amount of XP rewarded</param>
        /// <param name="Elevel">Level of Enemy</param>
        /// <param name="Clevel">Level of Character</param>
        public int GainXP(float amount, int Elevel, int Clevel) {
            // The *10 is a DEBUG multiplier to test the level functions
            XP += amount * ((float)Elevel/(float)Clevel) * 10;

            // every 100 experience points you gain a level
            if ((int)XP / 100 >= Level) {
                ShouldLevelUp = true;
            }
            return (int)XP;
        }
        /// <summary>
        /// Calls the LevelUp function defined in Mortal.cs and sets the should level up flag to false
        /// </summary>
        public override void LevelUp() {
            base.LevelUp();
            ShouldLevelUp = false;
        }

   
        public void BackToStart() {
            pos.row = Map.CurrentMap.CharacterStartRow;
            pos.col = Map.CurrentMap.CharacterStartCol;
            Position topleft = Map.RowColToTopLeft(pos);
            Pic.Left = topleft.col;
            Pic.Top = topleft.row;
        }
    
        /// <summary>
        /// Calls reset stats function in Mortal.cs
        /// Set XP to 0 to ensure no level ups occur
        /// </summary>
        public override void ResetStats() {
            base.ResetStats();
            XP = 0;
        }

        /// <summary>
        /// Moves the player according to input MoveDir
        /// </summary>
        /// <param name="dir">Direction to Move</param>
        public void Move(MoveDir dir) {
            Position newPos = pos;
            switch (dir) {
                case MoveDir.UP:
                    newPos.row--;
                    break;
                case MoveDir.DOWN:
                    newPos.row++;
                    break;
                case MoveDir.LEFT:
                    newPos.col--;
                    break;
                case MoveDir.RIGHT:
                    newPos.col++;
                    break;
            }
            Position? newNewPos = Map.CurrentMap.Enter(newPos);
            if (newNewPos != null) {
                pos = (Position) newNewPos;
                Position topleft = Map.RowColToTopLeft(pos);
                Pic.Left = topleft.col;
                Pic.Top = topleft.row;
                Pic.BackgroundImage = Map.CurrentMap.GetBackgroundImage(pos);
            }
        }
        
    }
}
