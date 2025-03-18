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

            // ��ʼ��Composition���
            // ��ȡImage�ؼ���Visual����
            imageVisual = ElementCompositionPreview.GetElementVisual(MainWindow.CurrentWindow.mainWindowImageBrush);
            // ��ȡCompositorʵ��
            compositor = imageVisual.Compositor;

            //��ȡ����
            GlobalConfig.SettingCfg.LoadConfig();

            GameIconGridView.ItemsSource = GameIconItemList;
            InfoBarGridView.ItemsSource = InfoBarItemList;

            AddGameIcon();
            AddInfoBarIcon();

            //���ݵ�ǰ�����д洢����Ϸ���ƣ�����ƥ��GameIconItemList

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
            // ִ������Ҫ�����ҳ�汻�رջ򵼺��뿪ʱ���еĴ���


            GlobalConfig.SettingCfg.SaveConfig();

            // �����Ҫ�����Ե��û���� OnNavigatedFrom ����
            base.OnNavigatedFrom(e);
        }

        public void AddInfoBarIcon()
        {

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "",
                Description = "����QQȺ",
                IconImage = "Assets/InfoBarIcon/QQGroup.png",
            });

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "https://discord.gg/sMdsGAptss",
                Description = "����Discord",
                IconImage = "Assets/InfoBarIcon/Discord.png",
            });

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "https://github.com/StarBobis/MMM",
                Description = "��Github���µ�ַ",
                IconImage = "Assets/InfoBarIcon/Github.png",
            });

            InfoBarItemList.Add(new InfoBarItem
            {
                Link = "https://github.com/StarBobis/MMM/issues",
                Description = "��Github��Issue�����ύ�����͸Ľ�����",
                IconImage = "Assets/InfoBarIcon/Feedback.png",
            });

            //InfoBarItemList.Add(new InfoBarItem
            //{
            //    Link = "https://afdian.com/a/NicoMico666",
            //    Description = "��������NicoMico��������",
            //    IconImage = "Assets/InfoBarIcon/Love.png",
            //});

        }

        public void AddGameIcon()
        {
            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/GI.png",
                GameName = "ԭ��",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/ԭ��.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/HI3.png",
                GameName = "������",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/������.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/HSR.png",
                GameName = "�������������",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/�������������.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/ZZZ.png",
                GameName = "������",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/������.png")))
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/����.png",
                GameName = "����",
                GameBackGroundImage = new BitmapImage(new Uri(Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/����.png")))
            });

        }

        private void CreateFadeAnimation()
        {
            // ����һ�����뵭������
            var fadeAnimation = compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(0.0f, 0.0f); // ��ʼ͸����0%
            fadeAnimation.InsertKeyFrame(1.0f, 1.0f); // Ŀ��͸����100%
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500); // ��������ʱ��300����
            fadeAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay; // �����ӳ���Ϊ

            // Ӧ�ö�����Image��Visual�����Opacity����
            imageVisual.StartAnimation("Opacity", fadeAnimation);
        }
        private void CreateScaleAnimation()
        {
            // ����һ�����Ŷ���
            var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.InsertKeyFrame(0.0f, new Vector3(1.05f, 1.05f, 1.05f)); // ��ʼ���ű���110%
            scaleAnimation.InsertKeyFrame(1.0f, new Vector3(1.0f, 1.0f, 1.0f)); // Ŀ�����ű���100%
            scaleAnimation.Duration = TimeSpan.FromMilliseconds(500); // ��������ʱ��300����
            scaleAnimation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay; // �����ӳ���Ϊ

            // Ӧ�ö�����Image��Visual�����Scale����
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
            //ѡ����Ϸ�ı��ִ������ķ�����
            if (GameIconGridView.SelectedItem != null)
            {

                int index = GameIconGridView.SelectedIndex;
                GameIconItem gameIconItem = GameIconItemList[index];

                Debug.WriteLine(GlobalConfig.SettingCfg.Value.GameName);
                //���õ�ǰ��Ϸ���ұ���
                GlobalConfig.SettingCfg.Value.GameName = gameIconItem.GameName;

                GlobalConfig.SettingCfg.SaveConfig();

                string BackgroundPath = Path.Combine(GlobalConfig.Path_Base, "Assets/GameBackground/" + gameIconItem.GameName + ".png");

                // TODO ����ͼ�л�����ǰ��Ϸ�ı���ͼ��
                CreateScaleAnimation();
                CreateFadeAnimation();
                MainWindow.CurrentWindow.mainWindowImageBrush.Source = gameIconItem.GameBackGroundImage;
                // VisualStateManager.GoToState(MainWindow.CurrentWindow.mainWindowImageBrush, "NewState", true);

                //������������ļ��洢��3Dmigoto·�����ȡ�����û�о�����
                if (File.Exists(GlobalConfig.Path_CurrentGameMainConfigJsonFile))
                {
                    JObject jObject = MMMJsonUtils.ReadJObjectFromFile(GlobalConfig.Path_CurrentGameMainConfigJsonFile);
                    string MigotoFolder = (string)jObject["MigotoFolder"];
                    MigotoPathTextBox.Text = MigotoFolder;
                    GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder = MigotoFolder;

                    //��ȡd3dx.ini�е�����
                    ReadPathSettingFromD3dxIni(Path.Combine(MigotoFolder,"d3dx.ini"));
                }
                else
                {
                    //���û�У��Ǳ�����յ�ǰ����������

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
        //        // ��ȡ��ǰ�����ͣ���������
        //        var index = GameIconGridView.IndexFromContainer(gridViewItem);
        //        if (index != -1)
        //        {
        //            // ���ø���Ϊѡ��״̬
        //            GameIconGridView.SelectedIndex = index;
        //        }
        //    }
        //}


        public void ReadPathSettingFromD3dxIni(string d3dxini_path)
        {
            ProcessPathTextBox.Text = D3dxIniConfig.ReadAttributeFromD3DXIni(d3dxini_path,"target").Trim();
            StarterPathTextBox.Text = D3dxIniConfig.ReadAttributeFromD3DXIni(d3dxini_path,"launch").Trim();
            TextBox_LaunchArgs.Text = D3dxIniConfig.ReadAttributeFromD3DXIni(d3dxini_path, "launch_args").Trim();

            //�����汾��DevΪ0�����ô��ֶΣ���˵����ʾ���֡�
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
                _ = MessageHelper.Show("����ǰѡ�е�Ŀ¼�в�δ����d3dx.ini�����ļ�����ȷ�����Ƿ�ѡ������ȷ��3DmigotoĿ¼��");
            }
            else
            {
                //��ȡ����
                ReadPathSettingFromD3dxIni(d3dxini_path);
                //�ѵ�ǰ��Ϸ�����ñ��浽Configs�ļ�����
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


                await MessageHelper.Show("����ɹ�");

            }
            catch (Exception ex)
            {
                await MessageHelper.Show("����ʧ�ܣ�" + ex.ToString());
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
                //�л���Play�汾��d3d11.dll
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
                    _ = MessageHelper.Show("�л�d3d11.dllʧ��! ��ǰ��Ϸʹ�õ�d3d11.dll�����ѱ�ռ�ã����ȹر���Ϸ���̺���Ϸ�Ĺٷ���������\n\n" + ex.ToString());
                }
            }
            else
            {
                //�л��������汾��d3d11.dll
                string Path_Dev3DmigotoDll = GlobalConfig.Path_3DmigotoGameModForkFolder + "ReleaseX64Dev\\d3d11.dll";
                string Path_CurrentGame3DmigotoDll = Path.Combine(GlobalConfig.Path_LoaderFolder, "d3d11.dll");
                try
                {
                    File.Copy(Path_Dev3DmigotoDll, Path_CurrentGame3DmigotoDll, true);
                    _ = CommandHelper.ShellOpenFolder(GlobalConfig.Path_LoaderFolder);
                }
                catch (Exception ex)
                {
                    _ = MessageHelper.Show("�л�d3d11.dllʧ��! ��ǰ��Ϸʹ�õ�d3d11.dll�����ѱ�ռ�ã����ȹر���Ϸ���̺���Ϸ�Ĺٷ���������\n\n" + ex.ToString());
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
                await MessageHelper.Show("����ʧ�ܣ�" + ex.ToString());
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
                await MessageHelper.Show("����ʧ�ܣ�" + ex.ToString());
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
                await MessageHelper.Show("����ʧ�ܣ�" + ex.ToString());
            }
        }
    }
}
