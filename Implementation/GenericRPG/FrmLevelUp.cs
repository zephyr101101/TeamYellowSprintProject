using GameLibrary;
using System;
using System.Media;
using System.Windows.Forms;

namespace GenericRPG {
    public partial class FrmLevelUp : Form {
        public FrmLevelUp() {
            InitializeComponent();
        }

        /// <summary>
        /// Function that loads information into labels and plays sounds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLevelUp_Load(object sender, EventArgs e) {
            SoundPlayer sp = new SoundPlayer(@"Resources\levelup.wav");
            sp.Play();

            Character character = Game.GetGame().Character;
            character.RefillHealthAndMana();

            // Old Health, Mana, Level
            lblOldLevel.Text  = character.Level.ToString();
            lblOldHealth.Text = ((float)Math.Round(character.Health)).ToString();
            lblOldMana.Text   = ((float)Math.Round(character.Mana)).ToString();
      
            // Old Stats
            lblOldStr.Text = ((float)Math.Round(character.Strength)).ToString();
            lblOldDex.Text = ((float)Math.Round(character.Dexterity)).ToString();
            lblOldInt.Text = ((float)Math.Round(character.Intelligence)).ToString();
            lblOldWis.Text = ((float)Math.Round(character.Wisdom)).ToString();
            lblOldCon.Text = ((float)Math.Round(character.Constitution)).ToString();
            lblOldChar.Text = ((float)Math.Round(character.Charisma)).ToString();
            lblOldLuck.Text = ((float)Math.Round(character.Luck)).ToString();

            character.LevelUp();

            // New Health, Mana, Level
            lblNewLevel.Text  = character.Level.ToString();
            lblNewHealth.Text = character.Health.ToString();
            lblNewMana.Text = character.Mana.ToString();

            // New Stats
            lblNewStr.Text = ((float)Math.Round(character.Strength)).ToString();
            lblNewDex.Text = ((float)Math.Round(character.Dexterity)).ToString();
            lblNewInt.Text = ((float)Math.Round(character.Intelligence)).ToString();
            lblNewWis.Text = ((float)Math.Round(character.Wisdom)).ToString();
            lblNewCon.Text = ((float)Math.Round(character.Constitution)).ToString();
            lblNewChar.Text = ((float)Math.Round(character.Charisma)).ToString();
            lblNewLuck.Text = ((float)Math.Round(character.Luck)).ToString();
            }

        private void btnClose_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
