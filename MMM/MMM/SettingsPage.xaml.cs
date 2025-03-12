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
using Newtonsoft.Json;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MMM
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        private ObservableCollection<CategoryItem> CategoryItemList = new ObservableCollection<CategoryItem>();
        private ObservableCollection<CharacterItem> CharacterItemList = new ObservableCollection<CharacterItem>();


        public SettingsPage()
        {
            this.InitializeComponent();

            CategoryItemGridView.ItemsSource = CategoryItemList;
            CharacterItemGridView.ItemsSource = CharacterItemList;

            ReadCategoryList();
            CategoryItemGridView.SelectedIndex = 0;

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

        public void SaveCharacterList()
        {
            string CurrentGameConfigCategoryFolder = GlobalConfig.Path_CurrentGameConfigsFolder + "Category\\";
            if (!Directory.Exists(CurrentGameConfigCategoryFolder))
            {
                Directory.CreateDirectory(CurrentGameConfigCategoryFolder);
            }
            CategoryItem currentCategoryItem = GetCurrentCategoryItem();

            string CharacterJsonFilePath = CurrentGameConfigCategoryFolder + currentCategoryItem.CategoryNameName + ".json";

            List<CharacterItem> characterItems = [];
            foreach (CharacterItem characterItem in CharacterItemList)
            {
                characterItem.Category = currentCategoryItem.CategoryNameName;
                characterItems.Add(characterItem);
            }

            string json = JsonConvert.SerializeObject(characterItems, Formatting.Indented);
            File.WriteAllText(CharacterJsonFilePath, json);
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
                string json = File.ReadAllText(CharacterJsonFilePath);
                List<CharacterItem> ReadedCharacterItems = JsonConvert.DeserializeObject<List<CharacterItem>>(json) ?? new List<CharacterItem>();
                foreach (CharacterItem characterItem in ReadedCharacterItems)
                {
                    if (CategoryName == characterItem.Category)
                    {
                        CharacterItemList.Add(characterItem);
                    }
                }
            }
        }

        public void SaveCategoryList()
        {
            string CategoryJsonFilePath = GlobalConfig.Path_CurrentGameConfigsFolder + "Category.json";
            List<CategoryItem> categoryItems = CategoryItemList.ToList<CategoryItem>();
            string json = JsonConvert.SerializeObject(categoryItems, Formatting.Indented);
            File.WriteAllText(CategoryJsonFilePath, json);
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


        private async void Button_ChooseCategoryImage_Click(object sender, RoutedEventArgs e)
        {
            string ImagePath = await CommandHelper.ChooseFileAndGetPath(".png");
            if (ImagePath != "")
            {
                TextBox_CategoryImage.Text = ImagePath;
            }
        }

        public CategoryItem CreateNewCategoryItem()
        {
            if (!File.Exists(TextBox_CategoryImage.Text))
            {
                _ = MessageHelper.Show("当前填写的分类图片路径不存在");
                return null;
            }

            if (TextBox_CategoryName.Text == "")
            {
                _ = MessageHelper.Show("分类名称不能为空");
                return null;
            }

            //拼接咱们路径下的图片路径，然后复制过去
            string CategoryImageFileName = Path.GetFileName(TextBox_CategoryImage.Text);

            string CategoryRelativePath = "Assets\\" + GlobalConfig.SettingCfg.Value.GameName + "\\CategoryImage\\";

            //创建目录防止不存在
            Directory.CreateDirectory(Path.Combine(GlobalConfig.Path_Base, CategoryRelativePath));

            string CategoryImageNewPath = CategoryRelativePath + TextBox_CategoryName.Text + ".png";

            string FinalNewCategoryImagePath = Path.Combine(GlobalConfig.Path_Base, CategoryImageNewPath);
            try
            {
                File.Copy(TextBox_CategoryImage.Text, FinalNewCategoryImagePath, true);

            }
            catch (Exception e)
            {
                _ = MessageHelper.Show("疑似当前已存在名为:" + TextBox_CategoryName.Text + " 的分类: " + e.ToString());
                return null;
            }
            
            Debug.WriteLine(Path.Combine(GlobalConfig.Path_Base, CategoryImageNewPath));

            CategoryItem categoryItem = new CategoryItem
            {
                CategoryImage = CategoryImageNewPath,
                CategoryNameName = TextBox_CategoryName.Text
            };

            return categoryItem;
        }


        private void Button_AddNewCategory_Click(object sender, RoutedEventArgs e)
        {

            CategoryItem categoryItem = CreateNewCategoryItem();
            if (categoryItem != null)
            {
                CategoryItemList.Add(categoryItem);
                SaveCategoryList();
            }
            
        }

        private void Button_ModifySelectedCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryItemGridView.SelectedItem != null)
            {
                int index = CategoryItemGridView.SelectedIndex;

                //在修改时，要把旧的图片删掉才行
                string oldImagePath = Path.Combine(GlobalConfig.Path_Base, CategoryItemList[index].CategoryImage);
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }

                CategoryItem categoryItem = CreateNewCategoryItem();
                if (categoryItem != null)
                {
                    CategoryItemList[index] = categoryItem;

                    //修改完记得保存
                    SaveCategoryList();
                }

            }
            else
            {
                _ = MessageHelper.Show("请先选中一个分类项");
            }
        }

        private void Button_DeleteSelectedCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryItemGridView.SelectedItem != null)
            {
                int index = CategoryItemGridView.SelectedIndex;

                //先解除引用

                string CategoryImageCachePath = Path.Combine(GlobalConfig.Path_Base, CategoryItemList[index].CategoryImage) ;

                CategoryItemList.RemoveAt(index);

                //删除缓存图片
                if (File.Exists(CategoryImageCachePath))
                {
                    File.Delete(CategoryImageCachePath);
                }

                SaveCategoryList();

            }
            else
            {
                _ = MessageHelper.Show("请先选中一个分类项");
            }
        }

        private void CategoryItemGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryItemGridView.SelectedItem != null)
            {
                int index = CategoryItemGridView.SelectedIndex;

                CategoryItem categoryItem = CategoryItemList[index];
                ReadCharacterList(categoryItem.CategoryNameName);

            }
        }

        private async void Button_ChooseCharacterBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            string ImagePath = await CommandHelper.ChooseFileAndGetPath(".png");
            if (ImagePath != "")
            {
                TextBox_CharacterBackgroundImage.Text = ImagePath;
            }
        }

        private async void Button_ChooseCharacterImage_Click(object sender, RoutedEventArgs e)
        {
            string ImagePath = await CommandHelper.ChooseFileAndGetPath(".png");
            if (ImagePath != "")
            {
                TextBox_CharacterImage.Text = ImagePath;
            }
        }


        public CharacterItem CreateNewCharacterItem()
        {
            if (!File.Exists(TextBox_CharacterBackgroundImage.Text))
            {
                _ = MessageHelper.Show("当前填写的背景图片路径不存在");
                return null;
            }

            if (!File.Exists(TextBox_CharacterImage.Text))
            {
                _ = MessageHelper.Show("当前填写的主体图片路径不存在");
                return null;
            }

            if (TextBox_CharacterName.Text == "")
            {
                _ = MessageHelper.Show("项目名称不能为空");
                return null;
            }

            string CharacterRelativePath = "Assets\\" + GlobalConfig.SettingCfg.Value.GameName + "\\CharacterImage\\";
            Directory.CreateDirectory(Path.Combine(GlobalConfig.Path_Base, CharacterRelativePath));


            //拼接咱们路径下的图片路径，然后复制过去
            string CharacterImageNewPath = CharacterRelativePath + TextBox_CharacterName.Text + "_Main.png";
            string FinalNewCharacterImagePath = Path.Combine(GlobalConfig.Path_Base, CharacterImageNewPath);

            string CharacterBackgroundImageNewPath = CharacterRelativePath + TextBox_CharacterName.Text + "_Background.png";
            string FinalNewCharacterBackgroundImagePath = Path.Combine(GlobalConfig.Path_Base, CharacterBackgroundImageNewPath);

            try
            {
                File.Copy(TextBox_CharacterImage.Text, FinalNewCharacterImagePath, true);
                File.Copy(TextBox_CharacterBackgroundImage.Text, FinalNewCharacterBackgroundImagePath, true);

            }
            catch (Exception e)
            {
                _ = MessageHelper.Show("疑似当前已存在名为:" + TextBox_CharacterName.Text + " 的项目:" + e.ToString());
                return null;
            }

            CategoryItem currentCategoryItem = GetCurrentCategoryItem();

            CharacterItem characterItem = new CharacterItem
            {
                CharacterImage = CharacterImageNewPath,
                BackgroundImage = CharacterBackgroundImageNewPath,
                CharacterName = TextBox_CharacterName.Text,
                Category = currentCategoryItem.CategoryNameName
            };

            return characterItem;
        }

        private void Button_AddNewCharacter_Click(object sender, RoutedEventArgs e)
        {
            CharacterItem characterItem = CreateNewCharacterItem();
            if (characterItem != null)
            {
                CharacterItemList.Add(characterItem);
                SaveCharacterList();
            }
        }

        private void Button_ModifiySelectedCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (CharacterItemGridView.SelectedItem != null)
            {
                int index = CharacterItemGridView.SelectedIndex;

               


                string CategoryImageCachePath = Path.Combine(GlobalConfig.Path_Base, CharacterItemList[index].CharacterImage);
                string CategoryBackgroundImageCachePath = Path.Combine(GlobalConfig.Path_Base, CharacterItemList[index].BackgroundImage);
                
                CharacterItemList[index].CharacterImage = "";
                CharacterItemList[index].BackgroundImage = "";

                //删除缓存图片
                if (File.Exists(CategoryImageCachePath))
                {
                    File.Delete(CategoryImageCachePath);
                }

                if (File.Exists(CategoryBackgroundImageCachePath))
                {
                    File.Delete(CategoryBackgroundImageCachePath);
                }

                CharacterItem characterItem = CreateNewCharacterItem();

                if (characterItem != null)
                {
                    CharacterItemList[index] = characterItem;

                    //修改完记得保存
                    SaveCharacterList();
                }

            }
            else
            {
                _ = MessageHelper.Show("请先选中一个项目");
            }
        }

        private void Button_DeleteSelectedCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (CharacterItemGridView.SelectedItem != null)
            {
                int index = CharacterItemGridView.SelectedIndex;

                //先解除引用

                string CategoryImageCachePath = Path.Combine(GlobalConfig.Path_Base, CharacterItemList[index].CharacterImage);
                string CategoryBackgroundImageCachePath = Path.Combine(GlobalConfig.Path_Base, CharacterItemList[index].BackgroundImage);

                CharacterItemList.RemoveAt(index);

                //删除缓存图片
                if (File.Exists(CategoryImageCachePath))
                {
                    File.Delete(CategoryImageCachePath);
                }

                if (File.Exists(CategoryBackgroundImageCachePath))
                {
                    File.Delete(CategoryBackgroundImageCachePath);
                }
                SaveCharacterList();

            }
            else
            {
                _ = MessageHelper.Show("请先选中一个项目");
            }
        }
    }
}
