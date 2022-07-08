using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace VRCTC_Installer
{
    public class InstallHelper
    {
        public static InstallHelper instance
        {
            get;
            private set;
        }

        static InstallHelper()
        {
            instance = new InstallHelper();
        }

        private const string APP_NAME = "VRCTCInstaller";
        private const string AUTHOR = "kamiya10";
        private const string PROJECT = "vrclocale";
        public const string MELON_LOADER_URL = "https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip";
        private readonly string tempPath;

        public string VRChatPath
        {
            get;
            set;
        }

        public string VRChatDirectory
        {
            get
            {
                if (VRChatPath == null)
                {
                    throw new InvalidOperationException("尚未設定 VRChat 安裝路徑");
                }
                return Path.GetDirectoryName(VRChatPath);
            }
        }

        public string installVersion
        {
            get;
            set;
        }

        public InstallHelper()
        {
            tempPath = createUniqueTempFolder();
            try
            {
                VRChatPath = getVRChatPath();
            }
            catch
            {
            }
        }

        ~InstallHelper()
        {
            try
            {
                delete(tempPath);
            }
            catch
            {
            }
        }

        public static string getSteamPath()
        {
            object ret = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Valve\Steam", "InstallPath", null);
            if (ret == null)
            {
                ret = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Wow6432Node\Valve\Steam", "InstallPath", null);
                if (ret == null)
                {
                    ret = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
                }
            }

            if (ret == null)
            {
                throw new FileNotFoundException("找不到 Steam 安裝路徑");
            }
            return ret.ToString().Trim().Replace("/", "\\");
        }

        public static string getVRChatPath()
        {
            const string STEAM_APP_FOLDER = "steamapps";
            string steamPath = getSteamPath();

            bool detect = false;
            string libraryPath = null;
            using (FileStream fs = new FileStream(Path.Combine(steamPath, STEAM_APP_FOLDER, "libraryfolders.vdf"), System.IO.FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    bool app = false;
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line.Contains("\"path\""))
                        {
                            app = false;

                            string[] split = line.Split(new string[] {
                                "	"
                            }, StringSplitOptions.RemoveEmptyEntries);
                            libraryPath = split[split.Length - 1].Trim().Replace("\"", string.Empty).Replace("\\\\", "\\");
                        }
                        else if (line.Contains("\"apps\""))
                        {
                            app = true;
                        }
                        else if (app && line.Contains("\"438100\""))
                        {
                            detect = true;
                            break;
                        }
                    }
                }
            }

            if (!detect)
            {
                throw new FileNotFoundException("找不到 VRChat 安裝路徑");
            }
            return Path.Combine(libraryPath, STEAM_APP_FOLDER, "common", "VRChat", "VRChat.exe");
        }

        public static IReadOnlyList<Release> fetchRelease()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue(APP_NAME));
            return client.Repository.Release.GetAll(AUTHOR, PROJECT).Result;
        }

        public string getTCURL()
        {
            if (installVersion == null)
            {
                throw new InvalidOperationException("尚未指定版本號碼");
            }
            return $"https://github.com/{AUTHOR}/{PROJECT}/releases/download/{installVersion}/Updater.zip";
        }

        public static string getLatestAutoTranslatorURL()
        {
            const string AUTO_TRANSLATOR_PROJECT = "XUnity.AutoTranslator";
            const string AUTO_TRANSLATOR_AUTHOR = "bbepis";

            GitHubClient client = new GitHubClient(new ProductHeaderValue(APP_NAME));
            IReadOnlyList<Release> release = client.Repository.Release.GetAll(AUTO_TRANSLATOR_AUTHOR, AUTO_TRANSLATOR_PROJECT).Result;
            Release latest = release.First();
            foreach (ReleaseAsset asset in latest.Assets)
            {
                if (asset.Name.Contains("XUnity.AutoTranslator-MelonMod-IL2CPP"))
                {
                    return asset.BrowserDownloadUrl;
                }
            }
            throw new InvalidDataException($"無法取得最新版本 {AUTO_TRANSLATOR_PROJECT} 下載網址");
        }

        public static string createUniqueTempFolder()
        {
            const int tryTime = 1000;
            const string prefix = "VRCTC$";

            Random random = new Random();
            for (int i = 1; i <= tryTime; ++i)
            {
                string ret = Path.Combine(Path.GetTempPath(), prefix + random.Next());
                if (!Directory.Exists(ret) && !File.Exists(ret))
                {
                    Directory.CreateDirectory(ret);
                    return ret;
                }
            }
            throw new InvalidOperationException("無法建立臨時資料夾");
        }

        public string downloadTempFile(string url, string fileName)
        {
            string path = Path.Combine(tempPath, fileName);
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, path);
                return path;
            }
        }

        public string readCurrentVersion()
        {
            using (FileStream fs = new FileStream(Path.Combine(VRChatDirectory, "AutoTranslator", "Translation", "zh", "version.txt"), System.IO.FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadLine();
                }
            }
        }

        public void extractZIPTemp(string zip)
        {
            ZipFile.ExtractToDirectory(Path.Combine(tempPath, zip), tempPath);
        }

        public void moveTempDirectory()
        {
            FileSystem.MoveDirectory(tempPath, VRChatDirectory);
        }

        public static bool delete(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                return true;
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }

        public void deleteTempFile(params string[] path)
        {
            string[] data = new string[path.Length + 1];
            data[0] = tempPath;
            path.CopyTo(data, 1);
            delete(Path.Combine(data));
        }

        public void removeUselessTempFile()
        {
            deleteTempFile("NOTICE.txt");
            deleteTempFile("MelonLoader", "Documentation");
            deleteTempFile("Mods", "README (AutoTranslator).md");
        }

        public void removeOldVersion()
        {
            string path = VRChatDirectory;
            delete(Path.Combine(path, "MelonLoader"));
            delete(Path.Combine(path, "version.dll"));

            delete(Path.Combine(path, "AutoTranslator"));
            delete(Path.Combine(path, "Mods", "XUnity.AutoTranslator.Plugin.MelonMod.dll"));

            string userLibraryPath = Path.Combine(path, "UserLibs");
            delete(Path.Combine(userLibraryPath, "Translators"));
            delete(Path.Combine(userLibraryPath, "ExIni.dll"));
            delete(Path.Combine(userLibraryPath, "XUnity.AutoTranslator.Plugin.Core.dll"));
            delete(Path.Combine(userLibraryPath, "XUnity.AutoTranslator.Plugin.ExtProtocol.dll"));
            delete(Path.Combine(userLibraryPath, "XUnity.Common.dll"));
            delete(Path.Combine(userLibraryPath, "XUnity.ResourceRedirector.dll"));
        }
    }
}