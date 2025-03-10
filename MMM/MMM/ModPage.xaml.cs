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

            TextBlockModNumber.Text = "Mod总数量: " + totalModNumber.ToString();

        }



        private void RefreshModInfoGrid()
        {
            //选中角色改变后，执行这里的方法。
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

                    //获取下面的所有文件，如果有以.png后缀的则作为Image,目前只识别第一个
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
                //选中后展示右侧Mod列表，Mod列表也是一个GridView

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
            //选中角色改变后，执行这里的方法。
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
            // 获取文件扩展名并转换为小写
            string fileExtension = System.IO.Path.GetExtension(filePath)?.ToLower();

            // 定义支持的图片格式扩展名
            string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".tif", ".jxr", ".wdp", ".ico", ".svg" };

            // 检查文件扩展名是否在支持的列表中
            return fileExtension != null && supportedExtensions.Contains(fileExtension);
        }

        private async void ModShowGrid_Drop(object sender, DragEventArgs e)
        {
            // 获取拖拽目标的 Grid
            var grid = sender as Grid;

            // 获取 Grid 所在的 GridView 项容器
            GridViewItem gridViewItem = FindAncestor<GridViewItem>(grid);

            if (gridViewItem == null)
            {
                return;
            }

            //找到当前Grid的父级GridView
            var gridView = FindAncestor<GridView>(gridViewItem);
            if (gridView == null)
            {
                return;
            }

            // 获取 GridViewItem 的索引
            int index = gridView.IndexFromContainer(gridViewItem);
            if (index == -1)
            {
                return;
            }

            // 从 ItemsSource 中获取对应的数据项
            ModItem modItem = (ModItem)gridView.Items[index];

            // 在这里可以对项进行操作
            string ModLocation = modItem.ModLoaction;

            // 处理拖拽进来的文件
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file)
                    {
                        // 处理文件
                        string filePath = file.Path;
                        // 在这里可以对文件路径进行进一步处理
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
                // 强制刷新 Image 控件
                image.Source = null;
                GC.Collect();

                string uniqueKey = Guid.NewGuid().ToString(); // 使用 GUID 作为唯一键
                //image.Source = new BitmapImage(new Uri($"{filePath}?t={uniqueKey}"));

                image.Source = new BitmapImage(new Uri(image.DataContext is ModItem modItem1 ? modItem1.ModImage : ""));
            }
            else
            {
                Debug.WriteLine("未找到控件");
            }

        }

        //不是很好的设计，因为用户可能会在移动鼠标的过程中选择错误。
        private void CharacterGridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is GridViewItem gridViewItem)
            {
                // 获取当前鼠标悬停的项的索引
                var index = StyledModGrid.IndexFromContainer(gridViewItem);
                if (index != -1)
                {
                    // 设置该项为选中状态
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
