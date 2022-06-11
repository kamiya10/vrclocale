using Microsoft.Win32;
using Octokit;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace VRCTC_Installer.Screen
{
    /// <summary>
    /// HomeScreen.xaml 的互動邏輯
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        private System.Collections.Generic.IReadOnlyList<Release> releasesTag;
        private string currentVersion;

        public HomeScreen()
        {
            InitializeComponent();
            fetchVersionTags();
        }

        async private void fetchVersionTags()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("vrclocale"));

            releasesTag = await client.Repository.Release.GetAll("kamiya10", "vrclocale");
            for (int i = 0; i < releasesTag.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = item.Tag = releasesTag[i].TagName;
                releaseTag.Items.Add(item);
            }
        }

        private void vrcPathBrowse_Click(object sender, RoutedEventArgs e)
        {
            string steamPath = string.Empty;

            try
            {
                steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Valve\Steam", "InstallPath", null) as string;

                if (string.IsNullOrEmpty(steamPath))
                {
                    steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Valve\Steam", "InstallPath", null) as string;

                    if (string.IsNullOrEmpty(steamPath))
                    {
                        steamPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string;
                    }
                }

                if (!string.IsNullOrEmpty(steamPath))
                {
                    steamPath = steamPath.Trim().Replace('/', '\\');
                }
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.ToString());
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = steamPath + "\\SteamApps\\common";
            openFileDialog.FileName = "VRChat.exe";
            openFileDialog.Filter = "VRChat |VRChat.exe";
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == true)
            {
                vrcPath.Text = openFileDialog.FileName;
                try
                {
                    currentVersion = System.IO.File.ReadAllText(openFileDialog.FileName.Substring(0, openFileDialog.FileName.Length - 10) + "\\AutoTranslator\\Translation\\zh\\version.txt");
                    for (int i = 0; i < releaseTag.Items.Count; i++)
                    {
                        if ((string)((ComboBoxItem)releaseTag.Items[i]).Content == currentVersion)
                        {
                            releaseTag.SelectedIndex = i;
                            installBtnText.Text = "變更";
                        }
                    }
                }
                catch (Exception) { }
                releaseTag.IsEnabled = true;
            }
        }

        private void releaseTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            installBtn.IsEnabled = (string)((ComboBoxItem)releaseTag.Items[releaseTag.SelectedIndex]).Content != currentVersion;
            releaseTagDescription.HereMarkdown = releasesTag[releaseTag.SelectedIndex].Body;
        }

        private void vrctc_Install(string version)
        {

        }

        private void installBtn_Click(object sender, RoutedEventArgs e)
        {
            installBtnIcon.Visibility = Visibility.Hidden;

        }
    }
}
