using MaterialDesignThemes.Wpf.Transitions;
using Octokit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace VRCTC_Installer.Screen
{
    public partial class HomeScreen : System.Windows.Controls.UserControl
    {
        private IReadOnlyList<Release> releaseList;

        private bool canInstall
        {
            get
            {
                return !string.IsNullOrWhiteSpace(txtVRChatPath.Text) && !string.IsNullOrWhiteSpace(cbReleaseTag.SelectedItem?.ToString());
            }
        }

        public HomeScreen()
        {
            InitializeComponent();
            initData();
        }

        private void setVRChatPath(string path)
        {
            txtVRChatPath.Text = path;
            if (path != null)
            {
                try
                {
                    string currentVersion = InstallHelper.instance.readCurrentVersion();
                    ItemCollection item = cbReleaseTag.Items;
                    for (int i = 0; i < item.Count; ++i)
                    {
                        if (item[i].ToString() == currentVersion)
                        {
                            cbReleaseTag.SelectedIndex = i;
                            break;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private void updateInstallButton()
        {
            btnInstall.IsEnabled = canInstall;
        }

        private async void initData()
        {
            try
            {
                releaseList = await Task.Run(() => InstallHelper.fetchRelease());
                foreach (Release release in releaseList)
                {
                    cbReleaseTag.Items.Add(release.TagName);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"無法取得版本資訊，請檢查網路連線是否正常 : {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown();
                return;
            }
            setVRChatPath(InstallHelper.instance.VRChatPath);
        }

        private void btnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog()
            {
                Filter = "VRChat 執行檔案|VRChat.exe",
                Title = "請選擇 VRChat 執行檔案"
            })
            {
                if (open.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                setVRChatPath(open.FileName);
            }
        }

        private void txtVRChatPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateInstallButton();
        }

        private void cbReleaseTag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mdReleaseTagDescription.Markdown = releaseList[cbReleaseTag.SelectedIndex].Body;
            }
            catch
            {
                mdReleaseTagDescription.Markdown = "讀取失敗";
            }
            updateInstallButton();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            InstallHelper helper = InstallHelper.instance;
            helper.VRChatPath = txtVRChatPath.Text;
            helper.installVersion = cbReleaseTag.SelectedItem.ToString();

            btnInstall.Command = Transitioner.MoveNextCommand;
        }
    }
}