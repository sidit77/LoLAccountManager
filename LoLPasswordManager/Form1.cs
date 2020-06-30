﻿using System;
using System.Windows.Forms;
using System.Collections.Generic;
using LoLPasswordManager.Properties;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Windows.Media.TextFormatting;

namespace LoLPasswordManager
{

    

    public partial class mainForm : Form
    {
        private byte[] entropy = BitConverter.GetBytes(8463123);
        private bool editmode = false;
        private List<Account> accounts;

        public mainForm()
        {
            accounts = new List<Account>();
            LoadAccounts();
            
            InitializeComponent();
            SetAccountButtons();
        }

        private void LoadAccounts() {
            accounts.Clear();
            if (!File.Exists(Settings.Default.FilePath))
                return;

            using (MemoryStream ms = new MemoryStream(ProtectedData.Unprotect(File.ReadAllBytes(Settings.Default.FilePath), entropy, DataProtectionScope.CurrentUser)))
            using (BinaryReader br = new BinaryReader(ms, Encoding.UTF8, true))
            {
                if (br.ReadInt32() != 123)
                    throw new Exception("Cannot read data");
                while (br.PeekChar() != -1)
                {
                    if (br.ReadChar() != '{')
                        throw new Exception("Cannot read data");
                    accounts.Add(new Account()
                    {
                        Name = br.ReadString(),
                        Username = br.ReadString(),
                        Password = br.ReadString(),
                        AdditionalInformation = br.ReadString()
                    });
                    if (br.ReadChar() != '}')
                        throw new Exception("Cannot read data");
                }
                
            }
        }

        private void SaveAccounts()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    bw.Write(123);
                    foreach (Account acc in accounts)
                    {
                        bw.Write('{');
                        bw.Write(acc.Name);
                        bw.Write(acc.Username);
                        bw.Write(acc.Password);
                        bw.Write(acc.AdditionalInformation);
                        bw.Write('}');
                    }
                }
                File.WriteAllBytes(Settings.Default.FilePath, ProtectedData.Protect(ms.ToArray(), entropy, DataProtectionScope.CurrentUser));
            }
        }

        private void SetAccountButtons()
        {
            editButton.Text = editmode ? "Exit Edit Mode" : "Enter Edit Mode";

            accountPanel.RowStyles.Clear();
            accountPanel.RowCount = 0;
            accountPanel.Controls.Clear();
            accountPanel.SuspendLayout();

            if (editmode)
            {
                Button b = new Button();
                b.Name = "addbutton";
                b.BackgroundImage = Resources.plus;
                b.BackgroundImageLayout = ImageLayout.Zoom;
                b.Size = new System.Drawing.Size(100, 40);
                b.Dock = DockStyle.Fill;
                b.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                b.Click += addButton_Click;
                accountPanel.RowCount += 1;
                accountPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                accountPanel.Controls.Add(b);

                foreach (var s in accounts)
                {
                    GroupBox p = new GroupBox
                    {
                        Size = new System.Drawing.Size(200, 50),
                        Dock = DockStyle.Fill,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink
                    };

                    Label name = new Label
                    {
                        Text = s.Name,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        Location = new System.Drawing.Point(12, 12),
                        Size = new Size(30, 31)
                    };
                    p.Controls.Add(name);

                    Button delete = new Button
                    {
                        Tag = s,
                        BackgroundImage = Resources.muell,
                        BackgroundImageLayout = ImageLayout.Zoom,
                        Size = new System.Drawing.Size(35, 35),
                        Location = new System.Drawing.Point(160, 10),
                        Anchor = AnchorStyles.Right
                    };
                    delete.Click += deleteButton_Click;
                    p.Controls.Add(delete);

                    Button edit = new Button
                    {
                        Tag = s,
                        BackgroundImage = Resources.bearbeiten,
                        BackgroundImageLayout = ImageLayout.Zoom,
                        Size = new System.Drawing.Size(35, 35),
                        Location = new System.Drawing.Point(122, 10),
                        Anchor = AnchorStyles.Right
                    };
                    edit.Click += editButton_Click;
                    p.Controls.Add(edit);

                    Button down = new Button
                    {
                        Tag = s,
                        BackgroundImage = Resources.down,
                        BackgroundImageLayout = ImageLayout.Zoom,
                        Size = new System.Drawing.Size(35, 35),
                        Location = new System.Drawing.Point(84, 10),
                        Anchor = AnchorStyles.Right,
                        Enabled = s != accounts.Last()
                    };
                    down.Click += downButton_Click;
                    p.Controls.Add(down);

                    Button up = new Button
                    {
                        Tag = s,
                        BackgroundImage = Resources.up,
                        BackgroundImageLayout = ImageLayout.Zoom,
                        Size = new System.Drawing.Size(35, 35),
                        Location = new System.Drawing.Point(46, 10),
                        Anchor = AnchorStyles.Right,
                        Enabled = s != accounts.First()
                    };
                    up.Click += upButton_Click;
                    p.Controls.Add(up);

                    accountPanel.RowCount += 1;
                    accountPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    accountPanel.Controls.Add(p);

                }

            } else
            {
                foreach (var s in accounts)
                {
                    Button b = new Button();
                    b.Text = s.Name;
                    b.Tag = s;
                    b.Size = new System.Drawing.Size(100, 40);
                    b.Dock = DockStyle.Fill;
                    b.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    b.Click += accountButton_Click;
                    accountPanel.RowCount += 1;
                    accountPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    accountPanel.Controls.Add(b);

                }
            }

            
            accountPanel.RowCount += 1;
            accountPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
            accountPanel.ResumeLayout();
            groupBox1.Width += 20;
            groupBox1.Width -= 20;

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            Account acc = new Account();
            if (ShowEditDialog(acc))
            {
                accounts.Add(acc);
                SaveAccounts();
                SetAccountButtons();
            }
            
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            int i = accounts.IndexOf((sender as Button).Tag as Account);
            Account temp = accounts[i];
            accounts[i] = accounts[i - 1];
            accounts[i - 1] = temp;
            SaveAccounts();
            SetAccountButtons();
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            int i = accounts.IndexOf((sender as Button).Tag as Account);
            Account temp = accounts[i];
            accounts[i] = accounts[i + 1];
            accounts[i + 1] = temp;
            SaveAccounts();
            SetAccountButtons();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            Account acc = (sender as Button).Tag as Account;
            if (MessageBox.Show(
                "Are you sure to delete this account?",
                "Confirm Delete!",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                accounts.Remove(acc);
                SaveAccounts();
                SetAccountButtons();
            }
            
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            Account acc = (sender as Button).Tag as Account;
            if (ShowEditDialog(acc))
            {
                SaveAccounts();
                SetAccountButtons();
            }
            
        }

        private void editModeButton_Click(object sender, EventArgs e)
        {
            editmode = !editmode;
            SetAccountButtons();
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            Form prompt = new Form()
            {
                Width = 0,
                Height = 0,
                AutoSize = true,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = "Settings"
            };
            CheckBox qal = new CheckBox()
            {
                Text = "Quit after Login? ",
                Checked = Settings.Default.CloseAfterLogin,
                Left = 12,
                Top = 12
            };
            LinkLabel credits = new LinkLabel()
            {
                Text = "Icons made by Smashicons, Freepik and Prosymbols",
                Links = { { 14, 10, "https://smashicons.com/" }, { 26, 7, "https://www.flaticon.com/de/autoren/freepik" }, { 38, 10, "https://www.flaticon.com/de/autoren/prosymbols" } },
                Left = 12,
                Top = 52,
                Width = 300
            };
            credits.LinkClicked += (send, args) =>
            {
                System.Diagnostics.Process.Start(args.Link.LinkData as string);
            };
            prompt.Controls.Add(qal);
            prompt.Controls.Add(credits) ;
            prompt.ShowDialog();
            Settings.Default.CloseAfterLogin = qal.Checked;
            Settings.Default.Save();
        }

        private void accountButton_Click(object sender, EventArgs e)
        {
            Account acc = (sender as Button).Tag as Account;
            switch (LoginHelper.LogIn(acc))
            {
                case LoginHelper.LoginResult.Error:
                    MessageBox.Show("An Error occured while filling in the data.");
                    break;
                case LoginHelper.LoginResult.NoClient:
                    MessageBox.Show("Can't find the Riot Client!\nMake sure that the Client is running!");
                    break;
                case LoginHelper.LoginResult.Success:
                    if (Settings.Default.CloseAfterLogin)
                        Application.Exit();
                    break;
            }

            //var client = Process.GetProcesses().Where(p => p.MainWindowTitle.Equals("Riot Client")).ToArray();
            //if(client.Length == 0)
            //{
            //    MessageBox.Show("Can't find the Riot Client!\nMake sure that the Client is running!");
            //    return;
            //}
            //
            //try
            //{
            //    var app = FlaUI.Core.Application.Attach(client[0]);
            //    using (var automation = new UIA3Automation())
            //    {
            //        var window = app.GetMainWindow(automation);
            //        window.Focus();
            //        var fields = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit));
            //        fields[0].Click();
            //        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            //        Keyboard.Type(acc.Username);
            //        fields[1].Click();
            //        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            //        Keyboard.Type(acc.Password);
            //        var buttons = window.FindAllDescendants(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button));
            //        buttons[1].Click();
            //
            //    }
            //    if (Settings.Default.CloseAfterLogin)
            //        Application.Exit();
            //} catch (Exception ex)
            //{
            //    MessageBox.Show("An Error occured while filling in the data.\nMake sure the Client isn't minimized!");
            //}
        }

        private bool ShowEditDialog(Account acc)
        {
            bool saved = false;
            Form prompt = new Form()
            {
                Width = 345,
                Height = 370,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = "Edit Account"
            };
            Label namelabel = new Label { Left = 12, Top = 12, Text = "Name: " };
            TextBox name = new TextBox { Left = 15, Top = 30, Width = 300, Height = 40, Text = acc.Name };
            Label usernamelabel = new Label { Left = 12, Top = 62, Text = "Username: " };
            TextBox username = new TextBox { Left = 15, Top = 80, Width = 300, Height = 40, Text = acc.Username };
            Label passwordlabel = new Label { Left = 12, Top = 112, Text = "Password: " };
            TextBox password = new TextBox { Left = 15, Top = 130, Width = 230, Height = 40, UseSystemPasswordChar = true, Text = acc.Password };
            Button showpw = new Button { Left = 250, Top = 129, Height = 22, Width = 65, Text = "Show" };
            showpw.Click += (sender, e) => { 
                password.UseSystemPasswordChar = !password.UseSystemPasswordChar;
                showpw.Text = password.UseSystemPasswordChar ? "Show" : "Hide";
            };
            Label infolabel = new Label { Left = 12, Top = 162, Text = "Additional Information: ", Width = 300 };
            TextBox info = new TextBox { Left = 15, Top = 180, Width = 300, Height = 100, Multiline = true, ScrollBars = ScrollBars.Both, Text = acc.AdditionalInformation };
            Button save = new Button { Left = 14, Top = 290, Height = 30, Width = 302, Text = "Save" };
            save.Click += (sender, e) => {
                acc.Name = name.Text;
                acc.Username = username.Text;
                acc.Password = password.Text;
                acc.AdditionalInformation = info.Text;
                saved = true;
                prompt.Close();
            };

            prompt.Controls.Add(save);
            prompt.Controls.Add(info);
            prompt.Controls.Add(infolabel);
            prompt.Controls.Add(name);
            prompt.Controls.Add(namelabel);
            prompt.Controls.Add(username);
            prompt.Controls.Add(usernamelabel);
            prompt.Controls.Add(password);
            prompt.Controls.Add(passwordlabel);
            prompt.Controls.Add(showpw);
            prompt.Controls.Add(infolabel);
            prompt.ShowDialog();

            return saved;
        }
    }

    public class Account
    {
        public string Name { get; set; }
        public string Username { get; set; }
        SecureString _password;
        public string Password {
            get {
                if (_password == null)
                    return string.Empty;
                string returnValue = string.Empty;
                IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(_password);
                try
                {
                    returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
                }
                return returnValue;
            }
            set {
                _password = new SecureString();
                foreach (char c in value)
                {
                    _password.AppendChar(c);
                }
                _password.MakeReadOnly();
            }
        }
        public string AdditionalInformation { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Account account &&
                   Username == account.Username;
        }

        public override int GetHashCode()
        {
            return -182246463 + EqualityComparer<string>.Default.GetHashCode(Username);
        }
    }
}