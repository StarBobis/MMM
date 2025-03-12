using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using MMM_Core;
using System.Threading.Channels;

namespace MMM
{
    public static class CommandHelper
    {
        public static async Task<bool> ShellOpenFile(string FilePath)
        {
            try
            {

                if (File.Exists(FilePath))
                {
                    try
                    {
                        string workingDirectory = System.IO.Path.GetDirectoryName(FilePath); // 获取程序所在目录

                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = FilePath,
                            UseShellExecute = true, // 允许操作系统决定如何打开文件
                            WorkingDirectory = workingDirectory // 设置工作路径为程序所在路径
                        };

                        Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        await MessageHelper.Show("打开文件出错: \n" + FilePath + "\n" + ex.Message);
                        return false;
                    }
                }
                else
                {
                    await MessageHelper.Show("要打开的文件路径不存在: \n" + FilePath);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                await MessageHelper.Show("Error: " + ex.ToString());
                return false;
            }

        }


        public static async Task<bool> ShellOpenFolder(string FolderPath)
        {
            try
            {
                if (Directory.Exists(FolderPath))
                {
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = FolderPath,
                            UseShellExecute = true, // 允许操作系统决定如何打开文件夹
                            WorkingDirectory = FolderPath // 设置工作路径为要打开的文件夹路径
                        };

                        Process.Start(startInfo);
                    }
                    catch (Exception ex)
                    {
                        await MessageHelper.Show("打开文件夹出错: \n" + FolderPath + "\n" + ex.Message);
                        return false;
                    }
                }
                else
                {
                    await MessageHelper.Show("要打开的文件夹路径不存在: \n" + FolderPath);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                await MessageHelper.Show("Error: " + ex.ToString());
                return false;
            }

        }

        public static FileOpenPicker Get_FileOpenPicker(string Suffix)
        {
            FileOpenPicker picker = new FileOpenPicker();
            // 获取当前窗口的HWND
            nint windowHandle = WindowNative.GetWindowHandle(App.m_window);
            InitializeWithWindow.Initialize(picker, windowHandle);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(Suffix);
            return picker;
        }

        public static FolderPicker Get_FolderPicker()
        {
            FolderPicker picker = new FolderPicker();
            // 获取当前窗口的HWND
            nint windowHandle = WindowNative.GetWindowHandle(App.m_window);
            InitializeWithWindow.Initialize(picker, windowHandle);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            return picker;
        }

        public static async Task<string> ChooseFileAndGetPath(string Suffix)
        {
            try
            {
                FileOpenPicker picker = CommandHelper.Get_FileOpenPicker(Suffix);
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    return file.Path;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception exception)
            {
                await MessageHelper.Show("此功能不支持管理员权限运行，请切换到普通用户打开DBMT。\n" + exception.ToString(), "This functio can't run on admin user please use normal user to open DBMT. \n" + exception.ToString());
            }
            return "";
        }

        public static async Task<string> ChooseFolderAndGetPath()
        {
            try
            {
                FolderPicker folderPicker = CommandHelper.Get_FolderPicker();
                folderPicker.FileTypeFilter.Add("*");
                StorageFolder folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    return folder.Path;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception exception)
            {
                await MessageHelper.Show("此功能不支持管理员权限运行，请切换到普通用户打开DBMT。\n" + exception.ToString());
            }
            return "";

        }


        public static void UnzipFileToFolder(string SourceCompressedFilePath, string TargetFolder)
        {
            string arugmentsstr = " x \"" + SourceCompressedFilePath  + "\" -o\"" + TargetFolder + "\" -aoa ";
            RunExeFile(GlobalConfig.Path_7ZipExe, arugmentsstr);
        }

        public static void RunExeFile(string ExeFilePath, string ArgumentsString)
        {
            Process process = new Process();
            process.StartInfo.FileName = ExeFilePath;
            process.StartInfo.Arguments = ArgumentsString;
            process.StartInfo.UseShellExecute = false;  // 不使用操作系统的shell启动程序
            process.StartInfo.RedirectStandardOutput = true;  // 重定向标准输出
            process.StartInfo.RedirectStandardError = true;   // 重定向标准错误输出
            process.StartInfo.CreateNoWindow = true;  // 不创建新窗口

            // 使用异步读取输出和错误流，避免死锁
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.WriteLine($"Standard Output: {e.Data}");
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.WriteLine($"Standard Error: {e.Data}");
                }
            };

            process.Start();

            // 开始异步读取输出和错误流
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            // 可选：在调试窗口中输出退出代码
            Debug.WriteLine($"Process exited with code: {process.ExitCode}");
        }

    }
}
