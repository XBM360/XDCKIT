﻿using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XDevkitTester.XDevkit.Dialogs
{
    public partial class ValueParser : DevExpress.XtraEditors.XtraForm
    {
        public ValueParser()
        {
            InitializeComponent();
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            new ValueTester(ParseBox.Text).Show();
        }
    }
}