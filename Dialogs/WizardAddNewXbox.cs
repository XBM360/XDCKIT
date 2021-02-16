﻿using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XDevkit.Dialogs
{
    public partial class WizardForm : DevExpress.XtraEditors.XtraForm
    {
        public string IPAddress
        {
            get; private set;
        }

        public WizardForm()
        {
            InitializeComponent();
            MainControl.SelectedTabPage = WizardWelcome;
            DisableControl(WBackButton);
            DisableControl(DefualtBackButton);
        }

        #region Handler's
        /// <summary>
        /// Any Cancel Button Will Cause Form TO Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }

        public void ShowNext(Control Tab)
        {
            Tab.Show();
        }
        public void DisableControl(BaseStyleControl Tab)
        {
            Tab.Appearance.BackColor = Color.Silver;
            Tab.ForeColor = Color.Black;//BaseControl
        }
        public void EnableControl(BaseStyleControl Tab)
        {
            Tab.Appearance.BackColor = Color.FromArgb(225, 225, 225);
            Tab.ForeColor = Color.Black;
        }
        private void ControlChecked(object sender, EventArgs e)
        {
            if (YesButton.Checked)
            {
                YesOrNoType.Text = "Yes";
            }
            if (NoButton.Checked)
            {
                YesOrNoType.Text = "No";
            }
        }

        /// <summary>
        /// Checks If Sender Has The Button Clicked Then Executes Commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clicked(object sender, EventArgs e)
        {
            if (sender.Equals(WNextButton))
            {
                ShowNext(WizardNameOrIp);
            }
            else if (sender.Equals(DefualtNext))//defualt selection
            {
                ShowNext(WizardFinish);
            }
            //Name Or IP Page
            else if (sender.Equals(NOPBack))
            {
                ShowNext(WizardWelcome);
            }
            else if (sender.Equals(NOPNext))
            {
                if(XboxClient.Connect(NameOrIP.Text,730))
                {
                    IPAddress = NameOrIP.Text;
                    ShowNext(WizardDefualtConsole);
                }
                else
                {
                    XtraMessageBox.Show("'" + NameOrIP.Text + "'" + " " + "Could Not Be Found", "Xbox 360 Neighborhood", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        #endregion

        private void NameOrIP_TextChanged(object sender, EventArgs e)
        {
            if (NameOrIP.Text.ToCharArray().Any(char.IsDigit))
            {
                EnableControl(NOPNext);
                NOPNext.Enabled = NameOrIP.Text.ToCharArray().Any(char.IsDigit);

            }
            else if (NameOrIP.Text.ToCharArray().Any(char.IsLetter))
            {
                EnableControl(NOPNext);
                NOPNext.Enabled = NameOrIP.Text.ToCharArray().Any(char.IsLetter);

            }
            else if (NameOrIP.Text.ToCharArray().Length == 0)
            {
                DisableControl(NOPNext);
                NOPNext.Enabled = false;

            }
        }

        private void MainControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if(WizardDefualtConsole.PageVisible)
            {
                IPAddressLabel.Text = IPAddress + " (" + IPAddress + ")";
                addresslabel.Text = "Would you like to use '" + IPAddress + "' as the Xbox 360 Development Kit?";
            }
        }
    }
}