
namespace EWX_TelegramBot_Installer
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.headerLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.devLinkLabel = new System.Windows.Forms.LinkLabel();
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.licenseUpperHeaderLabel = new System.Windows.Forms.Label();
            this.licenseLowerHeaderLabel = new System.Windows.Forms.Label();
            this.licenseText = new System.Windows.Forms.RichTextBox();
            this.licenseFooterLabel = new System.Windows.Forms.Label();
            this.programImage = new System.Windows.Forms.PictureBox();
            this.installingLabel = new System.Windows.Forms.Label();
            this.installingProgressBar = new System.Windows.Forms.ProgressBar();
            this.installStateLabel = new System.Windows.Forms.Label();
            this.studioLinkLabel = new System.Windows.Forms.LinkLabel();
            this.madeByLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programImage)).BeginInit();
            this.SuspendLayout();
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.headerLabel.Location = new System.Drawing.Point(180, 42);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(346, 44);
            this.headerLabel.TabIndex = 1;
            this.headerLabel.Text = "Вас приветствует мастер установки\r\nEWX TelegramBot";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.AutoSize = true;
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.descriptionLabel.Location = new System.Drawing.Point(180, 114);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(350, 39);
            this.descriptionLabel.TabIndex = 1;
            this.descriptionLabel.Text = "Мастер установит \"EWX TelegramBot\" на Ваш компьютер. \r\nНажмите кнопку \"Далее\", чт" +
    "обы продолжить или \"Отмена\", чтобы \r\nвыйти из мастера.";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.panel1.Controls.Add(this.madeByLabel);
            this.panel1.Controls.Add(this.studioLinkLabel);
            this.panel1.Controls.Add(this.devLinkLabel);
            this.panel1.Controls.Add(this.backButton);
            this.panel1.Controls.Add(this.nextButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Location = new System.Drawing.Point(0, 289);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(538, 56);
            this.panel1.TabIndex = 2;
            // 
            // devLinkLabel
            // 
            this.devLinkLabel.AutoSize = true;
            this.devLinkLabel.Location = new System.Drawing.Point(61, 30);
            this.devLinkLabel.Name = "devLinkLabel";
            this.devLinkLabel.Size = new System.Drawing.Size(47, 13);
            this.devLinkLabel.TabIndex = 5;
            this.devLinkLabel.TabStop = true;
            this.devLinkLabel.Text = "ewerestr";
            this.devLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // backButton
            // 
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(269, 17);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 4;
            this.backButton.Text = "< Назад";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(350, 17);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 4;
            this.nextButton.Text = "Далее >";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(451, 17);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Отмена";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // licenseUpperHeaderLabel
            // 
            this.licenseUpperHeaderLabel.AutoSize = true;
            this.licenseUpperHeaderLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.licenseUpperHeaderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.licenseUpperHeaderLabel.Location = new System.Drawing.Point(12, 9);
            this.licenseUpperHeaderLabel.Name = "licenseUpperHeaderLabel";
            this.licenseUpperHeaderLabel.Size = new System.Drawing.Size(168, 13);
            this.licenseUpperHeaderLabel.TabIndex = 3;
            this.licenseUpperHeaderLabel.Text = "Лицензионное соглашение";
            this.licenseUpperHeaderLabel.Visible = false;
            // 
            // licenseLowerHeaderLabel
            // 
            this.licenseLowerHeaderLabel.AutoSize = true;
            this.licenseLowerHeaderLabel.Location = new System.Drawing.Point(24, 26);
            this.licenseLowerHeaderLabel.Name = "licenseLowerHeaderLabel";
            this.licenseLowerHeaderLabel.Size = new System.Drawing.Size(430, 13);
            this.licenseLowerHeaderLabel.TabIndex = 4;
            this.licenseLowerHeaderLabel.Text = "Перед установкой EWX TelegramBot ознакомьтесь с лицензионным соглашением.";
            this.licenseLowerHeaderLabel.Visible = false;
            // 
            // licenseText
            // 
            this.licenseText.Enabled = false;
            this.licenseText.Location = new System.Drawing.Point(27, 52);
            this.licenseText.Name = "licenseText";
            this.licenseText.Size = new System.Drawing.Size(485, 179);
            this.licenseText.TabIndex = 5;
            this.licenseText.Text = "Соглашение тада тада и так далее";
            this.licenseText.Visible = false;
            // 
            // licenseFooterLabel
            // 
            this.licenseFooterLabel.AutoSize = true;
            this.licenseFooterLabel.Location = new System.Drawing.Point(27, 247);
            this.licenseFooterLabel.Name = "licenseFooterLabel";
            this.licenseFooterLabel.Size = new System.Drawing.Size(486, 26);
            this.licenseFooterLabel.TabIndex = 6;
            this.licenseFooterLabel.Text = "Если Вы принимаете условия соглашения, нажмите кнопку \"Установить\". Чтобы установ" +
    "ить \r\nпрограмму, необходимо принять соглашение.";
            this.licenseFooterLabel.Visible = false;
            // 
            // programImage
            // 
            this.programImage.BackColor = System.Drawing.Color.White;
            this.programImage.Image = global::EWX_TelegramBot_Installer.Properties.Resources.installer_leftside;
            this.programImage.Location = new System.Drawing.Point(0, 0);
            this.programImage.Name = "programImage";
            this.programImage.Size = new System.Drawing.Size(165, 289);
            this.programImage.TabIndex = 0;
            this.programImage.TabStop = false;
            // 
            // installingLabel
            // 
            this.installingLabel.AutoSize = true;
            this.installingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.installingLabel.Location = new System.Drawing.Point(12, 9);
            this.installingLabel.Name = "installingLabel";
            this.installingLabel.Size = new System.Drawing.Size(442, 44);
            this.installingLabel.TabIndex = 1;
            this.installingLabel.Text = "Подождите, пока EWX TelegramBot завершит \r\nустановку.";
            this.installingLabel.Visible = false;
            // 
            // installingProgressBar
            // 
            this.installingProgressBar.Location = new System.Drawing.Point(12, 114);
            this.installingProgressBar.Name = "installingProgressBar";
            this.installingProgressBar.Size = new System.Drawing.Size(514, 26);
            this.installingProgressBar.TabIndex = 7;
            this.installingProgressBar.Visible = false;
            // 
            // installStateLabel
            // 
            this.installStateLabel.AutoSize = true;
            this.installStateLabel.Location = new System.Drawing.Point(12, 95);
            this.installStateLabel.Name = "installStateLabel";
            this.installStateLabel.Size = new System.Drawing.Size(76, 13);
            this.installStateLabel.TabIndex = 8;
            this.installStateLabel.Text = "Подготовка...";
            this.installStateLabel.Visible = false;
            // 
            // studioLinkLabel
            // 
            this.studioLinkLabel.AutoSize = true;
            this.studioLinkLabel.Location = new System.Drawing.Point(13, 10);
            this.studioLinkLabel.Name = "studioLinkLabel";
            this.studioLinkLabel.Size = new System.Drawing.Size(127, 13);
            this.studioLinkLabel.TabIndex = 5;
            this.studioLinkLabel.TabStop = true;
            this.studioLinkLabel.Text = "EWX Simple Code Studio";
            this.studioLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // madeByLabel
            // 
            this.madeByLabel.AutoSize = true;
            this.madeByLabel.Location = new System.Drawing.Point(16, 31);
            this.madeByLabel.Name = "madeByLabel";
            this.madeByLabel.Size = new System.Drawing.Size(48, 13);
            this.madeByLabel.TabIndex = 6;
            this.madeByLabel.Text = "Made by";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 345);
            this.Controls.Add(this.installStateLabel);
            this.Controls.Add(this.installingProgressBar);
            this.Controls.Add(this.licenseFooterLabel);
            this.Controls.Add(this.licenseText);
            this.Controls.Add(this.licenseLowerHeaderLabel);
            this.Controls.Add(this.licenseUpperHeaderLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.installingLabel);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.programImage);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Установка EWX TelegramBot";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.programImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox programImage;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label licenseUpperHeaderLabel;
        private System.Windows.Forms.Label licenseLowerHeaderLabel;
        private System.Windows.Forms.RichTextBox licenseText;
        private System.Windows.Forms.Label licenseFooterLabel;
        private System.Windows.Forms.LinkLabel devLinkLabel;
        private System.Windows.Forms.Label installingLabel;
        private System.Windows.Forms.ProgressBar installingProgressBar;
        private System.Windows.Forms.Label installStateLabel;
        private System.Windows.Forms.LinkLabel studioLinkLabel;
        private System.Windows.Forms.Label madeByLabel;
    }
}

