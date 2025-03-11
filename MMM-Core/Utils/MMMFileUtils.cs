using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMM_Core.Utils
{
    public class MMMFileUtils
    {
        public static bool CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // 检查源目录是否存在
            if (!Directory.Exists(sourceDirName))
            {
                return false;
            }

            // 如果目标目录不存在，则创建它
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // 获取源目录中的所有文件并复制它们
            var files = Directory.GetFiles(sourceDirName);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(destDirName, fileName);
                File.Copy(file, destFile, true); // 第三个参数为true表示覆盖已存在的文件
            }

            // 如果需要复制子目录，则递归处理
            if (copySubDirs)
            {
                var directories = Directory.GetDirectories(sourceDirName);
                foreach (var subdir in directories)
                {
                    var dirName = Path.GetFileName(subdir);
                    var destSubDir = Path.Combine(destDirName, dirName);
                    CopyDirectory(subdir, destSubDir, copySubDirs);
                }
            }

            return true;
        }
    }
}
