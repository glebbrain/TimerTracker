using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TimerTracker.Libs
{
    public class Ini
    {
        public static string IniPath = "";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        //[DllImport("kernel32")] private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", SetLastError = true)] private static extern int WritePrivateProfileStruct(string pSection, string pKey, string pValue, int pValueLen, string pFile);
        [DllImport("kernel32", SetLastError = true)] private static extern int GetPrivateProfileString(string pSection, string pKey, string pDefault, byte[] prReturn, int pBufferLen, string pFile);
        [DllImport("kernel32", SetLastError = true)] private static extern int GetPrivateProfileStruct(string pSection, string pKey, byte[] prReturn, int pBufferLen, string pFile);

        /// <summary>
        /// Запись значения в ini файл
        /// </summary>
        /// <param name="s">Секция</param>
        /// <param name="k">Ключ</param>
        /// <param name="v">Значение</param>
        /// <param name="c">Комментарий к ключу</param>
        public static void writeValue(string s, string k, string v = " ", string c = "")
        {
            if (readValue(s, k) != null && readValue(s, k) != "")
            {
                WritePrivateProfileString(s, k, v, IniPath);
            }
            else
            {
                string sector = "";
                if (!searchInFile("[" + s + "]", IniPath))
                {
                    sector = "[" + s + "]";
                }
                using (StreamWriter sw = new StreamWriter(IniPath, true))
                {
                    if (sector != null && sector != "")
                    {
                        sw.WriteLine(sector);
                    }
                    if (c != null && c != "")
                    {
                        sw.WriteLine("; " + c);
                    }
                    if (k != null && k != "" && v != null)
                    {
                        if (v == "") v = " ";
                        sw.WriteLine(k + "=" + v);
                    }
                }
            }
        }
        /// <summary>
        /// Обновление значения
        /// </summary>
        /// <param name="s">Секция</param>
        /// <param name="k">Ключ</param>
        /// <param name="v">Значение</param>
        private static void updateValue(string s, string k, string v)
        {
            if (File.Exists(IniPath))
            {
                WritePrivateProfileString(s, k, v, IniPath);
            }
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <PARAM name="Section"></PARAM>
        /// <PARAM name="Key"></PARAM>
        /// <PARAM name="Path"></PARAM>
        /// <returns></returns>
        public static string readValue(string Section, string Key, string Def = "")
        {
            try
            {
                if (File.Exists(IniPath))
                {
                    int size = 2048;
                    StringBuilder temp = new StringBuilder(size);
                    string tt = z_GetString(Section, Key, Def, size);
                    if (tt == null || tt == "")
                    {
                        tt = Def;
                    }
                    return tt;
                }
                else
                {
                    return Def;
                }
            }
            catch
            {
                return Def;
            }
        }

        /// <summary>
        /// Call GetPrivateProfileString / GetPrivateProfileStruct API
        /// </summary>
        private static string z_GetString(string pSection, string pKey, string pDefault, int size = 256)
        {
            string sRet = pDefault;
            byte[] bRet = new byte[size];
            int i = GetPrivateProfileString(pSection, pKey, pDefault, bRet, size, IniPath);
            //sRet = System.Text.Encoding.GetEncoding(1252).GetString(bRet, 0, i).TrimEnd((char)0);
            // для понимания кирилицы
            sRet = System.Text.Encoding.Default.GetString(bRet, 0, i).TrimEnd((char)0);
            return (sRet);
        }

        /// <summary>
        /// Установить коммент в файл настроек, добавляется ; в начало строки
        /// </summary>
        /// <param name="comm"></param>
        public static void putComment(string comm)
        {
            if (comm.IndexOf("\r\n") > -1)
            {
                comm = comm.Replace("\r\n", "\r\n; ");
            }
            using (StreamWriter sw = new StreamWriter(IniPath, true))
            {
                sw.WriteLine("; " + comm);
            }
        }
        /// <summary>
        /// Поиск любого текста в файле настроек
        /// </summary>
        /// <param name="word">Текст который ищем</param>
        /// <param name="file">В каком файле ищем</param>
        /// <returns></returns>
        private static bool searchInFile(string word, string file)
        {
            if (File.Exists(file))
            {
                string line = "";
                using (StreamReader sr = new StreamReader(file, true))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line != null && line.IndexOf(word) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
