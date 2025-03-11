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
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using MMM_Core.Utils;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MMM
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModPage : Page
    {

        private ObservableCollection<CharacterItem> CharacterItemList = new ObservableCollection<CharacterItem>();
        private ObservableCollection<ModItem> ModItemList = new ObservableCollection<ModItem>();
        private ObservableCollection<CategoryItem> CategoryItemList = new ObservableCollection<CategoryItem>();

        public ModPage()
        {
            this.InitializeComponent();

            //设置各个GridView的数据源
            CharacterItemGridView.ItemsSource = CharacterItemList;
            ModItemGridView.ItemsSource = ModItemList;
            CategoryItemGridView.ItemsSource = CategoryItemList;

            //添加原神角色用于测试，后续需要修改
            ReadCategoryList();
            CategoryItemGridView.SelectedIndex = 0;

            //AddNewCharacter();

            //默认选中第一个角色
            //CharacterItemGridView.SelectedIndex = 0;
        }

        public void ReadCategoryList()
        {
            CategoryItemList.Clear();

            //读取Json文件中的CategoryList列表
            string CategoryJsonFilePath = GlobalConfig.Path_CurrentGameConfigsFolder + "Category.json";

            if (File.Exists(CategoryJsonFilePath))
            {
                string json = File.ReadAllText(CategoryJsonFilePath);
                List<CategoryItem> ReadedCategoryItems = JsonConvert.DeserializeObject<List<CategoryItem>>(json) ?? new List<CategoryItem>();
                foreach (CategoryItem categoryItem in ReadedCategoryItems)
                {

                    Debug.WriteLine("Add : " + categoryItem.CategoryNameName);
                    CategoryItemList.Add(categoryItem);
                }
            }
        }
       

        public void AddNewCharacter()
        {
            CharacterItemList.Clear();

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

                CharacterItemList.Add(characterItem);
            }

            TextBlockModNumber.Text = "Mod总数量: " + totalModNumber.ToString();

        }

        public CharacterItem GetCurrentCharacterItem()
        {
            if (CharacterItemGridView.SelectedItem != null)
            {
                int index = CharacterItemGridView.SelectedIndex;
                CharacterItem characterItem = CharacterItemList[index];
                return characterItem;
            }
            else
            {
                return null;
            }
        }

        private void RefreshModInfoGrid()
        {
            //选中角色改变后，执行这里的方法。
            if (CharacterItemGridView.SelectedItem != null)
            {
                ModItemList.Clear();

                int index = CharacterItemGridView.SelectedIndex;
                CharacterItem characterItem = CharacterItemList[index];

                string characterName = characterItem.CharacterName;

                CategoryItem categoryItem = GetCurrentCategoryItem();

                string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + GlobalConfig.SettingCfg.Value.GameName + "\\" + categoryItem.CategoryNameName + "\\" + characterName);
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

                    //对每个Mod都去检测对应加载器中对应的Mod位置是否存在，如果存在则Opacity设为1，否则设为0.5f
                    string TargetCharacterLocation = Path.Combine(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder, "Mods\\MMM\\" + categoryItem.CategoryNameName + "\\" + characterItem.CharacterName + "\\");
                    string TargetModLocation = TargetCharacterLocation + modItem.ModName;
                    if (Directory.Exists(TargetModLocation))
                    {
                        modItem.Color = 1.0f;
                    }
                    else
                    {
                        modItem.Color = 0.7f;
                    }

                    ModItemList.Add(modItem);

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

        public CategoryItem GetCurrentCategoryItem()
        {
            if (CategoryItemGridView.SelectedItem != null)
            {
                int index = CategoryItemGridView.SelectedIndex;

                CategoryItem categoryItem = CategoryItemList[index];
                return categoryItem;
            }
            else
            {
                return null;
            }
        }


        private async void Menu_OpenCharacterFolder_Click(object sender, RoutedEventArgs e)
        {
            //选中角色改变后，执行这里的方法。
            if (CharacterItemGridView.SelectedItem != null)
            {
                int index = CharacterItemGridView.SelectedIndex;
                CharacterItem characterItem = CharacterItemList[index];

                string characterName = characterItem.CharacterName;

                CategoryItem categoryItem = GetCurrentCategoryItem();

                string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + GlobalConfig.SettingCfg.Value.GameName + "\\" + categoryItem.CategoryNameName + "\\" + characterName);

                await CommandHelper.ShellOpenFolder(characterModsPath);
            }
        }

        private async void Menu_ModLocationFolder_Click(object sender, RoutedEventArgs e)
        {
            if (ModItemGridView.SelectedItem != null)
            {
                int index = ModItemGridView.SelectedIndex;
                ModItem modItem = ModItemList[index];
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
                            string TargetPictureLocation = Path.Combine(ModLocation, "Preview" + extension);
                            if (File.Exists(TargetPictureLocation))
                            {
                                File.Delete(TargetPictureLocation);
                            }
                            File.Copy(filePath, TargetPictureLocation,true);
                        }
                    }
                }
            }

            var image = grid.FindVisualChildren<Image>().FirstOrDefault();
            if (image != null)
            {
                ModItem modItem1 = (ModItem)image.DataContext;
                Debug.WriteLine("替换图片源为:");
                image.Visibility = Visibility.Collapsed;
                Debug.WriteLine(modItem1.ModImage);
                // 创建新的 BitmapImage 并重新加载图片
                using (var stream = new FileStream(modItem1.ModImage, FileMode.Open, FileAccess.Read))
                {
                    var bitmap = new BitmapImage();
                    bitmap.SetSource(stream.AsRandomAccessStream());
                    image.Source = bitmap;
                }

                image.Visibility = Visibility.Visible;

            }
            else
            {
                Debug.WriteLine("未找到控件");
            }

            RefreshModInfoGrid();

        }

        //不是很好的设计，因为用户可能会在移动鼠标的过程中选择错误。
        private void CharacterGridViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is GridViewItem gridViewItem)
            {
                // 获取当前鼠标悬停的项的索引
                var index = CharacterItemGridView.IndexFromContainer(gridViewItem);
                if (index != -1)
                {
                    // 设置该项为选中状态
                    CharacterItemGridView.SelectedIndex = index;
                }
            }
        }

        private async void Menu_ConfigsFolder_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFolder(GlobalConfig.Path_ConfigsFolder);
        }

        public void ReadCharacterList(string CategoryName = "")
        {
            CharacterItemList.Clear();

            //读取Json文件中的CategoryList列表
            string CurrentGameConfigCategoryFolder = GlobalConfig.Path_CurrentGameConfigsFolder + "Category\\";
            if (!Directory.Exists(CurrentGameConfigCategoryFolder))
            {
                Directory.CreateDirectory(CurrentGameConfigCategoryFolder);
            }

            string CharacterJsonFilePath = CurrentGameConfigCategoryFolder + CategoryName + ".json";

            if (File.Exists(CharacterJsonFilePath))
            {
                int totalModNumber = 0;

                string json = File.ReadAllText(CharacterJsonFilePath);
                List<CharacterItem> ReadedCharacterItems = JsonConvert.DeserializeObject<List<CharacterItem>>(json) ?? new List<CharacterItem>();
                foreach (CharacterItem characterItem in ReadedCharacterItems)
                {
                    string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" +GlobalConfig.SettingCfg.Value.GameName + "\\" + CategoryName + "\\" + characterItem.CharacterName);
                    if (!Directory.Exists(characterModsPath))
                    {
                        Directory.CreateDirectory(characterModsPath);
                    }

                    string[] modFiles = Directory.GetDirectories(characterModsPath);
                    characterItem.ModNumber = modFiles.Length.ToString();

                    totalModNumber = totalModNumber + modFiles.Length;

                    if (CategoryName == characterItem.Category)
                    {
                        CharacterItemList.Add(characterItem);
                    }
                }

                TextBlockModNumber.Text = "Mod总数量: " + totalModNumber.ToString();
            }
        }

        private void CategoryItemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryItemGridView.SelectedItem != null)
            {
                int index = CategoryItemGridView.SelectedIndex;

                CategoryItem categoryItem = CategoryItemList[index];
                ReadCharacterList(categoryItem.CategoryNameName);

                CharacterItemGridView.SelectedIndex = 0;
            }
        }

       

        private void ModShowGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (ModItemGridView.SelectedItem != null)
            {
                int index = ModItemGridView.SelectedIndex;
                ModItem modItem = ModItemList[index];
                //modItem.Color = 1.0f;
                ModItemList[index] = modItem;

                string ModLocation = modItem.ModLoaction;

                CategoryItem currentCategoryItem = GetCurrentCategoryItem();
                CharacterItem currentCharacterItem = GetCurrentCharacterItem();

                string TargetCharacterLocation = Path.Combine(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder, "Mods\\MMM\\" + currentCategoryItem.CategoryNameName + "\\" + currentCharacterItem.CharacterName + "\\");
                if (!Directory.Exists(TargetCharacterLocation))
                {
                    Directory.CreateDirectory(TargetCharacterLocation);
                }
                else
                {
                    Directory.Delete(TargetCharacterLocation, true);
                    Directory.CreateDirectory(TargetCharacterLocation);
                }

                string TargetModLocation = Path.Combine(TargetCharacterLocation, modItem.ModName);

                if (!Directory.Exists(TargetModLocation))
                {
                    Directory.CreateDirectory(TargetModLocation);
                }

                MMMFileUtils.CopyDirectory(ModLocation, TargetModLocation, true);

                RefreshModInfoGrid();

                _ = CommandHelper.ShellOpenFolder(TargetModLocation);
            }
        }
    }
}
