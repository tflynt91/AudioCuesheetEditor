﻿//This file is part of AudioCuesheetEditor.

//AudioCuesheetEditor is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//AudioCuesheetEditor is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Foobar.  If not, see
//<http: //www.gnu.org/licenses />.
using AudioCuesheetEditor.Controller;
using AudioCuesheetEditor.Model.Entity;
using AudioCuesheetEditor.Model.IO;
using AudioCuesheetEditor.Model.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioCuesheetEditor.Model.AudioCuesheet
{
    public enum MoveDirection
    {
        Up,
        Down
    }

    public class Cuesheet : Validateable
    {
        private readonly CuesheetController _cuesheetController;

        private readonly object syncLock = new object();

        private readonly List<Track> tracks;
        private String artist;
        private String title;
        private AudioFile audioFile;

        public Cuesheet(CuesheetController cuesheetController)
        {
            _cuesheetController = cuesheetController;
            tracks = new List<Track>();
            Validate();
        }
        public IReadOnlyCollection<Track> Tracks
        {
            get { return tracks.OrderBy(x => x.Position).ToList().AsReadOnly(); }
        }
        public uint NextFreePosition
        {
            get 
            {
                uint nextFreePosition = 1;
                if (Tracks.Count > 0)
                {
                    lock (syncLock)
                    {
                        var track = Tracks.Where(x => x.Position > 0).OrderBy(x => x.Position).LastOrDefault();
                        if (track != null)
                        {
                            nextFreePosition = track.Position + 1;
                        }
                    }
                }
                return nextFreePosition;
            }
        }
        public String Artist 
        {
            get { return artist; }
            set { artist = value; OnValidateablePropertyChanged(); }
        }
        public String Title 
        {
            get { return title; }
            set { title = value; OnValidateablePropertyChanged(); }
        }
        public AudioFile AudioFile 
        {
            get { return audioFile; }
            set { audioFile = value; OnValidateablePropertyChanged(); }
        }
        public Boolean CanWriteCuesheetFile
        {
            get
            {
                var cuesheetFile = new CuesheetFile(this);
                return cuesheetFile.IsExportable;
            }
        }

        public void AddTrack(Track track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }
            tracks.Add(track);
        }

        public void RemoveTrack(Track track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }
            tracks.Remove(track);
            RePositionTracks();
        }

        public void RemoveAllTracks()
        {
            tracks.Clear();
        }

        public Boolean MoveTrackPossible(Track track, MoveDirection moveDirection)
        {
            Boolean movePossible = false;
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }
            lock (syncLock)
            {
                var index = Tracks.ToList().IndexOf(track);
                if (moveDirection == MoveDirection.Up)
                {
                    if (index > 0)
                    {
                        movePossible = true;
                    }
                }
                if (moveDirection == MoveDirection.Down)
                {
                    if ((index + 1) < Tracks.Count())
                    {
                        movePossible = true;
                    }
                }
            }
            return movePossible;
        }

        public void MoveTrack(Track track, MoveDirection moveDirection)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }
            lock (syncLock)
            {
                var index = Tracks.ToList().IndexOf(track);
                if (moveDirection == MoveDirection.Up)
                {
                    if (index > 0)
                    {
                        var previousTrack = Tracks.ElementAt(index - 1);
                        var position = previousTrack.Position;
                        previousTrack.Position = track.Position;
                        track.Position = position;
                    }
                }
                if (moveDirection == MoveDirection.Down)
                {
                    if ((index + 1) < Tracks.Count())
                    {
                        var nextTrack = Tracks.ElementAt(index + 1);
                        var position = nextTrack.Position;
                        nextTrack.Position = track.Position;
                        track.Position = position;
                    }
                }
            }
        }

        protected override void Validate()
        {
            if (String.IsNullOrEmpty(Artist) == true)
            {
                validationErrors.Add(new ValidationError(String.Format(_cuesheetController.GetLocalizedString("HasNoValue"),_cuesheetController.GetLocalizedString("Artist")), FieldReference.Create(this, nameof(Artist)), ValidationErrorType.Warning));
            }
            if (String.IsNullOrEmpty(Title) == true)
            {
                validationErrors.Add(new ValidationError(String.Format(_cuesheetController.GetLocalizedString("HasNoValue"), _cuesheetController.GetLocalizedString("Title")), FieldReference.Create(this, nameof(Title)), ValidationErrorType.Warning));
            }
            if (AudioFile == null)
            {
                validationErrors.Add(new ValidationError(String.Format(_cuesheetController.GetLocalizedString("HasNoValue"), _cuesheetController.GetLocalizedString("Audiofile")), FieldReference.Create(this, nameof(AudioFile)), ValidationErrorType.Error));
            }
            //TODO: Check for track positions
        }

        private void RePositionTracks()
        {
            uint position = 1;
            foreach (var track in Tracks)
            {
                if (track.Position != position)
                {
                    track.Position = position;
                }
                position++;
            }
        }
    }
}