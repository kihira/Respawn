using System.Diagnostics;
using System.IO;

namespace Respawn
{
    public class Service
    {
        private const string InstallersDir = "./installers";

        public readonly string Name;
        public readonly string InstallDir;
        public readonly string Installer;
        public readonly string MainExec;

        private bool UseFileVersion = false;

        public Service(string name, string installDir, string installer, string mainExec)
        {
            Name = name;
            InstallDir = installDir;
            Installer = installer;
            MainExec = mainExec;
        }

        public bool IsInstalled()
        {
            return false; // todo
        }

        public void Install()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(Path.Combine(InstallersDir, Installer));
        }

        public string GetInstalledVersion()
        {
            string filePath = Path.Combine(InstallDir, MainExec);
            if (!File.Exists(filePath)) return "Not Installed";
            
            FileVersionInfo installed = FileVersionInfo.GetVersionInfo(filePath);
            return GetPrettyVersion(installed);
        }

        public string GetInstallerVersion()
        {
            string filePath = Path.Combine(InstallersDir, Installer);
            if (!File.Exists(filePath)) return "Not Installed";

            FileVersionInfo installed = FileVersionInfo.GetVersionInfo(filePath);
            return GetPrettyVersion(installed);
        }

        private string GetPrettyVersion(FileVersionInfo info)
        {
            if (info == null) return "Not Installed";
            
            return UseFileVersion ? info.FileMajorPart + "." + info.FileMinorPart + "." + info.FileBuildPart :
                info.ProductMajorPart + "." + info.ProductMinorPart + "." + info.ProductBuildPart;
        }

        public ServiceStatus GetServiceStatus()
        {
            if (!File.Exists(Path.Combine(InstallDir, MainExec))) return ServiceStatus.NOTINSTALLED;
            
            try
            {
                FileVersionInfo installer = FileVersionInfo.GetVersionInfo(Path.Combine(InstallersDir, Installer));
                FileVersionInfo installed = FileVersionInfo.GetVersionInfo(Path.Combine(InstallDir, MainExec));

                if (UseFileVersion)
                {
                    if (installer.FileMajorPart == installed.FileMajorPart &&
                        installer.FileMinorPart == installed.FileMinorPart &&
                        installer.FileBuildPart == installed.FileBuildPart)
                    {
                        return ServiceStatus.INSTALLED;
                    }
                    if (installer.FileMajorPart > installed.FileMajorPart ||
                        installer.FileMinorPart > installed.FileMinorPart ||
                        installer.FileBuildPart > installed.FileBuildPart)
                    {
                        return ServiceStatus.OUTOFDATE;
                    }
                }
                else
                {
                    if (installer.ProductMajorPart == installed.ProductMajorPart &&
                        installer.ProductMinorPart == installed.ProductMinorPart &&
                        installer.ProductBuildPart == installed.ProductBuildPart)
                    {
                        return ServiceStatus.INSTALLED;
                    }
                    if (installer.ProductMajorPart > installed.ProductMajorPart ||
                        installer.ProductMinorPart > installed.ProductMinorPart ||
                        installer.ProductBuildPart > installed.ProductBuildPart)
                    {
                        return ServiceStatus.OUTOFDATE;
                    }
                }

                return ServiceStatus.NEWER;
            }
            catch (FileNotFoundException e)
            {
                return ServiceStatus.NOTINSTALLED;
                // todo handle error
            }
        }
    }
}