using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WindowsFormsApp1.Classes;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetLanguagesCollections();
        }

        private IDictionary<string, Lang[]> _languages;

        private void GetLanguagesCollections()
        {
            //These are hard-coded here, but could be pulled from any settings source, Dependency Injected, and set as global constants for the entire App, if more than 1 Win Form is used.
            _languages = new Dictionary<string, Lang[]>
            {
                {
                    "en-US",
                    new[]
            {
                new Lang(0, "English", "en-US"),
                new Lang(1, "French", "fr-FR"),
            }
                },
                {
                    "fr-FR",
                    new[]
            {
                new Lang(0, "Anglais", "en-US"),
                new Lang(1, "Français", "fr-FR"),
            }
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetFormUiLanguage(_languages.First().Key);
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.SelectedValue.ToString()))
            {
                SetFormUiLanguage(comboBox1.SelectedValue.ToString());
            }
            else
            {
                SetFormUiLanguage("en-US");
            }
        }


        private void SetFormUiLanguage(string lang)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            this.Controls.Clear();
            this.InitializeComponent();
            SetComboBoxItemsLanguage(lang);
        }

        private void SetComboBoxItemsLanguage(string lang)
        {
            comboBox1.DataSource = _languages[lang];
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "Code";
            var index = Array.Find(_languages[lang], element => element.Code == lang).Index;
            comboBox1.SelectedIndex = index;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
