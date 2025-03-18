using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using MMM_Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MMM
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private ObservableCollection<GameIconItem> GameIconItemList = new ObservableCollection<GameIconItem>();
        private ObservableCollection<InfoBarItem> InfoBarItemList = new ObservableCollection<InfoBarItem>();
        private Compositor compositor;
        private Visual imageVisual;

        public GamePage()
        {
            this.InitializeComponent();

            // 初始化Composition组件
            // 获取Image控件的Visual对象
            imageVisual = ElementCompositionPreview.GetElementVisual(MainWindow.CurrentWindow.mainWindowImageBrush);
            // 获取Compositor实例
            compositor = imageVisual.Compositor;

            //读取配置
            GlobalConfig.SettingCfg.LoadConfig();

            GameIconGridView.ItemsSource = GameIconItemList;
            InfoBarGridView.ItemsSource = InfoBarItemList;

            AddGameIcon();
            AddInfoBarIcon();

            //根据当前配置中存储的游戏名称，依次匹配GameIconItemList

            int index = 0;
            foreach (GameIconItem gameIconItem in GameIconItemList)
            {
                if (gameIconItem.GameName == GlobalConfig.SettingCfg.Value.GameName)
                {
                    GameIconGridView.SelectedIndex = index;
                    break;
                }
                index += 1;
            }

        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // 执行你想要在这个页面被关闭或导航离开时运行的代码


            GlobalConfig.SettingCfg.SaveConfig();

            // 如果需要，可以调用基类的 OnNavigatedFrom 方法
            base.OnNavigatedFrom(e);
        }

        public void AddInfoBarIcon()
        {

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "",
                Description = "加入QQ群",
                IconImage = "Assets/InfoBarIcon/QQGroup.png",
            });

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "https://discord.gg/sMdsGAptss",
                Description = "加入Discord",
                IconImage = "Assets/InfoBarIcon/Discord.png",
            });

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "https://github.com/StarBobis/MMM",
                Description = "打开Github更新地址",
                IconImage = "Assets/InfoBarIcon/Github.png",
            });

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "https://github.com/StarBobis/MMM/issues",
                Description = "打开Github的Issue界面提交反馈和改进建议",
                IconImage = "Assets/InfoBarIcon/Feedback.png",
            });

            //InfoBarItemList.Add(new InfoBarItem
            //{
            //    Link = "https://afdian.com/a/NicoMico666",
            //    Description = "赞助作者NicoMico开发工具",
            //    IconImage = "Assets/InfoBarIcon/Love.png",
            //});

        }

        public void AddGameIcon()
        {
            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/GI.png",
                GameName = "原神",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/原神.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/HI3.png",
                GameName = "崩坏三",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/崩坏三.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/HSR.png",
                GameName = "崩坏：星穹铁道",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/崩坏：星穹铁道.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/ZZZ.png",
                GameName = "绝区零",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/绝区零.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/鸣潮.png",
                GameName = "鸣潮",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/鸣潮.png")))
            });

        }

        private void CreateFadeAnimation()
        {
            // 创建一个淡入淡出动画
            var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(0.0f, 0.0f); // 初始透明度0%
            fadeAnimation.InsertKeyFrame(1.0f, 1.0f); // 目标透明度100%
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500); // 动画持续时间300毫秒
            fadeAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay; // 动画延迟行为

            // 应用动画到Image的Visual对象的Opacity属性
            imageVisual.StartAnimation("Opacity", fadeAnimation);
        }
        private void CreateScaleAnimation()
        {
            // 创建一个缩放动画
            var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(0.0f, new Vector3(1.05f, 1.05f, 1.05f)); // 初始缩放比例110%
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3(1.0f, 1.0f, 1.0f)); // 目标缩放比例100%
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500); // 动画持续时间300毫秒
            scaleAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay; // 动画延迟行为

            // 应用动画到Image的Visual对象的Scale属性
            imageVisual.StartAnimation("Scale", scaleAnimation);

        }


        public GameIconItem GetCurrentSelectedGameIconItem()
        {
            if (GameIconGridView.SelectedItem != null)
            {

                int index = GameIconGridView.SelectedIndex;
                GameIconItem gameIconItem = GameIconItemList[index];
                return gameIconItem;
            }
            else
            {
                return null;
            }
        }

        private void GameIconGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //选中游戏改变后，执行这里的方法。
            if (GameIconGridView.SelectedItem != null)
            {

                int index = GameIconGridView.SelectedIndex;
                GameIconItem gameIconItem = GameIconItemList[index];

                Debug.WriteLine(GlobalConfig.SettingCfg.Value.GameName);
                //设置当前游戏并且保存
                GlobalConfig.SettingCfg.Value.GameName = gameIconItem.GameName;

                GlobalConfig.SettingCfg.SaveConfig();

                string BackgroundPath = Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/" + gameIconItem.GameName + ".png");

                // TODO 背景图切换到当前游戏的背景图？
                CreateScaleAnimation();
                CreateFadeAnimation();
                MainWindow.CurrentWindow.mainWindowImageBrush.Source = gameIconItem.GameBackGroundImage;
                // VisualStateManager.GoToState(MainWindow.CurrentWindow.mainWindowImageBrush, "NewState", true);

                //如果存在配置文件存储了3Dmigoto路径则读取，如果没有就算了
                if (File.Exists(GlobalConfig.Path_CurrentGameMainConfigJsonFile))
                {
                    JObject jObject = MMMJsonUtils.ReadJObjectFromFile(GlobalConfig.Path_CurrentGameMainConfigJsonFile);
                    string MigotoFolder = (string)jObject["MigotoFolder"];
                    MigotoPathTextBox.Text = MigotoFolder;
                    GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder = MigotoFolder;

                    //读取d3dx.ini中的配置
                    ReadPathSettingFromD3dxIni(Path.Combine(MigotoFolder,"d3dx.ini"));
                }
                else
                {
                    //如果没有，那必须清空当前的所有配置

                    MigotoPathTextBox.Text = "";

                    ProcessPathTextBox.Text = "";
                    StarterPathTextBox.Text = "";
                    TextBox_LaunchArgs.Text = "";
                }
                
            }

        }



        //private void GridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        //{
        //    if (sender is GridViewItem gridViewItem)
        //    {
        //        // 获取当前鼠标悬停的项的索引
        //        var index = GameIconGridView.IndexFromContainer(gridViewItem);
        //        if (index != -1)
        //        {
        //            // 设置该项为选中状态
        //            GameIconGridView.SelectedIndex = index;
        //        }
        //    }
        //}


        public void ReadPathSettingFromD3dxIni(string d3dxini_path)
        {
            ProcessPathTextBox.Text = D3dxIniConfig.ReadAttributeFromD3DXIni(d3dxini_path,"target").Trim();
            StarterPathTextBox.Text = D3dxIniConfig.ReadAttributeFromD3DXIni(d3dxini_path,"launch").Trim();
            TextBox_LaunchArgs.Text = D3dxIniConfig.ReadAttributeFromD3DXIni(d3dxini_path, "launch_args").Trim();

            //开发版本中Dev为0或不设置此字段，则说明显示红字。
            string DevStr = D3dxIniConfig.ReadAttributeFromD3DXIni(d3dxini_path, "dev").Trim();
            if (DevStr.Trim() == "1")
            {
                ToggleSwitch_ShowWarning.IsOn = false;
            }
            else if (DevStr.Trim() == "0")
            {
                ToggleSwitch_ShowWarning.IsOn = true;
            }
            else
            {
                ToggleSwitch_ShowWarning.IsOn = false;
            }
        }

        private async void Button_Choose3DmigotoFolder_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = await CommandHelper.ChooseFolderAndGetPath();
            if (folderPath != "")
            {
                MigotoPathTextBox.Text = folderPath;
            }
            else
            {
                return;
            }

            string d3dxini_path = Path.Combine(folderPath, "d3dx.ini");
            if (!File.Exists(d3dxini_path))
            {
                _ = MessageHelper.Show("您当前选中的目录中并未含有d3dx.ini配置文件，请确认您是否选中了正确的3Dmigoto目录。");
            }
            else
            {
                //读取配置
                ReadPathSettingFromD3dxIni(d3dxini_path);
                //把当前游戏的配置保存到Configs文件夹下
                GameIconItem gameIconItem = GetCurrentSelectedGameIconItem();
                gameIconItem.MigotoFolder = folderPath;

                gameIconItem.SaveToJson(GlobalConfig.Path_CurrentGameMainConfigJsonFile);

                GlobalConfig.SettingCfg.SaveConfig();
            }



        }

        private void Button_InitializePath_Click(object sender, RoutedEventArgs e)
        {
            ProcessPathTextBox.Text = "";
            StarterPathTextBox.Text = "";
            TextBox_LaunchArgs.Text = "";
        }

        private async void Button_ChooseProcessFile_Click(object sender, RoutedEventArgs e)
        {
            string filepath = await CommandHelper.ChooseFileAndGetPath(".exe");
            if (filepath != "")
            {
                ProcessPathTextBox.Text = filepath;
            }
        }

        private async void Button_ChooseStarterFile_Click(object sender, RoutedEventArgs e)
        {
            string filepath = await CommandHelper.ChooseFileAndGetPath(".exe");
            if (filepath != "")
            {
                StarterPathTextBox.Text = filepath;
            }
        }

        private async void Button_SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI,"[loader]", "target", ProcessPathTextBox.Text);
                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI, "[loader]", "launch", StarterPathTextBox.Text);
                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI, "[loader]", "launch_args", TextBox_LaunchArgs.Text);

                if (ToggleSwitch_ShowWarning.IsOn)
                {
                    D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI, "[Logging]", "dev", "0");
                }
                else
                {
                    D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI, "[Logging]", "dev", "1");

                }


                await MessageHelper.Show("保存成功");

            }
            catch (Exception ex)
            {
                await MessageHelper.Show("保存失败：" + ex.ToString());
            }
        }

        private async void Button_OpenD3DXINI_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFile(GlobalConfig.Path_D3DXINI);
        }

        private async void Button_Open3DmigotoFolder_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFolder(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder);

        }

        private async void Button_OpenShaderFixesFolder_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFolder(Path.Combine(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder, "ShaderFixes\\"));
        }

        private void ToggleSwitch_DllMode_Toggled(object sender, RoutedEventArgs e)
        {
            if (ToggleSwitch_DllMode.IsOn)
            {
                //切换到Play版本的d3d11.dll
                string Path_Dev3DmigotoDll = GlobalConfig.Path_3DmigotoGameModForkFolder + "ReleaseX64Play\\d3d11.dll";
                string Path_CurrentGame3DmigotoDll = Path.Combine(GlobalConfig.Path_LoaderFolder, "d3d11.dll");
                Debug.WriteLine(Path_CurrentGame3DmigotoDll);

                try
                {
                    File.Copy(Path_Dev3DmigotoDll, Path_CurrentGame3DmigotoDll, true);
                    _ = CommandHelper.ShellOpenFolder(GlobalConfig.Path_LoaderFolder);
                }
                catch (Exception ex)
                {
                    _ = MessageHelper.Show("切换d3d11.dll失败! 当前游戏使用的d3d11.dll可能已被占用，请先关闭游戏进程和游戏的官方启动器。\n\n" + ex.ToString());
                }
            }
            else
            {
                //切换到开发版本的d3d11.dll
                string Path_Dev3DmigotoDll = GlobalConfig.Path_3DmigotoGameModForkFolder + "ReleaseX64Dev\\d3d11.dll";
                string Path_CurrentGame3DmigotoDll = Path.Combine(GlobalConfig.Path_LoaderFolder, "d3d11.dll");
                try
                {
                    File.Copy(Path_Dev3DmigotoDll, Path_CurrentGame3DmigotoDll, true);
                    _ = CommandHelper.ShellOpenFolder(GlobalConfig.Path_LoaderFolder);
                }
                catch (Exception ex)
                {
                    _ = MessageHelper.Show("切换d3d11.dll失败! 当前游戏使用的d3d11.dll可能已被占用，请先关闭游戏进程和游戏的官方启动器。\n\n" + ex.ToString());
                }
            }
        }

        private async void Button_Run3DmigotoLoader_Click(object sender, RoutedEventArgs e)
        {
            string MigotoLoaderExePath1 = Path.Combine(GlobalConfig.Path_LoaderFolder, "3Dmigoto Loader.exe");
            if (!File.Exists(MigotoLoaderExePath1))
            {
                string OriginalMigotoLoaderExePath = Path.Combine(GlobalConfig.Path_Base, "3Dmigoto-GameMod-Fork\\3Dmigoto Loader.exe");
                File.Copy(OriginalMigotoLoaderExePath, MigotoLoaderExePath1,true);
            }

            await CommandHelper.ShellOpenFile(MigotoLoaderExePath1);
        }

        private void GridViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (InfoBarGridView.SelectedItem != null)
            {

                int index = InfoBarGridView.SelectedIndex;
                InfoBarItem infoBarItem = InfoBarItemList[index];
                string link = infoBarItem.Link;
                if (!string.IsNullOrEmpty(link))
                {
                    IAsyncOperation<bool> asyncOperation = Launcher.LaunchUriAsync(new Uri(link));
                }
            }
        }

        private void ToggleSwitch_ShowWarning_Toggled(object sender, RoutedEventArgs e)
        {
            if (ToggleSwitch_ShowWarning.IsOn)
            {
                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI,"[Logging]", "dev", "0");
            }
            else
            {
                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI,"[Logging]", "dev", "1");
            }
        }

        private async void ProcessPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI, "[loader]", "target", ProcessPathTextBox.Text);
            }
            catch (Exception ex)
            {
                await MessageHelper.Show("保存失败：" + ex.ToString());
            }
        }

        private async void StarterPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI, "[loader]", "launch", StarterPathTextBox.Text);
            }
            catch (Exception ex)
            {
                await MessageHelper.Show("保存失败：" + ex.ToString());
            }
        }

        private async void TextBox_LaunchArgs_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                D3dxIniConfig.SaveAttributeToD3DXIni(GlobalConfig.Path_D3DXINI, "[loader]", "launch_args", TextBox_LaunchArgs.Text);
            }
            catch (Exception ex)
            {
                await MessageHelper.Show("保存失败：" + ex.ToString());
            }
        }
    }
}
