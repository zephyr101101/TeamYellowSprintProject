using GameLibrary;
using GenericRPG.Properties;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace GenericRPG {
    public partial class FrmArena : Form {
    private Game game;
    private Character character;
    private Enemy enemy;
    private Random rand;
    
    public FrmArena() {
        InitializeComponent();
    }
    private void btnEndFight_Click(object sender, EventArgs e) {
        EndFight();
    }
    private void EndFight() {
        Game.GetGame().ChangeState(GameState.ON_MAP);
        Close();
    }
    private void FrmArena_Load(object sender, EventArgs e) {
        rand = new Random();

        game = Game.GetGame();
        character = game.Character;
        enemy = new Enemy(rand.Next(character.Level + 1), Resources.enemy);

        // stats
        UpdateStats();

        // pictures
        picCharacter.BackgroundImage = character.Pic.BackgroundImage;
        picEnemy.BackgroundImage = enemy.Img;

        // names
        lblPlayerName.Text = character.Name;
        lblEnemyName.Text = enemy.Name;
    }
    /// <summary>
    /// This function updates the stats on the FrmArena menu
    /// </summary>
    public void UpdateStats() {
        // Update the Player Level and Health
        lblPlayerLevel.Text = character.Level.ToString();
        lblPlayerHealth.Text = Math.Round(character.Health).ToString();
        
        // Update the Player Stats
        lblPlayerStr.Text = Math.Round(character.Strength).ToString();
        lblPlayerDex.Text = Math.Round(character.Dexterity).ToString();
        lblPlayerCon.Text = Math.Round(character.Constitution).ToString();
        lblPlayerInt.Text = Math.Round(character.Intelligence).ToString();
        lblPlayerWis.Text = Math.Round(character.Wisdom).ToString();
        lblPlayerChar.Text = Math.Round(character.Charisma).ToString();
        lblPlayerLuck.Text = Math.Round(character.Luck).ToString();

        // Update Player Mana and XP
        lblPlayerMana.Text = Math.Round(character.Mana).ToString();
        lblPlayerXp.Text = Math.Round(character.XP).ToString();
        
        // Update Enemy level, health, and stats
        lblEnemyLevel.Text = enemy.Level.ToString();
        lblEnemyHealth.Text = Math.Round(enemy.Health).ToString();
        lblEnemyStr.Text = Math.Round(enemy.Str).ToString();
        lblEnemyDef.Text = Math.Round(enemy.Def).ToString();
        lblEnemyMana.Text = Math.Round(enemy.Mana).ToString();

        // Duplicated code that reupdates the player and enemy health
        // This should do nothing but I didnt remove it just in case
        //lblPlayerHealth.Text = Math.Round(character.Health).ToString();
        //lblEnemyHealth.Text = Math.Round(enemy.Health).ToString();
    }
    /// <summary>
    /// This function executes on simple attack click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSimpleAttack_Click(object sender, EventArgs e) {
        float prevEnemyHealth = enemy.Health;
        character.SimpleAttack(enemy);
        float enemyDamage = (float)Math.Round(prevEnemyHealth - enemy.Health);
        lblEnemyDamage.Text = enemyDamage.ToString();
        lblEnemyDamage.Visible = true;
        tmrEnemyDamage.Enabled = true;
        if (enemy.Health <= 0) {
            character.GainXP(enemy.XpDropped);
            lblEndFightMessage.Text = "You Gained " + Math.Round(enemy.XpDropped) + " xp!";
            lblEndFightMessage.Visible = true;
            Refresh();
            Thread.Sleep(1200);
            EndFight();
            if (character.ShouldLevelUp) {
                FrmLevelUp frmLevelUp = new FrmLevelUp();
                frmLevelUp.Show();
            }
        }
        else {
            float prevPlayerHealth = character.Health;
            enemy.SimpleAttack(character);
            float playerDamage = (float)Math.Round(prevPlayerHealth - character.Health);
            lblPlayerDamage.Text = playerDamage.ToString();
            lblPlayerDamage.Visible = true;
            tmrPlayerDamage.Enabled = true;
            if (character.Health <= 0) {
                UpdateStats();
                game.ChangeState(GameState.DEAD);
                lblEndFightMessage.Text = "You Were Defeated!";
                lblEndFightMessage.Visible = true;
                Refresh();
                Thread.Sleep(1200);
                EndFight();
                FrmGameOver frmGameOver = new FrmGameOver();
                frmGameOver.Show();
            }
            else {
                UpdateStats();
            }
        }
    }
    /// <summary>
    /// This function executies on Run button click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRun_Click(object sender, EventArgs e) {
        if (rand.NextDouble() < 0.25) {
        lblEndFightMessage.Text = "You Ran Like a Coward!";
        lblEndFightMessage.Visible = true;
        Refresh();
        Thread.Sleep(1200);
        EndFight();
        }
        else {
        enemy.SimpleAttack(character);
        UpdateStats();
        }
    }

    private void tmrPlayerDamage_Tick(object sender, EventArgs e) {
        lblPlayerDamage.Top -= 2;
        if (lblPlayerDamage.Top < 10) {
            lblPlayerDamage.Visible = false;
            tmrPlayerDamage.Enabled = false;
            lblPlayerDamage.Top = 52;
        }
    }

    private void tmrEnemyDamage_Tick(object sender, EventArgs e) {
        lblEnemyDamage.Top -= 2;
        if (lblEnemyDamage.Top < 10) {
            lblEnemyDamage.Visible = false;
            tmrEnemyDamage.Enabled = false;
            lblEnemyDamage.Top = 52;
        }
    }
    }
}
