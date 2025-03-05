using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MMM
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static MainWindow CurrentWindow;
        public NavigationView navigationView => nvSample;


        public MainWindow()
        {
            this.InitializeComponent();
            CurrentWindow = this;

            //隐藏标题栏：
            this.ExtendsContentIntoTitleBar = true;

            //设置标题
            this.Title = "喵喵喵Mod管理器 V1.0.0.1";

            //设置窗口大小
            //1111 814   
            this.AppWindow.Resize(new SizeInt32(1200 + 16, 666 + 9));

            //设置图标
            this.AppWindow.SetIcon("Assets/Miao.ico");

            //默认进入主页界面 8
            if (nvSample.MenuItems.Count > 0)
            {
                nvSample.SelectedItem = nvSample.MenuItems[0];
                contentFrame.Navigate(typeof(GamePage));
            }

            MoveWindowToCenterScreen();
        }

        private void MoveWindowToCenterScreen()
        {
            // 获取与窗口关联的DisplayArea
            var displayArea = DisplayArea.GetFromWindowId(this.AppWindow.Id, DisplayAreaFallback.Nearest);
            // 获取窗口当前的尺寸
            var windowSize = this.AppWindow.Size;

            // 确保我们获取的是正确的显示器信息
            if (displayArea != null)
            {
                // 计算窗口居中所需的左上角坐标，考虑显示器的实际工作区（排除任务栏等）
                int x = (int)(displayArea.WorkArea.X + (displayArea.WorkArea.Width - windowSize.Width) / 2);
                int y = (int)(displayArea.WorkArea.Y + (displayArea.WorkArea.Height - windowSize.Height) / 2);

                // 设置窗口位置
                this.AppWindow.Move(new PointInt32 { X = x, Y = y });
            }

            int window_pos_x = 0;
            int window_pos_y = 0;

            window_pos_x = (int)(displayArea.WorkArea.X + (displayArea.WorkArea.Width - windowSize.Width) / 2);
            window_pos_y = (int)(displayArea.WorkArea.Y + (displayArea.WorkArea.Height - windowSize.Height) / 2);

            if (window_pos_x != -1 && window_pos_y != -1)
            {
                this.AppWindow.Move(new PointInt32(window_pos_x, window_pos_y));
            }
        }

        private void nvSample_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            // 如果点击的是设置按钮，则导航到设置页面
            if (args.IsSettingsInvoked)
            {
                contentFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.InvokedItemContainer is NavigationViewItem item)
            {
                var pageTag = item.Tag.ToString();
                Type pageType = null;

                switch (pageTag)
                {
                    case "GamePage":
                        pageType = typeof(GamePage);
                        break;
                    case "ModPage":
                        pageType = typeof(ModPage);
                        break;
                    case "CommunityPage":
                        pageType = typeof(CommunityPage);
                        break;
                }

                if (pageType != null && contentFrame.Content?.GetType() != pageType)
                {
                    contentFrame.Navigate(pageType);
                }
            }
        }
    }
}
