using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using ZefirCloudFileClient.settings;
using ZefirCloudFileClientLib.core;
using ZefirCloudFileCommon.core.file.Info;

namespace ZefirCloudFileClient.ui
{
    public class PicDrawer
    {
        public static PicList PicList { get; set; }
        public static void Draw(PrimeWindowVM context, ZefirDirectoryInfo info, Grid container)
        {
            if (info.Directories.Any() || info.Files.Any())
            {
                PicDrawer.PicList = new PicList();
                container.Children.Clear();
                var rowSize = container.ActualWidth;
                
                const int picHeight = 75;
                const int picWidth = 75;
                const int margin = 30;
                var marginRow = 50;
                var count = 0;
                var currentRow = 0;
                var currentColumn = 0;
                var itemsInRow = (int)Math.Ceiling(rowSize / (picWidth + (2 * margin)));
                var dirs = info.Directories;
                var files = info.Files;
                foreach (var dir in dirs)
                {
                    var newRect = new Rectangle()
                    {
                        Height = picHeight, 
                        Width = picWidth, 
                        Fill = Brushes.AntiqueWhite,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(currentColumn + margin, currentRow + marginRow, 15, 15),
                    };

                    var dirPathParts = dir.Name.Split(new[] {'/', '\\'});
                    var newTitle = new TextBlock()
                    {
                        Text = new string(dirPathParts[^1].ToCharArray().Take(25).ToArray()),
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Width = picWidth,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(currentColumn + margin, currentRow + marginRow + picHeight, 15, 15)
                    };

                    var myPic = new PicInfo
                    {
                        Container = container,
                        DirInfo = dir,
                        Source = info,
                        Title = newTitle,
                        Rectangle = newRect,
                        IsFile = false,
                        Guid = Guid.NewGuid().ToString()
                    };
                    newRect.DataContext = myPic;

                    PicDrawer.PicList.Pics.Add(myPic);

                    // Context menu
                    var dirContextMenu = new ContextMenu();
                    var openMenuItem = new MenuItem
                    {
                        Header = "Open",
                        DataContext = context,
                        Command = context.OpenDirCommand,
                        CommandParameter = myPic
                    };
                    dirContextMenu.Items.Add(openMenuItem);

                    var removeDirMenuItem = new MenuItem
                    {
                        Header = "Remove",
                        DataContext = context,
                        Command = context.RemoveDirCommand,
                        CommandParameter = myPic
                    };
                    dirContextMenu.Items.Add(removeDirMenuItem);

                    newRect.ContextMenu = dirContextMenu;

                    // Events for rect
                    newRect.MouseEnter += NewRect_MouseEnter;
                    newRect.MouseLeave += NewRect_MouseLeave;
                    newRect.MouseLeftButtonDown += NewRect_MouseLeftButtonDown;

                    container.Children.Add(newRect);
                    container.Children.Add(newTitle);

                    // Count column and rows
                    count++;
                    currentColumn += picWidth + margin;
                    if (count >= itemsInRow)
                    {
                        count = 0;
                        currentColumn = 0;
                        currentRow += (picHeight + marginRow);
                    }
                }

                foreach (var file in files)
                {
                    var newRect = new Rectangle()
                    {
                        Height = picHeight, 
                        Width = picWidth, 
                        Fill = Brushes.CadetBlue,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(currentColumn + margin, currentRow + marginRow, 15, 15),
                        ToolTip = new ToolTip()
                        {
                            Content = file.FileName
                        }
                    };
                    var newTitle = new TextBlock()
                    {
                        Text = new string(file.FileName.ToCharArray().Take(25).ToArray()),
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center,
                        Width = picWidth,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(currentColumn + margin, currentRow + marginRow + picHeight, 15, 15)
                    };

                    var myPic = new PicInfo
                    {
                        Container = container,
                        Source = info,
                        FileInfo = file,
                        Rectangle = newRect,
                        Title = newTitle,
                        IsFile = true,
                        Guid = Guid.NewGuid().ToString()
                    };
                    newRect.DataContext = myPic;

                    PicDrawer.PicList.Pics.Add(myPic);

                    // Context menu
                    var fileContextMenu = new ContextMenu();

                    // Download item
                    var downloadMenuItem = new MenuItem
                    {
                        Header = "Download",
                        DataContext = context,
                        Command = context.DownloadFileCommand,
                        CommandParameter = myPic
                    };
                    fileContextMenu.Items.Add(downloadMenuItem);

                    // Remove item
                    var removeMenuItem = new MenuItem
                    {
                        Header = "Remove", 
                        DataContext = context, 
                        Command = context.RemoveFileCommand, 
                        CommandParameter = myPic
                    };
                    fileContextMenu.Items.Add(removeMenuItem);
                    newRect.ContextMenu = fileContextMenu;

                    // Events for rect
                    newRect.MouseEnter += NewRect_MouseEnter;
                    newRect.MouseLeave += NewRect_MouseLeave;
                    newRect.MouseLeftButtonDown += NewRect_MouseLeftButtonDown;

                    container.Children.Add(newRect);
                    container.Children.Add(newTitle);

                    // Count column and rows
                    count++;
                    currentColumn += picWidth + margin;
                    if (count >= itemsInRow)
                    {
                        count = 0;
                        currentColumn = 0;
                        currentRow += (picHeight + marginRow);
                    }

                }
            }
            else
            {
                PicDrawer.PicList = new PicList();
                container.Children.Clear();
                container.Children.Add(new TextBlock
                {
                    Text = "No content",
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 200, 0, 0),
                    FontSize = 25,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.CadetBlue
                });
            }
        }

        private static void NewRect_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var myRecd = sender as Rectangle;
            var myInfo = myRecd.DataContext as PicInfo;
            // Clear selection
            foreach (var info in PicDrawer.PicList.Pics)
            {
                if (myInfo.Guid != info.Guid)
                {
                    info.Rectangle.Stroke = null;
                }
            }
            myRecd.Stroke = Brushes.Black;
        }

        private static void NewRect_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var myRecd = sender as Rectangle;
            var myPic = myRecd.DataContext as PicInfo;
            if (myPic.IsFile)
            {
                myRecd.Fill = Brushes.CadetBlue;
            }
            else
            {
                myRecd.Fill = Brushes.AntiqueWhite;
            }
        }

        private static void NewRect_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var myRecd = sender as Rectangle;
            var myPic = myRecd.DataContext as PicInfo;
            if (myPic.IsFile)
            {
                myRecd.Fill = Brushes.MediumAquamarine;
            }
            else
            {
                myRecd.Fill = Brushes.Beige;
            }
        }
    }
}
