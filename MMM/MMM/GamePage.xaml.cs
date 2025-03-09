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
        private Compositor compositor;
        private Visual imageVisual;

        public GamePage()
        {
            this.InitializeComponent();

            this.InitializeComponent();

            // 初始化Composition组件
            // 获取Image控件的Visual对象
            imageVisual = ElementCompositionPreview.GetElementVisual(MainWindow.CurrentWindow.mainWindowImageBrush);
            // 获取Compositor实例
            compositor = imageVisual.Compositor;

            //读取配置
            GlobalConfig.SettingCfg.LoadConfig();

            GameIconGridView.ItemsSource = GameIconItemList;

            AddGameIcon();

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

        private void GameIconGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //选中角色改变后，执行这里的方法。
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
            }

        }



        private void GridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is GridViewItem gridViewItem)
            {
                // 获取当前鼠标悬停的项的索引
                var index = GameIconGridView.IndexFromContainer(gridViewItem);
                if (index != -1)
                {
                    // 设置该项为选中状态
                    GameIconGridView.SelectedIndex = index;
                }
            }
        }

    }
}
