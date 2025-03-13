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
using Windows.Services.Maps;
using Microsoft.UI.Xaml.Hosting;
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

            //读取分类列表，默认选中第一个分类
            ReadCategoryList();
            CategoryItemGridView.SelectedIndex = 0;
        }

     
        private ModItem GetCurrentSelectedModItem()
        {
            if (ModItemGridView.SelectedItem != null)
            {
                int index = ModItemGridView.SelectedIndex;
                ModItem modItem = ModItemList[index];
                return modItem;
            }
            else
            {
                return null;
            }
        }


        public CategoryItem GetCurrentSelectedCategoryItem()
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
        public CharacterItem GetCurrentSelectedCharacterItem()
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
                    CategoryItemList.Add(categoryItem);
                }
            }
        }

        public bool IsCompressedFileFormat(string FilePath)
        {

            string Extension = Path.GetExtension(FilePath);
            //Debug.WriteLine(Extension);

            //只存储压缩格式的单个Mod
            if (Extension != ".zip" && Extension != ".7z" && Extension != ".rar")
            {
                //Debug.WriteLine(Extension);
                return false;
            }
            else
            {
                return true;
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
                CategoryItem categoryItem = GetCurrentSelectedCategoryItem();

                string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + GlobalConfig.SettingCfg.Value.GameName + "\\" + categoryItem.CategoryNameName + "\\" + characterName);
                if (!Directory.Exists(characterModsPath))
                {
                    Directory.CreateDirectory(characterModsPath);
                }

                string[] ModFiles = Directory.GetFiles(characterModsPath);

                foreach (string CompressedModFilePath in ModFiles)
                {
                    if (!IsCompressedFileFormat(CompressedModFilePath))
                    {
                        continue;
                    }

                    ModItem modItem = new ModItem();
                    modItem.ModImage = characterItem.CharacterImage;
                    modItem.ModName = Path.GetFileNameWithoutExtension(CompressedModFilePath);
                    modItem.ModCompressedFilePath = CompressedModFilePath;

                    //对每个Mod都去检测对应加载器中对应的Mod位置是否存在，如果存在则Opacity设为1，否则设为0.5f
                    string TargetCharacterLocation = Path.Combine(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder, "Mods\\MMM\\" + categoryItem.CategoryNameName + "\\" + characterItem.CharacterName + "\\");
                    if (Directory.Exists(TargetCharacterLocation))
                    {
                        string[] modfiles = Directory.GetFiles(TargetCharacterLocation);
                        if (modfiles.Length != 0)
                        {
                            modItem.Color = 1.0f;

                        }
                        else
                        {
                            modItem.Color = 0.5f;
                        }
                    }
                    else
                    {
                        modItem.Color = 0.5f;
                    }


                    //获取_Preview.png当作预览图
                    string PreviewPngFilePath = Path.Combine(characterModsPath, modItem.ModName + "_Preview.png");
                    Debug.WriteLine(PreviewPngFilePath);
                    if (File.Exists(PreviewPngFilePath))
                    {
                        modItem.ModImage = PreviewPngFilePath;
                    }

                    ModItemList.Add(modItem);

                }
                //选中后展示右侧Mod列表，Mod列表也是一个GridView

            }
        }

        /// <summary>
        /// 当前选中角色改变后，刷新Mod信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StyledModGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshModInfoGrid();
        }

        /// <summary>
        /// 打开Mod仓库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Menu_OpenModsRepositoryFolder_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFolder(GlobalConfig.Path_ModsFolder);
        }

        private async void Menu_OpenCharacterFolder_Click(object sender, RoutedEventArgs e)
        {
            CategoryItem categoryItem = GetCurrentSelectedCategoryItem();
            CharacterItem characterItem = GetCurrentSelectedCharacterItem();
            if (characterItem != null)
            {
                string characterName = characterItem.CharacterName;

                string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + GlobalConfig.SettingCfg.Value.GameName + "\\" + categoryItem.CategoryNameName + "\\" + characterName);
                await CommandHelper.ShellOpenFolder(characterModsPath);
            }
            
        }

        /// <summary>
        /// 打开当前Mod文件在Mod仓库中的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Menu_ModLocationFolder_Click(object sender, RoutedEventArgs e)
        {
            if (ModItemGridView.SelectedItem != null)
            {
                int index = ModItemGridView.SelectedIndex;
                ModItem modItem = ModItemList[index];
                //Debug.WriteLine(modItem.ModLoaction);
                string ModDirectory = Path.GetDirectoryName(modItem.ModCompressedFilePath);

                await CommandHelper.ShellOpenFolder(ModDirectory);
            }
        }

        /// <summary>
        /// 设置Mod展示信息的拖拽图标为Copy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            //string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".tif", ".jxr", ".wdp", ".ico", ".svg" };
            string[] supportedExtensions = { ".png" };

            // 检查文件扩展名是否在支持的列表中
            return fileExtension != null && supportedExtensions.Contains(fileExtension);
        }

        /// <summary>
        /// 通过拖拽来替换Mod的预览图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            string ModCompressedFilePath = modItem.ModCompressedFilePath;
            string ModFileName = modItem.ModName;
            string ModLocation = Path.GetDirectoryName(ModCompressedFilePath);

            string TargetPictureLocation = "";
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
                            TargetPictureLocation = Path.Combine(ModLocation, ModFileName + "_Preview" + extension);
                            if (File.Exists(TargetPictureLocation))
                            {
                                File.Delete(TargetPictureLocation);
                            }
                            File.Copy(filePath, TargetPictureLocation,true);

                            var image = grid.FindVisualChildren<Image>().FirstOrDefault();
                            if (image != null)
                            {
                                ModItem modItem1 = (ModItem)image.DataContext;
                                Debug.WriteLine("替换图片源为:");
                                image.Visibility = Visibility.Collapsed;
                                // 创建新的 BitmapImage 并重新加载图片
                                using (var stream = new FileStream(TargetPictureLocation, FileMode.Open, FileAccess.Read))
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
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 打开全局配置目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Menu_ConfigsFolder_Click(object sender, RoutedEventArgs e)
        {
            await CommandHelper.ShellOpenFolder(GlobalConfig.Path_ConfigsFolder);
        }



        public void ReadCharacterList(string CategoryName = "")
        {
            ModItemList.Clear();
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

                    string[] modFiles = Directory.GetFiles(characterModsPath);

                    int ModNumber = 0;
                    foreach (string ModFilePath in modFiles)
                    {
                        if (!IsCompressedFileFormat(ModFilePath))
                        {
                            continue;
                        }
                        ModNumber += 1;
                    }

                    characterItem.ModNumber = ModNumber.ToString();

                    totalModNumber = totalModNumber + ModNumber;

                    if (CategoryName == characterItem.Category)
                    {
                        CharacterItemList.Add(characterItem);
                    }
                }

                TextBlockModNumber.Text = "Mod总数量: " + totalModNumber.ToString();
            }
        }


        /// <summary>
        /// 分类选项改变时自动加载下面的角色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CategoryItemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CategoryItem categoryItem = GetCurrentSelectedCategoryItem();
            if (categoryItem != null)
            {
                ReadCharacterList(categoryItem.CategoryNameName);
                CharacterItemGridView.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 双击Mod即启用Mod
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModShowGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            ModItem modItem = GetCurrentSelectedModItem();
            if (modItem == null)
            {
                return;
            }

            string ModLocation = modItem.ModCompressedFilePath;

            CategoryItem currentCategoryItem = GetCurrentSelectedCategoryItem();
            CharacterItem currentCharacterItem = GetCurrentSelectedCharacterItem();

            string TargetCharacterLocation = Path.Combine(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder, "Mods\\MMM\\" + currentCategoryItem.CategoryNameName + "\\" + currentCharacterItem.CharacterName + "\\");
            
            //确保目标角色目录存在，并且双击时删除目标角色目录，再重新创建
            if (!Directory.Exists(TargetCharacterLocation))
            {
                Directory.CreateDirectory(TargetCharacterLocation);
            }
            else
            {
                Directory.Delete(TargetCharacterLocation, true);
                Directory.CreateDirectory(TargetCharacterLocation);
            }

            CommandHelper.UnzipFileToFolder(modItem.ModCompressedFilePath, TargetCharacterLocation);
            //MMMFileUtils.CopyDirectory(ModLocation, TargetModLocation, true);

            RefreshModInfoGrid();

            _ = CommandHelper.ShellOpenFolder(TargetCharacterLocation);
        }

        /// <summary>
        /// 确保右键时自动选中这个Mod项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModItem_GridViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (sender is GridViewItem gridViewItem)
            {
                var index = ModItemGridView.IndexFromContainer(gridViewItem);
                if (index != -1)
                {
                    ModItemGridView.SelectedIndex = index;
                }
            }
        }

        private void CharacterItem_GridViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (sender is GridViewItem gridViewItem)
            {
                var index = CharacterItemGridView.IndexFromContainer(gridViewItem);
                if (index != -1)
                {
                    CharacterItemGridView.SelectedIndex = index;
                }
            }
        }

        private void ModItemBorder_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void ModItemBorder_Drop(object sender, DragEventArgs e)
        {
            CategoryItem categoryItem = GetCurrentSelectedCategoryItem();
            if (categoryItem == null)
            {
                return;
            }
            CharacterItem characterItem = GetCurrentSelectedCharacterItem();
            if (characterItem == null)
            {
                return;
            }

            int CharacterIndex = CharacterItemGridView.SelectedIndex;

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

                        if (!IsCompressedFileFormat(filePath))
                        {
                            continue;
                        }

                        //复制到当前存储库中
                        string characterModsPath = Path.Combine(GlobalConfig.Path_Base, "Mods\\" + GlobalConfig.SettingCfg.Value.GameName + "\\" + categoryItem.CategoryNameName + "\\" + characterItem.CharacterName);
                        if (!Directory.Exists(characterModsPath))
                        {
                            Directory.CreateDirectory(characterModsPath);
                        }

                        string FileName = Path.GetFileName(filePath);
                        string DestnitaionFilePath = Path.Combine(characterModsPath, FileName);
                        if (!File.Exists(DestnitaionFilePath))
                        {
                            File.Copy(filePath, Path.Combine(characterModsPath, FileName));
                            ReadCharacterList(categoryItem.CategoryNameName);
                            CharacterItemGridView.SelectedIndex = CharacterIndex;

                        }
                        else
                        {
                            _ = MessageHelper.Show("当前已存在Mod: " + Path.GetFileNameWithoutExtension(filePath));
                        }
                    }
                }
            }
        }

        private void Menu_CloseMod_Click(object sender, RoutedEventArgs e)
        {
            ModItem modItem = GetCurrentSelectedModItem();
            CategoryItem categoryItem = GetCurrentSelectedCategoryItem();
            CharacterItem characterItem = GetCurrentSelectedCharacterItem();

            string TargetCharacterLocation = Path.Combine(GlobalConfig.SettingCfg.Value.CurrentGameMigotoFolder, "Mods\\MMM\\" + categoryItem.CategoryNameName + "\\" + characterItem.CharacterName + "\\");

            //确保目标角色目录存在，并且双击时删除目标角色目录，再重新创建
            if (Directory.Exists(TargetCharacterLocation))
            {
                Directory.Delete(TargetCharacterLocation, true);
                _ = MessageHelper.Show("已关闭此Mod");
                Directory.CreateDirectory(TargetCharacterLocation);
            }
            else
            {
                _ = MessageHelper.Show("此Mod尚未启用，无需关闭");
            }

        }

        private void Menu_DeleteMod_Click(object sender, RoutedEventArgs e)
        {
            ModItem modItem = GetCurrentSelectedModItem();

            //复制到当前存储库中
            if (File.Exists(modItem.ModCompressedFilePath))
            {
                File.Delete(modItem.ModCompressedFilePath);

                int CharacterIndex = CharacterItemGridView.SelectedIndex;


                CategoryItem categoryItem = GetCurrentSelectedCategoryItem();
                ReadCharacterList(categoryItem.CategoryNameName);
                CharacterItemGridView.SelectedIndex = CharacterIndex;
                _ = MessageHelper.Show("已删除此Mod的存储文件和缓存文件");

            }

        }
    }
}
