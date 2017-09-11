using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Ember.IO;
using System.IO;
using Ember.IO.Ini;
using Microsoft.Win32;

namespace Ember
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Properties being currently used
        /// </summary>
        public Dictionary<string, List<PropertyItem>> Properties { get; private set; } = new Dictionary<string, List<PropertyItem>>();

        /// <summary>
        /// Game Config
        /// </summary>
        public IniFile GameConfig { get; private set; }

        /// <summary>
        /// Ember Config
        /// </summary>
        public Dictionary<string, object> Config { get; private set; } = new Dictionary<string, object>();

        public MainWindow()
        {
            InitializeComponent();

            LoadConfig();
            LoadProperties();
            this.GameConfig = new IniFile((this.Config["lolpath"] as string) + "//Config//game.cfg");

            ConstructSections();
        }

        private void LoadProperties()
        {
            if (!File.Exists("properties.json"))
            {
                this.Properties = DefaultPropertiesCreator.DefaultProperties;
                DefaultPropertiesCreator.WriteDefaultProperties("properties.json");
            }
            else
            {
                this.Properties = JsonConvert.DeserializeObject<Dictionary<string, List<PropertyItem>>>(File.ReadAllText("properties.json"));
            }
        }

        private void LoadConfig()
        {
            if (!File.Exists("config.json"))
            {
                this.Config = new Dictionary<string, object>();
                string lolPath = GetLeaguePathFromRegistry();

                if (lolPath != "")
                {
                    MessageBoxResult result = MessageBox.Show("Ember has detected that you'r League of Legends path is: " + lolPath + "\n\rDo you want to keep it ?",
                        "League of Legends Path", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                    if (result == MessageBoxResult.No)
                    {
                        do
                        {
                            lolPath = GetLeaguePathFromBrowser();

                            if (lolPath == null)
                            {
                                MessageBox.Show("This is not a valid League of Legends folder", "Invalid League of Legends folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        while (lolPath == null);
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Ember couldn't find your League of Legends folder\r\nDo you want to specify it ?",
                        "League of Legends Path", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                    if (result == MessageBoxResult.Yes)
                    {
                        do
                        {
                            lolPath = GetLeaguePathFromBrowser();

                            if (lolPath == null)
                            {
                                MessageBox.Show("This is not a valid League of Legends folder", "Invalid League of Legends folder", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        while (lolPath == null);
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }

                this.Config.Add("lolpath", lolPath);
                File.WriteAllText("config.json", JsonConvert.SerializeObject(this.Config, Formatting.Indented));
            }
            else
            {
                this.Config = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText("config.json"));
            }
        }

        /// <summary>
        /// Creates Group Boxes from <see cref="Properties"/>
        /// </summary>
        private void ConstructSections()
        {
            string missingProperties = "The following properties could not be found in the config: \n\r";
            bool areMissingProperties = false;

            foreach (KeyValuePair<string, List<PropertyItem>> section in this.Properties)
            {
                GroupBox currentSection = new GroupBox()
                {
                    Name = section.Key,
                    Header = section.Key,
                    Content = new StackPanel()
                };

                foreach (PropertyItem property in section.Value)
                {
                    object defaultValue = null;

                    if (this.GameConfig.Entries.ContainsKey(section.Key))
                    {
                        if (this.GameConfig.Entries[section.Key].ContainsKey(property.ConfigName))
                        {
                            defaultValue = this.GameConfig.Entries[section.Key][property.ConfigName];
                            if (property.GetType() == typeof(bool))
                            {
                                defaultValue = (string)defaultValue == "1";
                            }
                            (currentSection.Content as StackPanel).Children.Add(property.GetElementFromType(defaultValue));
                        }
                        else
                        {
                            areMissingProperties = true;
                            missingProperties += string.Format("[{0}, {1}]", section.Key, property.ConfigName);
                        }
                    }
                }

                this.stackPropertyGroups.Children.Add(currentSection);
            }

            if (areMissingProperties)
            {
                MessageBox.Show(missingProperties, "Properties could not be found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Gets the League of Legends folder path from the registry
        /// </summary>
        /// <remarks>Returns a blank string if the path couldn't be found</remarks>
        private string GetLeaguePathFromRegistry()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Riot Games\\League of Legends"))
            {
                string path = (key?.GetValue("Path") as string).Replace(@"\", "//");
                return path.Remove(path.Length - 2, 2);
            }
        }

        /// <summary>
        /// Gets the League of Legends folder path from a folder browser
        /// </summary>
        /// <remarks>Returns a blank string if the path doesn't contain LeagueClient.exe</remarks>
        private string GetLeaguePathFromBrowser()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Select your League of Legends folder";
            dialog.ShowNewFolderButton = false;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (Directory.Exists(dialog.SelectedPath + "//Config"))
                {
                    return dialog.SelectedPath;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Application.Current.Shutdown();
            }

            return null;
        }

        private void buttomAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

        private void buttonDeleteLogs_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists((this.Config["lolpath"] as string) + "//Logs"))
            {
                Directory.Delete((this.Config["lolpath"] as string) + "//Logs", true);
            }

            MessageBox.Show("Your League of Legends logs were successfully deleted", "League of Legends Deleted Logs", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void buttonRecommended_Click(object sender, RoutedEventArgs e)
        {
            foreach (FrameworkElement section in this.stackPropertyGroups.Children)
            {
                foreach (FrameworkElement property in ((section as GroupBox).Content as StackPanel).Children)
                {
                    if (property is CheckBox)
                    {
                        bool isRecommended = this.Properties[section.Name].Find(x => x.ConfigName == property.Name).IsRecommended;
                        (property as CheckBox).IsChecked = isRecommended;
                    }
                }
            }
        }

        private void buttonInstall_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBoxResult.Yes;

            if (!File.Exists(this.Config["lolpath"] + "//Config//game.cfg"))
            {
                messageBoxResult = MessageBox.Show("League of Legends config couldn't be found.\r\nIf you install then no backup will be availible\r\nDo you want to install anyways ?", "League of Legends Config not found", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                foreach (FrameworkElement section in this.stackPropertyGroups.Children)
                {
                    foreach (FrameworkElement property in ((section as GroupBox).Content as StackPanel).Children)
                    {
                        if (property is CheckBox)
                        {
                            this.GameConfig.Entries[section.Name][property.Name] = ((CheckBox)property as CheckBox).IsChecked.Value ? "1" : "0";
                        }
                        else
                        {

                            this.GameConfig.Entries[section.Name][property.Name] = ((property as StackPanel).Children[1] as TextBox).Text;
                        }
                    }
                }

                if (!Directory.Exists("backup"))
                {
                    Directory.CreateDirectory("backup");
                }

                if (File.Exists("backup//game.cfg"))
                {
                    File.Delete("backup//game.cfg");
                }
                File.Copy(this.Config["lolpath"] + "//Config//game.cfg", "backup//game.cfg");
                this.GameConfig.Write(this.Config["lolpath"] + "//Config//game.cfg");

                MessageBox.Show("Installation Successfull!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void buttonUninstall_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("backup//game.cfg"))
            {
                MessageBoxResult result = MessageBox.Show("It seems like your Config backup couldn't be found\r\nDo you want to delete your config instead and let the game create a default one ?", "Config Not Found", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (File.Exists(this.Config["lolpath"] + "//Config//game.cfg"))
                    {
                        File.Delete(this.Config["lolpath"] + "//Config//game.cfg");
                    }
                }
            }
            else
            {
                File.Delete(this.Config["lolpath"] + "//Config//game.cfg");
                File.Copy("backup//game.cfg", this.Config["lolpath"] + "//Config//game.cfg");

                MessageBox.Show("Uninstallation Successfull!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
