using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using ZefirCloudFileCommon.core.file.Info;

namespace ZefirCloudFileClient.ui
{
    public class PicList
    {
        public List<PicInfo> Pics { get; set; } = new List<PicInfo>();
    }

    public class PicInfo
    {
        public string Guid { get; set; }
        public Grid Container { get; set; }
        public ZefirDirectoryInfo Source { get; set; }
        public Rectangle Rectangle { get; set; }
        public TextBlock Title { get; set; }
        public bool IsFile { get; set; }
        public ZefirFileInfo FileInfo { get; set; }
        public ZefirDirectoryInfo DirInfo { get; set; }
    }
}
