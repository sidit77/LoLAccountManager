using System;
using System.Windows.Forms;
using System.Collections.Generic;
using LoLPasswordManager.Properties;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace LoLPasswordManager
{

    public partial class mainForm : Form
    {
        private byte[] entropy = BitConverter.GetBytes(8463123);
        private bool editmode = false;
        private List<Account> accounts;
        private bool loaded = true;

        public mainForm()
        {
            accounts = new List<Account>();
            LoadAccounts();
            
            InitializeComponent();
            SetAccountButtons();
        }

        private void LoadAccounts() {
            try
            {
                accounts.Clear();

                CheckFile();

                using (var fs = File.OpenRead(Settings.Default.FilePath))
                {
                    using (var zf = new ZipFile(fs))
                    {
                        zf.Password = Encoding.UTF8.GetString(ProtectedData.Unprotect(Settings.Default.Password, entropy, DataProtectionScope.CurrentUser));
                        using (var zipStream = zf.GetInputStream(zf.GetEntry("accounts.json")))
                        {
                            using (var sr = new StreamReader(zipStream))
                            {
                                JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(sr));
                                foreach (var acc in o["accounts"])
                                {
                                    accounts.Add(new Account()
                                    {
                                        Name = acc.Value<String>("name"),
                                        Username = acc.Value<String>("username"),
                                        Password = acc.Value<String>("password"),
                                        AdditionalInformation = string.Join(Environment.NewLine, acc["notes"].Select(t => t.Value<string>()))
                                    });
                                }
                            }
                        }
                    }
                }
                if (Settings.Default.FirstStart)
                {
                    Settings.Default.FirstStart = false;
                    Settings.Default.Save();
                }
            }
            catch (Exception e)
            {
                loaded = false;
                MessageBox.Show("Reading the accounts was unsuccessful.\nAutomatic saving disabled.\nError: " + e.Message);
            }
        }

        private void SaveAccounts()
        {
            try
            {
                if (!loaded && MessageBox.Show("The account file wasn't loaded correctly.\nDo you still want to override it?",
                                     "Confirm Override",
                                     MessageBoxButtons.YesNo) == DialogResult.No)
                    return;

                string templocation = Path.GetTempFileName();
                using (var fs = File.Create(templocation))
                {
                    using (var outStream = new ZipOutputStream(fs))
                    {
                        outStream.Password = Encoding.UTF8.GetString(ProtectedData.Unprotect(Settings.Default.Password, entropy, DataProtectionScope.CurrentUser));
                        outStream.PutNextEntry(new ZipEntry("accounts.json"));
                        using (var sw = new StreamWriter(outStream))
                        {
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.Formatting = Formatting.Indented;

                                writer.WriteStartObject();

                                writer.WritePropertyName("accounts");
                                writer.WriteStartArray();

                                foreach(var acc in accounts)
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("name");
                                    writer.WriteValue(acc.Name);

                                    writer.WritePropertyName("username");
                                    writer.WriteValue(acc.Username);

                                    writer.WritePropertyName("password");
                                    writer.WriteValue(acc.Password);

                                    writer.WritePropertyName("notes");
                                    writer.WriteStartArray();

                                    foreach(var note in acc.AdditionalInformation.Replace(Environment.NewLine, "\n").Split('\n').Where(s => s.Length > 0))
                                    {
                                        writer.WriteValue(note);
                                    }

                                    writer.WriteEndArray();

                                    writer.WriteEndObject();
                                }

                                writer.WriteEndArray();
                                writer.WriteEndObject();
                            }
                        }
                    }
                }
                loaded = true;
                if (File.Exists(Settings.Default.FilePath))
                    File.Delete(Settings.Default.FilePath);
                File.Move(templocation, Settings.Default.FilePath);

            }catch(Exception e)
            {
                MessageBox.Show("Saving the accounts was unsuccessful.\nError: " + e.Message);
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
            Button uen = new Button()
            {
                Text = "Change Location",
                Left = 12,
                Top = 35,
                Width = 100
            };
            LinkLabel showpw = new LinkLabel()
            {
                Text = "Show Password",
                Links = { { 0, 13, "https://smashicons.com/" } },
                Left = 120,
                Top = 39,
                Width = 100
            };
            Button exp = new Button()
            {
                Text = "Export Accounts",
                Left = 12,
                Top = 62,
                Width = 100
            };
            LinkLabel credits = new LinkLabel()
            {
                Text = "Icons made by Smashicons, Freepik and Prosymbols",
                Links = { { 14, 10, "https://smashicons.com/" }, { 26, 7, "https://www.flaticon.com/de/autoren/freepik" }, { 38, 10, "https://www.flaticon.com/de/autoren/prosymbols" } },
                Left = 12,
                Top = 92,
                Width = 300
            };
            credits.LinkClicked += (send, args) =>
            {
                System.Diagnostics.Process.Start(args.Link.LinkData as string);
            };
            showpw.LinkClicked += (send, args) =>
            {
                MessageBox.Show(Encoding.UTF8.GetString(ProtectedData.Unprotect(Settings.Default.Password, entropy, DataProtectionScope.CurrentUser)));
            };
            uen.Click += (o, args) =>
            {
                Settings.Default.FirstStart = true;
                Settings.Default.Save();
                LoadAccounts();
            };
            exp.Click += (o, args) =>
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"  ;
                saveFileDialog1.FilterIndex = 0 ;
                saveFileDialog1.RestoreDirectory = true ;
 
                if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter wr = new StreamWriter(saveFileDialog1.OpenFile()))
                    {
                        foreach (var account in accounts)
                        {
                            wr.WriteLine("Name: {0}", account.Name);
                            wr.WriteLine("Username: {0}", account.Username);
                            wr.WriteLine("Password: {0}", account.Password);
                            wr.WriteLine("Notes:");
                            wr.WriteLine(account.AdditionalInformation);
                            wr.WriteLine();
                        }
                    }
                }
            };
            prompt.Controls.Add(showpw);
            prompt.Controls.Add(qal);
            prompt.Controls.Add(uen);
            prompt.Controls.Add(exp);
            prompt.Controls.Add(credits) ;
            prompt.ShowDialog();
            //Settings.Default.CloseAfterLogin = qal.Checked;
            //if (Settings.Default.Password != uen.Checked) {
            //    Settings.Default.Password = uen.Checked;
            //    SaveAccounts();
            //    LoadAccounts();
            //}
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
                case LoginHelper.LoginResult.NotVisible:
                    MessageBox.Show("Can't find the Riot Client!\nMake sure that the Client is not minimized!");
                    break;
                case LoginHelper.LoginResult.Success:
                    if (Settings.Default.CloseAfterLogin)
                        Application.Exit();
                    break;
            }

         
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

        private bool ShowPasswordPrompt(bool newuser)
        {
            bool success = false;


            Form prompt = new Form()
            {
                Width = 345,
                Height = newuser ? 195 : 140,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Text = "Set Password"
            };

            Label passwordlabel = new Label { Left = 12, Top = 12, Text = "Password: " };
            TextBox password = new TextBox { Left = 15, Top = 30, Width = 230, Height = 40, UseSystemPasswordChar = true};
            Button showpw = new Button { Left = 250, Top = 29, Height = 22, Width = 65, Text = "Show" };
            showpw.Click += (sender, e) => {
                password.UseSystemPasswordChar = !password.UseSystemPasswordChar;
                showpw.Text = password.UseSystemPasswordChar ? "Show" : "Hide";
            };

            Label passwordlabel2 = new Label { Left = 12, Top = 62, Text = "Confim Password: " };
            TextBox password2 = new TextBox { Left = 15, Top = 80, Width = 230, Height = 40, UseSystemPasswordChar = true };
            Button showpw2 = new Button { Left = 250, Top = 79, Height = 22, Width = 65, Text = "Show" };
            showpw2.Click += (sender, e) => {
                password2.UseSystemPasswordChar = !password2.UseSystemPasswordChar;
                showpw2.Text = password2.UseSystemPasswordChar ? "Show" : "Hide";
            };

            Button save = new Button { Left = 14, Top = newuser ? 115 : 60, Height = 30, Width = 302, Text = "Save" };
            save.Click += (sender, e) => {

                if(newuser && !password.Text.Equals(password2.Text))
                {
                    MessageBox.Show("The passwords do not match!");
                    return;
                }

                Settings.Default.Password = ProtectedData.Protect(Encoding.UTF8.GetBytes(password.Text), entropy, DataProtectionScope.CurrentUser);
                Settings.Default.Save();
                success = true;
                prompt.Close();
            };

            prompt.Controls.Add(save);
            prompt.Controls.Add(password);
            prompt.Controls.Add(passwordlabel);
            prompt.Controls.Add(showpw);
            if (newuser)
            {
                prompt.Controls.Add(password2);
                prompt.Controls.Add(passwordlabel2);
                prompt.Controls.Add(showpw2);
            }
            prompt.ShowDialog();

            return success;
        }

        private void CheckFile()
        {

            if (!File.Exists(Settings.Default.FilePath) || Settings.Default.FirstStart)
            {
                switch (MessageBox.Show((Settings.Default.FirstStart ?
                    "This appears to be the first start of this program." :
                    "Could not find the previously selected database") +
                    "\nPress Yes to create a new database or No to select an exisiting one!",
                                 "Fresh Start",
                                 MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:

                        var saveFileDialog1 = new SaveFileDialog
                        {
                            Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*",
                            DefaultExt = "zip",
                            InitialDirectory = Settings.Default.FilePath,
                            FilterIndex = 0
                        };

                        if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                            Environment.Exit(0);

                        if (!ShowPasswordPrompt(true))
                            Environment.Exit(0);

                        Settings.Default.FilePath = saveFileDialog1.FileName;
                        Settings.Default.Save();
                        accounts.Clear();
                        SaveAccounts();
                        break;
                    case DialogResult.No:

                        var openFileDialog1 = new OpenFileDialog
                        {
                            Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*",
                            DefaultExt = "zip",
                            InitialDirectory = Settings.Default.FilePath,
                            FilterIndex = 0
                        };

                        if (openFileDialog1.ShowDialog() != DialogResult.OK)
                            Environment.Exit(0);

                        if (!ShowPasswordPrompt(false))
                            Environment.Exit(0);

                        Settings.Default.FilePath = openFileDialog1.FileName;
                        Settings.Default.Save();
                        break;
                    default:
                        Environment.Exit(0);
                        break;
                }
            }

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