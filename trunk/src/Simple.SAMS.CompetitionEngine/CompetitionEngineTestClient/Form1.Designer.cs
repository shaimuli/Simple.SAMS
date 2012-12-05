namespace CompetitionEngineTestClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.quali = new System.Windows.Forms.Button();
            this.players = new System.Windows.Forms.ListBox();
            this.positions = new System.Windows.Forms.ListBox();
            this.PlayersCount = new System.Windows.Forms.TextBox();
            this.QCount = new System.Windows.Forms.TextBox();
            this.GPlayersCount = new System.Windows.Forms.TextBox();
            this.FinalGPlayersCount = new System.Windows.Forms.TextBox();
            this.FinalRPlayersCount = new System.Windows.Forms.TextBox();
            this.FinalPlayersCount = new System.Windows.Forms.TextBox();
            this.finalPositions = new System.Windows.Forms.ListBox();
            this.finalPlayers = new System.Windows.Forms.ListBox();
            this.FINALS = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // quali
            // 
            this.quali.Location = new System.Drawing.Point(28, 182);
            this.quali.Name = "quali";
            this.quali.Size = new System.Drawing.Size(333, 67);
            this.quali.TabIndex = 4;
            this.quali.Text = "Qualifying";
            this.quali.UseVisualStyleBackColor = true;
            this.quali.Click += new System.EventHandler(this.quali_Click);
            // 
            // players
            // 
            this.players.FormattingEnabled = true;
            this.players.Location = new System.Drawing.Point(12, 12);
            this.players.Name = "players";
            this.players.Size = new System.Drawing.Size(110, 134);
            this.players.TabIndex = 5;
            // 
            // positions
            // 
            this.positions.FormattingEnabled = true;
            this.positions.Location = new System.Drawing.Point(128, 11);
            this.positions.Name = "positions";
            this.positions.Size = new System.Drawing.Size(110, 134);
            this.positions.TabIndex = 6;
            // 
            // PlayersCount
            // 
            this.PlayersCount.Location = new System.Drawing.Point(255, 12);
            this.PlayersCount.Name = "PlayersCount";
            this.PlayersCount.Size = new System.Drawing.Size(100, 20);
            this.PlayersCount.TabIndex = 7;
            this.PlayersCount.Text = "32";
            // 
            // QCount
            // 
            this.QCount.Location = new System.Drawing.Point(255, 38);
            this.QCount.Name = "QCount";
            this.QCount.Size = new System.Drawing.Size(100, 20);
            this.QCount.TabIndex = 8;
            this.QCount.Text = "8";
            // 
            // GPlayersCount
            // 
            this.GPlayersCount.Location = new System.Drawing.Point(255, 64);
            this.GPlayersCount.Name = "GPlayersCount";
            this.GPlayersCount.Size = new System.Drawing.Size(100, 20);
            this.GPlayersCount.TabIndex = 9;
            this.GPlayersCount.Text = "32";
            // 
            // FinalGPlayersCount
            // 
            this.FinalGPlayersCount.Location = new System.Drawing.Point(604, 64);
            this.FinalGPlayersCount.Name = "FinalGPlayersCount";
            this.FinalGPlayersCount.Size = new System.Drawing.Size(100, 20);
            this.FinalGPlayersCount.TabIndex = 15;
            this.FinalGPlayersCount.Text = "32";
            // 
            // FinalRPlayersCount
            // 
            this.FinalRPlayersCount.Location = new System.Drawing.Point(604, 38);
            this.FinalRPlayersCount.Name = "FinalRPlayersCount";
            this.FinalRPlayersCount.Size = new System.Drawing.Size(100, 20);
            this.FinalRPlayersCount.TabIndex = 14;
            this.FinalRPlayersCount.Text = "8";
            // 
            // FinalPlayersCount
            // 
            this.FinalPlayersCount.Location = new System.Drawing.Point(604, 12);
            this.FinalPlayersCount.Name = "FinalPlayersCount";
            this.FinalPlayersCount.Size = new System.Drawing.Size(100, 20);
            this.FinalPlayersCount.TabIndex = 13;
            this.FinalPlayersCount.Text = "32";
            // 
            // finalPositions
            // 
            this.finalPositions.FormattingEnabled = true;
            this.finalPositions.Location = new System.Drawing.Point(477, 11);
            this.finalPositions.Name = "finalPositions";
            this.finalPositions.Size = new System.Drawing.Size(110, 134);
            this.finalPositions.TabIndex = 12;
            // 
            // finalPlayers
            // 
            this.finalPlayers.FormattingEnabled = true;
            this.finalPlayers.Location = new System.Drawing.Point(361, 12);
            this.finalPlayers.Name = "finalPlayers";
            this.finalPlayers.Size = new System.Drawing.Size(110, 134);
            this.finalPlayers.TabIndex = 11;
            // 
            // FINALS
            // 
            this.FINALS.Location = new System.Drawing.Point(377, 182);
            this.FINALS.Name = "FINALS";
            this.FINALS.Size = new System.Drawing.Size(333, 67);
            this.FINALS.TabIndex = 10;
            this.FINALS.Text = "Finals";
            this.FINALS.UseVisualStyleBackColor = true;
            this.FINALS.Click += new System.EventHandler(this.FINALS_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 272);
            this.Controls.Add(this.FinalGPlayersCount);
            this.Controls.Add(this.FinalRPlayersCount);
            this.Controls.Add(this.FinalPlayersCount);
            this.Controls.Add(this.finalPositions);
            this.Controls.Add(this.finalPlayers);
            this.Controls.Add(this.FINALS);
            this.Controls.Add(this.GPlayersCount);
            this.Controls.Add(this.QCount);
            this.Controls.Add(this.PlayersCount);
            this.Controls.Add(this.positions);
            this.Controls.Add(this.players);
            this.Controls.Add(this.quali);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button quali;
        private System.Windows.Forms.ListBox players;
        private System.Windows.Forms.ListBox positions;
        private System.Windows.Forms.TextBox PlayersCount;
        private System.Windows.Forms.TextBox QCount;
        private System.Windows.Forms.TextBox GPlayersCount;
        private System.Windows.Forms.TextBox FinalGPlayersCount;
        private System.Windows.Forms.TextBox FinalRPlayersCount;
        private System.Windows.Forms.TextBox FinalPlayersCount;
        private System.Windows.Forms.ListBox finalPositions;
        private System.Windows.Forms.ListBox finalPlayers;
        private System.Windows.Forms.Button FINALS;
    }
}

