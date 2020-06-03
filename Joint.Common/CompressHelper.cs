using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Joint.Common
{
    public class CompressHelper
    {
        public byte[] DictionaryToZip(string path)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = null;

            try
            {
                using (ZipFile file = ZipFile.Create(ms))
                {
                    file.BeginUpdate();
                    file.NameTransform = new MyNameTransfom();//通过这个名称格式化器，可以将里面的文件名进行一些处理。默认情况下，会自动根据文件的路径在zip中创建有关的文件夹。

                    DirectoryInfo TheFolder = new DirectoryInfo(System.Web.HttpContext.Current.Server.MapPath(path));
                    foreach (FileInfo NextFile in TheFolder.GetFiles())
                    {
                        //拓展名是excel低版本的，则打包
                        if (NextFile.Extension.ToLower() == ".xls")
                        {
                            file.Add(NextFile.FullName);
                        }
                    }
                    //file.Add(System.Web.HttpContext.Current.Server.MapPath("/Upload/TempletDownLoad/1.客户信息导入模版.xls"));
                    //file.Add(System.Web.HttpContext.Current.Server.MapPath("/Upload/TempletDownLoad/2.会员卡套餐模版.xls"));
                    file.CommitUpdate();

                    buffer = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                ms.Close();
            }

            return buffer;
        }

    }



    public class MyNameTransfom : ICSharpCode.SharpZipLib.Core.INameTransform
    {

        #region INameTransform 成员

        public string TransformDirectory(string name)
        {
            return null;
        }

        public string TransformFile(string name)
        {
            return Path.GetFileName(name);
        }

        #endregion
    }
}
