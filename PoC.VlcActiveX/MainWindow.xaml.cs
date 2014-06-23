using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AxAXVLC;
using AXVLC;
using Microsoft.Win32;

namespace PoC.VlcActiveX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly AxVLCPlugin2 _vlcPlugin;

        public MainWindow()
        {
            InitializeComponent();
            _vlcPlugin = new AxVLCPlugin2();
            VlcHost.Child = _vlcPlugin;
            _vlcPlugin.MediaPlayerPositionChanged += _vlcPlugin_MediaPlayerPositionChanged;
            _vlcPlugin.MediaPlayerTimeChanged += _vlcPlugin_MediaPlayerTimeChanged;
            Position.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(Position_MouseLeftButtonUp), true);

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _vlcPlugin.Toolbar = false;
        }

        void Position_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var seekTime = Math.Floor(Position.Value * _vlcPlugin.input.Length);
            _vlcPlugin.input.Time = seekTime;
        }

        void _vlcPlugin_MediaPlayerTimeChanged(object sender, DVLCEvents_MediaPlayerTimeChangedEvent e)
        {
        }

        void _vlcPlugin_MediaPlayerPositionChanged(object sender, DVLCEvents_MediaPlayerPositionChangedEvent e)
        {
            Position.Value = _vlcPlugin.input.Position;
            Info.Text = string.Format("Time: {0}", TimeSpan.FromMilliseconds(_vlcPlugin.input.Time));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vlcPlugin.playlist.stop();
            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog(this);
            if (!string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                var fileUri = new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute);
                _vlcPlugin.playlist.clear();
                _vlcPlugin.playlist.add(fileUri.AbsoluteUri, null, null);
                _vlcPlugin.playlist.play();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _vlcPlugin.playlist.play();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            _vlcPlugin.playlist.stop();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _vlcPlugin.playlist.pause();
        }

        void Position_OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _vlcPlugin.playlist.pause();
        }

        void Position_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var seekTime = Position.Value * _vlcPlugin.input.Length;
            _vlcPlugin.input.Time = seekTime;
            _vlcPlugin.playlist.play();
        }
    }
}
