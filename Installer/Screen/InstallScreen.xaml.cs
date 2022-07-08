using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace VRCTC_Installer.Screen
{
    public partial class InstallScreen : UserControl
    {
        private bool install;

        public InstallScreen()
        {
            InitializeComponent();
        }

        private void InstallScreen_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!install && (bool)e.NewValue)
            {
                install = true;
                startInstall();
            }
        }

        private void appendText(string text)
        {
            appendText(text, Brushes.White);
        }

        private void appendText(string text, Brush color)
        {
            tbDetail.Text = text;
            TextPointer end = txtDetail.Document.ContentEnd;
            TextRange range = new TextRange(end, end)
            {
                Text = $"{text}{Environment.NewLine}"
            };
            range.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void startInstall()
        {
            const string MELON_LOADER_ZIP = "MelonLoader.zip";
            const string AUTO_TRANSLATOR_ZIP = "XUnity.AutoTranslator.zip";
            const string VRCTC_ZIP = "VRCTC.zip";

            InstallHelper helper = InstallHelper.instance;
            try
            {
                appendText("移除先前版本");
                await Task.Run(() => helper.removeOldVersion());
                progressInstall.Value = 1;

                appendText("下載並解壓 Melon Loader");
                await Task.Run(() =>
                {
                    helper.downloadTempFile(InstallHelper.MELON_LOADER_URL, MELON_LOADER_ZIP);
                    helper.extractZIPTemp(MELON_LOADER_ZIP);
                });
                progressInstall.Value = 2;

                appendText("下載並解壓 X Unity 自動翻譯插件");
                await Task.Run(() =>
                {
                    helper.downloadTempFile(InstallHelper.getLatestAutoTranslatorURL(), AUTO_TRANSLATOR_ZIP);
                    helper.extractZIPTemp(AUTO_TRANSLATOR_ZIP);
                });
                progressInstall.Value = 3;

                appendText("下載並解壓繁體中文化文本");
                await Task.Run(() =>
                {
                    helper.downloadTempFile(helper.getTCURL(), VRCTC_ZIP);
                    helper.extractZIPTemp(VRCTC_ZIP);
                });
                progressInstall.Value = 4;

                appendText("移除無用檔案");
                await Task.Run(() => helper.removeUselessTempFile());
                progressInstall.Value = 5;

                appendText("清理壓縮檔案");
                await Task.Run(() =>
                {
                    helper.deleteTempFile(MELON_LOADER_ZIP);
                    helper.deleteTempFile(AUTO_TRANSLATOR_ZIP);
                    helper.deleteTempFile(VRCTC_ZIP);
                });
                progressInstall.Value = 6;

                appendText("安裝所有檔案");
                await Task.Run(() => helper.moveTempDirectory());
                progressInstall.Value = 7;

                tbTitle.Text = "安裝完成";
                appendText("安裝完成", Brushes.LightGreen);
                btnConfirm.Content = "完成";
            }
            catch (Exception ex)
            {
                tbTitle.Text = "安裝失敗";
                appendText("安裝失敗，請確認關閉 VRChat 後再進行安裝", Brushes.Red);
                appendText(ex.Message, Brushes.Red);
                btnConfirm.Content = "關閉";
            }
            btnConfirm.Visibility = Visibility.Visible;
        }
    }
}