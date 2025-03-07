using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using MMM_Core;
using MMM.Helper;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MMM
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModPage : Page
    {

        private ObservableCollection<CharacterItem> items = new ObservableCollection<CharacterItem>();

        public ModPage()
        {
            this.InitializeComponent();
            StyledModGrid.ItemsSource = items;

            AddNewCharacter();
        }

        public void AddNewCharacter()
        {
            CharacterItem item = new CharacterItem
            {
                CharacterImage = "Assets/GI/HeroPicture/Funingna.png",
                BackgroundImage = "Assets/GI/HeroBackground/Gold.png",
                Title = "ܽ����",
                ModNumber = "0"
            };

            items.Add(item);
        }

        private async void StyledModGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ѡ�н�ɫ�ı��ִ������ķ�����
            if (StyledModGrid.SelectedItem != null)
            {
                //string selectedItem = StyledModGrid.SelectedItem.ToString();
                //await MessageHelper.Show(selectedItem);

                //ѡ�к�չʾ�Ҳ�Mod�б�Mod�б�Ҳ��һ��GridView

            }
        }




    }
}
