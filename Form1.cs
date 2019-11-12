using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Xml;
using System.Xml.XPath;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO.Compression;
using Ionic.Zip;

namespace NewResourceManager
{
    public partial class Form1 : System.Windows.Forms.Form
    {

        /// <summary>
        /// 멤버 변수는 크로미움의 변수 이름 명명 스타일과 비슷하게 끝에 _가 붙습니다.
        /// 아무것도 붙이지 않는 것이 정석이지만 이제와서 바꿀 수는 없으므로 그대로 둡니다.
        /// </summary>
        /// 
        public string[] commandLines_;
        public string projectPath_ = @"E:\Games\161beta\Game.rpgproject";
        public List<string> fileList_ = new List<string>();
        public List<string> resources_ = new List<string>();
        public Dictionary<string, List<string>> xmlList_ = new Dictionary<string, List<string>>()
        {
            {"", new List<string> { "" } }
        };

        public string selectedFile = @"";

        public int lastFileIndex_ = 0;

        public int currentFolderIndex = 0;

        public bool isActivatedExtensionProgram = false;

        /// <summary>
        /// 디버그 모드에서 콘솔 창을 띄우기 위한 API를 kernel32.dll로부터 바인딩 합니다.
        /// 굳이 사용할 필요는 없으므로, 추후 System.Diagnostics.Debug.WriteLine으로 대체될 수 있습니다.
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public Form1(string[] args)
        {
            commandLines_ = args;
            InitializeComponent();
        }

        /// <summary>
        /// img 폴더 내의 모든 폴더를 재귀적으로 찾아 리스트화 합니다.
        /// </summary>
        /// <param name="concatDirs"></param>
        /// <param name="dirs"></param>
        public void GetDirs(List<string> concatDirs, string dirs)
        {
            try
            {
                foreach (string f in Directory.GetDirectories(dirs))
                {
                    concatDirs.Add(f);
                    foreach (string d in Directory.GetDirectories(f))
                    {
                        concatDirs.Add(d);
                        GetDirs(concatDirs, d);
                    }
                }
            } catch (System.Exception ex)
            {
               
            }
        }

        /// <summary>
        /// 프로젝트 파일을 선택하는 파일 선택 창을 띄웁니다.
        /// 프로그램이 MV 툴바에서 실행되지 않고, 독립적으로 실행되었을 때 실행됩니다.
        /// 프로젝트 파일은 프로젝트 경로를 취득하기 위해 사용됩니다.
        /// </summary>
        public void OpenMvGameFolder()
        {
            openFileDialog2.InitialDirectory = GetMvGameFolder();
            openFileDialog2.Title = Localization.OpenFileDialog2Title;
            openFileDialog2.FileName = "Game.rpgproject";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string targetPath = Path.GetDirectoryName(openFileDialog2.FileName);
                if (Path.GetFileName(openFileDialog2.FileName).Equals("Game.rpgproject"))
                {
                    projectPath_ = targetPath;
                }
                else
                {
                    MessageBox.Show(Localization.OpenMvGameFolderMessageBox, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    OpenMvGameFolder();
                }
            }
            else
            {
                OpenMvGameFolder();
            }
        }

        /// <summary>
        /// 이미지 폴더 목록을 새로 고침합니다.
        /// </summary>
        private void RefreshImgFolderList()
        {
            // 프로젝트 폴더에서 img 폴더를 읽습니다.
            List<string> concatDirs = new List<string>();

            string rpgprojectPath = Path.Combine(projectPath_, "Game.rpgproject");

            // img 폴더의 모든 폴더를 재귀적으로 나열합니다.
            GetDirs(concatDirs, Path.Combine(projectPath_, "img"));

            string[] dirs = concatDirs.ToArray();

            foreach (string s in dirs)
            {
                string[] items = s.Split(Path.DirectorySeparatorChar);
                int len = items.Length;

                string pathList = Path.Combine(items[len - 2], items[len - 1]);
                folderList.Items.Add(pathList);

                resources_.Add(s);
                xmlList_.Add(pathList.Replace('\\', '/'), new List<string>());
            }
        }

        /// <summary>
        /// 폼이 생성된 이후, 명령줄 인수 값을 취득하여 프로젝트 경로를 목적에 맞게 파싱합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            // 프로젝트 경로를 찾습니다.
            projectPath_ = Path.GetDirectoryName(projectPath_);

            // 프로젝트 경로에 공백 문자가 포함되어있을 때의 처리
            if (commandLines_.Length > 2) {

                projectPath_ = Path.GetDirectoryName(string.Join(" ", commandLines_.Skip(1)));

            } else
            {
                // 커맨드 라인 인수를 읽고 처리합니다.
                for (int i = 0; i < commandLines_.Length; i++)
                {
                    string targetPath = Path.GetDirectoryName(commandLines_[i]);
                    if (i == 1)
                    {
                        projectPath_ = targetPath;
                    }
                }
            }

            if (commandLines_.Length < 2)
            {
                OpenMvGameFolder();
            }

            RefreshImgFolderList();

            folderList.SelectedIndex = currentFolderIndex;
            
            RefreshXmlListBox();

        }

        /// <summary>
        /// 투명색 목록을 새로 고침합니다.
        /// </summary>
        private void RefreshXmlListBox()
        {
            int preCount = xml_listBox.Items.Count;
            int preSelectIndex = xml_listBox.SelectedIndex;

            string path = Path.Combine(projectPath_, "data", "transparentKey.xml");

            XmlDocument xmlDoc = new XmlDocument();

            xml_listBox.Items.Clear();

            // XML 파일이 있는지 확인합니다.
            if (File.Exists(path))
            {
                xmlDoc.Load(path);
                XmlNodeList objectNodes = xmlDoc.DocumentElement.ChildNodes;

                foreach(XmlNode node in objectNodes)
                {
                    var attributes = node.Attributes;
                    var resPath = attributes["path"].Value;
                    var name = attributes["name"].Value;
                    xml_listBox.Items.Add(resPath + " : " + name);
                    Console.WriteLine(attributes["name"].Value);

                    // 이중 배열처럼 접근
                    // xmlList_["img/pictures"][0]
                    xmlList_[resPath].Add(name);

                }
            }

            if(xml_listBox.Items.Count == 0)
            {
                xml_delete_btn.Enabled = false;
            } else
            {
                
                int currentCount = xml_listBox.Items.Count;
                if (preCount != currentCount && currentCount > 0)
                {
                    int value = currentCount - preCount;

                    // 이전 횟수가 더 많았는가
                    if (value < 0)
                    {
                        if(preSelectIndex > 0)
                        {
                            xml_listBox.SelectedIndex = preSelectIndex - 1;
                        } else if(preSelectIndex == 0)
                        {
                            xml_listBox.SelectedIndex = 0;
                        }
                    } else if(value > 0)
                    {
                        // 하나 더 증가했다면
                    }
                    
                    xml_listBox.Select();
                }
            }
            
        }

        /// <summary>
        /// 이미지 목록을 새로 고침합니다.
        /// </summary>
        public void RefreshFileList()
        {
            // 이전에 저장된 리스트를 삭제합니다.
            fileList.Items.Clear();
            fileList_.Clear();

            // 타겟 폴더를 탐색하고 리스트에 나열합니다.
            string targetFolder = resources_[folderList.SelectedIndex];
            string[] files = Directory.GetFiles(targetFolder);
            foreach (string f in files)
            {
                fileList.Items.Add(Path.GetFileName(f));
                fileList_.Add(f);
            }

            // 해당 이미지 폴더에 파일이 없으면 표시하지 않습니다.
            if (files.Length != 0)
            {
                export_btn.Enabled = true;
                if (currentFolderIndex != folderList.SelectedIndex)
                {
                    fileList.SelectedIndex = 0;
                } else
                {
                    fileList.SelectedIndex = lastFileIndex_ % fileList.Items.Count;
                }
            }

            if (files.Length == 0)
            {
                preview_btn.Enabled = false;
                delete_btn.Enabled = false;
                export_btn.Enabled = false;
            }

            currentFolderIndex = folderList.SelectedIndex;

        }

        /// <summary>
        /// 현재 선택된 파일이 png 파일인지 확인합니다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsPNG(int index)
        {
            return Path.GetExtension(fileList_[index]).ToLower() == ".png";
        }

        /// <summary>
        /// 이미지가 대상 경로에 실제로 존재하는 지 확인합니다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool Exists(int index)
        {
            return File.Exists(fileList_[index]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void folderList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        /// <summary>
        /// 이미지 목록의 인덱스 값이 변경되었을 때의 동작입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = fileList.SelectedIndex;
            selectedFile = fileList.SelectedItem.ToString();

            if(!Exists(index))
            {
                preview_btn.Enabled = false;
                delete_btn.Enabled = false;
            } else
            {
                preview_btn.Enabled = true;
                delete_btn.Enabled = true;
            }

            if(!IsPNG(index))
            {
                preview_btn.Enabled = false;
            }

        }

        /// <summary>
        /// 이미지 미리보기 창을 띄웁니다.
        /// </summary>
        public void StartPreviewWindow()
        {
            if (fileList.Enabled && !String.IsNullOrEmpty(selectedFile))
            {
                int index = fileList.SelectedIndex;

                if (Exists(index))
                {
                    var newForm = new Form2(fileList_[index]);
                    newForm.ShowDialog();
                    newForm.Disposed += NewForm_Disposed;
                    newForm.Dispose();
                }

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewForm_Disposed(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 미리 보기 버튼을 눌렀을 때의 동작입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void preview_btn_Click(object sender, EventArgs e)
        {
            StartPreviewWindow();
        }

        /// <summary>
        /// 가져오기 버튼을 눌렀을 때의 동작입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void import_btn_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = resources_[folderList.SelectedIndex];
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string targetFolder = resources_[folderList.SelectedIndex];
                var newForm = new ImportEditor(openFileDialog1.FileName, targetFolder);
                newForm.ShowDialog();

                if (newForm.isOk)
                {
                    AddTransparentKey(newForm.transparentKey.Split(':'));
                    RefreshFileList();

                    // 가져오기 창을 명시적으로 해제합니다. 
                    // 해제하지 않으면 바로 해제되지 않으면서 리소스 사용 중이라고 나옵니다.
                    newForm.Dispose();
                    newForm = null;

                }
            }
        }

        /// <summary>
        /// 투명색 설정 후 data/config.xml로 내보내기
        /// </summary>
        /// <param name="transparentKey"></param>
        public void AddTransparentKey(string transparentKey)
        {
            string path = Path.Combine(projectPath_, "data", "transparentKey.txt");
            File.AppendAllText(path, transparentKey + Environment.NewLine);
        }

        /// <summary>
        /// 프로젝트 폴더의 data 폴더에서 transparentKey.xml 파일을 로드합니다.
        /// 파일이 이미 존재하면 기존 데이터에 추가 기입합니다.
        /// 파일이 존재하지 않는 경우, 처음부터 새로 만듭니다.
        /// </summary>
        /// <param name="args"></param>
        public void AddTransparentKey(string[] args)
        {
            string path = Path.Combine(projectPath_, "data", "transparentKey.xml");

            XmlDocument xmlDoc = new XmlDocument();

            // XML 파일이 있는지 확인합니다.
            if (File.Exists(path))
            {
                xmlDoc.Load(path);
                XmlNode newNode = xmlDoc.CreateElement("object");
                AddNode(xmlDoc, newNode, args);
                xmlDoc.DocumentElement.AppendChild(newNode);

            } else
            {
                XmlNode root = xmlDoc.CreateElement("imagegroup");
                xmlDoc.AppendChild(root);

                XmlNode _object = xmlDoc.CreateElement("object");
                root.AppendChild(_object);

                AddNode(xmlDoc, _object, args);
            }

            xmlDoc.Save(path);
            RefreshXmlListBox();

            // XML에서 JSON 데이터로 변환합니다.
            string json = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path.Replace(".xml", ".json"), json, Encoding.UTF8);

        }

        /// <summary>
        /// 특정 투명색을 삭제합니다.
        /// 이 함수가 호출되면 투명색 목록이 기록된 파일인 transparentKey.xml 파일이 업데이트 됩니다.
        /// </summary>
        private void RemoveNode()
        {
            string path = Path.Combine(projectPath_, "data", "transparentKey.xml");

            XmlDocument xmlDoc = new XmlDocument();

            // XML 파일이 있는지 확인합니다.
            if (File.Exists(path))
            {
                xmlDoc.Load(path);

                XmlNodeList nodes = xmlDoc.DocumentElement.ChildNodes;

                foreach(XmlNode node in nodes)
                {
                    var filepath = node.Attributes["path"].Value;
                    var name = node.Attributes["name"].Value;
                    var key = string.Join(" : ", filepath, name);
                    string selectedKey = xml_listBox.SelectedItem.ToString();

                    Console.WriteLine(key);
                    Console.WriteLine(selectedKey);

                    if (selectedKey == key)
                    {
                        xmlDoc.DocumentElement.RemoveChild(node);
                    }
                }

                xmlDoc.Save(path);

                string json = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(path.Replace(".xml", ".json"), json, Encoding.UTF8);

                RefreshXmlListBox();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="_object"></param>
        /// <param name="args"></param>
        private void AddNode(XmlDocument xmlDoc, XmlNode _object, string[] args)
        {
            XmlAttribute attr_name = xmlDoc.CreateAttribute("name");
            attr_name.Value = args[0];
            _object.Attributes.Append(attr_name);

            XmlAttribute attr_res_path = xmlDoc.CreateAttribute("path");
            attr_res_path.Value = folderList.SelectedItem.ToString().Replace('\\', '/');
            _object.Attributes.Append(attr_res_path);

            XmlAttribute attr_color = xmlDoc.CreateAttribute("red");
            attr_color.Value = args[1];
            _object.Attributes.Append(attr_color);

            attr_color = xmlDoc.CreateAttribute("green");
            attr_color.Value = args[2];
            _object.Attributes.Append(attr_color);

            attr_color = xmlDoc.CreateAttribute("blue");
            attr_color.Value = args[3];
            _object.Attributes.Append(attr_color);

            attr_color = xmlDoc.CreateAttribute("alpha");
            attr_color.Value = args[4];
            _object.Attributes.Append(attr_color);
        }

        /// <summary>
        /// 이미지 목록에서 특정 이미지를 삭제합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void delete_btn_Click(object sender, EventArgs e)
        {
            try
            {
                int index = fileList.SelectedIndex;
                string selectedFile = fileList_[index];

                if (File.Exists(selectedFile))
                {
                    File.Delete(selectedFile);
                    if(fileList.SelectedIndex > 0)
                    {
                        fileList.SelectedIndex--;
                        lastFileIndex_ = fileList.SelectedIndex;
                    } else
                    {
                        lastFileIndex_ = 0;
                    }
                }

                RefreshFileList();

            } catch (SystemException ex)
            {

            }

        }

        /// <summary>
        /// 이미지 목록에서 더블 클릭을 눌렀을 때 미리 보기 창을 띄웁니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(preview_btn.Enabled)
            {
                StartPreviewWindow();
            }
            
        }

        /// <summary>
        /// 내보내기 버튼을 눌렀을 때 해당 이미지 파일을 대상 폴더로 복사합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void export_btn_Click(object sender, EventArgs e)
        {
            int index = fileList.SelectedIndex;
            string selectedFile = fileList_[index];
            string filename = Path.GetFileName(selectedFile);

            // 파일을 출력 폴더로 내보내기 위한 준비
            saveFileDialog1.InitialDirectory = resources_[folderList.SelectedIndex];
            saveFileDialog1.Filter = Localization.SaveFileDialog1Filter;
            saveFileDialog1.FileName = filename.Split('.')[0];            
                
            ImageFormat format = ImageFormat.Png;

            // 파일이 존재한다면
            if (File.Exists(selectedFile))
            {
                // 저장 버튼을 눌렀다면
                if(saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string ext = Path.GetExtension(saveFileDialog1.FileName);
                    switch(ext)
                    {
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case ".jpeg":
                            format = ImageFormat.Jpeg;
                            break;
                    }
                }

                Image tempImage;

                using (var bmpFile = new Bitmap(selectedFile))
                {
                    tempImage = new Bitmap(bmpFile);
                    tempImage.Save(saveFileDialog1.FileName, format);
                    tempImage.Dispose();
                }

            }
        }

        /// <summary>
        /// 컴포넌트 초기화가 완료되면 호출되는 폼 로드 이벤트입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
#if DEBUG
            AllocConsole();
#endif
            this.CenterToScreen();
            this.Text = Localization.Form1Title;
            preview_btn.Text = Localization.PreviewButton;
            import_btn.Text = Localization.ImportButton;
            delete_btn.Text = Localization.DeleteButton;
            export_btn.Text = Localization.ExportButton;
            groupBox1.Text = Localization.TransperantPanel;
            toolTip1.SetToolTip(groupBox1, Localization.TransperantPanelTooltip);
            add_extension_btn.Text = IsAddedExtension() ? Localization.RemoveExtensionButton : Localization.AddExtensionButton;

            // Identification.zip 압축 파일의 압축을 해제합니다.
            ExtractZip();

        }

        /// <summary>
        /// 확장 도구가 등록되어있는 지 여부를 확인합니다.
        /// </summary>
        /// <returns></returns>
        public bool IsAddedExtension()
        {
            string subkey = @"Software\KADOKAWA\RPGMV";
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(subkey, true);

            if (rk == null)
            {
                return false;
            }

            JArray jsonArray = JArray.Parse(Convert.ToString(rk.GetValue("mvTools")));

            // 배열이 없으면
            if (jsonArray.Count == 0)
            {
                return false;
            }
            else
            {
                // 배열을 읽어와 자동으로 목록화합니다.
                IList<RMMV.ExtensionSchema> data = jsonArray.ToObject<IList<RMMV.ExtensionSchema>>();
                RMMV.ExtensionSchema my_ex_data = data.FirstOrDefault(p => (p.name == Localization.RMMV_ExtensionSchema_name));

                // 확장 프로그램이 레지스트리에 등록된 적이 없습니다.
                if (my_ex_data == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 레지스트리에서 현재 프로젝트의 게임 폴더 경로를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public string GetMvGameFolder()
        {
            string subkey = @"Software\KADOKAWA\RPGMV";
            string ret = "";
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(subkey, true);

            if (rk == null)
            {
                rk = Registry.CurrentUser.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            ret = Convert.ToString(rk.GetValue("projectFileUrl"));

            return ret;

        }

        /// <summary>
        /// 레지스트리에 확장 도구를 등록하거나 해제합니다.
        /// </summary>
        /// <param name="isShowMessage"></param>
        public void UpdateRegistry(bool isShowMessage = true)
        {
            string subkey = @"Software\KADOKAWA\RPGMV";
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(subkey, true);
            if (rk == null)
            {
                rk = Registry.CurrentUser.CreateSubKey(subkey, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            // 새로운 데이터를 생성합니다.
            var process = Process.GetCurrentProcess();
            string[] paths = Path.GetDirectoryName(process.MainModule.FileName).Split('\\');

            string[] destPaths = new string[paths.Length - 1];
            Array.Copy(paths, 1, destPaths, 0, paths.Length - 1);

            string driveName = paths[0];
            string otherPartPaths = Path.Combine(destPaths).Replace("\\", "/");

            string fullPath = driveName + "/" + otherPartPaths;

            //string fullPath = Path.Combine(paths).Replace("\\", "/");

            Console.WriteLine(fullPath);

            RMMV.ExtensionSchema es = new RMMV.ExtensionSchema
            {
                appName = "NewResourceManager.exe",
                hint = Localization.RMMV_ExtensionSchema_hint,
                name = Localization.RMMV_ExtensionSchema_name,
                path = fullPath
            };

            JObject newData = (JObject)JToken.FromObject(es);

            string jsonData = Convert.ToString(rk.GetValue("mvTools"));
            Boolean isInvalidJsonData = jsonData.Equals("[]");

            JArray jsonArray = new JArray();

            // 배열이 없으면
            if (isInvalidJsonData)
            {
                // 배열을 새로 만듭니다.
                // 배열에 새로운 JSON 데이터를 추가합니다.
                jsonArray.Add(newData);

                if (isShowMessage)
                {
                    // Inject
                    MessageBox.Show(Localization.UpdateRegistryRetryProgramMsg);
                }
                 
                isActivatedExtensionProgram = true;

            }
            else
            {

                jsonArray = JArray.Parse(jsonData);

                // 배열을 읽어와 자동으로 목록화합니다.
                IList<RMMV.ExtensionSchema> data = jsonArray.ToObject<IList<RMMV.ExtensionSchema>>();

                RMMV.ExtensionSchema my_ex_data = data.FirstOrDefault(p => (p.name == Localization.RMMV_ExtensionSchema_name));

                // 확장 프로그램이 레지스트리에 등록된 적이 없습니다.
                if (my_ex_data == null)
                {
                    jsonArray.Add(newData);

                    if (isShowMessage)
                    {
                        // Inject
                        MessageBox.Show(Localization.UpdateRegistryRetryProgramMsg);
                    }
                    

                    isActivatedExtensionProgram = true;
                }
                else
                {
                    isActivatedExtensionProgram = true;

                    // 확장 프로그램이 이미 등록되어있습니다.
                    if (isShowMessage)
                    {
                        MessageBox.Show(Localization.UpdateRegistryAlreadyProgram);
                    }

                    if (isShowMessage)
                    {
                        DialogResult isOk = MessageBox.Show(Localization.UpdateRegistryDeleteProgram, Localization.InformationMsg, MessageBoxButtons.YesNo);
                        if (isOk == DialogResult.Yes)
                        {
                            // 배열에서 특정 값을 제외합니다.

                            IList<RMMV.ExtensionSchema> list = jsonArray.ToObject<IList<RMMV.ExtensionSchema>>();
                            JArray uniq = new JArray();

                            foreach (RMMV.ExtensionSchema node in list)
                            {
                                if (node.name != es.name)
                                {
                                    JObject uniqData = (JObject)JToken.FromObject(node);
                                    uniq.Add(uniqData);
                                }

                            }

                            jsonArray = uniq;

                            isActivatedExtensionProgram = false;

                            // Eject
                            MessageBox.Show(Localization.EjectMessage);

                        }
                    }
                }

            }

            // 레지스트리 값을 업데이트합니다.
            rk.SetValue("mvTools", jsonArray.ToString(Newtonsoft.Json.Formatting.None), RegistryValueKind.String);

            if (isActivatedExtensionProgram)
            {
                add_extension_btn.Text = Localization.RemoveExtensionButton;
            }
            else
            {
                add_extension_btn.Text = Localization.AddExtensionButton;
            }
        }

        /// <summary>
        /// 프로그램에 내장된 Identification.zip 파일의 압축을 해제합니다.
        /// </summary>
        private void ExtractZip()
        {
            
            try
            {
                // 실행 파일의 경로를 획득합니다.
                var process = Process.GetCurrentProcess();
                string root = Path.GetDirectoryName(process.MainModule.FileName);

                // app 파일이 있으면 압축 해제를 하지 않습니다.
                string app = Path.Combine(root, "Identification", "app");
                if(File.Exists(app))
                {
                    return;
                }

                string _zipPathMain = Path.Combine(root, "Identification.zip");

                File.WriteAllBytes(_zipPathMain, Properties.Resources.Identification);

                using (ZipFile zip = ZipFile.Read(_zipPathMain))
                {
                    foreach(ZipEntry e in zip)
                    {
                        e.Extract(Path.Combine(root, "Identification"));
                    }
                }

                File.Delete(_zipPathMain);
                

            } catch (Exception e)
            {
                MessageBox.Show(e.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 확장 도구를 추가하거나 해제합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void add_extension_btn_Click(object sender, EventArgs e)
        {
            UpdateRegistry();
        }

        /// <summary>
        /// 이미지가 선택된 상태에서 Delete 키를 눌렀을 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xml_listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                if (xml_listBox.Items.Count > 0)
                {
                    if (xml_listBox.SelectedItem == null)
                    {
                        xml_listBox.SelectedIndex = 0;
                    }
                }
                if (xml_listBox.SelectedItem == null)
                {
                    return;
                }
                RemoveNode();
                e.SuppressKeyPress = true;
            }
            
        }

        /// <summary>
        /// 투명색 목록에서 삭제 버튼을 눌렀을 때 동작합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xml_delete_btn_Click(object sender, EventArgs e)
        {
            if (xml_listBox.Items.Count > 0)
            {
                if(xml_listBox.SelectedItem == null)
                {
                    xml_listBox.SelectedIndex = 0;
                }
            }
            if(xml_listBox.SelectedItem == null)
            {
                return;
            }
            RemoveNode();
        }

        private void xml_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            xml_delete_btn.Enabled = true;
        }
    }

}

namespace RMMV
{
    /// <summary>
    /// 레지스트리 mvTools 키에 JSON을 작성하기 위한 JSON 스키마입니다.
    /// </summary>
    class ExtensionSchema
    {
        public string appName { get; set; }
        public string hint { get; set; }
        public string name { get; set; }
        public string path { get; set; }
    }
}