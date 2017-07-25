using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SC2_Translation.CommonClass.CommonDefine;
using System.IO;
using System.Globalization;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;

namespace SC2_Translation
{
    /// <summary>
    /// SC2_TranslationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SC2_TranslationWindow 
    {
        #region 常量

        #endregion

        #region 字段


        #endregion

        #region 依赖项属性

        /// <summary>
        /// 显示日志依赖项
        /// </summary>
        public DependencyProperty IsShowLogProperty = DependencyProperty.Register("IsShowLog", typeof(bool), typeof(SC2_TranslationWindow), new PropertyMetadata(true));

        public bool IsShowLog { set=>SetValue(IsShowLogProperty,value); get=> (bool)GetValue(IsShowLogProperty); }

        #endregion

        #region 属性

        /// <summary>
        /// UI使用的语言类别
        /// </summary>
        public Dictionary<string, ResourceDictionary> UILanguages { set; get; }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SC2_TranslationWindow()
        {
            SetHighlight(this);
            InitializeComponent();
            InitSelectableLanguage(ComboBox_SoftwareLanguage);
            LoadLogFile();
        }

        #endregion

        #region 方法

       /// <summary>
       /// 初始化语言选择控件
       /// </summary>
       /// <param name="ComboBox_SoftwareLanguage">语言切换控件</param>
        private void InitSelectableLanguage(Fluent.ComboBox ComboBox_SoftwareLanguage)
        {
            UILanguages = new Dictionary<string, ResourceDictionary>();
            ResourceDictionary defaultLanguage = new ResourceDictionary();
            defaultLanguage.Source = new Uri(SC2_ConstValue.LanguageDefalut, UriKind.Relative);
            string defaultName = defaultLanguage["LanguageName"] as string;
            UILanguages.Add(defaultName, defaultLanguage);
            ComboBox_SoftwareLanguage.Items.Add(defaultName);
            ComboBox_SoftwareLanguage.SelectedItem = defaultName;
            bool existCN = true;
            if (Directory.Exists("Language"))
            {
                foreach (FileInfo select in Directory.GetFiles("Language").Select(r => new FileInfo(r)))
                {
                    if (select.Extension == ".xaml")
                    {
                        try
                        {
                            string coltureName = select.Name.Substring(0, select.Name.Length - select.Extension.Length);
                            if (coltureName == "zh-CN") existCN = false;
                            CultureInfo colture = new CultureInfo(coltureName);
                            ResourceDictionary language = new ResourceDictionary();
                            language.Source = new Uri(select.FullName);
                            string itemName = language["LanguageName"] as string;
                            UILanguages.Add(itemName, language);
                            ComboBox_SoftwareLanguage.Items.Add(itemName);
                            if (CultureInfo.CurrentCulture.Name == coltureName)
                            {
                                ComboBox_SoftwareLanguage.SelectedItem = itemName;
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                            continue;
                        }
                    }
                }
            }
            if (existCN)
            {
                ResourceDictionary cnLanguage = new ResourceDictionary();
                cnLanguage.Source = new Uri(SC2_ConstValue.LanguageZHCN, UriKind.Relative);
                string cnName = cnLanguage["LanguageName"] as string;
                UILanguages.Add(cnName, cnLanguage);
                ComboBox_SoftwareLanguage.Items.Add(cnName);
                if (CultureInfo.CurrentCulture.Name == "zh-CN")
                {
                    ComboBox_SoftwareLanguage.SelectedItem = cnName;
                }
            }
        }


        #region 日志

        /// <summary>
        /// 添加一条日志
        /// </summary>
        /// <param name="message">添加的日志信息</param>
        private void SendLogMessage(string message)
        {
            if (AvalonTextEditor_Log.Text == "")
            {
                AvalonTextEditor_Log.Text = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss ") + message;
            }
            else
            {
                AvalonTextEditor_Log.Text = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss ") + message + "\r\n" + AvalonTextEditor_Log.Text;
            }

        }


        /// <summary>
        /// 加载日志
        /// </summary>
        private void LoadLogFile()
        {

            try
            {
                if (!File.Exists(SC2_ConstValue.LogFileName))
                {
                    File.Create(SC2_ConstValue.LogFileName).Close();
                }
                StreamReader sr = new StreamReader(SC2_ConstValue.LogFileName);
                AvalonTextEditor_Log.Text = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        #endregion

        /// <summary>
        /// 设置高亮
        /// </summary>
        private void SetHighlight(object window)
        {
            IHighlightingDefinition customHighlighting;
            using (Stream s = window.GetType().Assembly.GetManifestResourceStream("SC2_Translation.Assets.Resources.LocalText.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            HighlightingManager.Instance.RegisterHighlighting("LocalText", new string[] { ".txt" }, customHighlighting);
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 语言选择事件
        /// </summary>
        /// <param name="sender">事件控件</param>
        /// <param name="e">响应参数</param>
        private void ComboBox_SoftwareLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string itemName = ComboBox_SoftwareLanguage.SelectedItem as string;
            ResourceDictionary_WindowLanguage.MergedDictionaries.Clear();
            ResourceDictionary_WindowLanguage.MergedDictionaries.Add(UILanguages[itemName]);
            //RefreshWindowTitle();
            if (IsShowLog) SendLogMessage(ResourceDictionary_WindowLanguage["Log_ChangeSoftwareLanguageLogMessage_Text_0"].ToString() + itemName + ResourceDictionary_WindowLanguage["Log_ChangeSoftwareLanguageLogMessage_Text_1"].ToString());
        }


        #endregion
    }
}
