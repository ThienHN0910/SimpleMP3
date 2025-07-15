using SimpleMP3.Models;
using System.Collections.Generic;
using System.Windows;

namespace SimpleMP3.Views
{
    public partial class SelectTrackWindow : Window
    {
        public Track? SelectedTrack { get; private set; }

        public SelectTrackWindow(List<Track> tracks)
        {
            InitializeComponent();
            TrackListBox.ItemsSource = tracks;
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            SelectedTrack = TrackListBox.SelectedItem as Track;
            if (SelectedTrack == null)
            {
                MessageBox.Show("Hãy chọn một bài hát.");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}