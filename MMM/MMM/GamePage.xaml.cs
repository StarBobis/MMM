using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MMM_Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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

        public GamePage()
        {
            this.InitializeComponent();

            //��ȡ����
            GlobalConfig.SettingCfg.LoadConfig();

            GameIconGridView.ItemsSource = GameIconItemList;

            AddGameIcon();

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

        public void AddGameIcon()
        {
            GameIconItemList.Add(new GameIconItem { 
                GameIconImage = "Assets/GameIcon/GI.png",
                GameName = "ԭ��"
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/HI3.png",
                GameName = "������"
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/HSR.png",
                GameName = "�������������"
            });

            GameIconItemList.Add(new GameIconItem
            {
                GameIconImage = "Assets/GameIcon/ZZZ.png",
                GameName = "������"
            });
        }

        private void GameIconGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ѡ�н�ɫ�ı��ִ������ķ�����
            if (GameIconGridView.SelectedItem != null)
            {

                int index = GameIconGridView.SelectedIndex;
                GameIconItem gameIconItem = GameIconItemList[index];

                Debug.WriteLine(GlobalConfig.SettingCfg.Value.GameName);
                //���õ�ǰ��Ϸ���ұ���
                GlobalConfig.SettingCfg.Value.GameName = gameIconItem.GameName;
                
                GlobalConfig.SettingCfg.SaveConfig();

                // TODO ����ͼ�л�����ǰ��Ϸ�ı���ͼ��


            }
        
        }
    }
}
