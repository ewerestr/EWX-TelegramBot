using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EWX_TelegramBot_Installer
{
    public partial class Form1 : Form
    {
        private int currentState = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (currentState == 3)
            {
                this.Close();
            }
            if (currentState == 1)
            {
                currentState += 1;
                updateForm();
                FakeProgress();
                return;
            }
            currentState += 1;
            backButton.Enabled = true;
            updateForm();
        }

        private void updateForm()
        {
            switch (currentState)
            {
                case 0: // Welcome
                    {
                        installingLabel.Visible = false;
                        installStateLabel.Visible = false;
                        installingProgressBar.Visible = false;
                        backButton.Enabled = false;
                        nextButton.Text = "Далее >";
                        headerLabel.Text = "Вас приветствует мастер установки\nEWX TelegramBot";
                        descriptionLabel.Text = "Мастер установит \"EWX TelegramBot\" на ваш компьютер. \nНажмите кнопку \"Далее\", чтобы продолжить или \"Отмена\", чтобы \nвыйти из программы установки.";
                        licenseUpperHeaderLabel.Visible = false;
                        licenseLowerHeaderLabel.Visible = false;
                        licenseText.Visible = false;
                        licenseFooterLabel.Visible = false;
                        programImage.Visible = true;
                        headerLabel.Visible = true;
                        descriptionLabel.Visible = true;
                        break;
                    }
                case 1: // Licensing
                    {
                        backButton.Enabled = true;
                        nextButton.Text = "Установить";
                        licenseUpperHeaderLabel.Visible = true;
                        licenseLowerHeaderLabel.Visible = true;
                        licenseFooterLabel.Visible = true;
                        licenseText.Visible = true;
                        programImage.Visible = false;
                        headerLabel.Visible = false;
                        descriptionLabel.Visible = false;
                        break;
                    }
                case 2: // Progressbar
                    {
                        installingLabel.Visible = true;
                        installStateLabel.Visible = true;
                        installingProgressBar.Visible = true;
                        backButton.Enabled = false;
                        cancelButton.Enabled = false;
                        nextButton.Text = "Подождите...";
                        nextButton.Enabled = false;
                        licenseUpperHeaderLabel.Visible = false;
                        licenseLowerHeaderLabel.Visible = false;
                        licenseText.Visible = false;
                        licenseFooterLabel.Visible = false;
                        programImage.Visible = false;
                        headerLabel.Visible = false;
                        descriptionLabel.Visible = false;
                        break;
                    }
                case 3:
                    {
                        backButton.Enabled = false;
                        cancelButton.Enabled = false;
                        nextButton.Text = "Готово";
                        headerLabel.Text = "Завершение работы мастера \nустановки EWX TelegramBot";
                        descriptionLabel.Text = "Установка EWX TelegramBot успешно завершена. \n\nНажмите кнопку \"Готово\" для выхода из программы установки.";
                        licenseUpperHeaderLabel.Visible = false;
                        licenseLowerHeaderLabel.Visible = false;
                        licenseText.Visible = false;
                        licenseFooterLabel.Visible = false;
                        programImage.Visible = true;
                        headerLabel.Visible = true;
                        descriptionLabel.Visible = true;
                        break;
                    }
                default:
                    {
                        // WTF IS GOING ON ?!?
                        break;
                    }
            }
            Update();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            currentState -= 1;
            updateForm();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://ewerestr.ru");
        }

        private void FakeProgress()
        {
            Thread.Sleep(1000);
            installStateLabel.Text = "Создание папок...";
            installingProgressBar.Value = 20;
            Update();
            Thread.Sleep(1000);
            installStateLabel.Text = "Распаковка файлов...";
            installingProgressBar.Value = 40;
            Update();
            Thread.Sleep(1000);
            installStateLabel.Text = "Копирование файлов...";
            installingProgressBar.Value = 60;
            Update();
            Thread.Sleep(1000);
            installStateLabel.Text = "Создание записей в реестре...";
            installingProgressBar.Value = 80;
            Update();
            Thread.Sleep(1000);
            installStateLabel.Text = "Создание ярлыков...";
            installingProgressBar.Value = 90;
            Update();
            Thread.Sleep(1000);
            installStateLabel.Text = "Удаление временных файлов...";
            Update();
            Thread.Sleep(1000);
            installingProgressBar.Value = 100;
            
            Update();
            Thread.Sleep(3000);
            currentState += 1;

            installingLabel.Visible = false;
            installStateLabel.Visible = false;
            installingProgressBar.Visible = false;
            backButton.Enabled = false;
            cancelButton.Enabled = false;
            nextButton.Text = "Готово";
            nextButton.Enabled = true;
            headerLabel.Text = "Завершение работы мастера \nустановки EWX TelegramBot";
            descriptionLabel.Text = "Установка EWX TelegramBot успешно завершена. \n\nНажмите кнопку \"Готово\" для выхода из программы установки.";
            licenseUpperHeaderLabel.Visible = false;
            licenseLowerHeaderLabel.Visible = false;
            licenseText.Visible = false;
            licenseFooterLabel.Visible = false;
            programImage.Visible = true;
            headerLabel.Visible = true;
            descriptionLabel.Visible = true;
            Update();
        }
    }
}
