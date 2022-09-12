using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Data_Encryption_Standard
{
    public partial class MainForm : Form
    {
        private byte[] closeByteText;
        private string key;
        private Random rnd;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Событие запуска программы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoadMainForm(object sender, EventArgs e)
        {
            rnd = new Random(DateTime.Now.Millisecond);
            textBox_key.Text = "qwerty12";
        }

        /// <summary>
        /// Генерация ключа.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_generateKey(object sender, EventArgs e)
        {
            byte[] rndKey = new byte[8];
            for (var i = 0; i < rndKey.Length; i++)
                rndKey[i] = (byte)rnd.Next(0, 127);

            textBox_key.Text = Encoding.ASCII.GetString(rndKey);
        }
        private void OnTextChangedTextBox_key(object sender, EventArgs e)
        {
            key = textBox_key.Text;
        }

        /// <summary>
        /// Зашифровка текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_encryptionText(object sender, EventArgs e)
        {
            richTextBox_closedText.Text = DES.EncryptDES(richTextBox_openText.Text, key, out closeByteText);
        }

        /// <summary>
        /// Дешифровка текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_decryptionText(object sender, EventArgs e)
        {
            if (closeByteText == null) closeByteText = Encoding.Default.GetBytes(richTextBox_closedText.Text);
            richTextBox_openText.Text = DES.DecryptDES(closeByteText, key);
        }

        /// <summary>
        /// Загрузка открытого текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_importOpenText(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.txt";

            if (dialog.ShowDialog() == DialogResult.OK)
                using (var sr = new StreamReader(dialog.OpenFile(), Encoding.Default))
                    richTextBox_openText.Text = sr.ReadToEnd();
        }

        /// <summary>
        /// Сохранение открытого текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_exportOpenText(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveDialog.FilterIndex = 2;
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
                using (var sw = new StreamWriter(saveDialog.OpenFile(), Encoding.Default))
                    sw.WriteLine(richTextBox_openText.Text);

        }

        /// <summary>
        /// Загрузка закрытого текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_importClosedText(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files | *.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var infoDirect = new FileInfo(dialog.FileName);
                using (var fs = new FileStream(infoDirect.DirectoryName + "\\" + infoDirect.Name, FileMode.Open))
                {
                    closeByteText = new byte[fs.Length];
                    fs.Read(closeByteText, 0, closeByteText.Length);
                    richTextBox_closedText.Text = Encoding.Default.GetString(closeByteText);
                }
            }
        }

        /// <summary>
        /// Сохранение закрытого текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_exportClosedText(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveDialog.FilterIndex = 2;
            saveDialog.RestoreDirectory = true;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var infoDirect = new FileInfo(saveDialog.FileName);
                using (var fs = new FileStream(infoDirect.DirectoryName + "\\" + infoDirect.Name, FileMode.Open))
                {
                    fs.Write(closeByteText, 0, closeByteText.Length);
                }
            }
        }

        /// <summary>
        /// Очистка открытого текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_clearOpenText(object sender, EventArgs e)
        {
            richTextBox_openText.Clear();
        }

        /// <summary>
        /// Очистка закрытого текста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickButton_clearClosedText(object sender, EventArgs e)
        {
            richTextBox_closedText.Clear();
        }
    }
}