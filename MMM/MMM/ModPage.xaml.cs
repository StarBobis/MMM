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
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MMM
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModPage : Page
    {

        private ObservableCollection<CharacterItem> CharacterItemShowList = new ObservableCollection<CharacterItem>();
        private ObservableCollection<ModItem> ModItemShowList = new ObservableCollection<ModItem>();

        public ModPage()
        {
            this.InitializeComponent();


            StyledModGrid.ItemsSource = CharacterItemShowList;
            ModInfoGrid.ItemsSource = ModItemShowList;

            AddNewCharacter();

            StyledModGrid.SelectedIndex = 0;
        }

        public void AddNewCharacter()
        {
            CharacterItemShowList.Clear();

            int totalModNumber = 0;

            List<CharacterItem> characterItems = ConfigHelper.GetGICharacterItemList();
            foreach (CharacterItem characterItem in characterItems)
            {
                string characterName = characterItem.CharacterName;

                string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + characterName);
                if (!Directory.Exists(characterModsPath))
                {
                    Directory.CreateDirectory(characterModsPath);
                }

                string[] ModFiles = Directory.GetDirectories(characterModsPath);
                characterItem.ModNumber = ModFiles.Length.ToString();

                totalModNumber = totalModNumber + ModFiles.Length;

                CharacterItemShowList.Add(characterItem);
            }

            TextBlockModNumber.Text = "Mod������: " + totalModNumber.ToString();

        }



        private void RefreshModInfoGrid()
        {
            //ѡ�н�ɫ�ı��ִ������ķ�����
            if (StyledModGrid.SelectedItem != null)
            {
                ModItemShowList.Clear();

                int index = StyledModGrid.SelectedIndex;
                CharacterItem characterItem = CharacterItemShowList[index];

                string characterName = characterItem.CharacterName;

                string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + characterName);
                if (!Directory.Exists(characterModsPath))
                {
                    Directory.CreateDirectory(characterModsPath);
                }

                string[] ModFiles = Directory.GetDirectories(characterModsPath);



                foreach (string SingleModFolderPath in ModFiles)
                {
                    ModItem modItem = new ModItem();
                    modItem.ModImage = characterItem.CharacterImage;
                    modItem.ModName = Path.GetFileName(SingleModFolderPath);
                    modItem.ModLoaction = SingleModFolderPath;
                    ModItemShowList.Add(modItem);

                    //��ȡ����������ļ����������.png��׺������ΪImage,Ŀǰֻʶ���һ��
                    string[] SingleModFiles = Directory.GetFiles(SingleModFolderPath);
                    foreach (string SingleModFile in SingleModFiles)
                    {
                        if (Path.GetExtension(SingleModFile) == ".png")
                        {
                            modItem.ModImage = SingleModFile;
                            break;
                        }
                    }

                }
                //ѡ�к�չʾ�Ҳ�Mod�б�Mod�б�Ҳ��һ��GridView

            }
        }

        private void StyledModGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshModInfoGrid();
        }

        private async void Menu_OpenModsRepositoryFolder_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFolder(GlobalConfig.Path_ModsFolder);
        }

        private async void Menu_OpenCharacterFolder_Click(object sender, RoutedEventArgs e)
        {
            //ѡ�н�ɫ�ı��ִ������ķ�����
            if (StyledModGrid.SelectedItem != null)
            {
                int index = StyledModGrid.SelectedIndex;
                CharacterItem characterItem = CharacterItemShowList[index];

                string characterName = characterItem.CharacterName;

                string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + characterName);

                await CommandHelper.ShellOpenFolder(characterModsPath);
            }
        }

        private async void Menu_ModLocationFolder_Click(object sender, RoutedEventArgs e)
        {
            if (ModInfoGrid.SelectedItem != null)
            {
                int index = ModInfoGrid.SelectedIndex;
                ModItem modItem = ModItemShowList[index];
                //Debug.WriteLine(modItem.ModLoaction);
                await CommandHelper.ShellOpenFolder(modItem.ModLoaction);

            }
        }

        private void ModShowGrid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
        private T FindAncestor<T>(DependencyObject element) where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T)
                    return (T)element;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        public bool IsSupportedImageFormat(string filePath)
        {
            // ��ȡ�ļ���չ����ת��ΪСд
            string fileExtension = System.IO.Path.GetExtension(filePath)?.ToLower();

            // ����֧�ֵ�ͼƬ��ʽ��չ��
            string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".tif", ".jxr", ".wdp", ".ico", ".svg" };

            // ����ļ���չ���Ƿ���֧�ֵ��б���
            return fileExtension != null && supportedExtensions.Contains(fileExtension);
        }

        private async void ModShowGrid_Drop(object sender, DragEventArgs e)
        {
            // ��ȡ��קĿ��� Grid
            var grid = sender as Grid;

            // ��ȡ Grid ���ڵ� GridView ������
            GridViewItem gridViewItem = FindAncestor<GridViewItem>(grid);

            if (gridViewItem == null)
            {
                return;
            }

            //�ҵ���ǰGrid�ĸ���GridView
            var gridView = FindAncestor<GridView>(gridViewItem);
            if (gridView == null)
            {
                return;
            }

            // ��ȡ GridViewItem ������
            int index = gridView.IndexFromContainer(gridViewItem);
            if (index == -1)
            {
                return;
            }

            // �� ItemsSource �л�ȡ��Ӧ��������
            ModItem modItem = (ModItem)gridView.Items[index];

            // ��������Զ�����в���
            string ModLocation = modItem.ModLoaction;

            // ������ק�������ļ�
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // �����ļ�
                        string filePath = file.Path;
                        // ��������Զ��ļ�·�����н�һ������
                        Debug.WriteLine(filePath);

                        if (IsSupportedImageFormat(filePath))
                        {
                            string extension = Path.GetExtension(filePath);
                            GC.Collect();
                            File.Copy(filePath,Path.Combine(ModLocation, "Preview" + extension),true);
                            GC.Collect();
                        }
                    }
                }
            }

            var image = grid.FindVisualChildren<Image>().FirstOrDefault();
            if (image != null)
            {
                // ǿ��ˢ�� Image �ؼ�
                image.Source = null;
                GC.Collect();

                string uniqueKey = Guid.NewGuid().ToString(); // ʹ�� GUID ��ΪΨһ��
                //image.Source = new BitmapImage(new Uri($"{filePath}?t={uniqueKey}"));

                image.Source = new BitmapImage(new Uri(image.DataContext is ModItem modItem1 ? modItem1.ModImage : ""));
            }
            else
            {
                Debug.WriteLine("δ�ҵ��ؼ�");
            }

        }

        //���Ǻܺõ���ƣ���Ϊ�û����ܻ����ƶ����Ĺ�����ѡ�����
        private void CharacterGridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is GridViewItem gridViewItem)
            {
                // ��ȡ��ǰ�����ͣ���������
                var index = StyledModGrid.IndexFromContainer(gridViewItem);
                if (index != -1)
                {
                    // ���ø���Ϊѡ��״̬
                    StyledModGrid.SelectedIndex = index;
                }
            }
        }

        private async void Menu_ConfigsFolder_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFolder(GlobalConfig.Path_ConfigsFolder);
        }



    }
}
