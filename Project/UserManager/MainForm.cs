using System;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;

namespace UserManager
{
    public partial class MainForm : Form
    {
        //Connection vars:
        MongoClient dbClient;
        IMongoDatabase database;
        IMongoCollection<BsonDocument> collection;

        public MainForm()
        {
            InitializeComponent();
        }

        public void WriteToLog(String content)
        {
            DateTime now = DateTime.Now;
            logOutput.AppendText(now + "\r\n" + content + "\r\n\r\n");
        }

        private void ConnectToDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Get login info from login
                PopupWindow popup = new PopupWindow();
                popup.ShowDialog();
                string atlas_connection_string = popup.EnteredText;
                popup.Dispose();

                //connect to my DB cluster 
                dbClient = new MongoClient(atlas_connection_string);
                database = dbClient.GetDatabase("austria");
                collection = database.GetCollection<BsonDocument>("austrian_users");

                //get db list
                var dbList = dbClient.ListDatabases().ToList();
                String databases_str = "";
                foreach (var db in dbList)
                    databases_str += db.ToString();

                //output it to list
                WriteToLog("Connected! \r\nThe list of databases on this server is: \r\n" + databases_str);
                //labelStatus.Text = "connected";
                //labelStatus.ForeColor = Color.Black;
                //labelStatus.Font = new Font(labelStatus.Font, FontStyle.Regular);
                labelStatus.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please check your internet connection and user credentials.\r\n\r\nError message:" + ex.Message, "Login failed");
            }
        }

        private void ViewDocument(object sender, EventArgs e)
        {
            try
            {
                String svnr = svnrSearchViewDoc.Text;
                WriteToLog("Searching for user with the SVNR " + svnr + "...");

                //search for svnr (needs to be converted from string to long in order to find entries!)
                var filter = Builders<BsonDocument>.Filter.Eq("svnr", Int64.Parse(svnr));
                var docToView = collection.Find(filter).FirstOrDefault();

                //Output user data
                if (docToView != null)
                {
                    WriteToLog(docToView.ToString());
                    textBox67.Text = docToView.GetValue("first_name").ToString();
                    textBox66.Text = docToView.GetValue("last_name").ToString();
                    textBox58.Text = docToView.GetValue("phone_num").ToString();
                    textBox65.Text = docToView.GetValue("svnr").ToString();
                    textBox57.Text = docToView.GetValue("country_code").ToString();
                    textBox53.Text = docToView.GetValue("email").ToString();
                    textBox60.Text = docToView.GetValue("mother_maiden_name").ToString();

                    textBox52.Text = docToView.GetValue("website").ToString();
                    textBox51.Text = docToView.GetValue("username").ToString();
                    textBox50.Text = docToView.GetValue("password").ToString();
                    textBox49.Text = docToView.GetValue("browser_user_agent").ToString();

                    textBox48.Text = docToView.GetValue("card_number").ToString();
                    textBox47.Text = docToView.GetValue("card_expiry").ToString();
                    textBox46.Text = docToView.GetValue("card_cvc2").ToString();

                    textBox37.Text = docToView.GetValue("fav_color").ToString();
                    textBox36.Text = docToView.GetValue("vehicle").ToString();
                    textBox35.Text = docToView.GetValue("guid").ToString();

                    textBox64.Text = docToView.GetValue("street_name").ToString();
                    textBox63.Text = docToView.GetValue("street_num").ToString();
                    textBox62.Text = docToView.GetValue("area_code").ToString();
                    textBox61.Text = docToView.GetValue("area_name").ToString();

                    textBox59.Text = docToView.GetValue("geo_coord").ToString();
                    textBox56.Text = docToView.GetValue("birthday").ToString();
                    textBox55.Text = docToView.GetValue("age").ToString();
                    textBox54.Text = docToView.GetValue("zodiac_sign").ToString();

                    textBox45.Text = docToView.GetValue("company").ToString();
                    textBox44.Text = docToView.GetValue("occupation").ToString();

                    textBox43.Text = docToView.GetValue("height").ToString();
                    textBox42.Text = docToView.GetValue("weight").ToString();
                    textBox41.Text = docToView.GetValue("blood_type").ToString();

                    textBox40.Text = docToView.GetValue("ups_tracking_num").ToString();
                    textBox39.Text = docToView.GetValue("western_union_num").ToString();
                    textBox38.Text = docToView.GetValue("money_gram_num").ToString();
                }
                else
                {
                    MessageBox.Show("Your search returned no results!", "Not Found");
                    WriteToLog("No results found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Did you connect to the database?\r\n\r\nError message:" + ex.Message, "Error!");
            }
        }

        private void CreateUser(object sender, EventArgs e)
        {
            try
            {
                //Get required fields
                if (string.IsNullOrWhiteSpace(textBox_svnr.Text) ||
                    string.IsNullOrWhiteSpace(textBox_lastname.Text) ||
                    string.IsNullOrWhiteSpace(textBox_firstname.Text) ||
                    string.IsNullOrWhiteSpace(textBox_phonenumber.Text) ||
                    string.IsNullOrWhiteSpace(textBox_phonecountrynumber.Text) ||
                    string.IsNullOrWhiteSpace(textBox_email.Text))
                {
                    MessageBox.Show("Bitte fülle alle erforderlichen Felder aus.", "Feld benötigt");
                }
                else
                {
                    //Check if the values are correct phone numbers, country calling codes, emails and birthdays.
                    //Also check if SVNR and DOB match (see last 'else if' statement)
                    //And first check if a user with the same SVNR does already exists (unique id)
                    String dateStr = dateTimePickerDOB.Value.ToString("MMMM dd yyyy");
                    String svnrStr = textBox_svnr.Text;
                    long svnrAsLong = Int64.Parse(svnrStr);
                    String dateStrShort = dateTimePickerDOB.Value.ToString("ddMMyy");
                    String firstName = textBox_firstname.Text;
                    String lastName = textBox_lastname.Text;

                    var filter = Builders<BsonDocument>.Filter.Eq("svnr", svnrAsLong);
                    var docToView = collection.Find(filter).FirstOrDefault();

                    if (docToView != null)
                        MessageBox.Show("A user with this SVNR does already exist!", "Unique Identifier Error");
                    else if (!(svnrAsLong >= 10100 && svnrAsLong <= 9999311299)) //lowest possible: 0000010100, highest possible: 9999311299
                        MessageBox.Show("Please enter a valid SVNR (social security number[Austria]).\n\nExample: \"3926010180\"", "Incorrect SVNR format");
                    else if (!Regex.Match(textBox_phonenumber.Text, @"^([0-9]{4} [0-9]{3} [0-9]{2} [0-9]{2})$").Success)
                        MessageBox.Show("Please enter a valid phone number.\n\nFormat: \"0123 456 78 90\"", "Incorrect phone number format");
                    else if (!Regex.Match(textBox_phonecountrynumber.Text, @"^([0-9]{1,4})$").Success)
                        MessageBox.Show("Please enter a valid country dial-in code.\n\nExample: \"43\"", "Incorrect country calling code format");
                    else if (!Regex.Match(textBox_email.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
                        MessageBox.Show("Please enter a valid email address.", "Incorrect email format");
                    else if (!svnrStr.Substring(svnrStr.Length - 6).Equals(dateStrShort))
                        MessageBox.Show("Date of birth and SVNR don't match!\n\nA user with e.g. a DOB of 01 01 1990 would have a SVNR of XXXX010190", "Data inconsistency");
                    else
                    {
                        //Creating and inserting the BSON Document (one user)
                        var document = new BsonDocument {
                        {"svnr", svnrAsLong}, //svnr as long in database
                        {"first_name", firstName},
                        {"last_name", lastName},
                        {"street_name", textBox5.Text},
                        {"street_num", textBox6.Text},
                        {"area_code", textBox7.Text},
                        {"area_name", textBox8.Text},
                        {"geo_coord", textBox10.Text},
                        {"phone_num", textBox_phonenumber.Text},
                        {"country_code", textBox_phonecountrynumber.Text},
                        {"birthday", dateStr},
                        {"age", textBox14.Text},
                        {"mother_maiden_name", textBox9.Text},
                        {"zodiac_sign", textBox15.Text},
                        {"email", textBox_email.Text},
                        {"username", textBox18.Text},
                        {"password", textBox19.Text},
                        {"website", textBox17.Text},
                        {"browser_user_agent", textBox20.Text},
                        {"card_number", textBox21.Text},
                        {"card_expiry", textBox22.Text},
                        {"card_cvc2", textBox23.Text},
                        {"company", textBox24.Text},
                        {"occupation", textBox25.Text},
                        {"height", textBox27.Text},
                        {"weight", textBox28.Text},
                        {"blood_type", textBox26.Text},
                        {"ups_tracking_num", textBox29.Text},
                        {"western_union_num", textBox30.Text},
                        {"money_gram_num", textBox31.Text},
                        {"fav_color", textBox32.Text},
                        {"vehicle", textBox33.Text},
                        {"guid", textBox34.Text}
                    };
                        collection.InsertOne(document);
                    }

                    String messageStr = "User #" + svnrStr + " (" + firstName + " " + lastName + ") added.";
                    MessageBox.Show(messageStr, "Success");
                    WriteToLog(messageStr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Did you connect to the database?\r\n\r\nError message:" + ex.Message, "Error!");
            }
        }

        private void DeleteUser(object sender, EventArgs e)
        {
            try
            {
                //Get key and value, handle svnr number as long
                String key = comboBoxDelete.Text;
                String value;
                long valueLong = 0;
                if (key.Equals("svnr"))
                    valueLong = Int64.Parse(textBoxDelete.Text);
                value = textBoxDelete.Text;

                if (key.Equals("Choose value...") || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                {
                    MessageBox.Show("Bitte fülle alle erforderlichen Felder aus.", "Feld benötigt");
                }
                else
                {
                    DialogResult dr = MessageBox.Show("Delete first user with a " + key + " of " + value + "?", "Delete?", MessageBoxButtons.YesNo);
                    switch (dr)
                    {
                        case DialogResult.Yes:
                            if (!(key.Equals("svnr")))
                            {
                                var deleteFilter = Builders<BsonDocument>.Filter.Eq(key, value);
                                collection.DeleteOne(deleteFilter);
                                String messageStr = "User  (" + key + ": " + value + ") deleted.";
                                MessageBox.Show(messageStr, "Success");
                                WriteToLog(messageStr);
                            }
                            else
                            {
                                var deleteFilter = Builders<BsonDocument>.Filter.Eq(key, valueLong);
                                collection.DeleteOne(deleteFilter);
                                String messageStr = "User  (" + key + ": " + value + ") deleted.";
                                MessageBox.Show(messageStr, "Success");
                                WriteToLog(messageStr);
                            }
                            break;
                        case DialogResult.No:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Did you connect to the database?\r\n\r\nError message:" + ex.Message, "Error!");
            }
        }

        private void UpdateUser(object sender, EventArgs e)
        {
            try
            {
                //Get all user input
                long baseSvnr = Int64.Parse(textBox99.Text);
                String first_name = textBox101.Text;
                String last_name = textBox100.Text;
                String street_name = textBox98.Text;
                String street_num = textBox97.Text;
                String area_code = textBox96.Text;
                String area_name = textBox95.Text;
                String geo_coord = textBox93.Text;
                String phone_num = textBox92.Text;
                String country_code = textBox91.Text;
                String birthday = textBox90.Text;
                String age = textBox89.Text;
                String mother_maiden_name = textBox94.Text;
                String zodiac_sign = textBox88.Text;
                String email = textBox87.Text;
                String username = textBox85.Text;
                String password = textBox84.Text;
                String website = textBox86.Text;
                String browser_user_agent = textBox83.Text;
                String card_number = textBox82.Text;
                String card_expiry = textBox81.Text;
                String card_cvc2 = textBox80.Text;
                String company = textBox79.Text;
                String occupation = textBox78.Text;
                String height = textBox76.Text;
                String weight = textBox75.Text;
                String blood_type = textBox77.Text;
                String ups_tracking_num = textBox74.Text;
                String western_union_num = textBox73.Text;
                String money_gram_num = textBox72.Text;
                String fav_color = textBox71.Text;
                String vehicle = textBox70.Text;
                String guid = textBox69.Text;

                //Change only things that the user inputs (don't write empty fields)
                var update = Builders<BsonDocument>.Update.Set("svnr", baseSvnr);
                if (!string.IsNullOrWhiteSpace(first_name))
                    update = update.Set("first_name", first_name);
                if (!string.IsNullOrWhiteSpace(last_name))
                    update = update.Set("last_name", last_name);
                if (!string.IsNullOrWhiteSpace(street_name))
                    update = update.Set("street_name", street_name);
                if (!string.IsNullOrWhiteSpace(street_num))
                    update = update.Set("street_num", street_num);
                if (!string.IsNullOrWhiteSpace(area_code))
                    update = update.Set("area_code", area_code);
                if (!string.IsNullOrWhiteSpace(area_name))
                    update = update.Set("area_name", area_name);
                if (!string.IsNullOrWhiteSpace(geo_coord))
                    update = update.Set("geo_coord", geo_coord);
                if (!string.IsNullOrWhiteSpace(phone_num))
                    update = update.Set("phone_num", phone_num);
                if (!string.IsNullOrWhiteSpace(country_code))
                    update = update.Set("country_code", country_code);
                if (!string.IsNullOrWhiteSpace(birthday))
                    update = update.Set("birthday", birthday);
                if (!string.IsNullOrWhiteSpace(age))
                    update = update.Set("age", age);
                if (!string.IsNullOrWhiteSpace(mother_maiden_name))
                    update = update.Set("mother_maiden_name", mother_maiden_name);
                if (!string.IsNullOrWhiteSpace(zodiac_sign))
                    update = update.Set("zodiac_sign", zodiac_sign);
                if (!string.IsNullOrWhiteSpace(email))
                    update = update.Set("email", email);
                if (!string.IsNullOrWhiteSpace(username))
                    update = update.Set("username", username);
                if (!string.IsNullOrWhiteSpace(password))
                    update = update.Set("password", password);
                if (!string.IsNullOrWhiteSpace(website))
                    update = update.Set("website", website);
                if (!string.IsNullOrWhiteSpace(browser_user_agent))
                    update = update.Set("browser_user_agent", browser_user_agent);
                if (!string.IsNullOrWhiteSpace(card_number))
                    update = update.Set("card_number", card_number);
                if (!string.IsNullOrWhiteSpace(card_expiry))
                    update = update.Set("card_expiry", card_expiry);
                if (!string.IsNullOrWhiteSpace(card_cvc2))
                    update = update.Set("card_cvc2", card_cvc2);
                if (!string.IsNullOrWhiteSpace(company))
                    update = update.Set("company", company);
                if (!string.IsNullOrWhiteSpace(occupation))
                    update = update.Set("occupation", occupation);
                if (!string.IsNullOrWhiteSpace(height))
                    update = update.Set("height", height);
                if (!string.IsNullOrWhiteSpace(weight))
                    update = update.Set("weight", weight);
                if (!string.IsNullOrWhiteSpace(blood_type))
                    update = update.Set("blood_type", blood_type);
                if (!string.IsNullOrWhiteSpace(ups_tracking_num))
                    update = update.Set("ups_tracking_num", ups_tracking_num);
                if (!string.IsNullOrWhiteSpace(western_union_num))
                    update = update.Set("western_union_num", western_union_num);
                if (!string.IsNullOrWhiteSpace(money_gram_num))
                    update = update.Set("money_gram_num", money_gram_num);
                if (!string.IsNullOrWhiteSpace(fav_color))
                    update = update.Set("fav_color", fav_color);
                if (!string.IsNullOrWhiteSpace(vehicle))
                    update = update.Set("vehicle", vehicle);
                if (!string.IsNullOrWhiteSpace(guid))
                    update = update.Set("guid", guid);

                var filter = Builders<BsonDocument>.Filter.Eq("svnr", baseSvnr);
                collection.UpdateOne(filter, update);

                String messageStr = "User #" + baseSvnr + " updated.";
                MessageBox.Show(messageStr, "Success");
                WriteToLog(messageStr);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Did you connect to the database?\r\n\r\nError message:" + ex.Message, "Error!");
            }
        }

        private void ReadUsers(object sender, EventArgs e)
        {
            //default: always clear datagridview (table) before populating it
            if (checkBox1.Checked)
            {
                dataGridViewUsers.Rows.Clear();
                dataGridViewUsers.Refresh();
            }

            try
            {
                //Get Users of a Svnr range (users from # to #)
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Gte("svnr", Int64.Parse(textBoxSvnrFrom.Text)) & builder.Lte("svnr", Int64.Parse(textBoxSvnrTo.Text));

                //Let the user add up to 3 filters
                String key1 = comboBoxKey1.Text;
                if (!key1.Equals("Choose value...") && !string.IsNullOrWhiteSpace(key1))
                    filter &= builder.Eq(key1, textBoxValue1.Text);
                String key2 = comboBoxKey2.Text;
                if (!key2.Equals("Choose value...") && !string.IsNullOrWhiteSpace(key2))
                    filter &= builder.Eq(key2, textBoxValue2.Text);
                String key3 = comboBoxKey3.Text;
                if (!key3.Equals("Choose value...") && !string.IsNullOrWhiteSpace(key3))
                    filter &= builder.Eq(key3, textBoxValue3.Text);

                //fill table with all users (that were returned by the search)
                var docs = collection.Find(filter).ToList();
                foreach (var doc in docs)
                {
                    this.dataGridViewUsers.Rows.Add(
                        doc.GetValue("first_name").ToString(),
                        doc.GetValue("last_name").ToString(),
                        doc.GetValue("svnr").ToString(),
                        doc.GetValue("street_name").ToString(),
                        doc.GetValue("street_num").ToString(),
                        doc.GetValue("area_code").ToString(),
                        doc.GetValue("area_name").ToString(),
                        doc.GetValue("geo_coord").ToString(),
                        doc.GetValue("phone_num").ToString(),
                        doc.GetValue("country_code").ToString(),
                        doc.GetValue("birthday").ToString(),
                        doc.GetValue("age").ToString(),
                        doc.GetValue("mother_maiden_name").ToString(),
                        doc.GetValue("zodiac_sign").ToString(),
                        doc.GetValue("email").ToString(),
                        doc.GetValue("website").ToString(),
                        doc.GetValue("username").ToString(),
                        doc.GetValue("password").ToString(),
                        doc.GetValue("browser_user_agent").ToString(),
                        doc.GetValue("card_number").ToString(),
                        doc.GetValue("card_expiry").ToString(),
                        doc.GetValue("card_cvc2").ToString(),
                        doc.GetValue("company").ToString(),
                        doc.GetValue("occupation").ToString(),
                        doc.GetValue("blood_type").ToString(),
                        doc.GetValue("height").ToString(),
                        doc.GetValue("weight").ToString(),
                        doc.GetValue("ups_tracking_num").ToString(),
                        doc.GetValue("western_union_num").ToString(),
                        doc.GetValue("money_gram_num").ToString(),
                        doc.GetValue("fav_color").ToString(),
                        doc.GetValue("vehicle").ToString(),
                        doc.GetValue("guid").ToString()
                    );
                }
                WriteToLog(("Reading user data - svnr (" + textBoxSvnrFrom.Text + "-" + textBoxSvnrTo.Text + "), " + (key1 + ":" + textBoxValue1.Text + ", " + key2 + ":" + textBoxValue2.Text + ", " + key3 + ":" + textBoxValue3.Text + ", \r\nReturned ").Replace(":, ", "") + docs.Count + " results.").Replace("Choose value...", ""));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Did you connect to the database?\r\n\r\nError message:" + ex.Message, "Error!");
            }
        }

        private void ClearDataGridView(object sender, EventArgs e)
        {
            dataGridViewUsers.Rows.Clear();
            dataGridViewUsers.Refresh();
        }

        private void SaveLogTxt(object sender, EventArgs e)
        {
            //Open a filechooser and save text from log in a file
            String[] content = logOutput.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            SaveFileDialog oSaveFileDialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            if (oSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = oSaveFileDialog.FileName;
                File.AppendAllLines(fileName, content);
            }
        }

        private void ÜberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("© Program and data set by GerH, 2020.\r\n\r\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\r\n\r\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\r\n\r\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.\r\n\r\nIcon from Flaticon, made by \"Eucalyp\" (www.flaticon.com/de/autoren/eucalyp) \r\n\r\nVisit developer Github page?", "About", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://github.com/gh28942");
            }
        }

        private void DokumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //opens readme pdf
                var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"GerH\aum\aum-ReadMe.pdf");

                Process myProcess = new Process();
                myProcess.StartInfo.FileName = "acroRd32.exe";
                myProcess.StartInfo.Arguments = "/A \"page=2=OpenActions\" /n " + fileName;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Couldn't open ReadMe Pdf.\r\n\r\nError message:" + ex.Message, "Error!");
            }
        }

        private void RunCommand(object sender, EventArgs e)
        {
            String commandStr = "";
            try
            {
                //console command (MongoDB query) and diagnostic commands
                commandStr = textBoxConsCommand.Text;
                textBoxConsCommand.Text = "";
                WriteToLog("Run command: \r\n" + commandStr);

                //clear screen command
                if (commandStr.Equals("cls"))
                    textBoxConsResult.Text = ("");
                //quit program
                else if(commandStr.Equals("quit") || commandStr.Equals("exit"))
                    this.Close();
                //don't allow user to drop / delete stuff in bulk here 
                else if (commandStr.ToLower().Contains("drop") || commandStr.ToLower().Contains("delete") || commandStr.ToLower().Contains("remove"))
                    textBoxConsResult.AppendText("\r\n>" + commandStr + "\r\n" + "Sorry, but I won't let you do that.\r\n");
                //else: is allowed command
                else
                {
                    //get command results
                    var command = new BsonDocument { { commandStr, 1 } };
                    var result = database.RunCommand<BsonDocument>(command);

                    //print to console
                    textBoxConsResult.AppendText("\r\n>" + commandStr + "\r\n" + result.ToJson().Replace(",", ",\r\n") + "\r\n");
                }
            }
            catch (Exception ex)
            {
                textBoxConsResult.AppendText("\r\n>" + commandStr + "\r\nError - could not run command. \r\n"+ ex.Message+"\r\n");
            }
        }
    }
}
